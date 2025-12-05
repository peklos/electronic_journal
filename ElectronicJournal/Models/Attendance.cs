using System;

namespace ElectronicJournal.Models
{
    public class Attendance
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public DateTime Date { get; set; }
        public string Status { get; set; } = string.Empty; // "Присутствовал", "Отсутствовал", "По уважительной причине"
        public string Reason { get; set; } = string.Empty;

        // Навигационное свойство
        public string? StudentName { get; set; }
    }
}
