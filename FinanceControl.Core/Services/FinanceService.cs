using System;
using System.Linq;
using FinanceControl.Core.DTOs;
using FinanceControl.Core.Entities;
using FinanceControl.Core.Interfaces.IRepositories;
using FinanceControl.Core.Interfaces.IServices;
using FinanceControl.Core.ViewModels;

namespace FinanceControl.Core.Services
{
    public class FinanceService : IFinanceService
    {
        private readonly IFinanceRepository _financeRepository;

        public FinanceService(IFinanceRepository financeRepository)
        {
            _financeRepository = financeRepository;
        }

        public FinanceListagemViewModel ListarPorUsuario(FiltroFinancaDto filtro)
        {
            int totalRegistros;
            var financas = _financeRepository.ListarPorUsuario(filtro, out totalRegistros);
            var financasDoResumo = _financeRepository.ListarResumoPorUsuario(filtro);

            var totalGastos = financasDoResumo.Where(x => x.Tipo == Enums.FinanceType.Gasto).Sum(x => x.Valor);
            var totalGanhos = financasDoResumo.Where(x => x.Tipo == Enums.FinanceType.Ganho).Sum(x => x.Valor);

            return new FinanceListagemViewModel
            {
                Financas = financas,
                Pagina = filtro.PaginaSegura(),
                RegistrosPorPagina = filtro.RegistrosSeguros(),
                TotalRegistros = totalRegistros,
                DataInicial = filtro.DataInicial,
                DataFinal = filtro.DataFinal,
                Tipo = filtro.Tipo,
                Termo = filtro.Termo,
                TotalGastos = totalGastos,
                TotalGanhos = totalGanhos,
                Resultado = totalGanhos - totalGastos
            };
        }

        public Finance ObterPorIdDoUsuario(int id, int userId)
        {
            var financa = _financeRepository.ObterPorId(id);
            if (financa == null || financa.UserId != userId)
                return null;

            return financa;
        }

        public ResultadoOperacao Salvar(FinancaFormViewModel model, int userId)
        {
            if (!model.Valor.HasValue || model.Valor.Value <= 0)
                return ResultadoOperacao.Falha("O valor deve ser maior que zero.");

            if (!model.Tipo.HasValue)
                return ResultadoOperacao.Falha("Informe o tipo.");

            if (model.Id == 0)
                _financeRepository.Adicionar(new Finance
                {
                    UserId = userId,
                    Tipo = model.Tipo.Value,
                    Valor = model.Valor.Value,
                    Descricao = model.Descricao,
                    DataInclusao = model.DataInclusao
                });
            else
            {
                var financa = ObterPorIdDoUsuario(model.Id, userId);
                if (financa == null)
                    return ResultadoOperacao.Falha("Finança não localizada.");

                financa.Tipo = model.Tipo.Value;
                financa.Valor = model.Valor.Value;
                financa.Descricao = model.Descricao;
                financa.DataInclusao = model.DataInclusao;
                financa.DataAlteracao = DateTime.Now;
                _financeRepository.Atualizar(financa);
            }

            _financeRepository.Salvar();
            return ResultadoOperacao.Ok("Finança salva com sucesso.");
        }

        public ResultadoOperacao Excluir(int id, int userId)
        {
            var financa = ObterPorIdDoUsuario(id, userId);
            if (financa == null)
                return ResultadoOperacao.Falha("Finança não localizada.");

            _financeRepository.Excluir(financa);
            _financeRepository.Salvar();
            return ResultadoOperacao.Ok("Finança excluída com sucesso.");
        }
    }
}
