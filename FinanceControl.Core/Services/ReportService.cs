using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using FinanceControl.Core.DTOs;
using FinanceControl.Core.Enums;
using FinanceControl.Core.Interfaces.IRepositories;
using FinanceControl.Core.Interfaces.IServices;
using FinanceControl.Core.Util;

namespace FinanceControl.Core.Services
{
    public class ReportService : IReportService
    {
        private readonly IFinanceRepository _financeRepository;
        private static readonly CultureInfo CulturaBrasil = new CultureInfo("pt-BR");

        public ReportService(IFinanceRepository financeRepository)
        {
            _financeRepository = financeRepository;
        }

        public RelatorioDto GerarMensal(int userId, int mes, int ano)
        {
            var inicio = new DateTime(ano, mes, 1);
            var fim = inicio.AddMonths(1).AddDays(-1);
            var financas = _financeRepository.ListarPorPeriodo(userId, inicio, fim);
            var relatorio = CriarRelatorio("Relatório mensal", inicio.ToString("MMMM/yyyy", CulturaBrasil));

            for (var data = inicio; data <= fim; data = data.AddDays(1))
                AdicionarItem(relatorio, data, data.ToString("dd/MM/yyyy"), financas.Where(x => x.DataInclusao.Date == data.Date));

            PreencherTotais(relatorio);
            return relatorio;
        }

        public RelatorioDto GerarPorPeriodo(int userId, DateTime dataInicial, DateTime dataFinal)
        {
            var inicio = dataInicial.Date;
            var fim = dataFinal.Date;
            var financas = _financeRepository.ListarPorPeriodo(userId, inicio, fim);
            var relatorio = CriarRelatorio("Relatório por período", inicio.ToString("dd/MM/yyyy") + " a " + fim.ToString("dd/MM/yyyy"));

            for (var data = inicio; data <= fim; data = data.AddDays(1))
                AdicionarItem(relatorio, data, data.ToString("dd/MM/yyyy"), financas.Where(x => x.DataInclusao.Date == data.Date));

            PreencherTotais(relatorio);
            return relatorio;
        }

        public byte[] GerarXlsx(RelatorioDto relatorio)
        {
            using (var memoria = new MemoryStream())
            {
                using (var pacote = Package.Open(memoria, FileMode.Create, FileAccess.ReadWrite))
                {
                    var workbook = CriarParte(pacote, "/xl/workbook.xml", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml", Workbook());
                    CriarParte(pacote, "/xl/styles.xml", "application/vnd.openxmlformats-officedocument.spreadsheetml.styles+xml", Estilos());
                    CriarParte(pacote, "/xl/worksheets/sheet1.xml", "application/vnd.openxmlformats-officedocument.spreadsheetml.worksheet+xml", Planilha(relatorio));

                    pacote.CreateRelationship(
                        new Uri("xl/workbook.xml", UriKind.Relative),
                        TargetMode.Internal,
                        "http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument",
                        "rId1");

                    workbook.CreateRelationship(
                        new Uri("worksheets/sheet1.xml", UriKind.Relative),
                        TargetMode.Internal,
                        "http://schemas.openxmlformats.org/officeDocument/2006/relationships/worksheet",
                        "rId1");

                    workbook.CreateRelationship(
                        new Uri("styles.xml", UriKind.Relative),
                        TargetMode.Internal,
                        "http://schemas.openxmlformats.org/officeDocument/2006/relationships/styles",
                        "rId2");
                }

                return memoria.ToArray();
            }
        }

        public byte[] GerarPdf(RelatorioDto relatorio)
        {
            var linhas = new List<string>
            {
                relatorio.Titulo,
                "Período: " + relatorio.Periodo,
                "Referência | Gastos | Ganhos | Resultado | Valor"
            };

            foreach (var item in relatorio.Itens)
                linhas.Add(string.Format("{0} | {1} | {2} | {3} | {4}",
                    item.Rotulo,
                    item.Gastos.ToMoedaBr(),
                    item.Ganhos.ToMoedaBr(),
                    item.SinalResultado,
                    item.Resultado.ToMoedaBr()));

            linhas.Add(string.Format("TOTAL | {0} | {1} | {2} | {3}",
                relatorio.TotalGastos.ToMoedaBr(),
                relatorio.TotalGanhos.ToMoedaBr(),
                relatorio.TotalResultado >= 0 ? "+" : "-",
                relatorio.TotalResultado.ToMoedaBr()));

            return CriarPdfSimples(linhas);
        }

        private static RelatorioDto CriarRelatorio(string titulo, string periodo)
        {
            return new RelatorioDto { Titulo = titulo, Periodo = periodo };
        }

        private static void AdicionarItem(RelatorioDto relatorio, DateTime data, string rotulo, IEnumerable<Entities.Finance> financas)
        {
            var gastos = financas.Where(x => x.Tipo == FinanceType.Gasto).Sum(x => x.Valor);
            var ganhos = financas.Where(x => x.Tipo == FinanceType.Ganho).Sum(x => x.Valor);
            var resultado = ganhos - gastos;

            relatorio.Itens.Add(new ItemRelatorioDto
            {
                Data = data,
                Rotulo = CulturaBrasil.TextInfo.ToTitleCase(rotulo),
                Gastos = gastos,
                Ganhos = ganhos,
                Resultado = resultado,
                SinalResultado = resultado >= 0 ? "+" : "-"
            });
        }

        private static void PreencherTotais(RelatorioDto relatorio)
        {
            relatorio.TotalGastos = relatorio.Itens.Sum(x => x.Gastos);
            relatorio.TotalGanhos = relatorio.Itens.Sum(x => x.Ganhos);
            relatorio.TotalResultado = relatorio.TotalGanhos - relatorio.TotalGastos;
        }

        private static PackagePart CriarParte(Package pacote, string caminho, string tipo, string conteudo)
        {
            var parte = pacote.CreatePart(new Uri(caminho, UriKind.Relative), tipo, CompressionOption.Maximum);
            using (var stream = parte.GetStream())
            using (var writer = new StreamWriter(stream, new UTF8Encoding(false)))
            {
                writer.Write(conteudo);
            }

            return parte;
        }

        private static string Workbook()
        {
            return "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                   "<workbook xmlns=\"http://schemas.openxmlformats.org/spreadsheetml/2006/main\" xmlns:r=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships\">" +
                   "<sheets><sheet name=\"Relatório\" sheetId=\"1\" r:id=\"rId1\"/></sheets></workbook>";
        }

        private static string Estilos()
        {
            return "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                   "<styleSheet xmlns=\"http://schemas.openxmlformats.org/spreadsheetml/2006/main\">" +
                   "<fonts count=\"2\"><font><sz val=\"11\"/><name val=\"Calibri\"/></font><font><b/><sz val=\"11\"/><name val=\"Calibri\"/></font></fonts>" +
                   "<fills count=\"1\"><fill><patternFill patternType=\"none\"/></fill></fills>" +
                   "<borders count=\"1\"><border><left/><right/><top/><bottom/><diagonal/></border></borders>" +
                   "<cellStyleXfs count=\"1\"><xf numFmtId=\"0\" fontId=\"0\" fillId=\"0\" borderId=\"0\"/></cellStyleXfs>" +
                   "<cellXfs count=\"2\"><xf numFmtId=\"0\" fontId=\"0\" fillId=\"0\" borderId=\"0\" xfId=\"0\"/><xf numFmtId=\"0\" fontId=\"1\" fillId=\"0\" borderId=\"0\" xfId=\"0\"/></cellXfs>" +
                   "</styleSheet>";
        }

        private static string Planilha(RelatorioDto relatorio)
        {
            var sb = new StringBuilder();
            sb.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            sb.Append("<worksheet xmlns=\"http://schemas.openxmlformats.org/spreadsheetml/2006/main\"><sheetData>");
            LinhaTexto(sb, 1, 1, relatorio.Titulo, true);
            LinhaTexto(sb, 2, 1, "Período: " + relatorio.Periodo, false);
            LinhaCabecalho(sb, 4, new[] { "Referência", "Gastos", "Ganhos", "Resultado", "Valor do resultado" });

            var linha = 5;
            foreach (var item in relatorio.Itens)
                LinhaRelatorio(sb, linha++, item.Rotulo, item.Gastos, item.Ganhos, item.SinalResultado, item.Resultado);

            LinhaRelatorio(sb, linha, "TOTAL", relatorio.TotalGastos, relatorio.TotalGanhos, relatorio.TotalResultado >= 0 ? "+" : "-", relatorio.TotalResultado, true);
            sb.Append("</sheetData></worksheet>");
            return sb.ToString();
        }

        private static void LinhaCabecalho(StringBuilder sb, int linha, string[] valores)
        {
            sb.Append("<row r=\"" + linha + "\">");
            for (var i = 0; i < valores.Length; i++)
                CelulaTexto(sb, linha, i + 1, valores[i], true);
            sb.Append("</row>");
        }

        private static void LinhaTexto(StringBuilder sb, int linha, int coluna, string valor, bool negrito)
        {
            sb.Append("<row r=\"" + linha + "\">");
            CelulaTexto(sb, linha, coluna, valor, negrito);
            sb.Append("</row>");
        }

        private static void LinhaRelatorio(StringBuilder sb, int linha, string rotulo, decimal gastos, decimal ganhos, string sinal, decimal resultado, bool negrito = false)
        {
            sb.Append("<row r=\"" + linha + "\">");
            CelulaTexto(sb, linha, 1, rotulo, negrito);
            CelulaNumero(sb, linha, 2, gastos, negrito);
            CelulaNumero(sb, linha, 3, ganhos, negrito);
            CelulaTexto(sb, linha, 4, sinal, negrito);
            CelulaNumero(sb, linha, 5, resultado, negrito);
            sb.Append("</row>");
        }

        private static void CelulaTexto(StringBuilder sb, int linha, int coluna, string valor, bool negrito)
        {
            sb.AppendFormat("<c r=\"{0}{1}\" t=\"inlineStr\" s=\"{2}\"><is><t>{3}</t></is></c>", Coluna(coluna), linha, negrito ? 1 : 0, EscaparXml(valor));
        }

        private static void CelulaNumero(StringBuilder sb, int linha, int coluna, decimal valor, bool negrito)
        {
            sb.AppendFormat(CultureInfo.InvariantCulture, "<c r=\"{0}{1}\" s=\"{2}\"><v>{3}</v></c>", Coluna(coluna), linha, negrito ? 1 : 0, valor);
        }

        private static string Coluna(int indice)
        {
            return ((char)('A' + indice - 1)).ToString();
        }

        private static string EscaparXml(string valor)
        {
            return (valor ?? string.Empty).Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;");
        }

        private static byte[] CriarPdfSimples(IList<string> linhas)
        {
            var objetos = new List<string>();
            var paginas = PaginarLinhasPdf(linhas);
            var fontObj = 3 + (paginas.Count * 2);
            var kids = new StringBuilder();

            objetos.Add("1 0 obj << /Type /Catalog /Pages 2 0 R >> endobj");
            objetos.Add("2 0 obj << /Type /Pages /Kids [");

            for (var i = 0; i < paginas.Count; i++)
            {
                var pageObj = 3 + (i * 2);
                var contentObj = pageObj + 1;
                kids.Append(pageObj + " 0 R ");
                objetos.Add(pageObj + " 0 obj << /Type /Page /Parent 2 0 R /MediaBox [0 0 595 842] /Resources << /Font << /F1 " + fontObj + " 0 R >> >> /Contents " + contentObj + " 0 R >> endobj");

                var conteudo = ConteudoPaginaPdf(paginas[i]);
                objetos.Add(contentObj + " 0 obj << /Length " + Encoding.ASCII.GetByteCount(conteudo) + " >> stream\n" + conteudo + "\nendstream endobj");
            }

            objetos[1] = "2 0 obj << /Type /Pages /Kids [" + kids + "] /Count " + paginas.Count + " >> endobj";
            objetos.Add(fontObj + " 0 obj << /Type /Font /Subtype /Type1 /BaseFont /Helvetica >> endobj");

            using (var memoria = new MemoryStream())
            using (var writer = new StreamWriter(memoria, Encoding.ASCII))
            {
                writer.Write("%PDF-1.4\n");
                var offsets = new List<long> { 0 };
                foreach (var objeto in objetos)
                {
                    writer.Flush();
                    offsets.Add(memoria.Position);
                    writer.Write(objeto + "\n");
                }

                writer.Flush();
                var inicioXref = memoria.Position;
                writer.Write("xref\n0 " + (objetos.Count + 1) + "\n");
                writer.Write("0000000000 65535 f \n");
                foreach (var offset in offsets.Skip(1))
                    writer.Write(offset.ToString("0000000000") + " 00000 n \n");
                writer.Write("trailer << /Size " + (objetos.Count + 1) + " /Root 1 0 R >>\nstartxref\n" + inicioXref + "\n%%EOF");
                writer.Flush();
                return memoria.ToArray();
            }
        }

        private static IList<IList<string>> PaginarLinhasPdf(IList<string> linhas)
        {
            const int linhasPorPagina = 55;
            var paginas = new List<IList<string>>();

            if (linhas == null || linhas.Count == 0)
            {
                paginas.Add(new List<string>());
                return paginas;
            }

            if (linhas.Count <= linhasPorPagina)
            {
                paginas.Add(linhas);
                return paginas;
            }

            var cabecalho = linhas.Take(3).ToList();
            var detalhes = linhas.Skip(3).ToList();
            var detalhesPorPagina = linhasPorPagina - cabecalho.Count;

            for (var indice = 0; indice < detalhes.Count; indice += detalhesPorPagina)
            {
                var pagina = new List<string>();
                pagina.AddRange(cabecalho);
                pagina.AddRange(detalhes.Skip(indice).Take(detalhesPorPagina));
                paginas.Add(pagina);
            }

            return paginas;
        }

        private static string ConteudoPaginaPdf(IEnumerable<string> linhas)
        {
            var conteudo = new StringBuilder();
            conteudo.Append("BT /F1 10 Tf 40 790 Td ");

            foreach (var linha in linhas)
                conteudo.Append("(" + EscaparPdf(linha) + ") Tj 0 -14 Td ");

            conteudo.Append("ET");
            return conteudo.ToString();
        }

        private static string EscaparPdf(string texto)
        {
            return RemoverAcentos(texto ?? string.Empty).Replace("\\", "\\\\").Replace("(", "\\(").Replace(")", "\\)");
        }

        private static string RemoverAcentos(string texto)
        {
            var normalizado = texto.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();
            foreach (var caractere in normalizado)
                if (CharUnicodeInfo.GetUnicodeCategory(caractere) != UnicodeCategory.NonSpacingMark && caractere < 128)
                    sb.Append(caractere);

            return sb.ToString();
        }
    }
}
