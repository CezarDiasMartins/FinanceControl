using System.Configuration;
using System.Security.Claims;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using FinanceControl.Core.ViewModels;
using FinanceControl.Web.Libs;

namespace FinanceControl.Web.Controllers
{
    public class AuthController : Controller
    {
        public ActionResult Login()
        {
            if (Session["UsuarioId"] != null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(new LoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (var servicos = new ServicoFactory())
            {
                var authService = servicos.CriarAuthService();
                var resultado = authService.Autenticar(model, SegredoJwt());
                if (!resultado.Sucesso)
                {
                    TempData["MensagemErro"] = "E-mail ou senha inválidos.";
                    return View(model);
                }

                var usuario = resultado.Dados;
                var token = authService.GerarJwt(usuario, SegredoJwt());
                Session["UsuarioId"] = usuario.Id;
                Session["UsuarioNome"] = usuario.Nome;
                Session["UsuarioRole"] = usuario.Role.ToString();
                Thread.CurrentPrincipal = new ClaimsPrincipal(authService.CriarClaims(usuario));
                Response.Cookies.Add(new HttpCookie("FinanceControlToken", token) { HttpOnly = true });
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult Cadastro()
        {
            return View(new CadastroViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Cadastro(CadastroViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (var servicos = new ServicoFactory())
            {
                var resultado = servicos.CriarAuthService().Cadastrar(model);
                if (!resultado.Sucesso)
                {
                    ModelState.AddModelError("", resultado.Mensagem);
                    return View(model);
                }

                TempData["MensagemSucesso"] = resultado.Mensagem;
                return RedirectToAction("Login");
            }
        }

        public ActionResult RecuperarSenha()
        {
            return View(new RecuperarSenhaViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RecuperarSenha(RecuperarSenhaViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (var servicos = new ServicoFactory())
            {
                var resultado = servicos.CriarAuthService().SolicitarRecuperacaoSenha(model.Email, Request.Url.GetLeftPart(System.UriPartial.Authority));
                if (!resultado.Sucesso)
                {
                    TempData["MensagemErro"] = resultado.Mensagem;
                    return View(model);
                }

                TempData["MensagemSucesso"] = resultado.Mensagem;
                if (!string.IsNullOrWhiteSpace(resultado.Dados))
                {
                    TempData["LinkRecuperacaoSenha"] = resultado.Dados;
                }

                return RedirectToAction("Login");
            }
        }

        public ActionResult NovaSenha(string token)
        {
            return View(new NovaSenhaViewModel { Token = token });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NovaSenha(NovaSenhaViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (var servicos = new ServicoFactory())
            {
                var resultado = servicos.CriarAuthService().RedefinirSenha(model);
                if (!resultado.Sucesso)
                {
                    ModelState.AddModelError("", resultado.Mensagem);
                    return View(model);
                }

                TempData["MensagemSucesso"] = resultado.Mensagem;
                return RedirectToAction("Login");
            }
        }

        public ActionResult Logout()
        {
            Session.Clear();
            if (Request.Cookies["FinanceControlToken"] != null)
            {
                var cookie = new HttpCookie("FinanceControlToken") { Expires = System.DateTime.Now.AddDays(-1) };
                Response.Cookies.Add(cookie);
            }

            return RedirectToAction("Login");
        }

        private static string SegredoJwt()
        {
            return ConfigurationManager.AppSettings["JwtSecret"] ?? "FinanceControl-Segredo-Desenvolvimento-48";
        }
    }
}
