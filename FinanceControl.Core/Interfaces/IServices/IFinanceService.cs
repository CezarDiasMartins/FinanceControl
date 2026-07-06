using FinanceControl.Core.DTOs;
using FinanceControl.Core.Entities;
using FinanceControl.Core.ViewModels;

namespace FinanceControl.Core.Interfaces.IServices
{
    public interface IFinanceService
    {
        FinanceListagemViewModel ListarPorUsuario(FiltroFinancaDto filtro);
        Finance ObterPorIdDoUsuario(int id, int userId);
        ResultadoOperacao Salvar(FinancaFormViewModel model, int userId);
        ResultadoOperacao Excluir(int id, int userId);
    }
}
