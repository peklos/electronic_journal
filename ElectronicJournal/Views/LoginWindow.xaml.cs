using System;
using System.Windows;
using System.Windows.Documents;
using ElectronicJournal.Services;

namespace ElectronicJournal.Views
{
    public partial class LoginWindow : Window
    {
        private readonly DatabaseService _databaseService;

        public LoginWindow()
        {
            InitializeComponent();
            _databaseService = new DatabaseService();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text.Trim();
            string password = PasswordBox.Password;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ErrorTextBlock.Text = "Пожалуйста, заполните все поля";
                return;
            }

            var user = _databaseService.AuthenticateUser(username, password);

            if (user != null)
            {
                if (user.Role == "Учитель")
                {
                    var teacherWindow = new TeacherWindow(user);
                    teacherWindow.Show();
                    this.Close();
                }
                else if (user.Role == "Ученик")
                {
                    var studentWindow = new StudentWindow(user);
                    studentWindow.Show();
                    this.Close();
                }
            }
            else
            {
                ErrorTextBlock.Text = "Неверное имя пользователя или пароль";
            }
        }

        private void RegisterLink_Click(object sender, RoutedEventArgs e)
        {
            var registerWindow = new RegisterWindow();
            registerWindow.ShowDialog();
        }
    }
}
