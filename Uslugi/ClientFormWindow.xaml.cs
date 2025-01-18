using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using Uslugi.Data;

namespace Uslugi
{
    /// <summary>
    /// Логика взаимодействия для ClientFormWindow.xaml
    /// </summary>
    public partial class ClientFormWindow : Window
    {
        private Client _client;
        private Yslygi_MDKEntities _connection;

        public ClientFormWindow(Client client = null)
        {
            InitializeComponent();

            _connection = new Yslygi_MDKEntities();
            _client = client;

            if (_client != null)
            {
                IdTextBox.Text = _client.ID.ToString();
                LastNameTextBox.Text = _client.LastName;
                FirstNameTextBox.Text = _client.FirstName;
                PatronymicTextBox.Text = _client.Patronymic;
                EmailTextBox.Text = _client.Email;
                PhoneTextBox.Text = _client.Phone;
                BirthdayDatePicker.SelectedDate = _client.Birthday;

                if (_client.GenderCode == "1")
                    MaleRadioButton.IsChecked = true;
                else
                    FemaleRadioButton.IsChecked = true;
            }
            else
            {
                IdTextBox.Visibility = Visibility.Collapsed;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput())
                return;

            if (_client == null)
            {
                _client = new Client
                {
                    RegistrationDate = DateTime.Now
                };
            }

            _client.LastName = LastNameTextBox.Text;
            _client.FirstName = FirstNameTextBox.Text;
            _client.Patronymic = PatronymicTextBox.Text;
            _client.Email = EmailTextBox.Text;
            _client.Phone = PhoneTextBox.Text;
            _client.Birthday = BirthdayDatePicker.SelectedDate.Value;
            _client.GenderCode = MaleRadioButton.IsChecked == true ? "1" : "2";

            if (_client.ID == 0)
            {
                _connection.Client.Add(_client);
            }

            _connection.SaveChanges();
            DialogResult = true;
            Close();
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(LastNameTextBox.Text) ||
                string.IsNullOrWhiteSpace(FirstNameTextBox.Text) ||
                string.IsNullOrWhiteSpace(EmailTextBox.Text) ||
                string.IsNullOrWhiteSpace(PhoneTextBox.Text) ||
                !BirthdayDatePicker.SelectedDate.HasValue)
            {
                MessageBox.Show("Все поля должны быть заполнены.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (!Regex.IsMatch(LastNameTextBox.Text, @"^[a-zA-Zа-яА-Я\s-]+$") ||
                !Regex.IsMatch(FirstNameTextBox.Text, @"^[a-zA-Zа-яА-Я\s-]+$") ||
                !Regex.IsMatch(PatronymicTextBox.Text, @"^[a-zA-Zа-яА-Я\s-]+$"))
            {
                MessageBox.Show("ФИО может содержать только буквы, пробелы и дефисы.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (!Regex.IsMatch(EmailTextBox.Text, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                MessageBox.Show("Некорректный email.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (!Regex.IsMatch(PhoneTextBox.Text, @"^[\+\-\(\)\s\d]+$"))
            {
                MessageBox.Show("Телефон может содержать только цифры, пробелы, плюсы, минусы и скобки.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }
    }
}