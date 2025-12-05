using System;
using System.Windows;
using ElectronicJournal.Models;
using ElectronicJournal.Services;

namespace ElectronicJournal.Views
{
    public partial class AddEditStudentDialog : Window
    {
        private readonly DatabaseService _databaseService;
        private readonly Student? _studentToEdit;
        private readonly bool _isEditMode;

        public AddEditStudentDialog(Student? student = null)
        {
            InitializeComponent();
            _databaseService = new DatabaseService();
            _studentToEdit = student;
            _isEditMode = student != null;

            if (_isEditMode && _studentToEdit != null)
            {
                TitleTextBlock.Text = "Редактировать ученика";
                FullNameTextBox.Text = _studentToEdit.FullName;
                ClassTextBox.Text = _studentToEdit.Class;
                BirthDatePicker.SelectedDate = _studentToEdit.BirthDate;
                ParentPhoneTextBox.Text = _studentToEdit.ParentPhone;
                AddressTextBox.Text = _studentToEdit.Address;
                NotesTextBox.Text = _studentToEdit.Notes;
            }
            else
            {
                BirthDatePicker.SelectedDate = DateTime.Now.AddYears(-15);
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(FullNameTextBox.Text) ||
                string.IsNullOrWhiteSpace(ClassTextBox.Text) ||
                BirthDatePicker.SelectedDate == null)
            {
                MessageBox.Show("Пожалуйста, заполните все обязательные поля (ФИО, Класс, Дата рождения)",
                              "Предупреждение",
                              MessageBoxButton.OK,
                              MessageBoxImage.Warning);
                return;
            }

            var student = new Student
            {
                FullName = FullNameTextBox.Text.Trim(),
                Class = ClassTextBox.Text.Trim(),
                BirthDate = BirthDatePicker.SelectedDate.Value,
                ParentPhone = ParentPhoneTextBox.Text.Trim(),
                Address = AddressTextBox.Text.Trim(),
                Notes = NotesTextBox.Text.Trim()
            };

            bool success;
            if (_isEditMode && _studentToEdit != null)
            {
                student.Id = _studentToEdit.Id;
                success = _databaseService.UpdateStudent(student);
            }
            else
            {
                success = _databaseService.AddStudent(student);
            }

            if (success)
            {
                MessageBox.Show(_isEditMode ? "Ученик успешно обновлен" : "Ученик успешно добавлен",
                              "Успех",
                              MessageBoxButton.OK,
                              MessageBoxImage.Information);
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Ошибка при сохранении данных",
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
