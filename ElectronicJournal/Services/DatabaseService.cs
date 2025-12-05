using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Data.SQLite;
using ElectronicJournal.Models;

namespace ElectronicJournal.Services
{
    public class DatabaseService
    {
        private readonly string _connectionString;

        public DatabaseService()
        {
            string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Database", "electronic_journal.db");
            string dbDirectory = Path.GetDirectoryName(dbPath)!;

            if (!Directory.Exists(dbDirectory))
            {
                Directory.CreateDirectory(dbDirectory);
            }

            _connectionString = $"Data Source={dbPath};Version=3;";
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            using var connection = new SQLiteConnection(_connectionString);
            connection.Open();

            string createTables = @"
                CREATE TABLE IF NOT EXISTS Users (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Username TEXT UNIQUE NOT NULL,
                    Password TEXT NOT NULL,
                    FullName TEXT NOT NULL,
                    Role TEXT NOT NULL,
                    StudentId INTEGER
                );

                CREATE TABLE IF NOT EXISTS Students (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    FullName TEXT NOT NULL,
                    Class TEXT NOT NULL,
                    BirthDate TEXT NOT NULL,
                    ParentPhone TEXT,
                    Address TEXT,
                    Notes TEXT
                );

                CREATE TABLE IF NOT EXISTS Subjects (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    TeacherName TEXT NOT NULL
                );

                CREATE TABLE IF NOT EXISTS Grades (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    StudentId INTEGER NOT NULL,
                    SubjectId INTEGER NOT NULL,
                    GradeValue INTEGER NOT NULL,
                    Date TEXT NOT NULL,
                    Topic TEXT,
                    Notes TEXT,
                    FOREIGN KEY (StudentId) REFERENCES Students(Id),
                    FOREIGN KEY (SubjectId) REFERENCES Subjects(Id)
                );

                CREATE TABLE IF NOT EXISTS Attendance (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    StudentId INTEGER NOT NULL,
                    Date TEXT NOT NULL,
                    Status TEXT NOT NULL,
                    Reason TEXT,
                    FOREIGN KEY (StudentId) REFERENCES Students(Id)
                );
            ";

            using var command = new SQLiteCommand(createTables, connection);
            command.ExecuteNonQuery();

            // Добавим тестового учителя, если база пустая
            string checkUsers = "SELECT COUNT(*) FROM Users";
            using var checkCommand = new SQLiteCommand(checkUsers, connection);
            long userCount = (long)checkCommand.ExecuteScalar()!;

            if (userCount == 0)
            {
                string insertDefaultData = @"
                    INSERT INTO Users (Username, Password, FullName, Role)
                    VALUES ('admin', 'admin', 'Администратор', 'Учитель');

                    INSERT INTO Subjects (Name, TeacherName) VALUES
                    ('Математика', 'Иванов И.И.'),
                    ('Русский язык', 'Петрова П.П.'),
                    ('Физика', 'Сидоров С.С.'),
                    ('История', 'Васильева В.В.'),
                    ('Английский язык', 'Смирнова С.С.');
                ";

                using var insertCommand = new SQLiteCommand(insertDefaultData, connection);
                insertCommand.ExecuteNonQuery();
            }
        }

        // Методы для работы с пользователями
        public User? AuthenticateUser(string username, string password)
        {
            using var connection = new SQLiteConnection(_connectionString);
            connection.Open();

            string query = "SELECT * FROM Users WHERE Username = @username AND Password = @password";
            using var command = new SQLiteCommand(query, connection);
            command.Parameters.AddWithValue("@username", username);
            command.Parameters.AddWithValue("@password", password);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new User
                {
                    Id = reader.GetInt32(0),
                    Username = reader.GetString(1),
                    Password = reader.GetString(2),
                    FullName = reader.GetString(3),
                    Role = reader.GetString(4),
                    StudentId = reader.IsDBNull(5) ? null : reader.GetInt32(5)
                };
            }

            return null;
        }

        public bool RegisterUser(string username, string password, string fullName, string role, int? studentId = null)
        {
            try
            {
                using var connection = new SQLiteConnection(_connectionString);
                connection.Open();

                string query = "INSERT INTO Users (Username, Password, FullName, Role, StudentId) VALUES (@username, @password, @fullName, @role, @studentId)";
                using var command = new SQLiteCommand(query, connection);
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@password", password);
                command.Parameters.AddWithValue("@fullName", fullName);
                command.Parameters.AddWithValue("@role", role);
                command.Parameters.AddWithValue("@studentId", studentId ?? (object)DBNull.Value);

                command.ExecuteNonQuery();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Методы для работы со студентами
        public List<Student> GetAllStudents()
        {
            var students = new List<Student>();

            using var connection = new SQLiteConnection(_connectionString);
            connection.Open();

            string query = "SELECT * FROM Students ORDER BY Class, FullName";
            using var command = new SQLiteCommand(query, connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                students.Add(new Student
                {
                    Id = reader.GetInt32(0),
                    FullName = reader.GetString(1),
                    Class = reader.GetString(2),
                    BirthDate = DateTime.Parse(reader.GetString(3)),
                    ParentPhone = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                    Address = reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
                    Notes = reader.IsDBNull(6) ? string.Empty : reader.GetString(6)
                });
            }

            return students;
        }

        public Student? GetStudentById(int id)
        {
            using var connection = new SQLiteConnection(_connectionString);
            connection.Open();

            string query = "SELECT * FROM Students WHERE Id = @id";
            using var command = new SQLiteCommand(query, connection);
            command.Parameters.AddWithValue("@id", id);
            using var reader = command.ExecuteReader();

            if (reader.Read())
            {
                return new Student
                {
                    Id = reader.GetInt32(0),
                    FullName = reader.GetString(1),
                    Class = reader.GetString(2),
                    BirthDate = DateTime.Parse(reader.GetString(3)),
                    ParentPhone = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                    Address = reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
                    Notes = reader.IsDBNull(6) ? string.Empty : reader.GetString(6)
                };
            }

            return null;
        }

        public bool AddStudent(Student student)
        {
            try
            {
                using var connection = new SQLiteConnection(_connectionString);
                connection.Open();

                string query = "INSERT INTO Students (FullName, Class, BirthDate, ParentPhone, Address, Notes) VALUES (@fullName, @class, @birthDate, @parentPhone, @address, @notes)";
                using var command = new SQLiteCommand(query, connection);
                command.Parameters.AddWithValue("@fullName", student.FullName);
                command.Parameters.AddWithValue("@class", student.Class);
                command.Parameters.AddWithValue("@birthDate", student.BirthDate.ToString("yyyy-MM-dd"));
                command.Parameters.AddWithValue("@parentPhone", student.ParentPhone);
                command.Parameters.AddWithValue("@address", student.Address);
                command.Parameters.AddWithValue("@notes", student.Notes);

                command.ExecuteNonQuery();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool UpdateStudent(Student student)
        {
            try
            {
                using var connection = new SQLiteConnection(_connectionString);
                connection.Open();

                string query = "UPDATE Students SET FullName = @fullName, Class = @class, BirthDate = @birthDate, ParentPhone = @parentPhone, Address = @address, Notes = @notes WHERE Id = @id";
                using var command = new SQLiteCommand(query, connection);
                command.Parameters.AddWithValue("@id", student.Id);
                command.Parameters.AddWithValue("@fullName", student.FullName);
                command.Parameters.AddWithValue("@class", student.Class);
                command.Parameters.AddWithValue("@birthDate", student.BirthDate.ToString("yyyy-MM-dd"));
                command.Parameters.AddWithValue("@parentPhone", student.ParentPhone);
                command.Parameters.AddWithValue("@address", student.Address);
                command.Parameters.AddWithValue("@notes", student.Notes);

                command.ExecuteNonQuery();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool DeleteStudent(int id)
        {
            try
            {
                using var connection = new SQLiteConnection(_connectionString);
                connection.Open();

                string query = "DELETE FROM Students WHERE Id = @id";
                using var command = new SQLiteCommand(query, connection);
                command.Parameters.AddWithValue("@id", id);

                command.ExecuteNonQuery();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Методы для работы с оценками
        public List<Grade> GetGradesByStudent(int studentId)
        {
            var grades = new List<Grade>();

            using var connection = new SQLiteConnection(_connectionString);
            connection.Open();

            string query = @"SELECT g.*, s.FullName, sub.Name
                           FROM Grades g
                           LEFT JOIN Students s ON g.StudentId = s.Id
                           LEFT JOIN Subjects sub ON g.SubjectId = sub.Id
                           WHERE g.StudentId = @studentId
                           ORDER BY g.Date DESC";
            using var command = new SQLiteCommand(query, connection);
            command.Parameters.AddWithValue("@studentId", studentId);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                grades.Add(new Grade
                {
                    Id = reader.GetInt32(0),
                    StudentId = reader.GetInt32(1),
                    SubjectId = reader.GetInt32(2),
                    GradeValue = reader.GetInt32(3),
                    Date = DateTime.Parse(reader.GetString(4)),
                    Topic = reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
                    Notes = reader.IsDBNull(6) ? string.Empty : reader.GetString(6),
                    StudentName = reader.IsDBNull(7) ? null : reader.GetString(7),
                    SubjectName = reader.IsDBNull(8) ? null : reader.GetString(8)
                });
            }

            return grades;
        }

        public List<Grade> GetAllGrades()
        {
            var grades = new List<Grade>();

            using var connection = new SQLiteConnection(_connectionString);
            connection.Open();

            string query = @"SELECT g.*, s.FullName, sub.Name
                           FROM Grades g
                           LEFT JOIN Students s ON g.StudentId = s.Id
                           LEFT JOIN Subjects sub ON g.SubjectId = sub.Id
                           ORDER BY g.Date DESC";
            using var command = new SQLiteCommand(query, connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                grades.Add(new Grade
                {
                    Id = reader.GetInt32(0),
                    StudentId = reader.GetInt32(1),
                    SubjectId = reader.GetInt32(2),
                    GradeValue = reader.GetInt32(3),
                    Date = DateTime.Parse(reader.GetString(4)),
                    Topic = reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
                    Notes = reader.IsDBNull(6) ? string.Empty : reader.GetString(6),
                    StudentName = reader.IsDBNull(7) ? null : reader.GetString(7),
                    SubjectName = reader.IsDBNull(8) ? null : reader.GetString(8)
                });
            }

            return grades;
        }

        public bool AddGrade(Grade grade)
        {
            try
            {
                using var connection = new SQLiteConnection(_connectionString);
                connection.Open();

                string query = "INSERT INTO Grades (StudentId, SubjectId, GradeValue, Date, Topic, Notes) VALUES (@studentId, @subjectId, @gradeValue, @date, @topic, @notes)";
                using var command = new SQLiteCommand(query, connection);
                command.Parameters.AddWithValue("@studentId", grade.StudentId);
                command.Parameters.AddWithValue("@subjectId", grade.SubjectId);
                command.Parameters.AddWithValue("@gradeValue", grade.GradeValue);
                command.Parameters.AddWithValue("@date", grade.Date.ToString("yyyy-MM-dd"));
                command.Parameters.AddWithValue("@topic", grade.Topic);
                command.Parameters.AddWithValue("@notes", grade.Notes);

                command.ExecuteNonQuery();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool DeleteGrade(int id)
        {
            try
            {
                using var connection = new SQLiteConnection(_connectionString);
                connection.Open();

                string query = "DELETE FROM Grades WHERE Id = @id";
                using var command = new SQLiteCommand(query, connection);
                command.Parameters.AddWithValue("@id", id);

                command.ExecuteNonQuery();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Методы для работы с посещаемостью
        public List<Attendance> GetAttendanceByStudent(int studentId)
        {
            var attendance = new List<Attendance>();

            using var connection = new SQLiteConnection(_connectionString);
            connection.Open();

            string query = @"SELECT a.*, s.FullName
                           FROM Attendance a
                           LEFT JOIN Students s ON a.StudentId = s.Id
                           WHERE a.StudentId = @studentId
                           ORDER BY a.Date DESC";
            using var command = new SQLiteCommand(query, connection);
            command.Parameters.AddWithValue("@studentId", studentId);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                attendance.Add(new Attendance
                {
                    Id = reader.GetInt32(0),
                    StudentId = reader.GetInt32(1),
                    Date = DateTime.Parse(reader.GetString(2)),
                    Status = reader.GetString(3),
                    Reason = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                    StudentName = reader.IsDBNull(5) ? null : reader.GetString(5)
                });
            }

            return attendance;
        }

        public List<Attendance> GetAllAttendance()
        {
            var attendance = new List<Attendance>();

            using var connection = new SQLiteConnection(_connectionString);
            connection.Open();

            string query = @"SELECT a.*, s.FullName
                           FROM Attendance a
                           LEFT JOIN Students s ON a.StudentId = s.Id
                           ORDER BY a.Date DESC";
            using var command = new SQLiteCommand(query, connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                attendance.Add(new Attendance
                {
                    Id = reader.GetInt32(0),
                    StudentId = reader.GetInt32(1),
                    Date = DateTime.Parse(reader.GetString(2)),
                    Status = reader.GetString(3),
                    Reason = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                    StudentName = reader.IsDBNull(5) ? null : reader.GetString(5)
                });
            }

            return attendance;
        }

        public bool AddAttendance(Attendance attendance)
        {
            try
            {
                using var connection = new SQLiteConnection(_connectionString);
                connection.Open();

                string query = "INSERT INTO Attendance (StudentId, Date, Status, Reason) VALUES (@studentId, @date, @status, @reason)";
                using var command = new SQLiteCommand(query, connection);
                command.Parameters.AddWithValue("@studentId", attendance.StudentId);
                command.Parameters.AddWithValue("@date", attendance.Date.ToString("yyyy-MM-dd"));
                command.Parameters.AddWithValue("@status", attendance.Status);
                command.Parameters.AddWithValue("@reason", attendance.Reason);

                command.ExecuteNonQuery();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool DeleteAttendance(int id)
        {
            try
            {
                using var connection = new SQLiteConnection(_connectionString);
                connection.Open();

                string query = "DELETE FROM Attendance WHERE Id = @id";
                using var command = new SQLiteCommand(query, connection);
                command.Parameters.AddWithValue("@id", id);

                command.ExecuteNonQuery();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Методы для работы с предметами
        public List<Subject> GetAllSubjects()
        {
            var subjects = new List<Subject>();

            using var connection = new SQLiteConnection(_connectionString);
            connection.Open();

            string query = "SELECT * FROM Subjects ORDER BY Name";
            using var command = new SQLiteCommand(query, connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                subjects.Add(new Subject
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    TeacherName = reader.GetString(2)
                });
            }

            return subjects;
        }
    }
}
