namespace FinanceControl.Core.Interfaces.IRepositories
{
    public interface IRepository<TEntity> where TEntity : class
    {
        TEntity ObterPorId(int id);
        void Adicionar(TEntity entidade);
        void Atualizar(TEntity entidade);
        void Excluir(TEntity entidade);
        void Salvar();
    }
}
