namespace FinanceControl.Core.Interfaces.IServices
{
    public interface IEmailService
    {
        void EnviarRecuperacaoSenha(string destinatario, string nome, string link);
    }
}
