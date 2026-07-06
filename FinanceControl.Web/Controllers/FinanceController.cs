using System;
using System.Web.Mvc;
using FinanceControl.Core.Enums;
using FinanceControl.Core.ViewModels;
using FinanceControl.Web.Libs;

namespace FinanceControl.Web.Controllers
{
    public class FinanceController : FinanceControlControllerBase
    {
        public ActionResult Form(int? id, bool visualizar = false)
        {
            var model = new FinancaFormViewModel
            {
                DataInclusao = DateTime.Today,
                SomenteLeitura = visualizar
            };

            if (id.HasValue)
            {
                using (var servicos = CriarServicos())
                {
                    var financa = servicos.CriarFinanceService().ObterPorIdDoUsuario(id.Value, UsuarioLogadoId);
                    if (financa == null)
                    {
                        TempData["MensagemErro"] = "Finança não localizada.";
                        return RedirectToAction("Index", "Home");
                    }

                    model.Id = financa.Id;
                    model.Tipo = financa.Tipo;
                    model.Valor = financa.Valor;
                    model.Descricao = financa.Descricao;
                    model.DataInclusao = financa.DataInclusao;
                }
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Form(FinancaFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            using (var servicos = CriarServicos())
            {
                var resultado = servicos.CriarFinanceService().Salvar(model, UsuarioLogadoId);
                if (!resultado.Sucesso)
                {
                    ModelState.AddModelError("", resultado.Mensagem);
                    return View(model);
                }

                TempData["MensagemSucesso"] = resultado.Mensagem;
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Excluir(int id)
        {
            using (var servicos = CriarServicos())
            {
                var resultado = servicos.CriarFinanceService().Excluir(id, UsuarioLogadoId);
                return Json(new { sucesso = resultado.Sucesso, mensagem = resultado.Mensagem });
            }
        }
    }
}
