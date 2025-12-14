using System;

namespace ElectronicJournal.Models
{
    public class Student
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Group { get; set; } = string.Empty; // Группа (например "3ИС1-23")
        public DateTime BirthDate { get; set; }
        public string ParentPhone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
    }
}
