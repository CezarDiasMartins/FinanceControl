using FinanceControl.Core.DTOs;
using FinanceControl.Core.Entities;
using System;
using System.Collections.Generic;

namespace FinanceControl.Core.Interfaces.IRepositories
{
    public interface IFinanceRepository : IRepository<Finance>
    {
        IList<Finance> ListarPorUsuario(FiltroFinancaDto filtro, out int totalRegistros);
        IList<Finance> ListarResumoPorUsuario(FiltroFinancaDto filtro);
        IList<Finance> ListarPorPeriodo(int userId, DateTime dataInicial, DateTime dataFinal);
        void ExcluirPorUsuario(int userId);
    }
}
