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
                TitleTextBlock.Text = "Редактировать студента";
                FullNameTextBox.Text = _studentToEdit.FullName;
                GroupTextBox.Text = _studentToEdit.Group;
                BirthDatePicker.SelectedDate = _studentToEdit.BirthDate;
                PhoneTextBox.Text = _studentToEdit.Phone;
                EmailTextBox.Text = _studentToEdit.Email;
                PassportTextBox.Text = _studentToEdit.Passport;
                AddressTextBox.Text = _studentToEdit.Address;
                ParentNameTextBox.Text = _studentToEdit.ParentName;
                ParentPhoneTextBox.Text = _studentToEdit.ParentPhone;
                ParentWorkplaceTextBox.Text = _studentToEdit.ParentWorkplace;
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
                string.IsNullOrWhiteSpace(GroupTextBox.Text) ||
                BirthDatePicker.SelectedDate == null)
            {
                MessageBox.Show("Пожалуйста, заполните все обязательные поля (ФИО, Группа, Дата рождения)",
                              "Предупреждение",
                              MessageBoxButton.OK,
                              MessageBoxImage.Warning);
                return;
            }

            var student = new Student
            {
                FullName = FullNameTextBox.Text.Trim(),
                Group = GroupTextBox.Text.Trim(),
                BirthDate = BirthDatePicker.SelectedDate.Value,
                Phone = PhoneTextBox.Text.Trim(),
                Email = EmailTextBox.Text.Trim(),
                Passport = PassportTextBox.Text.Trim(),
                Address = AddressTextBox.Text.Trim(),
                ParentName = ParentNameTextBox.Text.Trim(),
                ParentPhone = ParentPhoneTextBox.Text.Trim(),
                ParentWorkplace = ParentWorkplaceTextBox.Text.Trim(),
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
                MessageBox.Show(_isEditMode ? "Студент успешно обновлен" : "Студент успешно добавлен",
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
