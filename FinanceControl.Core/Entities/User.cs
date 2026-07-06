using System;
using System.Collections.Generic;
using FinanceControl.Core.Enums;

namespace FinanceControl.Core.Entities
{
    public class User
    {
        public User()
        {
            Financas = new List<Finance>();
            TokensRecuperacaoSenha = new List<PasswordResetToken>();
            DataInclusao = DateTime.Now;
            Role = UserRole.User;
        }

        public int Id { get; set; }
        public string CPF { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
        public UserRole Role { get; set; }
        public DateTime DataInclusao { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public virtual ICollection<Finance> Financas { get; set; }
        public virtual ICollection<PasswordResetToken> TokensRecuperacaoSenha { get; set; }

        public bool EhAdministrador()
        {
            return Role == UserRole.Admin;
        }
    }
}
