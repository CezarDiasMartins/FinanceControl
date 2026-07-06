using System.Web.Mvc;
using FinanceControl.Core.DTOs;
using FinanceControl.Core.Enums;
using FinanceControl.Web.Libs;

namespace FinanceControl.Web.Controllers
{
    public class HomeController : FinanceControlControllerBase
    {
        public ActionResult Index(string termo, FinanceType? tipo, System.DateTime? dataInicial, System.DateTime? dataFinal, int pagina = 1)
        {
            using (var servicos = CriarServicos())
            {
                var filtro = new FiltroFinancaDto
                {
                    UserId = UsuarioLogadoId,
                    DataInicial = dataInicial,
                    DataFinal = dataFinal,
                    Tipo = tipo,
                    Termo = termo,
                    Pagina = pagina,
                    RegistrosPorPagina = 10
                };

                return View(servicos.CriarFinanceService().ListarPorUsuario(filtro));
            }
        }

        public ActionResult Grid(string termo, FinanceType? tipo, System.DateTime? dataInicial, System.DateTime? dataFinal, int pagina = 1)
        {
            using (var servicos = CriarServicos())
            {
                var filtro = new FiltroFinancaDto
                {
                    UserId = UsuarioLogadoId,
                    DataInicial = dataInicial,
                    DataFinal = dataFinal,
                    Tipo = tipo,
                    Termo = termo,
                    Pagina = pagina,
                    RegistrosPorPagina = 10
                };

                return PartialView("~/Views/Shared/Components/_ListagemAtualizada.cshtml", servicos.CriarFinanceService().ListarPorUsuario(filtro));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Relatorio(string tipoRelatorio, int? mes, int? ano, System.DateTime? dataInicial, System.DateTime? dataFinal, string formato)
        {
            tipoRelatorio = (tipoRelatorio ?? string.Empty).ToLowerInvariant();
            formato = (formato ?? string.Empty).ToLowerInvariant();

            if (formato != "xlsx" && formato != "pdf")
            {
                TempData["MensagemErro"] = "Informe um formato válido para gerar o relatório.";
                return RedirectToAction("Index", "Home");
            }

            if (tipoRelatorio == "mensal")
            {
                if (!mes.HasValue || mes.Value < 1 || mes.Value > 12)
                {
                    TempData["MensagemErro"] = "Informe o mês para gerar o relatório mensal.";
                    return RedirectToAction("Index", "Home");
                }

                if (!ano.HasValue || ano.Value < 1900 || ano.Value > 9999)
                {
                    TempData["MensagemErro"] = "Informe um ano válido com 4 números.";
                    return RedirectToAction("Index", "Home");
                }
            }
            else if (tipoRelatorio == "periodo")
            {
                if (!dataInicial.HasValue || !dataFinal.HasValue)
                {
                    TempData["MensagemErro"] = "Informe a data inicial e a data final para gerar o relatório.";
                    return RedirectToAction("Index", "Home");
                }

                if (dataInicial.Value.Date > dataFinal.Value.Date)
                {
                    TempData["MensagemErro"] = "A data inicial não pode ser maior que a data final.";
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                TempData["MensagemErro"] = "Informe um tipo válido para gerar o relatório.";
                return RedirectToAction("Index", "Home");
            }

            using (var servicos = CriarServicos())
            {
                var reportService = servicos.CriarReportService();
                var relatorio = tipoRelatorio == "periodo"
                    ? reportService.GerarPorPeriodo(UsuarioLogadoId, dataInicial.Value, dataFinal.Value)
                    : reportService.GerarMensal(UsuarioLogadoId, mes.Value, ano.Value);

                if (formato == "pdf")
                    return File(reportService.GerarPdf(relatorio), "application/pdf", "relatorio-financecontrol.pdf");

                return File(reportService.GerarXlsx(relatorio), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "relatorio-financecontrol.xlsx");
            }
        }
    }
}
