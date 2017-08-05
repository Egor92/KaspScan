using System;
using System.IO;
using System.Text;
using System.Windows;

namespace KaspScan
{
    public partial class App : Application
    {
        private const string TimeFormat = @"yyyy-MM-dd HH-mm-ss";

        #region Ctor

        public App()
        {
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
        }

        #endregion

        protected override void OnStartup(StartupEventArgs e)
        {
            var bootstrapper = new Bootstrapper();
            bootstrapper.Run();
        }

        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            string timeString = DateTime.Now.ToString(TimeFormat);
            string path = $@"{Directory.GetCurrentDirectory()}\crash reports\crash report ({timeString}).txt";

            StringBuilder contentBuilder = new StringBuilder();

            var exception = (Exception) e.ExceptionObject;

            while (exception != null)
            {
                contentBuilder.AppendLine(exception.Message);
                contentBuilder.AppendLine();
                contentBuilder.AppendLine(exception.StackTrace);
                contentBuilder.AppendLine();
                contentBuilder.AppendLine("========================================");
                contentBuilder.AppendLine();

                exception = exception.InnerException;
            }

            File.WriteAllText(path, contentBuilder.ToString());
            string message =
                $@"Произошла необработанная ошибка. Приложение будет закрыто. Файл с логами находится по пути: ""{path}""";
            MessageBox.Show(message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
