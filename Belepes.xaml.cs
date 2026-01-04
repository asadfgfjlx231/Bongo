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
        Adatbazis adatbazis;
        public Belepes()
        {
            adatbazis = new Adatbazis();

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

            if (adatbazis.egySzoveg($"SELECT name FROM users WHERE name='{username}';") == "hiba")
            {
                MessageBox.Show("Ilyen nevű felhasználó nem létezik!!!");
                return;
            }

            string titkos = Titkositas.HashJelszo(password);
            if (adatbazis.egySzoveg($"SELECT password FROM users WHERE name='{username}';") != titkos)
            {
                MessageBox.Show("Hibás a jelszó!!!");
                return;
            }

            int id = adatbazis.egySzam($"SELECT id FROM users WHERE name='{username}';");
            MainWindow ablak = new MainWindow(id); // Jövőbeli paraméter: felhasználó id-je
            ablak.Show();
            Hide();
        }
    }
}
