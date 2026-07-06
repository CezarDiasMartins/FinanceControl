using System.ComponentModel.DataAnnotations;

namespace FinanceControl.Core.ViewModels
{
    public class NovaSenhaViewModel
    {
        [Required(ErrorMessage = "Token inválido.")]
        public string Token { get; set; }

        [Required(ErrorMessage = "Informe a nova senha.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "A senha deve ter entre 6 e 100 caracteres.")]
        [DataType(DataType.Password)]
        public string Senha { get; set; }

        [Required(ErrorMessage = "Confirme a nova senha.")]
        [Compare("Senha", ErrorMessage = "As senhas não conferem.")]
        [DataType(DataType.Password)]
        public string ConfirmacaoSenha { get; set; }
    }
}
