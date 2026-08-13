using System.Linq;
using XavMiraExam.Core.Models;

namespace XavMiraExam.Core.Services;

/// <summary>
/// Serviço responsável pela correção automática de uma prova já respondida
/// (Fase 3 do plano — Correção): conta acertos, erros, não respondidas,
/// calcula a percentagem de aproveitamento e a nota final.
/// </summary>
public class EvaluationService
{
    /// <summary>
    /// Avalia as respostas de um aluno a uma prova e produz o <see cref="ExamResult"/> final.
    /// </summary>
    /// <remarks>
    /// Regra do plano (secção 5): questões sem resposta são consideradas erradas.
    /// Por isso <c>Erros</c> = TotalQuestoes - Acertos (inclui as não respondidas),
    /// e <c>NaoRespondidas</c> é reportado à parte apenas como informação adicional.
    /// </remarks>
    public ExamResult Evaluate(Exam exam, Student student, IReadOnlyList<Answer> answers)
    {
        ArgumentNullException.ThrowIfNull(exam);
        ArgumentNullException.ThrowIfNull(student);
        ArgumentNullException.ThrowIfNull(answers);

        int total = exam.Questoes.Count;
        int acertos = answers.Count(a => a.Correta);
        int naoRespondidas = answers.Count(a => !a.FoiRespondida);
        int erros = total - acertos;

        double percentagem = total == 0 ? 0 : (double)acertos / total * 100.0;
        double nota = total == 0 ? 0 : (double)acertos / total * exam.NotaMaxima;

        var respostasErradas = answers
            .Where(a => !a.Correta)
            .OrderBy(a => a.QuestionId)
            .ToList();

        return new ExamResult
        {
            StudentId = student.Id,
            ExamId = exam.Id,
            TotalQuestoes = total,
            Acertos = acertos,
            Erros = erros,
            NaoRespondidas = naoRespondidas,
            Percentagem = Math.Round(percentagem, 2),
            Nota = Math.Round(nota, 2),
            DataRealizacao = DateTime.Now,
            RespostasErradas = respostasErradas,
        };
    }
}
