using System.Collections.Generic;
using FinanceControl.Core.DTOs;
using FinanceControl.Core.Entities;
using FinanceControl.Core.ViewModels;

namespace FinanceControl.Core.Interfaces.IServices
{
    public interface IUserService
    {
        User ObterPorId(int id);
        IList<User> ListarTodos();
        PerfilViewModel ObterPerfil(int id);
        ResultadoOperacao AtualizarPerfil(PerfilViewModel model);
        ResultadoOperacao Excluir(int id, int usuarioLogadoId);
    }
}
