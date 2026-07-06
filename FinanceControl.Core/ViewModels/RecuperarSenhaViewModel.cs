using System.ComponentModel.DataAnnotations;

namespace FinanceControl.Core.ViewModels
{
    public class RecuperarSenhaViewModel
    {
        [Required(ErrorMessage = "Informe o e-mail.")]
        [EmailAddress(ErrorMessage = "Informe um e-mail válido.")]
        public string Email { get; set; }
    }
}
