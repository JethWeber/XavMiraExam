# XavMira Exam System — V0.1

Aplicação desktop offline para realização de provas informatizadas no
Centro de Formação XavMira.

Stack: **.NET 10** · **Avalonia UI 11** · **MVVM** (CommunityToolkit.Mvvm) · **SQLite** (fases seguintes) · **JSON**

---

## Estado atual: FASE 1 — FUNDAÇÃO ✅ concluída

De acordo com o plano de projeto (secção 17), a Fase 1 entrega:

- [x] Solução .NET 10 criada (`XavMiraExam.slnx`).
- [x] Projeto Avalonia 11 configurado (`XavMiraExam.Desktop`).
- [x] Modelos criados (`Exam`, `Question`, `Student`, `Answer`, `ExamResult`).
- [x] Leitura do JSON implementada (`ExamJsonReader`, `System.Text.Json`, sem dependências externas).
- [x] Validação básica implementada (`ExamValidator`).

**Resultado da fase, confirmado por execução real:**
> O sistema consegue carregar uma prova.

Isto foi verificado a compilar e correr o projeto `XavMiraExam.Fase1Console`,
que carrega `Exams/Informatica.json` (prova válida) e `Exams/ProvaInvalida.json`
(prova propositadamente inválida), mostrando que a leitura e a validação
funcionam nos dois sentidos — aceitar o que é válido e rejeitar o que não é.

---

## Estrutura da solução

```
XavMiraExam/
├── XavMiraExam.Core/              # Modelos, interfaces e serviços de domínio
│   ├── Models/                    # Exam, Question, Student, Answer, ExamResult
│   ├── Services/                  # ExamService (carregar + validar)
│   └── Interfaces/                # IExamJsonReader, IExamValidator
│
├── XavMiraExam.Infrastructure/    # Implementações concretas
│   └── Json/                      # ExamJsonReader, ExamValidator
│       (Database/ e Reports/ chegam nas fases 3-4)
│
├── XavMiraExam.Desktop/           # Interface gráfica Avalonia 11 (MVVM)
│   ├── Views/                     # MainWindow (funcional) + stubs das telas seguintes
│   ├── ViewModels/
│   └── Assets/
│
├── XavMiraExam.Fase1Console/      # Consola de verificação da Fase 1 (não faz parte do produto final)
│
├── Exams/                         # Provas de exemplo (JSON)
│   ├── Informatica.json           # prova válida, usada nos testes
│   └── ProvaInvalida.json         # prova inválida, para testar a validação
│
├── Results/                       # Onde os relatórios em PDF serão guardados (Fase 4)
└── Data/                          # Base de dados SQLite (Fase 3+)
```

---

## ⚠️ Nota importante sobre o ambiente onde isto foi gerado

Este projeto foi criado num ambiente sandbox sem acesso a `nuget.org`.
Por isso:

- `XavMiraExam.Core`, `XavMiraExam.Infrastructure` e `XavMiraExam.Fase1Console`
  **foram compilados e executados com sucesso** — só usam a biblioteca padrão
  do .NET (`System.Text.Json`), sem pacotes externos.
- `XavMiraExam.Desktop` **não pôde ser restaurado nem compilado** aqui, porque
  depende de pacotes do Avalonia (`Avalonia`, `Avalonia.Desktop`,
  `Avalonia.Themes.Fluent`, `CommunityToolkit.Mvvm`) que só existem no
  nuget.org. O código está escrito e a estrutura MVVM está correta — falta
  apenas correr `dotnet restore` numa máquina com internet normal.

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

# (opcional) Correr a verificação da Fase 1 pela consola
dotnet run --project XavMiraExam.Fase1Console
```

Ao correr `XavMiraExam.Desktop`, a janela principal mostra um botão
**"Verificar Fase 1"** que carrega `Exams/Informatica.json` através do mesmo
`ExamService` usado na consola — prova visual de que a UI já está ligada ao
Core e à Infrastructure.

---

## Próximas fases

| Fase | Objetivo |
|------|----------|
| 2 — Execução da Prova | Tela de identificação, tela da questão, alternativas, cronómetro, navegação, registo de respostas |
| 3 — Correção | Avaliar respostas, contabilizar acertos/erros/não respondidas, calcular percentagem e nota |
| 4 — Resultado | Tela de resultado, relatório, geração de PDF, guardar resultado localmente |
| 5 — Testes | Bateria de testes end-to-end antes da prova de sexta-feira |

Ver `Plano de Projeto — MVP V0.1` para o detalhe completo de cada fase.
