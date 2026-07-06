using System.Data.Entity;
using FinanceControl.Core.Interfaces.IRepositories;
using FinanceControl.Infrastructure.Data;

namespace FinanceControl.Infrastructure.Repositories
{
    public abstract class RepositoryBase<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly FinanceControlDbContext Contexto;
        protected readonly DbSet<TEntity> Set;

        protected RepositoryBase(FinanceControlDbContext contexto)
        {
            Contexto = contexto;
            Set = contexto.Set<TEntity>();
        }

        public TEntity ObterPorId(int id)
        {
            return Set.Find(id);
        }

        public void Adicionar(TEntity entidade)
        {
            Set.Add(entidade);
        }

        public void Atualizar(TEntity entidade)
        {
            Contexto.Entry(entidade).State = EntityState.Modified;
        }

        public void Excluir(TEntity entidade)
        {
            Set.Remove(entidade);
        }

        public void Salvar()
        {
            Contexto.SaveChanges();
        }
    }
}
