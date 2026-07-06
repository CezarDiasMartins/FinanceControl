using System;

namespace FinanceControl.Core.DTOs
{
    public class ItemRelatorioDto
    {
        public DateTime Data { get; set; }
        public string Rotulo { get; set; }
        public decimal Gastos { get; set; }
        public decimal Ganhos { get; set; }
        public decimal Resultado { get; set; }
        public string SinalResultado { get; set; }
    }
}
