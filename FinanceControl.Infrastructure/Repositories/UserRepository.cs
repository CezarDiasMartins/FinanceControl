using System.Collections.Generic;
using System.Linq;
using FinanceControl.Core.Entities;
using FinanceControl.Core.Interfaces.IRepositories;
using FinanceControl.Infrastructure.Data;

namespace FinanceControl.Infrastructure.Repositories
{
    public class UserRepository : RepositoryBase<User>, IUserRepository
    {
        public UserRepository(FinanceControlDbContext contexto)
            : base(contexto) { }

        public User ObterPorEmail(string email)
        {
            return Contexto.Users.FirstOrDefault(x => x.Email == email);
        }

        public User ObterPorCpf(string cpf)
        {
            return Contexto.Users.FirstOrDefault(x => x.CPF == cpf);
        }

        public IList<User> ListarTodos()
        {
            return Contexto.Users.OrderBy(x => x.Nome).ToList();
        }
    }
}
