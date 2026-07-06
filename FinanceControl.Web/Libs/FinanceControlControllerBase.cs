using System.Web.Mvc;

namespace FinanceControl.Web.Libs
{
    [Autorizacao]
    public abstract class FinanceControlControllerBase : Controller
    {
        protected int UsuarioLogadoId
        {
            get { return int.Parse(Session["UsuarioId"].ToString()); }
        }

        protected bool UsuarioEhAdmin
        {
            get { return (Session["UsuarioRole"] ?? string.Empty).ToString() == "Admin"; }
        }

        protected ServicoFactory CriarServicos()
        {
            return new ServicoFactory();
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ViewBag.UsuarioNome = Session["UsuarioNome"];
            ViewBag.UsuarioRole = Session["UsuarioRole"];
            ViewBag.UsuarioEhAdmin = UsuarioEhAdmin;
            base.OnActionExecuting(filterContext);
        }
    }
}
