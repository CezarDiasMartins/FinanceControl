using System.Collections.Generic;

namespace FinanceControl.Core.DTOs
{
    public class RelatorioDto
    {
        public RelatorioDto()
        {
            Itens = new List<ItemRelatorioDto>();
        }

        public string Titulo { get; set; }
        public string Periodo { get; set; }
        public IList<ItemRelatorioDto> Itens { get; set; }
        public decimal TotalGastos { get; set; }
        public decimal TotalGanhos { get; set; }
        public decimal TotalResultado { get; set; }
    }
}
