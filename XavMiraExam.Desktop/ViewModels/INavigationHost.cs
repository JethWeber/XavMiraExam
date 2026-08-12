namespace XavMiraExam.Desktop.ViewModels;

public interface INavigationHost
{
    void NavigateToHome();
    void NavigateToProfessor();
    void NavigateToStudentLogin();
    void NavigateToExam();
    void NavigateToExamComplete();
}
