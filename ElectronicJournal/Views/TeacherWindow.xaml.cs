using System;
using System.Windows;
using System.Windows.Controls;
using ElectronicJournal.Models;
using ElectronicJournal.Services;

namespace ElectronicJournal.Views
{
    public partial class TeacherWindow : Window
    {
        private readonly DatabaseService _databaseService;
        private readonly User _currentUser;

        public TeacherWindow(User user)
        {
            InitializeComponent();
            _databaseService = new DatabaseService();
            _currentUser = user;

            WelcomeTextBlock.Text = $"Добро пожаловать, {_currentUser.FullName}!";

            LoadData();
        }

        private void LoadData()
        {
            LoadStudents();
            LoadGrades();
            LoadAttendance();
        }

        private void LoadStudents()
        {
            var students = _databaseService.GetAllStudents();
            StudentsDataGrid.ItemsSource = students;
            GradeFilterComboBox.ItemsSource = students;
            AttendanceFilterComboBox.ItemsSource = students;
            StudentsCountTextBlock.Text = $"Всего учеников: {students.Count}";
        }

        private void LoadGrades()
        {
            var grades = _databaseService.GetAllGrades();
            GradesDataGrid.ItemsSource = grades;
        }

        private void LoadAttendance()
        {
            var attendance = _databaseService.GetAllAttendance();
            AttendanceDataGrid.ItemsSource = attendance;
        }

        // Обработчики для студентов
        private void AddStudentButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AddEditStudentDialog();
            if (dialog.ShowDialog() == true)
            {
                LoadStudents();
            }
        }

        private void EditStudentButton_Click(object sender, RoutedEventArgs e)
        {
            if (StudentsDataGrid.SelectedItem is Student selectedStudent)
            {
                var dialog = new AddEditStudentDialog(selectedStudent);
                if (dialog.ShowDialog() == true)
                {
                    LoadStudents();
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите ученика для редактирования",
                              "Предупреждение",
                              MessageBoxButton.OK,
                              MessageBoxImage.Warning);
            }
        }

        private void DeleteStudentButton_Click(object sender, RoutedEventArgs e)
        {
            if (StudentsDataGrid.SelectedItem is Student selectedStudent)
            {
                var result = MessageBox.Show($"Вы уверены, что хотите удалить ученика {selectedStudent.FullName}?",
                                            "Подтверждение",
                                            MessageBoxButton.YesNo,
                                            MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    if (_databaseService.DeleteStudent(selectedStudent.Id))
                    {
                        MessageBox.Show("Ученик успешно удален",
                                      "Успех",
                                      MessageBoxButton.OK,
                                      MessageBoxImage.Information);
                        LoadStudents();
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при удалении ученика",
                                      "Ошибка",
                                      MessageBoxButton.OK,
                                      MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите ученика для удаления",
                              "Предупреждение",
                              MessageBoxButton.OK,
                              MessageBoxImage.Warning);
            }
        }

        private void RefreshStudentsButton_Click(object sender, RoutedEventArgs e)
        {
            LoadStudents();
        }

        // Обработчики для оценок
        private void AddGradeButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AddGradeDialog();
            if (dialog.ShowDialog() == true)
            {
                LoadGrades();
            }
        }

        private void DeleteGradeButton_Click(object sender, RoutedEventArgs e)
        {
            if (GradesDataGrid.SelectedItem is Grade selectedGrade)
            {
                var result = MessageBox.Show($"Вы уверены, что хотите удалить эту оценку?",
                                            "Подтверждение",
                                            MessageBoxButton.YesNo,
                                            MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    if (_databaseService.DeleteGrade(selectedGrade.Id))
                    {
                        MessageBox.Show("Оценка успешно удалена",
                                      "Успех",
                                      MessageBoxButton.OK,
                                      MessageBoxImage.Information);
                        LoadGrades();
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при удалении оценки",
                                      "Ошибка",
                                      MessageBoxButton.OK,
                                      MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите оценку для удаления",
                              "Предупреждение",
                              MessageBoxButton.OK,
                              MessageBoxImage.Warning);
            }
        }

        private void RefreshGradesButton_Click(object sender, RoutedEventArgs e)
        {
            LoadGrades();
        }

        private void GradeFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GradeFilterComboBox.SelectedItem is Student selectedStudent)
            {
                var grades = _databaseService.GetGradesByStudent(selectedStudent.Id);
                GradesDataGrid.ItemsSource = grades;
            }
        }

        private void ClearGradeFilterButton_Click(object sender, RoutedEventArgs e)
        {
            GradeFilterComboBox.SelectedItem = null;
            LoadGrades();
        }

        // Обработчики для посещаемости
        private void AddAttendanceButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AddAttendanceDialog();
            if (dialog.ShowDialog() == true)
            {
                LoadAttendance();
            }
        }

        private void DeleteAttendanceButton_Click(object sender, RoutedEventArgs e)
        {
            if (AttendanceDataGrid.SelectedItem is Attendance selectedAttendance)
            {
                var result = MessageBox.Show($"Вы уверены, что хотите удалить эту запись?",
                                            "Подтверждение",
                                            MessageBoxButton.YesNo,
                                            MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    if (_databaseService.DeleteAttendance(selectedAttendance.Id))
                    {
                        MessageBox.Show("Запись успешно удалена",
                                      "Успех",
                                      MessageBoxButton.OK,
                                      MessageBoxImage.Information);
                        LoadAttendance();
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при удалении записи",
                                      "Ошибка",
                                      MessageBoxButton.OK,
                                      MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите запись для удаления",
                              "Предупреждение",
                              MessageBoxButton.OK,
                              MessageBoxImage.Warning);
            }
        }

        private void RefreshAttendanceButton_Click(object sender, RoutedEventArgs e)
        {
            LoadAttendance();
        }

        private void AttendanceFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AttendanceFilterComboBox.SelectedItem is Student selectedStudent)
            {
                var attendance = _databaseService.GetAttendanceByStudent(selectedStudent.Id);
                AttendanceDataGrid.ItemsSource = attendance;
            }
        }

        private void ClearAttendanceFilterButton_Click(object sender, RoutedEventArgs e)
        {
            AttendanceFilterComboBox.SelectedItem = null;
            LoadAttendance();
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }
    }
}
