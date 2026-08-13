namespace XavMiraExam.Desktop.ViewModels;

public interface INavigationHost
{
    void NavigateToHome();
    void NavigateToProfessorLogin();
    void NavigateToProfessor();
    void NavigateToStudentLogin();
    void NavigateToStudentRegister();
    void NavigateToExam();
    void NavigateToExamComplete();
}
