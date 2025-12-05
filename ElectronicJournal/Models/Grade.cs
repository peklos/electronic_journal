namespace ElectronicJournal.Models
{
    public class Grade
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int SubjectId { get; set; }
        public int GradeValue { get; set; } // Оценка от 2 до 5
        public DateTime Date { get; set; }
        public string Topic { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;

        // Навигационные свойства
        public string? StudentName { get; set; }
        public string? SubjectName { get; set; }
    }
}
