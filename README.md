# XavMira Exam System — V0.1

Aplicação desktop offline para realização de provas informatizadas no
Centro de Formação XavMira.

Stack: **.NET 10** · **Avalonia UI 11** · **MVVM** (CommunityToolkit.Mvvm) · **SQLite** (fases seguintes) · **JSON**

---

## Estado atual: FASE 2 — EXECUÇÃO DA PROVA ✅ concluída

### Fase 1 — Fundação ✅

- [x] Solução .NET 10 criada (`XavMiraExam.slnx`).
- [x] Projeto Avalonia 11 configurado (`XavMiraExam.Desktop`).
- [x] Modelos criados (`Exam`, `Question`, `Student`, `Answer`, `ExamResult`).
- [x] Leitura do JSON implementada (`ExamJsonReader`, `System.Text.Json`, sem dependências externas).
- [x] Validação básica implementada (`ExamValidator`).

**Resultado, confirmado por execução real (`XavMiraExam.Fase1Console`):**
> O sistema consegue carregar uma prova (e rejeitar provas inválidas).

### Fase 2 — Execução da Prova ✅

- [x] Ecrã inicial (`HomeView`) com escolha entre Modo Professor e Modo Aluno.
- [x] Modo Professor: importar ficheiro JSON via diálogo (`FilePickerService`),
      validar, configurar o tempo por questão e iniciar a sessão.
- [x] Ecrã de identificação do aluno (`StudentLoginView`), com lookup no
      ficheiro `Students/students.json` (`StudentJsonReader`).
- [x] Ecrã da questão (`ExamView`): uma questão de cada vez, alternativas,
      cronómetro visível, avanço automático.
- [x] `ExamSessionService`: máquina de estados que impõe as regras da secção 5
      do plano — uma resposta por questão, impossível voltar atrás, tempo
      esgotado = errada, término automático na última questão.
- [x] Ecrã de conclusão (`ExamCompleteView`) com contagem de respondidas/não
      respondidas (a correção/nota fica para a Fase 3).

**Resultado, confirmado por execução real (`XavMiraExam.Fase2Console`):**
> Um aluno consegue realizar uma prova completa.

Este console simula, com o `ExamSessionService` real (não um mock):
1. Identificação de um aluno pelo código (incluindo minúsculas) e rejeição de
   um código inexistente.
2. Resposta rápida a uma questão (tempo usado ≈ 0s).
3. Tempo esgotado numa questão (o teste espera mesmo os segundos passarem, e
   confirma que a resposta fica marcada como não respondida/errada e que o
   tempo é corretamente limitado ao `TempoPorQuestao`).
4. Avanço automático até à última questão e conclusão da sessão
   (`IsExamComplete = true`).

```
Resumo da sessão:
   Aluno: Maria Silva (A002)
   Prova: Avaliação de Informática
   Respondidas: 4 | Não respondidas: 1 | Corretas: 4
```

---

## Estrutura da solução

```
XavMiraExam/
├── XavMiraExam.Core/               # Modelos, interfaces e serviços de domínio
│   ├── Models/                     # Exam, Question, Student, Answer, ExamResult
│   ├── Services/                   # ExamService, ExamSessionService
│   └── Interfaces/                 # IExamJsonReader, IExamValidator, IStudentService
│
├── XavMiraExam.Infrastructure/     # Implementações concretas
│   └── Json/                       # ExamJsonReader, ExamValidator, StudentJsonReader
│       (Database/ e Reports/ chegam nas fases 3-4)
│
├── XavMiraExam.Desktop/            # Interface gráfica Avalonia 11 (MVVM)
│   ├── Views/                      # HomeView, ProfessorView, StudentLoginView,
│   │                                 ExamView, ExamCompleteView, ResultView (stub, Fase 4)
│   ├── ViewModels/                 # + INavigationHost (navegação entre ecrãs)
│   ├── Services/                   # ProjectPaths, IFilePickerService/FilePickerService
│   └── Assets/
│
├── XavMiraExam.Fase1Console/       # Consola de verificação da Fase 1 (não faz parte do produto final)
├── XavMiraExam.Fase2Console/       # Consola de verificação da Fase 2 (idem)
│
├── Exams/                          # Provas de exemplo (JSON)
│   ├── Informatica.json            # prova válida, usada nos testes
│   └── ProvaInvalida.json          # prova inválida, para testar a validação
│
├── Students/                       # Lista de alunos (JSON), preparada pelo professor
│   └── students.json                # 10 alunos fictícios, para os testes da secção 19
│
├── Results/                        # Onde os relatórios em PDF serão guardados (Fase 4)
└── Data/                           # Base de dados SQLite (Fase 3+)
```

---

## ⚠️ Nota importante sobre o ambiente onde isto foi gerado/validado

Este projeto foi gerado/validado num ambiente sandbox sem acesso a `nuget.org`.
Por isso:

- `XavMiraExam.Core`, `XavMiraExam.Infrastructure`, `XavMiraExam.Fase1Console`
  e `XavMiraExam.Fase2Console` **foram compilados e executados com sucesso**
  — só usam a biblioteca padrão do .NET (`System.Text.Json`), sem pacotes
  externos.
- `XavMiraExam.Desktop` **não pôde ser restaurado nem compilado** aqui,
  porque depende de pacotes do Avalonia (`Avalonia`, `Avalonia.Desktop`,
  `Avalonia.Themes.Fluent`, `CommunityToolkit.Mvvm`) que só existem no
  nuget.org (erro `NU1301: 403 Forbidden` neste sandbox). O código foi
  revisto manualmente (bindings XAML ↔ propriedades das ViewModels,
  assinaturas de construtores entre `MainViewModel` e cada ecrã) e está
  consistente — falta apenas correr `dotnet restore` numa máquina com
  internet normal para confirmar a compilação da UI.

### Como abrir e compilar na sua máquina

```bash
# 1. Extrair o projeto e entrar na pasta
cd XavMiraExam

# 2. Restaurar todos os pacotes (precisa de internet)
dotnet restore

# 3. Compilar tudo
dotnet build

# 4. Correr a aplicação desktop
dotnet run --project XavMiraExam.Desktop

# (opcional) Correr as verificações por consola
dotnet run --project XavMiraExam.Fase1Console
dotnet run --project XavMiraExam.Fase2Console
```

Fluxo para testar a aplicação: **Modo Professor** → "Importar ficheiro JSON"
→ escolher `Exams/Informatica.json` → "Iniciar sessão" → **Modo Aluno** →
identificar-se com um código de `Students/students.json` (ex: `A001`) →
"Iniciar prova" → responder às questões (ou deixar o tempo esgotar) → ecrã
de conclusão.

---

## Próximas fases

| Fase | Objetivo |
|------|----------|
| 3 — Correção | Avaliar respostas, contabilizar acertos/erros/não respondidas, calcular percentagem e nota |
| 4 — Resultado | Tela de resultado, relatório, geração de PDF, guardar resultado localmente |
| 5 — Testes | Bateria de testes end-to-end antes da prova de sexta-feira |

Ver `Plano de Projeto — MVP V0.1` para o detalhe completo de cada fase.
