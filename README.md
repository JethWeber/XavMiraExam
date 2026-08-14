# XavMira Exam System

> **Sistema desktop offline para realização, correção e gestão de provas informatizadas.**

O **XavMira Exam System** é uma aplicação desktop desenvolvida para o **Centro de Formação XavMira**, concebida para informatizar o processo de realização de avaliações presenciais.

O sistema permite ao professor preparar e iniciar uma prova, autenticar-se, configurar a avaliação e acompanhar o processo, enquanto os alunos realizam a prova individualmente através de uma interface controlada por tempo.

Todo o processo foi desenvolvido com uma abordagem **offline-first**, permitindo que as avaliações sejam realizadas sem depender de uma conexão permanente à Internet.

---

## 📌 Estado do projeto

**Versão:** `V0.1`

**Estado:** 🟢 **100% concluído e operacional**

O MVP encontra-se funcional e preparado para utilização no ambiente real do Centro de Formação XavMira.

---

# ✨ Principais funcionalidades

### 👨‍🏫 Autenticação do professor

O sistema possui uma área destinada ao professor, protegida por autenticação.

O professor pode aceder às funcionalidades administrativas da aplicação antes de preparar e iniciar uma avaliação.

### 📝 Gestão e importação de provas

As provas podem ser preparadas através de ficheiros JSON e importadas para o sistema.

Antes de uma sessão ser iniciada, a prova é validada para garantir que possui uma estrutura compatível com o sistema.

### 👨‍🎓 Identificação dos alunos

Os alunos são identificados através do sistema de alunos configurado para a instituição.

A identificação ocorre antes do início da sessão individual de prova.

### ⏱️ Controle de tempo

O professor pode configurar o tempo destinado à prova.

Durante a realização, o aluno possui um cronómetro visível.

Quando o tempo da questão termina, o sistema encerra automaticamente a questão de acordo com as regras definidas para a avaliação.

### 📋 Execução controlada da prova

A prova é apresentada uma questão de cada vez.

O sistema controla o estado da sessão e impede comportamentos que possam comprometer a integridade da avaliação.

Entre as regras implementadas:

* uma resposta por questão;
* avanço controlado entre questões;
* impossibilidade de voltar a questões anteriores;
* controle individual do tempo;
* encerramento automático de questões;
* encerramento automático da sessão;
* proteção do estado da prova durante a execução.

### 🧮 Correção automática

Após a realização da prova, o sistema processa as respostas e determina:

* respostas corretas;
* respostas incorretas;
* questões não respondidas;
* total de questões;
* percentagem;
* nota final.

### 📊 Resultados

Após a conclusão, o resultado da avaliação pode ser apresentado ao utilizador autorizado.

O sistema mantém as informações necessárias para consulta e geração dos resultados.

### 📄 Relatórios PDF

O XavMira Exam System possui geração de relatórios em **PDF**, permitindo transformar os resultados das avaliações em documentos utilizáveis pela instituição.

### 💾 Funcionamento offline

Uma das características fundamentais do projeto é a capacidade de funcionamento sem dependência de Internet durante a realização das provas.

A aplicação pode ser instalada diretamente nos computadores utilizados no centro de formação.

---

# 🏗️ Arquitetura

O projeto foi estruturado utilizando separação de responsabilidades entre domínio, infraestrutura e interface.

```text
┌─────────────────────────────────────────┐
│          XavMiraExam.Desktop             │
│       Avalonia UI + MVVM + Views         │
└────────────────────┬────────────────────┘
                     │
                     ▼
┌─────────────────────────────────────────┐
│             XavMiraExam.Core             │
│ Models · Services · Interfaces           │
│ Regras de negócio · Sessão de exame      │
└────────────────────┬────────────────────┘
                     │
                     ▼
┌─────────────────────────────────────────┐
│       XavMiraExam.Infrastructure         │
│ JSON · Persistência · Implementações     │
│ Dados · Resultados · Relatórios          │
└─────────────────────────────────────────┘
```

A camada de domínio contém as regras fundamentais do sistema, enquanto a infraestrutura fornece as implementações concretas e a aplicação Desktop é responsável pela experiência de utilização.

---

# 🧰 Tecnologias

| Tecnologia                | Utilização                         |
| ------------------------- | ---------------------------------- |
| **C#**                    | Linguagem principal                |
| **.NET 10**               | Plataforma de desenvolvimento      |
| **Avalonia UI 11**        | Interface gráfica                  |
| **MVVM**                  | Arquitetura da interface           |
| **CommunityToolkit.Mvvm** | Implementação MVVM                 |
| **System.Text.Json**      | Manipulação de JSON                |
| **SQLite**                | Persistência local                 |
| **PDF**                   | Geração de relatórios              |
| **Inno Setup**            | Distribuição do instalador Windows |

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
│   ├── Json/
│   ├── Database/
│   └── Reports/
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
├── Students/
├── Results/
├── Data/
│
├── installer/
│   └── script_setup.iss
│
├── XavMiraExam.slnx
├── .gitignore
└── README.md
```

Os projetos de consola foram utilizados como ferramentas de validação durante o desenvolvimento do sistema.

---

# 🔄 Fluxo da aplicação

O fluxo principal da aplicação pode ser representado da seguinte forma:

```text
                    ┌──────────────┐
                    │    Início    │
                    └──────┬───────┘
                           │
                           ▼
                 ┌──────────────────┐
                 │ Autenticação      │
                 │ do Professor      │
                 └────────┬─────────┘
                          │
                          ▼
                 ┌──────────────────┐
                 │ Preparar /       │
                 │ importar prova   │
                 └────────┬─────────┘
                          │
                          ▼
                 ┌──────────────────┐
                 │ Configurar       │
                 │ avaliação/tempo  │
                 └────────┬─────────┘
                          │
                          ▼
                 ┌──────────────────┐
                 │ Identificação    │
                 │ do aluno         │
                 └────────┬─────────┘
                          │
                          ▼
                 ┌──────────────────┐
                 │ Realização da    │
                 │ prova            │
                 └────────┬─────────┘
                          │
                          ▼
                 ┌──────────────────┐
                 │ Correção         │
                 │ automática       │
                 └────────┬─────────┘
                          │
                          ▼
                 ┌──────────────────┐
                 │ Resultado        │
                 └────────┬─────────┘
                          │
                          ▼
                 ┌──────────────────┐
                 │ Relatório PDF    │
                 └──────────────────┘
```

---

# 📝 Modelo de prova

As provas são representadas através de JSON.

Um exemplo simplificado:

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

O sistema valida o conteúdo antes de permitir que a prova seja utilizada.

Essa abordagem permite preparar diferentes avaliações sem necessidade de alterar o código-fonte da aplicação.

---

# 👨‍🏫 Modo Professor

O professor possui acesso a funcionalidades administrativas através de autenticação.

O fluxo principal é:

```text
Login
  │
  ▼
Área do Professor
  │
  ├── Preparar prova
  ├── Importar prova
  ├── Validar prova
  ├── Configurar tempo
  └── Iniciar sessão
```

O professor controla o início da avaliação, enquanto o sistema assume o controle da execução após o início da sessão.

---

# 👨‍🎓 Modo Aluno

O aluno não possui acesso às funcionalidades administrativas.

Após a identificação, a aplicação apresenta a avaliação:

```text
Identificação
      │
      ▼
Início da prova
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

A interface foi projetada para manter o foco do aluno exclusivamente na avaliação.

---

# ⏱️ Controle e integridade da sessão

A execução da prova é controlada pelo serviço de sessão do domínio.

O `ExamSessionService` mantém o estado da avaliação e aplica as regras definidas para a sessão.

Isso evita colocar regras críticas exclusivamente na interface gráfica.

Entre as regras implementadas estão:

* controle do estado da sessão;
* controle da questão atual;
* controle de respostas;
* limite de tempo;
* avanço automático;
* encerramento de questões;
* encerramento da prova;
* prevenção de alterações após a conclusão.

---

# 🧪 Testes e validação

Durante o desenvolvimento foram criadas aplicações de consola para validar componentes importantes independentemente da interface gráfica.

Foram realizados testes envolvendo:

* carregamento de provas;
* validação de JSON;
* identificação de alunos;
* respostas;
* expiração do tempo;
* avanço automático;
* conclusão da sessão;
* processamento dos resultados.

Além dos testes de componentes, o sistema foi executado através da interface gráfica e empacotado em um instalador Windows para validação no ambiente real.

---

# 📦 Build e publicação

Para desenvolvimento, restaure as dependências:

```bash
dotnet restore
```

Compile a solução:

```bash
dotnet build
```

Execute a aplicação:

```bash
dotnet run --project XavMiraExam.Desktop
```

Para criar uma publicação Windows:

```bash
dotnet publish \
  -c Release \
  -r win-x64 \
  --self-contained true
```

O instalador do sistema é posteriormente gerado utilizando o **Inno Setup**.

Os artefactos de build e publicação não fazem parte do repositório Git.

---

# 🖥️ Distribuição

O sistema é distribuído através de um instalador Windows:

```text
XavMiraSetup.exe
```

O instalador inclui os componentes necessários para executar a aplicação publicada.

A publicação utilizada para o ambiente de produção é **self-contained**, reduzindo a necessidade de configurar manualmente o runtime .NET nas máquinas destinadas à realização das provas.

---

# 🔐 Segurança e operação

O sistema foi desenvolvido considerando o contexto de utilização presencial.

Entre as medidas implementadas encontram-se:

* autenticação do professor;
* separação entre área administrativa e área do aluno;
* validação das provas;
* controle do fluxo da sessão;
* restrição de navegação durante a avaliação;
* controle de tempo;
* persistência local dos dados;
* funcionamento offline.

A aplicação não depende de um servidor remoto para executar uma avaliação.

---

# 🎯 Objetivo

O XavMira Exam System foi criado para resolver um problema concreto:

> **Disponibilizar uma plataforma simples e confiável para realização de provas informatizadas em ambientes de formação onde a Internet não deve ser um requisito obrigatório.**

O sistema privilegia:

**Confiabilidade · Simplicidade · Offline-first · Controle · Automatização**

---

# 🏫 Contexto institucional

O sistema foi desenvolvido para utilização no:

**Centro de Formação XavMira**

A primeira versão foi desenvolvida com foco na utilização prática em avaliações presenciais, permitindo validar a solução diretamente no ambiente para o qual foi concebida.

---

# 🚀 Próximas evoluções

A versão atual está concluída como MVP.

O desenvolvimento futuro poderá expandir o sistema para funcionalidades institucionais, tais como:

* gestão centralizada de provas;
* gestão de turmas;
* gestão de professores;
* banco de questões;
* estatísticas avançadas;
* sincronização entre computadores;
* API institucional;
* painel administrativo;
* gestão centralizada de resultados;
* distribuição automática de avaliações.

Essas funcionalidades pertencem a uma possível evolução do produto e **não são requisitos da versão atual**.

---

# 📜 Licença

Este projeto é propriedade dos seus respetivos autores e/ou da organização responsável pelo desenvolvimento.

A utilização, distribuição ou modificação do código deve respeitar os termos definidos pelo proprietário do projeto.

> **Licença: a definir.**

---

# 👨‍💻 Desenvolvimento

Desenvolvido por **Jeth Weber**.

Tecnologias principais:

```text
C# · .NET 10 · Avalonia UI 11 · MVVM · SQLite · JSON
```

---

<p align="center">

**XavMira Exam System**

*Offline-first examination software for education.*

</p>
