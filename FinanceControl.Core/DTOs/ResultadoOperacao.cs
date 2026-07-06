namespace FinanceControl.Core.DTOs
{
    public class ResultadoOperacao
    {
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; }

        public static ResultadoOperacao Ok(string mensagem)
        {
            return new ResultadoOperacao { Sucesso = true, Mensagem = mensagem };
        }

        public static ResultadoOperacao Falha(string mensagem)
        {
            return new ResultadoOperacao { Sucesso = false, Mensagem = mensagem };
        }
    }

    public class ResultadoOperacao<T> : ResultadoOperacao
    {
        public T Dados { get; set; }

        public static ResultadoOperacao<T> Ok(T dados, string mensagem)
        {
            return new ResultadoOperacao<T> { Sucesso = true, Mensagem = mensagem, Dados = dados };
        }

        public new static ResultadoOperacao<T> Falha(string mensagem)
        {
            return new ResultadoOperacao<T> { Sucesso = false, Mensagem = mensagem };
        }
    }
}
