using System;
using System.Windows;
using System.Windows.Controls;
using ElectronicJournal.Models;
using ElectronicJournal.Services;

namespace ElectronicJournal.Views
{
    public partial class AddGradeDialog : Window
    {
        private readonly DatabaseService _databaseService;

        public AddGradeDialog()
        {
            InitializeComponent();
            _databaseService = new DatabaseService();

            LoadData();
            DatePicker.SelectedDate = DateTime.Now;
            GradeComboBox.SelectedIndex = 0;
        }

        private void LoadData()
        {
            StudentComboBox.ItemsSource = _databaseService.GetAllStudents();
            SubjectComboBox.ItemsSource = _databaseService.GetAllSubjects();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (StudentComboBox.SelectedItem == null ||
                SubjectComboBox.SelectedItem == null ||
                GradeComboBox.SelectedItem == null ||
                DatePicker.SelectedDate == null)
            {
                MessageBox.Show("Пожалуйста, заполните все обязательные поля",
                              "Предупреждение",
                              MessageBoxButton.OK,
                              MessageBoxImage.Warning);
                return;
            }

            var student = (Student)StudentComboBox.SelectedItem;
            var subject = (Subject)SubjectComboBox.SelectedItem;
            var gradeValue = int.Parse(((ComboBoxItem)GradeComboBox.SelectedItem).Tag.ToString()!);

            var grade = new Grade
            {
                StudentId = student.Id,
                SubjectId = subject.Id,
                GradeValue = gradeValue,
                Date = DatePicker.SelectedDate.Value,
                Topic = TopicTextBox.Text.Trim(),
                Notes = NotesTextBox.Text.Trim()
            };

            if (_databaseService.AddGrade(grade))
            {
                MessageBox.Show("Оценка успешно добавлена",
                              "Успех",
                              MessageBoxButton.OK,
                              MessageBoxImage.Information);
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Ошибка при добавлении оценки",
                              "Ошибка",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
