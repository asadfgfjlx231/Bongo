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
    /// Interaction logic for Kedvencek.xaml
    /// </summary>
    public partial class Kedvencek : Window
    {
        int userId;
        MainWindow Bongeszo;
        Adatbazis adatbazis;

        public Kedvencek(int id, MainWindow bongeszo)
        {
            userId = id;
            Bongeszo = bongeszo;
            adatbazis = new Adatbazis();

            InitializeComponent();

            feltoltes();
        }

        private void feltoltes()
        {
            int[] idk = adatbazis.sokSzam(""); // lekérdezés a kapcsolás id-je

            int top = 10;

            foreach (int id in idk)
            {
                int url_id = adatbazis.egySzam(""); // lekérdezés az url id-je kapcsolás alapján
                string url = adatbazis.egySzoveg(""); // lekérdezés az url url_id alapján

                new Kedvenc(id, url, this)
                {
                    VerticalAlignment = VerticalAlignment.Top,
                    Margin = new Thickness(50, top, 50, 0),
                };
            }
        }

        public void megnyitas(string url)
        {
            MessageBox.Show($"Megnyitás: {url}");

            Close();
        }

        public void torles(int id)
        {
            MessageBox.Show($"Törlés: {id}");


        }

        private void ujratoltes()
        {
            Content = string.Empty;

            feltoltes();
        }
    }
}
