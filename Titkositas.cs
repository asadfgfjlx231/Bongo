using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Bongo
{
    public static class Titkositas
    {
        public static string HashJelszo(string jelszo)
        {
            using (SHA256 HashKod = SHA256.Create())
            {
                byte[] TitkositottByteHalmaz = HashKod.ComputeHash(Encoding.UTF8.GetBytes(jelszo));
                return Convert.ToBase64String(TitkositottByteHalmaz);
            }
        }
    }
}
