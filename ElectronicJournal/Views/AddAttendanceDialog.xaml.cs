using System;
using System.Windows;
using System.Windows.Controls;
using ElectronicJournal.Models;
using ElectronicJournal.Services;

namespace ElectronicJournal.Views
{
    public partial class AddAttendanceDialog : Window
    {
        private readonly DatabaseService _databaseService;

        public AddAttendanceDialog()
        {
            InitializeComponent();
            _databaseService = new DatabaseService();

            LoadData();
            DatePicker.SelectedDate = DateTime.Now;
        }

        private void LoadData()
        {
            StudentComboBox.ItemsSource = _databaseService.GetAllStudents();
        }

        private void StatusComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Если выбран статус "Присутствовал", очищаем и блокируем поле причины
            if (StatusComboBox.SelectedIndex == 0)
            {
                ReasonTextBox.IsEnabled = false;
                ReasonTextBox.Text = string.Empty;
            }
            else
            {
                ReasonTextBox.IsEnabled = true;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (StudentComboBox.SelectedItem == null ||
                DatePicker.SelectedDate == null)
            {
                MessageBox.Show("Пожалуйста, выберите ученика и дату",
                              "Предупреждение",
                              MessageBoxButton.OK,
                              MessageBoxImage.Warning);
                return;
            }

            var student = (Student)StudentComboBox.SelectedItem;
            var status = ((ComboBoxItem)StatusComboBox.SelectedItem).Tag.ToString()!;

            var attendance = new Attendance
            {
                StudentId = student.Id,
                Date = DatePicker.SelectedDate.Value,
                Status = status,
                Reason = ReasonTextBox.Text.Trim()
            };

            if (_databaseService.AddAttendance(attendance))
            {
                MessageBox.Show("Посещаемость успешно отмечена",
                              "Успех",
                              MessageBoxButton.OK,
                              MessageBoxImage.Information);
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Ошибка при добавлении записи",
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
