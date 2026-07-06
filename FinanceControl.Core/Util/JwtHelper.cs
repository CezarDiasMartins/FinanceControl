using System;
using System.Security.Cryptography;
using System.Text;
using FinanceControl.Core.Entities;

namespace FinanceControl.Core.Util
{
    public static class JwtHelper
    {
        public static string GerarToken(User usuario, string segredo, int minutosValidade)
        {
            var agora = DateTimeOffset.UtcNow;
            var expiracao = agora.AddMinutes(minutosValidade);
            var cabecalho = "{\"alg\":\"HS256\",\"typ\":\"JWT\"}";
            var payload = string.Format(
                "{{\"sub\":\"{0}\",\"nome\":\"{1}\",\"email\":\"{2}\",\"role\":\"{3}\",\"iat\":{4},\"exp\":{5}}}",
                usuario.Id,
                EscaparJson(usuario.Nome),
                EscaparJson(usuario.Email),
                usuario.Role,
                agora.ToUnixTimeSeconds(),
                expiracao.ToUnixTimeSeconds());

            var cabecalhoBase64 = ConverterBase64Url(Encoding.UTF8.GetBytes(cabecalho));
            var payloadBase64 = ConverterBase64Url(Encoding.UTF8.GetBytes(payload));
            var assinatura = Assinar(cabecalhoBase64 + "." + payloadBase64, segredo);
            return cabecalhoBase64 + "." + payloadBase64 + "." + assinatura;
        }

        private static string Assinar(string conteudo, string segredo)
        {
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(segredo)))
            {
                return ConverterBase64Url(hmac.ComputeHash(Encoding.UTF8.GetBytes(conteudo)));
            }
        }

        private static string ConverterBase64Url(byte[] bytes)
        {
            return Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');
        }

        private static string EscaparJson(string texto)
        {
            return (texto ?? string.Empty).Replace("\\", "\\\\").Replace("\"", "\\\"");
        }
    }
}
