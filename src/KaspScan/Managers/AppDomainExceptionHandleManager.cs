using System;
using System.IO;
using System.Text;
using System.Windows;

namespace KaspScan.Managers
{
    public class AppDomainExceptionHandleManager
    {
        #region Fields

        private readonly AppDomain _appDomain;
        private const string TimeFormat = @"yyyy-MM-dd HH-mm-ss";

        #endregion

        #region Ctor

        public AppDomainExceptionHandleManager(AppDomain appDomain)
        {
            _appDomain = appDomain;
        }

        #endregion

        public void Attach()
        {
            _appDomain.UnhandledException += OnUnhandledException;
        }

        public void Detach()
        {
            _appDomain.UnhandledException -= OnUnhandledException;
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
