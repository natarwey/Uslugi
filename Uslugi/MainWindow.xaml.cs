using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Uslugi.Data;

namespace Uslugi
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static Yslygi_MDKEntities _connection = new Yslygi_MDKEntities();
        private List<Visit> visits;

        public MainWindow()
        {
            InitializeComponent();
            LoadGenders();
            Load_Client(null, null);
        }

        private void LoadGenders()
        {
            var genders = _connection.Gender.ToList();
            genders.Insert(0, new Gender { Name = "Все" });
            GenderComboBox.ItemsSource = genders;
            GenderComboBox.DisplayMemberPath = "Name";
            GenderComboBox.SelectedValuePath = "Code";
            GenderComboBox.SelectedIndex = 0;
        }

        private void GenderComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Load_Client(sender, e);
        }

        private void BirthdayComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Load_Client(sender, e);
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Load_Client(sender, e);
        }

        private void Load_Client(object sender, RoutedEventArgs e)
        {
            var selectedGender = GenderComboBox.SelectedItem as Gender;
            var genderCode = selectedGender?.Code;

            var clientsQuery = _connection.Client.AsQueryable();

            if (genderCode != null && selectedGender.Name != "Все")
            {
                clientsQuery = clientsQuery.Where(x => x.GenderCode == genderCode);
            }

            var searchText = SearchTextBox.Text.ToLower();
            if (!string.IsNullOrEmpty(searchText))
            {
                clientsQuery = clientsQuery.Where(x =>
                    x.FirstName.ToLower().Contains(searchText) ||
                    x.LastName.ToLower().Contains(searchText) ||
                    x.Patronymic.ToLower().Contains(searchText) ||
                    x.Email.ToLower().Contains(searchText) ||
                    x.Phone.ToLower().Contains(searchText)
                );
            }

            if (BirthdayComboBox.SelectedItem is ComboBoxItem selectedItem && selectedItem.Content.ToString() == "Показать дни рождения в этом месяце")
            {
                int currentMonth = DateTime.Now.Month;
                clientsQuery = clientsQuery.Where(x => x.Birthday.HasValue && x.Birthday.Value.Month == currentMonth);
            }

            var gend = clientsQuery.ToArray();
            visits = _connection.Visit.ToList();

            var clients = gend.Select(x => new
            {
                id = x.ID,
                gender = x.Gender.Name,
                firstName = x.FirstName,
                lastName = x.LastName,
                patronymic = x.Patronymic,
                birthday = x.Birthday?.ToString("dd-MM-yyyy") ?? "Не указано",
                phone = x.Phone,
                email = x.Email,
                registrationDate = x.RegistrationDate.ToString("dd-MM-yyyy HH:mm:ss"),
                lastDate = x.Visit.Any() ? x.Visit.OrderByDescending(v => v.Date).FirstOrDefault()?.Date.ToString("dd-MM-yyyy HH:mm:ss") : x.RegistrationDate.ToString("dd-MM-yyyy HH:mm:ss"),
                countVisit = visits.Where(y => y.Id_client == x.ID).Count() == 0 ? 1 : visits.Where(y => y.Id_client == x.ID).Count() + 1,
            }) ;

            dataClient.ItemsSource = clients;
        }

        private void SortButton_Click(object sender, RoutedEventArgs e)
        {
            var clients = dataClient.ItemsSource.Cast<object>().ToList();
            
            var selectedSort = SortComboBox.SelectedItem as ComboBoxItem;
            if (selectedSort != null)
            {
                switch (selectedSort.Content.ToString())
                {
                    case "Фамилия (А-Я)":
                        clients = clients.OrderBy(x => GetPropertyValue(x, "firstName")).ToList();
                        break;
                    case "Дата последнего посещения (новые)":
                        clients = clients.OrderByDescending(x => DateTime.Parse(GetPropertyValue(x, "lastDate").ToString())).ToList();
                        break;
                    case "Количество посещений (по убыванию)":
                        clients = clients.OrderByDescending(x => (int)GetPropertyValue(x, "countVisit")).ToList();
                        break;
                }

                dataClient.ItemsSource = clients;
            }
        }
        private object GetPropertyValue(object obj, string propertyName)
        {
            return obj.GetType().GetProperty(propertyName).GetValue(obj, null);
        }

        private void AddClientButton_Click(object sender, RoutedEventArgs e)
        {
            var clientForm = new ClientFormWindow();
            if (clientForm.ShowDialog() == true)
            {
                Load_Client(null, null);
            }
        }

        private void EditClientButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedClient = dataClient.SelectedItem as dynamic;
            if (selectedClient == null)
            {
                MessageBox.Show("Выберите клиента для редактирования.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var client = _connection.Client.Find(selectedClient.id);
            if (client == null)
            {
                MessageBox.Show("Клиент не найден.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var clientForm = new ClientFormWindow(client);
            if (clientForm.ShowDialog() == true)
            {
                Load_Client(null, null);
            }
        }
    }
}
