using FinanceControl.Core.Entities;

namespace FinanceControl.Core.Interfaces.IRepositories
{
    public interface IPasswordResetTokenRepository : IRepository<PasswordResetToken>
    {
        PasswordResetToken ObterPorToken(string token);
        void InvalidarTokensDoUsuario(int userId);
        void ExcluirPorUsuario(int userId);
    }
}
