using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace Bongo
{
    internal class Adatbazis
    {
        string kapcs_string;
        MySqlConnection kapcsolat;

        public Adatbazis(string server = "localhost", 
            string database = "bongoweb", string user = "root", string password = "")
        {
            kapcs_string = $"server={server};database={database};user={user};password={password};";
        }

        private bool megnyitas()
        {
            try
            {
                if (kapcsolat == null)
                {
                    kapcsolat = new MySqlConnection(kapcs_string);
                }
                kapcsolat.Open();
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show("Adatbázis megnyitásánál probléma: " + e.Message);
                return false;
            }
        }

        private bool bezaras()
        {
            try
            {
                if (kapcsolat != null)
                {
                    kapcsolat.Close();
                }
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show("Adatbázis bezárásánál probléma: " + e.Message);
                return false;
            }
        }
    }
}
