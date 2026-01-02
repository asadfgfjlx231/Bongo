using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Bongo
{
    /// <summary>
    /// Interaction logic for Belepes.xaml
    /// </summary>
    public partial class Belepes : Window
    {
        public Belepes()
        {
            InitializeComponent();
        }

        private void reg_btn_Click(object sender, RoutedEventArgs e)
        {
            Regisztracio regisztracioAblak = new Regisztracio();
            regisztracioAblak.Show();
            Hide();
            Close();
        }

        private void bej_btn_Click(object sender, RoutedEventArgs e)
        {
            string username = usernameInput.Text;
            string password = passwordInput.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Minden mezőt ki kell tölteni!!!");
                return;
            }

            if (false) // Ha nem létezik a felhasználó az adatbázisban
            {
                MessageBox.Show("Ilyen nevű felhasználó nem létezik!!!");
                return;
            }

            if (false) // Ha nem jó a jelszó
            {
                MessageBox.Show("Hibás a jelszó!!!");
                return;
            }

            MainWindow ablak = new MainWindow(); // Jövőbeli paraméter: felhasználó id-je
            ablak.Show();
            Hide();
        }
    }
}
