using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace FinanceControl.Core.Util
{
    public static class CriptografiaHelper
    {
        private const int TamanhoSalt = 16;
        private const int TamanhoHash = 32;
        private const int Iteracoes = 100000;

        public static string GerarHashSenha(string senha)
        {
            var salt = new byte[TamanhoSalt];
            using (var random = RandomNumberGenerator.Create())
            {
                random.GetBytes(salt);
            }

            using (var pbkdf2 = new Rfc2898DeriveBytes(senha, salt, Iteracoes))
            {
                var hash = pbkdf2.GetBytes(TamanhoHash);
                return string.Format("PBKDF2:{0}:{1}:{2}", Iteracoes, Convert.ToBase64String(salt), Convert.ToBase64String(hash));
            }
        }

        public static bool VerificarSenha(string senha, string hashSalvo)
        {
            if (string.IsNullOrWhiteSpace(senha) || string.IsNullOrWhiteSpace(hashSalvo))
                return false;

            if (hashSalvo.StartsWith("SHA256:", StringComparison.OrdinalIgnoreCase))
                return GerarSha256Base64(senha) == hashSalvo.Substring("SHA256:".Length);

            try
            {
                var partes = hashSalvo.Split(':');
                if (partes.Length != 4 || partes[0] != "PBKDF2")
                    return false;

                var iteracoes = int.Parse(partes[1]);
                var salt = Convert.FromBase64String(partes[2]);
                var hash = Convert.FromBase64String(partes[3]);

                using (var pbkdf2 = new Rfc2898DeriveBytes(senha, salt, iteracoes))
                {
                    return pbkdf2.GetBytes(hash.Length).SequenceEqual(hash);
                }
            }
            catch (FormatException)
            {
                return false;
            }
            catch (OverflowException)
            {
                return false;
            }
            catch (CryptographicException)
            {
                return false;
            }
        }

        public static string GerarTokenSeguro()
        {
            var bytes = new byte[48];
            using (var random = RandomNumberGenerator.Create())
            {
                random.GetBytes(bytes);
            }

            return ConverterBase64Url(bytes);
        }

        private static string GerarSha256Base64(string texto)
        {
            using (var sha = SHA256.Create())
            {
                return Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes(texto)));
            }
        }

        public static string ConverterBase64Url(byte[] bytes)
        {
            return Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');
        }
    }
}
