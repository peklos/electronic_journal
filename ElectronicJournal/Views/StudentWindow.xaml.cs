using System;
using System.Linq;
using System.Windows;
using ElectronicJournal.Models;
using ElectronicJournal.Services;

namespace ElectronicJournal.Views
{
    public partial class StudentWindow : Window
    {
        private readonly DatabaseService _databaseService;
        private readonly User _currentUser;
        private Student? _studentInfo;

        public StudentWindow(User user)
        {
            InitializeComponent();
            _databaseService = new DatabaseService();
            _currentUser = user;

            LoadStudentInfo();
            LoadData();
        }

        private void LoadStudentInfo()
        {
            if (_currentUser.StudentId.HasValue)
            {
                _studentInfo = _databaseService.GetStudentById(_currentUser.StudentId.Value);

                if (_studentInfo != null)
                {
                    WelcomeTextBlock.Text = $"Добро пожаловать, {_studentInfo.FullName}!";
                    FullNameTextBlock.Text = _studentInfo.FullName;
                    ClassTextBlock.Text = _studentInfo.Class;
                    BirthDateTextBlock.Text = _studentInfo.BirthDate.ToString("dd.MM.yyyy");
                    ParentPhoneTextBlock.Text = _studentInfo.ParentPhone;
                    AddressTextBlock.Text = _studentInfo.Address;
                    NotesTextBlock.Text = string.IsNullOrEmpty(_studentInfo.Notes) ? "Нет примечаний" : _studentInfo.Notes;
                }
            }
        }

        private void LoadData()
        {
            LoadGrades();
            LoadAttendance();
            UpdateStatistics();
        }

        private void LoadGrades()
        {
            if (_studentInfo != null)
            {
                var grades = _databaseService.GetGradesByStudent(_studentInfo.Id);
                GradesDataGrid.ItemsSource = grades;
                GradesInfoTextBlock.Text = $"Всего оценок: {grades.Count}";
            }
        }

        private void LoadAttendance()
        {
            if (_studentInfo != null)
            {
                var attendance = _databaseService.GetAttendanceByStudent(_studentInfo.Id);
                AttendanceDataGrid.ItemsSource = attendance;

                int totalDays = attendance.Count;
                int presentDays = attendance.Count(a => a.Status == "Присутствовал");
                int absentDays = attendance.Count(a => a.Status == "Отсутствовал");

                AttendanceInfoTextBlock.Text = $"Всего дней: {totalDays} | Присутствовал: {presentDays} | Отсутствовал: {absentDays}";
            }
        }

        private void UpdateStatistics()
        {
            if (_studentInfo != null)
            {
                var grades = _databaseService.GetGradesByStudent(_studentInfo.Id);

                TotalGradesTextBlock.Text = grades.Count.ToString();

                if (grades.Count > 0)
                {
                    double average = grades.Average(g => g.GradeValue);
                    AverageGradeTextBlock.Text = average.ToString("F2");
                }
                else
                {
                    AverageGradeTextBlock.Text = "—";
                }
            }
        }

        private void RefreshGradesButton_Click(object sender, RoutedEventArgs e)
        {
            LoadGrades();
            UpdateStatistics();
        }

        private void RefreshAttendanceButton_Click(object sender, RoutedEventArgs e)
        {
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
