using XavMiraExam.Core.Interfaces;
using XavMiraExam.Core.Models;

namespace XavMiraExam.Infrastructure.Json;

/// <summary>
/// Validação básica de uma prova antes de esta poder ser iniciada.
/// Garante que o ficheiro JSON está estruturalmente correto e semanticamente coerente
/// (ver secção 5 — Regras da Prova, e secção 16 — Segurança Básica, do plano de projeto).
/// </summary>
public class ExamValidator : IExamValidator
{
    public ValidationResult Validate(Exam exam)
    {
        var result = new ValidationResult();

        if (exam is null)
        {
            result.AddError("A prova não foi carregada corretamente.");
            return result;
        }

        // --- Título ---
        if (string.IsNullOrWhiteSpace(exam.Titulo))
            result.AddError("A prova não tem título.");

        // --- Tempo por questão ---
        if (exam.TempoPorQuestao <= 0)
            result.AddError("O tempo por questão deve ser maior que zero.");

        // --- Nota máxima ---
        if (exam.NotaMaxima <= 0)
            result.AddError("A nota máxima deve ser maior que zero.");

        // --- Questões ---
        if (exam.Questoes is null || exam.Questoes.Count == 0)
        {
            result.AddError("A prova não contém nenhuma questão.");
            return result; // sem questões, não vale a pena continuar a validar
        }

        var idsVistos = new HashSet<int>();

        for (int i = 0; i < exam.Questoes.Count; i++)
        {
            var q = exam.Questoes[i];
            string prefixo = $"Questão {i + 1} (id={q.Id})";

            if (string.IsNullOrWhiteSpace(q.Pergunta))
                result.AddError($"{prefixo}: enunciado vazio.");

            if (!idsVistos.Add(q.Id))
                result.AddError($"{prefixo}: id duplicado.");

            if (q.Alternativas is null || q.Alternativas.Count < 2)
            {
                result.AddError($"{prefixo}: deve ter pelo menos 2 alternativas.");
                continue; // sem alternativas suficientes, não valida o índice "correta"
            }

            if (q.Alternativas.Any(string.IsNullOrWhiteSpace))
                result.AddError($"{prefixo}: existem alternativas vazias.");

            if (q.Correta < 0 || q.Correta >= q.Alternativas.Count)
                result.AddError($"{prefixo}: índice da resposta correta ({q.Correta}) fora do intervalo de alternativas.");
        }

        return result;
    }
}
