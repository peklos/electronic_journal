using System;
using System.Windows;
using System.Windows.Controls;
using ElectronicJournal.Services;

namespace ElectronicJournal.Views
{
    public partial class RegisterWindow : Window
    {
        private readonly DatabaseService _databaseService;

        public RegisterWindow()
        {
            InitializeComponent();
            _databaseService = new DatabaseService();
            LoadStudents();
        }

        private void LoadStudents()
        {
            var students = _databaseService.GetAllStudents();
            StudentComboBox.ItemsSource = students;
        }

        private void RoleComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (StudentSelectionPanel == null) return; // Еще не загружено

            if (RoleComboBox.SelectedIndex == 1) // Студент
            {
                StudentSelectionPanel.Visibility = Visibility.Visible;
            }
            else
            {
                StudentSelectionPanel.Visibility = Visibility.Collapsed;
            }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            if (RoleComboBox.SelectedItem == null)
            {
                ErrorTextBlock.Text = "Пожалуйста, выберите роль";
                return;
            }

            string username = UsernameTextBox.Text.Trim();
            string password = PasswordBox.Password;
            string confirmPassword = ConfirmPasswordBox.Password;
            string fullName = FullNameTextBox.Text.Trim();
            string role = ((ComboBoxItem)RoleComboBox.SelectedItem).Content.ToString()!;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) ||
                string.IsNullOrEmpty(fullName))
            {
                ErrorTextBlock.Text = "Пожалуйста, заполните все поля";
                return;
            }

            if (password != confirmPassword)
            {
                ErrorTextBlock.Text = "Пароли не совпадают";
                return;
            }

            if (username.Length < 3)
            {
                ErrorTextBlock.Text = "Имя пользователя должно быть не менее 3 символов";
                return;
            }

            if (password.Length < 3)
            {
                ErrorTextBlock.Text = "Пароль должен быть не менее 3 символов";
                return;
            }

            int? studentId = null;
            if (role == "Студент" || role == "Преподаватель")
            {
                role = role == "Преподаватель" ? "Учитель" : role;
            }
            if (role == "Студент")
            {
                if (StudentComboBox.SelectedItem == null)
                {
                    ErrorTextBlock.Text = "Пожалуйста, выберите себя из списка студентов";
                    return;
                }
                studentId = ((Models.Student)StudentComboBox.SelectedItem).Id;
            }

            bool success = _databaseService.RegisterUser(username, password, fullName, role, studentId);

            if (success)
            {
                MessageBox.Show("Регистрация прошла успешно! Теперь вы можете войти в систему.",
                              "Успех",
                              MessageBoxButton.OK,
                              MessageBoxImage.Information);
                this.Close();
            }
            else
            {
                ErrorTextBlock.Text = "Пользователь с таким именем уже существует";
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
