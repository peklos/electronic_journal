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
                TitleTextBlock.Text = "Редактирование данных студента";

                // Личные данные студента
                LastNameTextBox.Text = _studentToEdit.LastName;
                FirstNameTextBox.Text = _studentToEdit.FirstName;
                MiddleNameTextBox.Text = _studentToEdit.MiddleName;
                GroupTextBox.Text = _studentToEdit.Group;
                BirthDatePicker.SelectedDate = _studentToEdit.BirthDate;
                PhoneTextBox.Text = _studentToEdit.Phone;
                AddressTextBox.Text = _studentToEdit.Address;

                // Данные матери
                MotherLastNameTextBox.Text = _studentToEdit.MotherLastName;
                MotherFirstNameTextBox.Text = _studentToEdit.MotherFirstName;
                MotherMiddleNameTextBox.Text = _studentToEdit.MotherMiddleName;
                MotherPhoneTextBox.Text = _studentToEdit.MotherPhone;
                MotherWorkplaceTextBox.Text = _studentToEdit.MotherWorkplace;

                // Данные отца
                FatherLastNameTextBox.Text = _studentToEdit.FatherLastName;
                FatherFirstNameTextBox.Text = _studentToEdit.FatherFirstName;
                FatherMiddleNameTextBox.Text = _studentToEdit.FatherMiddleName;
                FatherPhoneTextBox.Text = _studentToEdit.FatherPhone;
                FatherWorkplaceTextBox.Text = _studentToEdit.FatherWorkplace;

                // Дополнительно
                NotesTextBox.Text = _studentToEdit.Notes;
            }
            else
            {
                BirthDatePicker.SelectedDate = DateTime.Now.AddYears(-17);
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Проверка обязательных полей
            if (string.IsNullOrWhiteSpace(LastNameTextBox.Text) ||
                string.IsNullOrWhiteSpace(FirstNameTextBox.Text) ||
                string.IsNullOrWhiteSpace(GroupTextBox.Text) ||
                BirthDatePicker.SelectedDate == null)
            {
                MessageBox.Show("Пожалуйста, заполните все обязательные поля:\n- Фамилия\n- Имя\n- Группа\n- Дата рождения",
                              "Внимание",
                              MessageBoxButton.OK,
                              MessageBoxImage.Warning);
                return;
            }

            var student = new Student
            {
                // Личные данные студента
                LastName = LastNameTextBox.Text.Trim(),
                FirstName = FirstNameTextBox.Text.Trim(),
                MiddleName = MiddleNameTextBox.Text.Trim(),
                Group = GroupTextBox.Text.Trim(),
                BirthDate = BirthDatePicker.SelectedDate.Value,
                Phone = PhoneTextBox.Text.Trim(),
                Address = AddressTextBox.Text.Trim(),

                // Данные матери
                MotherLastName = MotherLastNameTextBox.Text.Trim(),
                MotherFirstName = MotherFirstNameTextBox.Text.Trim(),
                MotherMiddleName = MotherMiddleNameTextBox.Text.Trim(),
                MotherPhone = MotherPhoneTextBox.Text.Trim(),
                MotherWorkplace = MotherWorkplaceTextBox.Text.Trim(),

                // Данные отца
                FatherLastName = FatherLastNameTextBox.Text.Trim(),
                FatherFirstName = FatherFirstNameTextBox.Text.Trim(),
                FatherMiddleName = FatherMiddleNameTextBox.Text.Trim(),
                FatherPhone = FatherPhoneTextBox.Text.Trim(),
                FatherWorkplace = FatherWorkplaceTextBox.Text.Trim(),

                // Дополнительно
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
                MessageBox.Show(_isEditMode ? "Данные студента успешно обновлены" : "Студент успешно добавлен в систему",
                              "Успешно",
                              MessageBoxButton.OK,
                              MessageBoxImage.Information);
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Произошла ошибка при сохранении данных. Попробуйте еще раз.",
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
