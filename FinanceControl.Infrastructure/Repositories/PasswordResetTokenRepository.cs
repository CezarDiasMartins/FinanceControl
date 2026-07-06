using System;
using System.Data.Entity;
using System.Linq;
using FinanceControl.Core.Entities;
using FinanceControl.Core.Interfaces.IRepositories;
using FinanceControl.Infrastructure.Data;

namespace FinanceControl.Infrastructure.Repositories
{
    public class PasswordResetTokenRepository : RepositoryBase<PasswordResetToken>, IPasswordResetTokenRepository
    {
        public PasswordResetTokenRepository(FinanceControlDbContext contexto)
            : base(contexto) { }

        public PasswordResetToken ObterPorToken(string token)
        {
            return Contexto.PasswordResetTokens.FirstOrDefault(x => x.Token == token);
        }

        public void InvalidarTokensDoUsuario(int userId)
        {
            var tokens = Contexto.PasswordResetTokens.Where(x => x.UserId == userId && !x.Usado).ToList();
            foreach (var token in tokens)
            {
                token.Usado = true;
                Contexto.Entry(token).State = EntityState.Modified;
            }
        }

        public void ExcluirPorUsuario(int userId)
        {
            var tokens = Contexto.PasswordResetTokens.Where(x => x.UserId == userId).ToList();
            Contexto.PasswordResetTokens.RemoveRange(tokens);
        }
    }
}
