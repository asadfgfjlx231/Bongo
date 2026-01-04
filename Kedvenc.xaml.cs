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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Bongo
{
    /// <summary>
    /// Interaction logic for Kedvenc.xaml
    /// </summary>
    public partial class Kedvenc : UserControl
    {
        int Id;
        string Url;
        Kedvencek Ablak;
        public Kedvenc(int id, string url, Kedvencek ablak)
        {
            Id = id;
            Url = url;
            Ablak = ablak;

            InitializeComponent();
        }

        private void kuka_Click(object sender, RoutedEventArgs e)
        {
            Ablak.torles(Id);
        }

        private void megnyitas_Click(object sender, RoutedEventArgs e)
        {
            Ablak.megnyitas(Url);
        }
    }
}
