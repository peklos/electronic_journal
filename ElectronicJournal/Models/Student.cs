namespace ElectronicJournal.Models
{
    public class Student
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Class { get; set; } = string.Empty; // Класс (например "9-А")
        public DateTime BirthDate { get; set; }
        public string ParentPhone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
    }
}
