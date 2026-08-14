# XavMira Exam System

> **Sistema desktop offline para realização de provas informatizadas no Centro de Formação XavMira.**

O **XavMira Exam System** é uma aplicação desktop desenvolvida para permitir a realização de avaliações informatizadas em ambientes onde a disponibilidade de Internet não pode ser assumida como requisito.

O sistema foi concebido inicialmente como um **MVP operacional**, com foco em confiabilidade, execução offline e simplicidade de utilização durante avaliações presenciais.

---

## ✨ Visão geral

Em vez de depender de uma plataforma web, servidor ou conexão permanente à Internet, o XavMira Exam funciona localmente na máquina onde a prova é realizada.

O fluxo básico é:

```text
Professor
   │
   ├── Importa a prova (.json)
   │
   ├── Configura o tempo
   │
   └── Inicia a sessão
             │
             ▼
        Identificação
          do aluno
             │
             ▼
       Execução da prova
             │
             ├── Questão 1
             ├── Questão 2
             ├── Questão 3
             └── ...
             │
             ▼
       Finalização da prova
```

A arquitetura foi pensada para que a aplicação possa evoluir posteriormente para um sistema institucional completo, sem abandonar o princípio de funcionamento **offline-first**.

---

# 🚀 Estado do projeto

**Versão:** `V0.1`

**Estado:** 🟢 MVP operacional

| Componente                  | Estado      |
| --------------------------- | ----------- |
| Fundação do projeto         | ✅ Concluído |
| Leitura de provas JSON      | ✅ Concluído |
| Validação de provas         | ✅ Concluído |
| Identificação de alunos     | ✅ Concluído |
| Execução da prova           | ✅ Concluído |
| Cronómetro                  | ✅ Concluído |
| Controle de tempo           | ✅ Concluído |
| Máquina de estados da prova | ✅ Concluído |
| Correção automática         | 🚧 Fase 3   |
| Resultados                  | 🚧 Fase 4   |
| Relatórios PDF              | 🚧 Fase 4   |
| Persistência SQLite         | 🚧 Fase 3   |
| Testes end-to-end           | 🚧 Fase 5   |

---

# 🧰 Stack tecnológica

O projeto utiliza tecnologias modernas do ecossistema .NET:

* **.NET 10**
* **C#**
* **Avalonia UI 11**
* **MVVM**
* **CommunityToolkit.Mvvm**
* **System.Text.Json**
* **SQLite** — planejado para as fases seguintes

### Arquitetura

O projeto segue uma separação por responsabilidades:

```text
┌─────────────────────────────────────┐
│       XavMiraExam.Desktop           │
│       Avalonia + MVVM               │
└─────────────────┬───────────────────┘
                  │
                  ▼
┌─────────────────────────────────────┐
│          XavMiraExam.Core            │
│ Models · Services · Interfaces       │
└─────────────────┬───────────────────┘
                  │
                  ▼
┌─────────────────────────────────────┐
│      XavMiraExam.Infrastructure      │
│ JSON · Persistência · Implementações │
└─────────────────────────────────────┘
```

Essa separação permite que a lógica principal do sistema permaneça independente da interface gráfica.

---

# 📁 Estrutura da solução

```text
XavMiraExam/
│
├── XavMiraExam.Core/
│   ├── Models/
│   ├── Services/
│   └── Interfaces/
│
├── XavMiraExam.Infrastructure/
│   └── Json/
│
├── XavMiraExam.Desktop/
│   ├── Views/
│   ├── ViewModels/
│   ├── Services/
│   └── Assets/
│
├── XavMiraExam.Fase1Console/
├── XavMiraExam.Fase2Console/
│
├── Exams/
│   ├── Informatica.json
│   └── ProvaInvalida.json
│
├── Students/
│   └── students.json
│
├── Results/
├── Data/
│
├── XavMiraExam.slnx
└── README.md
```

### Principais projetos

#### `XavMiraExam.Core`

Contém o núcleo da aplicação:

* modelos de domínio;
* regras da sessão;
* interfaces;
* serviços de negócio.

Não depende da interface gráfica.

#### `XavMiraExam.Infrastructure`

Contém implementações concretas das interfaces do Core.

Atualmente:

* leitura de provas JSON;
* validação;
* leitura de alunos.

Nas próximas fases:

* SQLite;
* persistência de resultados;
* relatórios.

#### `XavMiraExam.Desktop`

Interface gráfica construída com **Avalonia UI 11**, seguindo o padrão MVVM.

É responsável por:

* navegação;
* interação com o professor;
* identificação do aluno;
* apresentação das questões;
* cronómetro;
* execução visual da sessão.

#### `XavMiraExam.Fase1Console`

Projeto de verificação da fundação e leitura de provas.

Não faz parte do produto final.

#### `XavMiraExam.Fase2Console`

Projeto de verificação da máquina de estados da sessão de prova.

Também não faz parte do produto final.

---

# 📝 Formato das provas

As provas são definidas através de ficheiros JSON.

Exemplo simplificado:

```json
{
  "title": "Avaliação de Informática",
  "questions": [
    {
      "text": "O que significa CPU?",
      "options": [
        "Central Processing Unit",
        "Computer Personal Unit",
        "Central Program Utility",
        "Computer Processing Utility"
      ],
      "correctAnswer": 0
    }
  ]
}
```

A utilização de JSON nesta fase permite preparar e transportar provas facilmente sem depender de uma base de dados ou servidor.

---

# 👨‍🏫 Fluxo do professor

O professor inicia a aplicação e seleciona:

```text
Modo Professor
      │
      ▼
Importar prova
      │
      ▼
Validação
      │
      ▼
Configuração do tempo
      │
      ▼
Iniciar sessão
```

O sistema valida o ficheiro antes de permitir a execução da prova.

Isso evita iniciar uma avaliação utilizando uma prova estruturalmente inválida.

---

# 👨‍🎓 Fluxo do aluno

Depois de a sessão ser iniciada:

```text
Identificação
     │
     ▼
Validação do código
     │
     ▼
Questão atual
     │
     ▼
Resposta
     │
     ▼
Próxima questão
     │
     ▼
...
     │
     ▼
Conclusão
```

O aluno responde a uma questão de cada vez.

A sessão não permite retornar às questões anteriores.

---

# ⏱️ Controle de tempo

Cada questão possui um limite de tempo configurado pelo professor.

Quando o tempo termina:

```text
Tempo esgotado
      │
      ▼
Questão encerrada
      │
      ▼
Sem resposta
      │
      ▼
Próxima questão
```

O controle de tempo é realizado pelo `ExamSessionService`, e não apenas pela interface gráfica.

Isso é importante porque as regras da prova permanecem centralizadas na camada de domínio.

---

# 🔒 Regras da sessão

A sessão de exame implementa regras explícitas:

* uma resposta por questão;
* não é possível retornar a questões anteriores;
* uma questão encerrada não pode ser respondida novamente;
* o tempo de cada questão é limitado;
* o término do tempo encerra automaticamente a questão;
* a última questão encerra automaticamente a sessão;
* uma sessão concluída não pode continuar.

Essas regras são implementadas no domínio através do:

```text
ExamSessionService
```

---

# 🧪 Verificação

O projeto possui aplicações de consola destinadas a validar o comportamento do sistema sem depender da interface gráfica.

## Fase 1

Valida:

* carregamento de provas;
* leitura de JSON;
* rejeição de provas inválidas.

## Fase 2

Valida:

* identificação do aluno;
* resposta rápida;
* expiração real do tempo;
* avanço automático;
* conclusão da sessão;
* estado final da prova.

Exemplo de execução validada:

```text
Resumo da sessão:

Aluno: Maria Silva (A002)
Prova: Avaliação de Informática

Respondidas: 4
Não respondidas: 1
Corretas: 4
```

---

# 💻 Requisitos

Para desenvolvimento:

* .NET 10 SDK
* Sistema operacional compatível com .NET e Avalonia
* acesso à Internet para restauração dos pacotes NuGet

Para execução do produto publicado:

* Windows x64
* não é necessário instalar o .NET Runtime quando utilizada a publicação self-contained.

> O instalador distribuído para o ambiente de provas é gerado separadamente através do Inno Setup.

---

# 🛠️ Executar localmente

Clone o repositório:

```bash
git clone <URL_DO_REPOSITORIO>
cd XavMiraExam
```

Restaure as dependências:

```bash
dotnet restore
```

Compile:

```bash
dotnet build
```

Execute a aplicação:

```bash
dotnet run --project XavMiraExam.Desktop
```

### Executar as verificações

```bash
dotnet run --project XavMiraExam.Fase1Console
```

```bash
dotnet run --project XavMiraExam.Fase2Console
```

---

# 📦 Publicação

O repositório contém o código-fonte e os ficheiros necessários para construir o sistema.

Os artefactos gerados pelo processo de build não são versionados no Git.

Exemplo:

```bash
dotnet publish \
  -c Release \
  -r win-x64 \
  --self-contained true
```

O diretório `publish/` é ignorado pelo Git.

Da mesma forma, o instalador gerado pelo Inno Setup não faz parte do código-fonte versionado.

---

# 🏗️ Roadmap

## Fase 3 — Correção

* [ ] Avaliação automática das respostas
* [ ] Contagem de acertos
* [ ] Contagem de erros
* [ ] Contagem de questões não respondidas
* [ ] Cálculo da percentagem
* [ ] Cálculo da nota final

## Fase 4 — Resultados

* [ ] Ecrã de resultado
* [ ] Persistência dos resultados
* [ ] SQLite
* [ ] Histórico de provas
* [ ] Geração de relatórios PDF
* [ ] Consulta dos resultados

## Fase 5 — Validação

* [ ] Testes end-to-end
* [ ] Testes com várias provas
* [ ] Testes com múltiplos alunos
* [ ] Testes de expiração de tempo
* [ ] Testes de recuperação de erros
* [ ] Teste completo no ambiente real do Centro de Formação XavMira

---

# 🔮 Evolução futura

O MVP foi desenvolvido com uma arquitetura que permite evoluir o sistema além da utilização local.

Possíveis versões futuras:

```text
V0.1
Offline Desktop
     │
     ▼
V0.2
Correção + Resultados
     │
     ▼
V0.3
Gestão de alunos + provas
     │
     ▼
V1.0
Sistema institucional
     │
     ▼
Futuro
Desktop + API + sincronização
```

Possíveis funcionalidades futuras:

* banco centralizado de questões;
* gestão de turmas;
* gestão de professores;
* múltiplos tipos de prova;
* estatísticas;
* relatórios administrativos;
* exportação de resultados;
* sincronização entre computadores;
* API institucional;
* painel administrativo;
* gestão centralizada das avaliações.

---

# 🎯 Objetivo do projeto

O objetivo inicial não é criar uma plataforma de e-learning completa.

O objetivo é muito mais direto:

> **Permitir que uma instituição de ensino realize provas informatizadas de forma simples, controlada e confiável, mesmo sem depender da Internet.**

A arquitetura foi construída para resolver primeiro o problema real do ambiente de utilização e, posteriormente, permitir a evolução do sistema.

---

# 🏫 Contexto

O XavMira Exam foi desenvolvido para utilização no:

**Centro de Formação XavMira**

O projeto nasceu como uma solução prática para informatizar avaliações presenciais e reduzir a dependência de processos manuais durante a realização e gestão das provas.

---

# 📜 Licença

Este projeto é propriedade dos seus respetivos autores e/ou da organização responsável pelo desenvolvimento.

A utilização, redistribuição ou modificação do código deve respeitar os termos definidos pelo proprietário do projeto.

> **Licença definitiva: a definir.**

---

# 👨‍💻 Desenvolvimento

Desenvolvido com:

**C# · .NET · Avalonia UI · MVVM · JSON**

por **Jeth Weber**.

---

<p align="center">
  <strong>XavMira Exam System</strong><br>
  Offline-first examination software for education.
</p>
