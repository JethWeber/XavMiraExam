using Avalonia;

namespace XavMiraExam.Desktop;

internal sealed class Program
{
    // A linha de código inicial deve ser esta. As coisas não vão correr bem
    // se o método AppMain for removido ou o seu nome for alterado.
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);

    public static AppBuilder BuildAvaloniaApp() => AppBuilder.Configure<App>()
        .UsePlatformDetect()
        .WithInterFont()
        .LogToTrace();
}
