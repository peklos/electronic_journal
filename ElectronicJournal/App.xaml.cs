using System;
using System.Windows;
using System.Windows.Threading;

namespace ElectronicJournal
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Глобальная обработка ошибок UI потока
            DispatcherUnhandledException += App_DispatcherUnhandledException;

            // Глобальная обработка всех необработанных исключений
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            string errorMessage = $"Произошла ошибка:\n\n{e.Exception.Message}\n\nПодробности:\n{e.Exception.StackTrace}";

            MessageBox.Show(errorMessage,
                          "Ошибка приложения",
                          MessageBoxButton.OK,
                          MessageBoxImage.Error);

            e.Handled = true;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
            string errorMessage = $"Критическая ошибка:\n\n{ex?.Message}\n\nПодробности:\n{ex?.StackTrace}";

            MessageBox.Show(errorMessage,
                          "Критическая ошибка",
                          MessageBoxButton.OK,
                          MessageBoxImage.Error);
        }
    }
}
