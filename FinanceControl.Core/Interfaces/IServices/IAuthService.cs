using System.Security.Claims;
using FinanceControl.Core.DTOs;
using FinanceControl.Core.Entities;
using FinanceControl.Core.ViewModels;

namespace FinanceControl.Core.Interfaces.IServices
{
    public interface IAuthService
    {
        ResultadoOperacao<User> Autenticar(LoginViewModel model, string segredoJwt);
        ResultadoOperacao Cadastrar(CadastroViewModel model);
        ResultadoOperacao<string> SolicitarRecuperacaoSenha(string email, string urlBase);
        ResultadoOperacao RedefinirSenha(NovaSenhaViewModel model);
        ClaimsIdentity CriarClaims(User usuario);
        string GerarJwt(User usuario, string segredoJwt);
    }
}
