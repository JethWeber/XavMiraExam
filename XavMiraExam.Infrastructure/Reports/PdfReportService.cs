using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using XavMiraExam.Core.Interfaces;
using XavMiraExam.Core.Models;

namespace XavMiraExam.Infrastructure.Reports;

/// <summary>
/// Gera um relatório PDF simples (PDF 1.4, Helvetica, WinAnsi) sem dependências
/// externas — alinhado com a prioridade Estabilidade > Complexidade.
/// </summary>
public class PdfReportService : IPdfReportService
{
    public string GenerateAndSave(
        Exam exam,
        Student student,
        ExamResult result,
        string resultsRootFolder)
    {
        ArgumentNullException.ThrowIfNull(exam);
        ArgumentNullException.ThrowIfNull(student);
        ArgumentNullException.ThrowIfNull(result);
        ArgumentException.ThrowIfNullOrWhiteSpace(resultsRootFolder);

        string studentFolderName = SanitizeFileName(student.Nome);
        string studentDir = Path.Combine(resultsRootFolder, studentFolderName);
        Directory.CreateDirectory(studentDir);

        string examFilePart = SanitizeFileName(exam.Titulo);
        if (string.IsNullOrWhiteSpace(examFilePart))
            examFilePart = "Prova";

        string fileName = $"Resultado_{examFilePart}.pdf";
        string fullPath = Path.Combine(studentDir, fileName);

        var lines = BuildReportLines(exam, student, result);
        byte[] pdfBytes = MinimalPdfWriter.Write(lines);
        File.WriteAllBytes(fullPath, pdfBytes);

        return fullPath;
    }

    private static List<string> BuildReportLines(Exam exam, Student student, ExamResult result)
    {
        var lines = new List<string>
        {
            "XAVMIRA EXAM SYSTEM",
            "RELATORIO DE RESULTADO",
            "",
            $"Aluno: {student.Nome}",
            $"Codigo: {student.Codigo}",
            $"Turma: {student.Turma}",
            $"Prova: {exam.Titulo}",
            $"Data: {result.DataRealizacao:dd/MM/yyyy HH:mm}",
            "",
            "------------------------------------------------",
            $"Total de questoes:     {result.TotalQuestoes}",
            $"Respondidas:           {result.TotalQuestoes - result.NaoRespondidas}",
            $"Nao respondidas:       {result.NaoRespondidas}",
            $"Acertos:               {result.Acertos}",
            $"Erros:                 {result.Erros}",
            $"Aproveitamento:        {result.Percentagem.ToString("0.##", CultureInfo.InvariantCulture)}%",
            $"Nota final:            {result.Nota.ToString("0.##", CultureInfo.InvariantCulture)} / {exam.NotaMaxima.ToString("0.##", CultureInfo.InvariantCulture)}",
            "------------------------------------------------",
            "",
        };

        if (result.RespostasErradas.Count == 0)
        {
            lines.Add("Nenhuma questao errada.");
        }
        else
        {
            lines.Add("QUESTOES ERRADAS");
            lines.Add("");

            foreach (Answer wrong in result.RespostasErradas.OrderBy(a => a.QuestionId))
            {
                Question? q = exam.Questoes.FirstOrDefault(x => x.Id == wrong.QuestionId);
                if (q is null)
                    continue;

                string respostaAluno =
                    wrong.AlternativaSelecionada.HasValue &&
                    wrong.AlternativaSelecionada.Value >= 0 &&
                    wrong.AlternativaSelecionada.Value < q.Alternativas.Count
                        ? q.Alternativas[wrong.AlternativaSelecionada.Value]
                        : "(nao respondida)";

                string respostaCorreta =
                    q.Correta >= 0 && q.Correta < q.Alternativas.Count
                        ? q.Alternativas[q.Correta]
                        : "?";

                lines.Add($"QUESTAO {q.Id:D2}");
                lines.Add($"Pergunta: {q.Pergunta}");
                lines.Add($"Resposta do aluno: {respostaAluno}");
                lines.Add($"Resposta correta: {respostaCorreta}");
                lines.Add("");
            }
        }

        lines.Add("------------------------------------------------");
        lines.Add("Centro de Formacao XavMira — V0.1");
        lines.Add("Documento gerado automaticamente.");

        return lines;
    }

    /// <summary>
    /// Converte o nome do aluno/prova num identificador seguro para pasta/ficheiro.
    /// Ex.: "Joaquim Santareno" → "Joaquim_Santareno"
    /// </summary>
    internal static string SanitizeFileName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return "Desconhecido";

        string normalized = name.Trim()
            .Normalize(NormalizationForm.FormD);

        var sb = new StringBuilder(normalized.Length);
        foreach (char c in normalized)
        {
            UnicodeCategory cat = CharUnicodeInfo.GetUnicodeCategory(c);
            if (cat == UnicodeCategory.NonSpacingMark)
                continue;

            if (char.IsLetterOrDigit(c))
                sb.Append(c);
            else if (char.IsWhiteSpace(c) || c is '-' or '_')
                sb.Append('_');
        }

        string result = Regex.Replace(sb.ToString(), "_+", "_").Trim('_');
        return string.IsNullOrEmpty(result) ? "Desconhecido" : result;
    }
}

/// <summary>
/// Escritor PDF mínimo (PDF 1.4) com Helvetica e codificação WinAnsi.
/// Suficiente para o relatório textual offline da V0.1.
/// </summary>
internal static class MinimalPdfWriter
{
    private const int PageWidth = 595;   // A4
    private const int PageHeight = 842;
    private const int MarginLeft = 50;
    private const int MarginTop = 50;
    private const int LineHeight = 14;
    private const int FontSize = 11;
    private const int LinesPerPage = (PageHeight - MarginTop * 2) / LineHeight;

    public static byte[] Write(IReadOnlyList<string> lines)
    {
        // Particiona linhas por página
        var pages = new List<List<string>>();
        for (int i = 0; i < lines.Count; i += LinesPerPage)
        {
            pages.Add(lines.Skip(i).Take(LinesPerPage).ToList());
        }

        if (pages.Count == 0)
            pages.Add(new List<string> { "(relatorio vazio)" });

        var objects = new List<byte[]>();
        // Object 1: Catalog
        objects.Add(Encoding.ASCII.GetBytes("<< /Type /Catalog /Pages 2 0 R >>"));

        // Object 2: Pages (Kids preenchidos depois — usamos placeholders)
        // Object 3..: page objects, content streams, font

        int firstPageObj = 4;

        // Font object
        var fontObj = Encoding.ASCII.GetBytes(
            "<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica /Encoding /WinAnsiEncoding >>");

        var pageContentObjs = new List<(int pageObj, int contentObj, byte[] content)>();
        int nextObj = firstPageObj;

        for (int p = 0; p < pages.Count; p++)
        {
            int pageObj = nextObj++;
            int contentObj = nextObj++;
            byte[] stream = BuildPageStream(pages[p]);
            pageContentObjs.Add((pageObj, contentObj, stream));
        }

        // Rebuild object list with correct numbering
        // 1 Catalog, 2 Pages, 3 Font, then pairs of Page+Content
        var finalObjects = new List<byte[]>
        {
            Encoding.ASCII.GetBytes("<< /Type /Catalog /Pages 2 0 R >>"),
        };

        // Pages dictionary
        var kids = new StringBuilder("[ ");
        foreach (var (pageObj, _, _) in pageContentObjs)
            kids.Append($"{pageObj} 0 R ");
        kids.Append(']');

        finalObjects.Add(Encoding.ASCII.GetBytes(
            $"<< /Type /Pages /Kids {kids} /Count {pageContentObjs.Count} >>"));

        finalObjects.Add(fontObj); // obj 3

        // Ensure object numbers match: page objects start at 4
        foreach (var (pageObj, contentObj, stream) in pageContentObjs)
        {
            // Page
            finalObjects.Add(Encoding.ASCII.GetBytes(
                $"<< /Type /Page /Parent 2 0 R /MediaBox [0 0 {PageWidth} {PageHeight}] " +
                $"/Contents {contentObj} 0 R /Resources << /Font << /F1 3 0 R >> >> >>"));

            // Content stream
            var streamHeader = Encoding.ASCII.GetBytes(
                $"<< /Length {stream.Length} >>\nstream\n");
            var streamFooter = Encoding.ASCII.GetBytes("\nendstream");
            var fullStream = new byte[streamHeader.Length + stream.Length + streamFooter.Length];
            Buffer.BlockCopy(streamHeader, 0, fullStream, 0, streamHeader.Length);
            Buffer.BlockCopy(stream, 0, fullStream, streamHeader.Length, stream.Length);
            Buffer.BlockCopy(streamFooter, 0, fullStream, streamHeader.Length + stream.Length, streamFooter.Length);
            finalObjects.Add(fullStream);
        }

        return AssemblePdf(finalObjects);
    }

    private static byte[] BuildPageStream(IReadOnlyList<string> pageLines)
    {
        var sb = new StringBuilder();
        sb.AppendLine("BT");
        sb.AppendLine($"/F1 {FontSize} Tf");

        int y = PageHeight - MarginTop;
        foreach (string raw in pageLines)
        {
            string line = EscapePdfString(ToWinAnsi(raw));
            sb.AppendLine($"1 0 0 1 {MarginLeft} {y} Tm");
            sb.AppendLine($"({line}) Tj");
            y -= LineHeight;
        }

        sb.AppendLine("ET");
        return Encoding.GetEncoding("ISO-8859-1").GetBytes(sb.ToString());
    }

    private static string EscapePdfString(string s)
    {
        return s
            .Replace("\\", "\\\\")
            .Replace("(", "\\(")
            .Replace(")", "\\)");
    }

    /// <summary>
    /// Converte texto Unicode para WinAnsi (ISO-8859-1 aproximado),
    /// substituindo caracteres fora do intervalo por '?' ou equivalentes ASCII.
    /// </summary>
    private static string ToWinAnsi(string text)
    {
        var sb = new StringBuilder(text.Length);
        foreach (char c in text)
        {
            if (c <= 0xFF)
                sb.Append(c);
            else
            {
                // Aproximações comuns em português
                sb.Append(c switch
                {
                    '–' or '—' => '-',
                    '“' or '”' or '„' => '"',
                    '‘' or '’' => '\'',
                    '…' => "...",
                    _ => '?',
                });
            }
        }
        return sb.ToString();
    }

    private static byte[] AssemblePdf(List<byte[]> objects)
    {
        using var ms = new MemoryStream();
        void WriteAscii(string s)
        {
            var b = Encoding.ASCII.GetBytes(s);
            ms.Write(b, 0, b.Length);
        }

        WriteAscii("%PDF-1.4\n");

        var offsets = new List<long> { 0 }; // index 0 unused
        for (int i = 0; i < objects.Count; i++)
        {
            offsets.Add(ms.Position);
            WriteAscii($"{i + 1} 0 obj\n");
            ms.Write(objects[i], 0, objects[i].Length);
            WriteAscii("\nendobj\n");
        }

        long xrefPos = ms.Position;
        WriteAscii($"xref\n0 {objects.Count + 1}\n");
        WriteAscii("0000000000 65535 f \n");
        for (int i = 1; i <= objects.Count; i++)
            WriteAscii($"{offsets[i]:D10} 00000 n \n");

        WriteAscii("trailer\n");
        WriteAscii($"<< /Size {objects.Count + 1} /Root 1 0 R >>\n");
        WriteAscii("startxref\n");
        WriteAscii($"{xrefPos}\n");
        WriteAscii("%%EOF\n");

        return ms.ToArray();
    }
}
