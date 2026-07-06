using System.Web.Mvc;
using FinanceControl.Core.ViewModels;
using FinanceControl.Web.Libs;

namespace FinanceControl.Web.Controllers
{
    public class UserController : FinanceControlControllerBase
    {
        [Autorizacao(true)]
        public ActionResult Index()
        {
            using (var servicos = CriarServicos())
            {
                return View(new UsuarioListagemViewModel { Usuarios = servicos.CriarUserService().ListarTodos() });
            }
        }

        public ActionResult Perfil()
        {
            using (var servicos = CriarServicos())
            {
                var model = servicos.CriarUserService().ObterPerfil(UsuarioLogadoId);
                if (model == null)
                    return RedirectToAction("Logout", "Auth");

                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Perfil(PerfilViewModel model)
        {
            model.Id = UsuarioLogadoId;
            
            if (!ModelState.IsValid)
                return View(model);

            using (var servicos = CriarServicos())
            {
                var resultado = servicos.CriarUserService().AtualizarPerfil(model);
                if (!resultado.Sucesso)
                {
                    ModelState.AddModelError("", resultado.Mensagem);
                    return View(model);
                }

                Session["UsuarioNome"] = model.Nome;
                TempData["MensagemSucesso"] = resultado.Mensagem;
                return RedirectToAction("Perfil");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ExcluirPerfil()
        {
            using (var servicos = CriarServicos())
            {
                var resultado = servicos.CriarUserService().Excluir(UsuarioLogadoId, 0);
                if (!resultado.Sucesso)
                {
                    TempData["MensagemErro"] = resultado.Mensagem;
                    return RedirectToAction("Perfil");
                }
            }

            Session.Clear();
            return RedirectToAction("Login", "Auth");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Autorizacao(true)]
        public JsonResult Excluir(int id)
        {
            using (var servicos = CriarServicos())
            {
                var resultado = servicos.CriarUserService().Excluir(id, UsuarioLogadoId);
                return Json(new { sucesso = resultado.Sucesso, mensagem = resultado.Mensagem });
            }
        }
    }
}
