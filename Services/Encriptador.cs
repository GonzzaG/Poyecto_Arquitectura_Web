using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace Services
{
    public class Encriptador
    {
        public string Encriptar(string textoPlano)
        {
            if (textoPlano == null)
                throw new ArgumentNullException(nameof(textoPlano));

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(textoPlano));
                StringBuilder sb = new StringBuilder();
                foreach (byte b in bytes)
                    sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }

        public bool VerificarEncriptacion(string textoPlano, string textoEncriptado)
        {
            if (textoPlano == null || textoEncriptado == null)
                return false;

            string hashCalculado = Encriptar(textoPlano);
            return string.Equals(hashCalculado, textoEncriptado, StringComparison.OrdinalIgnoreCase);
        }
    }
}
