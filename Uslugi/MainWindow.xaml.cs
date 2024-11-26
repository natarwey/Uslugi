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
            Load_Client(null, null); // Загрузка данных при запуске
        }

        private void LoadGenders()
        {
            var genders = _connection.Gender.ToList();
            genders.Insert(0, new Gender { Name = "Все" }); // Добавляем опцию "Все"
            GenderComboBox.ItemsSource = genders;
            GenderComboBox.DisplayMemberPath = "Name";
            GenderComboBox.SelectedValuePath = "Code";
            GenderComboBox.SelectedIndex = 0; // Выбираем "Все" по умолчанию
        }

        private void GenderComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
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


            var gend = clientsQuery.ToArray();
            visits = _connection.Visit.ToList();

            dataClient.ItemsSource = gend.Select(x => new
            {
                id = x.ID,
                gender = x.Gender.Name,
                firstName = x.FirstName,
                lastName = x.LastName,
                patronymic = x.Patronymic,
                birthday = x.Birthday,
                phone = x.Phone,
                email = x.Email,
                registrationDate = x.RegistrationDate.ToString("yyyy-MM-dd HH:mm:ss"),
                lastDate = x.Visit.Any() ? x.Visit.OrderByDescending(v => v.Date).FirstOrDefault()?.Date.ToString("yyyy-MM-dd HH:mm:ss") : x.RegistrationDate.ToString("yyyy-MM-dd HH:mm:ss"),
                countVisit = visits.Where(y => y.Id_client == x.ID).Count() == 0 ? 1 : visits.Where(y => y.Id_client == x.ID).Count() + 1,
            }) ;
        }
    }
}
