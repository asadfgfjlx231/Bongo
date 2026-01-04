using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace Bongo
{
    internal class Adatbazis
    {
        string kapcs_szoveg;
        MySqlConnection kapcsolat;

        public Adatbazis()
        {
            kapcs_szoveg = "server=localhost;database=bongoweb;user=root;password=;";
        }

        private bool megnyitas()
        {
            try
            {
                if (kapcsolat == null)
                {
                    kapcsolat = new MySqlConnection(kapcs_szoveg);
                }
                kapcsolat.Open();
                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool bezaras()
        {
            try
            {
                if (kapcsolat == null) { return false; }
                kapcsolat.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }
        
        public string egySzoveg(string lks)
        {
            bezaras();
            if (megnyitas())
            {
                MySqlCommand c = new MySqlCommand(lks, kapcsolat);
                MySqlDataReader olvaso = c.ExecuteReader();
                if (olvaso.Read())
                {
                    return olvaso[0].ToString();
                }
            }

            return "hiba";
        }

        public int[] sokSzam(string lks)
        {
            bezaras();
            if (megnyitas())
            {
                MySqlCommand c = new MySqlCommand(lks, kapcsolat);
                MySqlDataReader olvaso = c.ExecuteReader();
                List<int> eredmeny = new List<int>();
                while (olvaso.Read())
                {
                    eredmeny.Add(Convert.ToInt32(olvaso[0]));
                }
                return eredmeny.ToArray();
            }

            return new int[] {};
        }

        public int egySzam(string lks)
        {
            bezaras();
            if (megnyitas())
            {
                MySqlCommand c = new MySqlCommand(lks, kapcsolat);
                MySqlDataReader olvaso = c.ExecuteReader();
                if (olvaso.Read())
                {
                    return Convert.ToInt32(olvaso[0]);
                }
            }

            return -1;
        }

        public bool feltoltes(string lks)
        {
            bezaras();
            if (megnyitas())
            {
                MySqlCommand c = new MySqlCommand(lks, kapcsolat);
                int result = c.ExecuteNonQuery();
                if (result >= 0) { return true; }
            }
            return false;
        }
    }
}
