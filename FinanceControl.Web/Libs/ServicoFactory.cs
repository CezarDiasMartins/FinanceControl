using System;
using FinanceControl.Core.Services;
using FinanceControl.Infrastructure.Data;
using FinanceControl.Infrastructure.Repositories;

namespace FinanceControl.Web.Libs
{
    public class ServicoFactory : IDisposable
    {
        private readonly FinanceControlDbContext _contexto;
        private readonly UserRepository _usuarioRepository;
        private readonly FinanceRepository _financeRepository;
        private readonly PasswordResetTokenRepository _tokenRepository;

        public ServicoFactory()
        {
            _contexto = new FinanceControlDbContext();
            _usuarioRepository = new UserRepository(_contexto);
            _financeRepository = new FinanceRepository(_contexto);
            _tokenRepository = new PasswordResetTokenRepository(_contexto);
        }

        public AuthService CriarAuthService()
        {
            return new AuthService(_usuarioRepository, _tokenRepository, new EmailService());
        }

        public FinanceService CriarFinanceService()
        {
            return new FinanceService(_financeRepository);
        }

        public ReportService CriarReportService()
        {
            return new ReportService(_financeRepository);
        }

        public UserService CriarUserService()
        {
            return new UserService(_usuarioRepository, _financeRepository, _tokenRepository);
        }

        public void Dispose()
        {
            _contexto.Dispose();
        }
    }
}
