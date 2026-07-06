using System.Linq;

namespace FinanceControl.Core.Util
{
    public static class StringExtensions
    {
        public static string SoNumeros(this string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return string.Empty;

            return new string(texto.Where(char.IsDigit).ToArray());
        }

        public static string FormatarCpf(this string cpf)
        {
            var numeros = cpf.SoNumeros();
            if (numeros.Length != 11)
                return cpf;

            return string.Format("{0}.{1}.{2}-{3}",
                numeros.Substring(0, 3),
                numeros.Substring(3, 3),
                numeros.Substring(6, 3),
                numeros.Substring(9, 2));
        }
    }
}
