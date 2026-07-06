using System;
using FinanceControl.Core.Enums;

namespace FinanceControl.Core.Entities
{
    public class Finance
    {
        public Finance()
        {
            DataInclusao = DateTime.Now;
        }

        public int Id { get; set; }
        public int UserId { get; set; }
        public FinanceType Tipo { get; set; }
        public decimal Valor { get; set; }
        public string Descricao { get; set; }
        public DateTime DataInclusao { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public virtual User User { get; set; }
    }
}
