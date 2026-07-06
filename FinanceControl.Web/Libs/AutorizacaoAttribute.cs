using System.Web.Mvc;

namespace FinanceControl.Web.Libs
{
    public class AutorizacaoAttribute : ActionFilterAttribute
    {
        private readonly bool _exigirAdmin;

        public AutorizacaoAttribute(bool exigirAdmin = false)
        {
            _exigirAdmin = exigirAdmin;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var sessao = filterContext.HttpContext.Session;
            if (sessao == null || sessao["UsuarioId"] == null)
            {
                filterContext.Result = new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary
                {
                    { "controller", "Auth" },
                    { "action", "Login" }
                });
                return;
            }

            if (_exigirAdmin && !(sessao["UsuarioRole"] ?? string.Empty).ToString().Equals("Admin"))
            {
                filterContext.Result = new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary
                {
                    { "controller", "Home" },
                    { "action", "Index" }
                });
                return;
            }

            base.OnActionExecuting(filterContext);
        }
    }
}
