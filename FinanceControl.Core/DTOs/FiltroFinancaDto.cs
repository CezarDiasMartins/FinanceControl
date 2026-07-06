using System;
using FinanceControl.Core.Enums;

namespace FinanceControl.Core.DTOs
{
    public class FiltroFinancaDto
    {
        public int UserId { get; set; }
        public DateTime? DataInicial { get; set; }
        public DateTime? DataFinal { get; set; }
        public FinanceType? Tipo { get; set; }
        public string Termo { get; set; }
        public int Pagina { get; set; }
        public int RegistrosPorPagina { get; set; }

        public int PaginaSegura()
        {
            return Pagina < 1 ? 1 : Pagina;
        }

        public int RegistrosSeguros()
        {
            return RegistrosPorPagina < 1 ? 10 : RegistrosPorPagina;
        }
    }
}
