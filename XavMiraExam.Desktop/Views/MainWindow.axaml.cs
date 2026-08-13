using Avalonia.Controls;
using XavMiraExam.Core.Interfaces;
using XavMiraExam.Core.Services;
using XavMiraExam.Desktop.Services;
using XavMiraExam.Desktop.ViewModels;
using XavMiraExam.Infrastructure.Database;
using XavMiraExam.Infrastructure.Json;
using XavMiraExam.Infrastructure.Reports;

namespace XavMiraExam.Desktop.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    public void InitializeServices(IFilePickerService filePicker)
    {
        if (filePicker is FilePickerService concretePicker)
            concretePicker.SetOwner(this);

        // SQLite em Documentos/XavMiraExam/Data/
        var db = new SqliteDb(AppPaths.DatabasePath);
        var studentService = new StudentSqliteService(db);
        var resultStore = new ResultSqliteStore(db);

        // Seed opcional a partir de Students/students.json (só se a BD estiver vazia)
        try
        {
            string studentsJson = ProjectPaths.FindFile("Students", "students.json");
            studentService.SeedFromJsonIfEmpty(studentsJson);
        }
        catch (DirectoryNotFoundException)
        {
            // Sem pasta Students no disco — alunos criam conta pela UI
        }
        catch (FileNotFoundException)
        {
        }

        IExamJsonReader examReader = new ExamJsonReader();
        IExamValidator examValidator = new ExamValidator();
        var examService = new ExamService(examReader, examValidator);
        var sessionService = new ExamSessionService();
        var evaluationService = new EvaluationService();
        IPdfReportService pdfReport = new PdfReportService();
        var resultService = new ResultService(pdfReport, resultStore);

        DataContext = new MainViewModel(
            examService,
            studentService,
            sessionService,
            evaluationService,
            resultService,
            filePicker);
    }
}
