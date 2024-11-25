using System;
using System.Collections.Generic;
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
        private Gender _gender;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Load_Client(object sender, RoutedEventArgs e)
        {
            var gend = _connection.Client
                .Where(x => Gender.Code == x.GenderCode)
                .ToArray();


            dataClient.ItemsSource = gend.Select(x => new
            {
                id = x.id,
                gender = x.Gender.Name,
                firstName = x.FirstName,
                lastName = x.LastName,
                patronymic = x.Patronymic,
                birthday = x.Birthday,
                phone = x.Phone,
                email = x.Email,
                registrationDate = x.RegistrationDate
            });
        }
    }
}
