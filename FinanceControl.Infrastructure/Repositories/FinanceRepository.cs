using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using FinanceControl.Core.DTOs;
using FinanceControl.Core.Entities;
using FinanceControl.Core.Interfaces.IRepositories;
using FinanceControl.Infrastructure.Data;

namespace FinanceControl.Infrastructure.Repositories
{
    public class FinanceRepository : RepositoryBase<Finance>, IFinanceRepository
    {
        public FinanceRepository(FinanceControlDbContext contexto)
            : base(contexto) { }

        public IList<Finance> ListarPorUsuario(FiltroFinancaDto filtro, out int totalRegistros)
        {
            var consulta = MontarConsulta(filtro);

            totalRegistros = consulta.Count();
            return consulta
                .OrderByDescending(x => x.DataInclusao)
                .ThenByDescending(x => x.Id)
                .Skip((filtro.PaginaSegura() - 1) * filtro.RegistrosSeguros())
                .Take(filtro.RegistrosSeguros())
                .ToList();
        }

        public IList<Finance> ListarResumoPorUsuario(FiltroFinancaDto filtro)
        {
            return MontarConsulta(filtro).ToList();
        }

        public IList<Finance> ListarPorPeriodo(int userId, DateTime dataInicial, DateTime dataFinal)
        {
            var inicio = dataInicial.Date;
            var fim = dataFinal.Date;
            return Contexto.Finances
                .Where(x => x.UserId == userId && DbFunctions.TruncateTime(x.DataInclusao) >= inicio && DbFunctions.TruncateTime(x.DataInclusao) <= fim)
                .ToList();
        }

        private IQueryable<Finance> MontarConsulta(FiltroFinancaDto filtro)
        {
            var consulta = Contexto.Finances.Where(x => x.UserId == filtro.UserId);

            if (filtro.DataInicial.HasValue)
            {
                var dataInicial = filtro.DataInicial.Value.Date;
                consulta = consulta.Where(x => DbFunctions.TruncateTime(x.DataInclusao) >= dataInicial);
            }

            if (filtro.DataFinal.HasValue)
            {
                var dataFinal = filtro.DataFinal.Value.Date;
                consulta = consulta.Where(x => DbFunctions.TruncateTime(x.DataInclusao) <= dataFinal);
            }

            if (filtro.Tipo.HasValue)
                consulta = consulta.Where(x => x.Tipo == filtro.Tipo.Value);

            if (!string.IsNullOrWhiteSpace(filtro.Termo))
                consulta = consulta.Where(x => x.Descricao.Contains(filtro.Termo));

            return consulta;
        }

        public void ExcluirPorUsuario(int userId)
        {
            var financas = Contexto.Finances.Where(x => x.UserId == userId).ToList();
            Contexto.Finances.RemoveRange(financas);
        }
    }
}
