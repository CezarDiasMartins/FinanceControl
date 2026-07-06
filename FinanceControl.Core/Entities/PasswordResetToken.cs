using System;

namespace FinanceControl.Core.Entities
{
    public class PasswordResetToken
    {
        public PasswordResetToken()
        {
            DataInclusao = DateTime.Now;
        }

        public int Id { get; set; }
        public int UserId { get; set; }
        public string Token { get; set; }
        public DateTime Expiracao { get; set; }
        public bool Usado { get; set; }
        public DateTime DataInclusao { get; set; }
        public virtual User User { get; set; }

        public bool EstaValido()
        {
            return !Usado && Expiracao >= DateTime.Now;
        }
    }
}
