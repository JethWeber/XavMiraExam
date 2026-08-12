using Avalonia.Controls;
using XavMiraExam.Core.Interfaces;
using XavMiraExam.Core.Services;
using XavMiraExam.Desktop.Services;
using XavMiraExam.Desktop.ViewModels;
using XavMiraExam.Infrastructure.Json;

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

        IExamJsonReader examReader = new ExamJsonReader();
        IExamValidator examValidator = new ExamValidator();
        var examService = new ExamService(examReader, examValidator);
        var sessionService = new ExamSessionService();

        string studentsFile = ProjectPaths.FindFile("Students", "students.json");
        IStudentService studentService = new StudentJsonReader(studentsFile);

        DataContext = new MainViewModel(examService, studentService, sessionService, filePicker);
    }
}
