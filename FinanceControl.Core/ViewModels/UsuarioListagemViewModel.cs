using System.Collections.Generic;
using FinanceControl.Core.Entities;

namespace FinanceControl.Core.ViewModels
{
    public class UsuarioListagemViewModel
    {
        public UsuarioListagemViewModel()
        {
            Usuarios = new List<User>();
        }

        public IList<User> Usuarios { get; set; }
    }
}
