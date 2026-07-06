using System;
using System.ComponentModel.DataAnnotations;
using FinanceControl.Core.Enums;

namespace FinanceControl.Core.ViewModels
{
    public class FinancaFormViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Informe o tipo.")]
        public FinanceType? Tipo { get; set; }

        [Required(ErrorMessage = "Informe o valor.")]
        [Range(0.01, 999999999.99, ErrorMessage = "Informe um valor maior que zero.")]
        public decimal? Valor { get; set; }

        [Required(ErrorMessage = "Informe a descrição.")]
        [StringLength(255, ErrorMessage = "A descrição deve ter no máximo 255 caracteres.")]
        public string Descricao { get; set; }

        [Required(ErrorMessage = "Informe a data.")]
        [DataType(DataType.Date)]
        public DateTime DataInclusao { get; set; }

        public bool SomenteLeitura { get; set; }
    }
}
