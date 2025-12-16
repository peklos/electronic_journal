using System;

namespace ElectronicJournal.Models
{
    public class Student
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Group { get; set; } = string.Empty; // Группа (например "3ИС1-23")
        public DateTime BirthDate { get; set; }
        public string Phone { get; set; } = string.Empty; // Телефон студента
        public string Email { get; set; } = string.Empty; // Электронная почта студента
        public string Passport { get; set; } = string.Empty; // Паспортные данные
        public string Address { get; set; } = string.Empty;
        public string ParentName { get; set; } = string.Empty; // Имена родителей (мать/отец)
        public string ParentPhone { get; set; } = string.Empty; // Телефон родителей
        public string ParentWorkplace { get; set; } = string.Empty; // Место работы родителей
        public string Notes { get; set; } = string.Empty;
    }
}
