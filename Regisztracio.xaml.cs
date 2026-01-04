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
    /// Interaction logic for Regisztracio.xaml
    /// </summary>
    public partial class Regisztracio : Window
    {
        Adatbazis adatbazis;
        public Regisztracio()
        {
            adatbazis = new Adatbazis();

            InitializeComponent();
        }

        private void let_btn_Click(object sender, RoutedEventArgs e)
        {
            string username = usernameInput.Text;
            string password = passwordInput.Text;
            string passwordAgain = passwordAgainInput.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password)
                || string.IsNullOrEmpty(passwordAgain))
            {
                MessageBox.Show("Minden mezőt ki kell tölteni!!!");
                return;
            }

            if (false) // Ha már létezik ilyen nevű felhasználó adatbázisban
            {
                MessageBox.Show("Ilyen nevű felhasználó már létezik!!!");
                return;
            }

            if (password.Length < 8 || password.Length > 20)
            {
                MessageBox.Show("A jelszó legalább 8 és legfeljebb 20 karakter lehet!!");
                return;
            }

            if (password != passwordAgain)
            {
                MessageBox.Show("A két jelszónak egyeznie kell!!!");
                return;
            }

            //
            // új fiók létrehozása adatbázisban

            string titkositott = Titkositas.HashJelszo(password);
            if (!adatbazis.feltoltes($"INSERT INTO users (name, password) VALUES ('{username}', '{titkositott}');"))
            {
                MessageBox.Show("Sikertelen regisztráció!! Probléma az adatbázisban!!!");
                return;
            }
            //

            MessageBox.Show("Sikeres regisztráció!!!");

            visszaLepes();
        }

        private void vissza_btn_Click(object sender, RoutedEventArgs e)
        {
            visszaLepes();
        }

        private void visszaLepes()
        {
            Belepes belepo = new Belepes();
            belepo.Show();
            Hide();
            Close();
        }
    }
}
