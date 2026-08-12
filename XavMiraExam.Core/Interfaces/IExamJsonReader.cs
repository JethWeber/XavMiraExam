using XavMiraExam.Core.Models;

namespace XavMiraExam.Core.Interfaces;

/// <summary>
/// Contrato para leitura de ficheiros de prova em formato JSON.
/// </summary>
public interface IExamJsonReader
{
    /// <summary>
    /// Lê e desserializa um ficheiro JSON de prova a partir de um caminho no disco.
    /// </summary>
    /// <param name="filePath">Caminho completo para o ficheiro .json.</param>
    /// <returns>O objeto <see cref="Exam"/> carregado.</returns>
    Exam ReadFromFile(string filePath);

    /// <summary>
    /// Lê e desserializa uma prova a partir de uma string JSON já em memória.
    /// </summary>
    Exam ReadFromString(string json);
}
