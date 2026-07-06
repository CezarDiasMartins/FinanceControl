using System.Globalization;

namespace FinanceControl.Core.Util
{
    public static class DecimalExtensions
    {
        private static readonly CultureInfo CulturaBrasil = new CultureInfo("pt-BR");

        public static string ToMoedaBr(this decimal valor)
        {
            return valor.ToString("C", CulturaBrasil);
        }

        public static string ToMoedaSemSimbolo(this decimal valor)
        {
            return valor.ToString("N2", CulturaBrasil);
        }
    }
}
