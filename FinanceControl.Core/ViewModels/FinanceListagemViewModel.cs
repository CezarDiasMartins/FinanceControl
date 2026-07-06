using System;
using System.Collections.Generic;
using FinanceControl.Core.Entities;
using FinanceControl.Core.Enums;

namespace FinanceControl.Core.ViewModels
{
    public class FinanceListagemViewModel
    {
        public FinanceListagemViewModel()
        {
            Financas = new List<Finance>();
            Pagina = 1;
            RegistrosPorPagina = 10;
        }

        public IList<Finance> Financas { get; set; }
        public int Pagina { get; set; }
        public int RegistrosPorPagina { get; set; }
        public int TotalRegistros { get; set; }
        public DateTime? DataInicial { get; set; }
        public DateTime? DataFinal { get; set; }
        public FinanceType? Tipo { get; set; }
        public string Termo { get; set; }
        public decimal TotalGastos { get; set; }
        public decimal TotalGanhos { get; set; }
        public decimal Resultado { get; set; }

        public int TotalPaginas()
        {
            if (TotalRegistros <= 0)
                return 1;
            
            return (TotalRegistros + RegistrosPorPagina - 1) / RegistrosPorPagina;
        }
    }
}
