using System;

namespace ElectronicJournal.Models
{
    public class Student
    {
        public int Id { get; set; }

        // Личные данные студента
        public string LastName { get; set; } = string.Empty; // Фамилия
        public string FirstName { get; set; } = string.Empty; // Имя
        public string MiddleName { get; set; } = string.Empty; // Отчество
        public string FullName => $"{LastName} {FirstName} {MiddleName}".Trim();

        public string Group { get; set; } = string.Empty; // Группа (например "ИС-301")
        public DateTime BirthDate { get; set; }
        public string Phone { get; set; } = string.Empty; // Телефон студента
        public string Address { get; set; } = string.Empty; // Адрес проживания

        // Данные матери
        public string MotherLastName { get; set; } = string.Empty;
        public string MotherFirstName { get; set; } = string.Empty;
        public string MotherMiddleName { get; set; } = string.Empty;
        public string MotherFullName => $"{MotherLastName} {MotherFirstName} {MotherMiddleName}".Trim();
        public string MotherPhone { get; set; } = string.Empty;
        public string MotherWorkplace { get; set; } = string.Empty; // Место работы матери

        // Данные отца
        public string FatherLastName { get; set; } = string.Empty;
        public string FatherFirstName { get; set; } = string.Empty;
        public string FatherMiddleName { get; set; } = string.Empty;
        public string FatherFullName => $"{FatherLastName} {FatherFirstName} {FatherMiddleName}".Trim();
        public string FatherPhone { get; set; } = string.Empty;
        public string FatherWorkplace { get; set; } = string.Empty; // Место работы отца

        // Дополнительно
        public string Notes { get; set; } = string.Empty;
    }
}
