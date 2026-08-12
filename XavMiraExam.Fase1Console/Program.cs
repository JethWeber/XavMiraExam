// ============================================================================
// XavMira Exam System — Verificação da FASE 1 (Fundação)
//
// Este pequeno programa de consola não faz parte da aplicação final (que será
// a interface gráfica em Avalonia, construída nas fases seguintes). Serve
// apenas para provar, de forma concreta, o resultado exigido pela Fase 1:
//
//      "O sistema consegue carregar uma prova."
//
// Carrega a prova válida (Informatica.json) e a prova inválida
// (ProvaInvalida.json) para demonstrar tanto o caminho de sucesso como a
// validação de erros.
// ============================================================================

using XavMiraExam.Core.Interfaces;
using XavMiraExam.Core.Models;
using XavMiraExam.Core.Services;
using XavMiraExam.Infrastructure.Json;

IExamJsonReader reader = new ExamJsonReader();
IExamValidator validator = new ExamValidator();
var examService = new ExamService(reader, validator);

string baseDir = AppContext.BaseDirectory;
string examsDir = FindExamsDirectory(baseDir);

Console.WriteLine("============================================================");
Console.WriteLine(" XAVMIRA EXAM SYSTEM — Verificação da Fase 1 (Fundação)");
Console.WriteLine("============================================================\n");

// ---------------------------------------------------------------------
// 1) Caso de sucesso: carregar uma prova válida
// ---------------------------------------------------------------------
Console.WriteLine(">> Teste 1: carregar prova válida (Informatica.json)\n");
string caminhoValido = Path.Combine(examsDir, "Informatica.json");

try
{
    Exam exam = examService.LoadAndValidate(caminhoValido);

    Console.WriteLine($"   Prova carregada com sucesso: \"{exam.Titulo}\"");
    Console.WriteLine($"   Tempo por questão: {exam.TempoPorQuestao}s");
    Console.WriteLine($"   Nota máxima: {exam.NotaMaxima}");
    Console.WriteLine($"   Número de questões: {exam.Questoes.Count}\n");

    Console.WriteLine("   Questões carregadas:");
    foreach (var q in exam.Questoes)
    {
        Console.WriteLine($"     [{q.Id}] {q.Pergunta}");
        for (int i = 0; i < q.Alternativas.Count; i++)
        {
            string marcador = i == q.Correta ? "*" : " ";
            Console.WriteLine($"          {marcador} {(char)('A' + i)}) {q.Alternativas[i]}");
        }
    }

    Console.WriteLine("\n   RESULTADO: SUCESSO — a prova válida foi carregada e validada.\n");
}
catch (ExamValidationException ex)
{
    Console.WriteLine("   RESULTADO: FALHOU (inesperado) — " + ex.Message + "\n");
}
catch (Exception ex)
{
    Console.WriteLine("   RESULTADO: ERRO INESPERADO — " + ex.Message + "\n");
}

Console.WriteLine("------------------------------------------------------------\n");

// ---------------------------------------------------------------------
// 2) Caso de erro: carregar uma prova inválida e ver os erros reportados
// ---------------------------------------------------------------------
Console.WriteLine(">> Teste 2: carregar prova inválida (ProvaInvalida.json)\n");
string caminhoInvalido = Path.Combine(examsDir, "ProvaInvalida.json");

try
{
    examService.LoadAndValidate(caminhoInvalido);
    Console.WriteLine("   RESULTADO: FALHOU (inesperado) — a prova inválida foi aceite.\n");
}
catch (ExamValidationException ex)
{
    Console.WriteLine("   A validação rejeitou a prova corretamente. Erros encontrados:");
    foreach (var erro in ex.Errors)
        Console.WriteLine($"     - {erro}");

    Console.WriteLine("\n   RESULTADO: SUCESSO — a prova inválida foi corretamente rejeitada.\n");
}

Console.WriteLine("============================================================");
Console.WriteLine(" FASE 1 — FUNDAÇÃO: CONCLUÍDA");
Console.WriteLine(" O sistema consegue carregar uma prova (e rejeitar provas inválidas).");
Console.WriteLine("============================================================");

// ---------------------------------------------------------------------
static string FindExamsDirectory(string startDir)
{
    var dir = new DirectoryInfo(startDir);
    while (dir is not null)
    {
        string candidate = Path.Combine(dir.FullName, "Exams");
        if (Directory.Exists(candidate))
            return candidate;
        dir = dir.Parent;
    }
    throw new DirectoryNotFoundException("Não foi possível localizar a pasta 'Exams' a partir de " + startDir);
}
