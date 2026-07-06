using FinanceControl.Core.Entities;
using System.Collections.Generic;

namespace FinanceControl.Core.Interfaces.IRepositories
{
    public interface IUserRepository : IRepository<User>
    {
        User ObterPorEmail(string email);
        User ObterPorCpf(string cpf);
        IList<User> ListarTodos();
    }
}
