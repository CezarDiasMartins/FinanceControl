using System;
using FinanceControl.Core.DTOs;

namespace FinanceControl.Core.Interfaces.IServices
{
    public interface IReportService
    {
        RelatorioDto GerarMensal(int userId, int mes, int ano);
        RelatorioDto GerarPorPeriodo(int userId, DateTime dataInicial, DateTime dataFinal);
        byte[] GerarXlsx(RelatorioDto relatorio);
        byte[] GerarPdf(RelatorioDto relatorio);
    }
}
