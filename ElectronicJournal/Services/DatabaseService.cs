using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Data.Sqlite;
using ElectronicJournal.Models;

namespace ElectronicJournal.Services
{
    public class DatabaseService
    {
        private readonly string _connectionString;

        public DatabaseService()
        {
            try
            {
                string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Database", "college_ais.db");
                string dbDirectory = Path.GetDirectoryName(dbPath)!;

                if (!Directory.Exists(dbDirectory))
                {
                    Directory.CreateDirectory(dbDirectory);
                }

                _connectionString = $"Data Source={dbPath}";
                InitializeDatabase();
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка инициализации базы данных: {ex.Message}", ex);
            }
        }

        private void InitializeDatabase()
        {
            using var connection = new SqliteConnection(_connectionString);
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
                    LastName TEXT NOT NULL,
                    FirstName TEXT NOT NULL,
                    MiddleName TEXT,
                    [Group] TEXT NOT NULL,
                    BirthDate TEXT NOT NULL,
                    Phone TEXT,
                    Address TEXT,
                    MotherLastName TEXT,
                    MotherFirstName TEXT,
                    MotherMiddleName TEXT,
                    MotherPhone TEXT,
                    MotherWorkplace TEXT,
                    FatherLastName TEXT,
                    FatherFirstName TEXT,
                    FatherMiddleName TEXT,
                    FatherPhone TEXT,
                    FatherWorkplace TEXT,
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

            using var command = new SqliteCommand(createTables, connection);
            command.ExecuteNonQuery();

            // Добавим тестовые данные, если база пустая
            string checkUsers = "SELECT COUNT(*) FROM Users";
            using var checkCommand = new SqliteCommand(checkUsers, connection);
            long userCount = (long)checkCommand.ExecuteScalar()!;

            if (userCount == 0)
            {
                string insertDefaultData = @"
                    INSERT INTO Users (Username, Password, FullName, Role)
                    VALUES ('admin', 'admin', 'Петрова Елена Сергеевна', 'Учитель');

                    INSERT INTO Subjects (Name, TeacherName) VALUES
                    ('Математика', 'Иванов И.И.'),
                    ('Русский язык', 'Петрова Е.С.'),
                    ('Информатика', 'Сидоров А.В.'),
                    ('История', 'Васильева М.А.'),
                    ('Английский язык', 'Смирнова О.П.');

                    INSERT INTO Students (LastName, FirstName, MiddleName, [Group], BirthDate, Phone, Address,
                        MotherLastName, MotherFirstName, MotherMiddleName, MotherPhone, MotherWorkplace,
                        FatherLastName, FatherFirstName, FatherMiddleName, FatherPhone, FatherWorkplace, Notes) VALUES
                    ('Иванов', 'Петр', 'Сергеевич', 'ИС-301', '2005-05-15', '+7 900 111-22-33', 'г. Москва, ул. Ленина, д. 10, кв. 5',
                        'Иванова', 'Елена', 'Владимировна', '+7 999 123-45-67', 'Школа №15, учитель начальных классов',
                        'Иванов', 'Сергей', 'Петрович', '+7 999 765-43-21', 'ООО Техностар, инженер-программист', 'Отличник, староста группы'),
                    ('Петрова', 'Мария', 'Ивановна', 'ИС-301', '2005-08-22', '+7 900 222-33-44', 'г. Москва, ул. Пушкина, д. 25, кв. 12',
                        'Петрова', 'Ольга', 'Сергеевна', '+7 999 234-56-78', 'Поликлиника №3, врач-терапевт',
                        'Петров', 'Иван', 'Николаевич', '+7 999 876-54-32', 'Банк Открытие, старший менеджер', 'Активная студентка'),
                    ('Сидоров', 'Александр', 'Петрович', 'ПР-201', '2006-03-10', '+7 900 333-44-55', 'г. Москва, ул. Гагарина, д. 5, кв. 8',
                        'Сидорова', 'Наталья', 'Ивановна', '+7 999 345-67-89', 'Магазин Пятерочка, продавец-кассир',
                        'Сидоров', 'Петр', 'Александрович', '+7 999 987-65-43', 'Завод Прогресс, мастер цеха', 'Спортсмен, член сборной'),
                    ('Козлова', 'Анна', 'Дмитриевна', 'ИС-101', '2007-11-30', '+7 900 444-55-66', 'г. Москва, ул. Мира, д. 15, кв. 20',
                        'Козлова', 'Татьяна', 'Сергеевна', '+7 999 456-78-90', 'Салон красоты Элегант, парикмахер-стилист',
                        'Козлов', 'Дмитрий', 'Викторович', '+7 999 098-76-54', 'ИП Козлов, директор', 'Творческая личность'),
                    ('Смирнов', 'Дмитрий', 'Андреевич', 'ИС-301', '2005-07-18', '+7 900 555-66-77', 'г. Москва, ул. Советская, д. 8, кв. 3',
                        'Смирнова', 'Людмила', 'Петровна', '+7 999 567-89-01', 'Детский сад №10, воспитатель',
                        'Смирнов', 'Андрей', 'Дмитриевич', '+7 999 109-87-65', 'АО Энергосбыт, электромонтер', 'Хороший студент');

                    INSERT INTO Grades (StudentId, SubjectId, GradeValue, Date, Topic, Notes) VALUES
                    (1, 1, 5, '2025-12-01', 'Квадратные уравнения', 'Отлично решил все задачи'),
                    (1, 2, 5, '2025-12-02', 'Сложноподчиненные предложения', 'Прекрасный ответ'),
                    (1, 3, 5, '2025-12-03', 'Базы данных SQL', 'Отличная работа'),
                    (2, 1, 4, '2025-12-01', 'Квадратные уравнения', 'Хорошо'),
                    (2, 2, 5, '2025-12-02', 'Сложноподчиненные предложения', 'Отлично'),
                    (2, 4, 5, '2025-12-04', 'Великая Отечественная война', 'Отличный доклад'),
                    (3, 1, 3, '2025-12-01', 'Квадратные уравнения', 'Нужно подтянуть'),
                    (3, 3, 4, '2025-12-03', 'Базы данных SQL', 'Хорошо'),
                    (4, 1, 4, '2025-12-01', 'Квадратные уравнения', 'Хорошо'),
                    (4, 2, 5, '2025-12-02', 'Сложноподчиненные предложения', 'Отлично'),
                    (4, 5, 5, '2025-12-05', 'Present Perfect', 'Excellent work'),
                    (5, 1, 4, '2025-12-01', 'Квадратные уравнения', 'Хорошо'),
                    (5, 2, 4, '2025-12-02', 'Сложноподчиненные предложения', 'Хорошо');

                    INSERT INTO Attendance (StudentId, Date, Status, Reason) VALUES
                    (1, '2025-12-01', 'Присутствовал', ''),
                    (1, '2025-12-02', 'Присутствовал', ''),
                    (1, '2025-12-03', 'Присутствовал', ''),
                    (2, '2025-12-01', 'Присутствовал', ''),
                    (2, '2025-12-02', 'Присутствовал', ''),
                    (2, '2025-12-03', 'По уважительной причине', 'Справка от врача'),
                    (3, '2025-12-01', 'Присутствовал', ''),
                    (3, '2025-12-02', 'Отсутствовал', 'Без уважительной причины'),
                    (3, '2025-12-03', 'Присутствовал', ''),
                    (4, '2025-12-01', 'Присутствовал', ''),
                    (4, '2025-12-02', 'Присутствовал', ''),
                    (4, '2025-12-03', 'Присутствовал', ''),
                    (5, '2025-12-01', 'Присутствовал', ''),
                    (5, '2025-12-02', 'Присутствовал', ''),
                    (5, '2025-12-03', 'Присутствовал', '');

                    INSERT INTO Users (Username, Password, FullName, Role, StudentId) VALUES
                    ('ivanov', 'ivanov', 'Иванов Петр Сергеевич', 'Студент', 1);
                ";

                using var insertCommand = new SqliteCommand(insertDefaultData, connection);
                insertCommand.ExecuteNonQuery();
            }
        }

        // Методы для работы с пользователями
        public User? AuthenticateUser(string username, string password)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            string query = "SELECT * FROM Users WHERE Username = @username AND Password = @password";
            using var command = new SqliteCommand(query, connection);
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
                using var connection = new SqliteConnection(_connectionString);
                connection.Open();

                string query = "INSERT INTO Users (Username, Password, FullName, Role, StudentId) VALUES (@username, @password, @fullName, @role, @studentId)";
                using var command = new SqliteCommand(query, connection);
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

            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            string query = "SELECT * FROM Students ORDER BY [Group], LastName, FirstName";
            using var command = new SqliteCommand(query, connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                students.Add(ReadStudent(reader));
            }

            return students;
        }

        public Student? GetStudentById(int id)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            string query = "SELECT * FROM Students WHERE Id = @id";
            using var command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@id", id);
            using var reader = command.ExecuteReader();

            if (reader.Read())
            {
                return ReadStudent(reader);
            }

            return null;
        }

        private Student ReadStudent(SqliteDataReader reader)
        {
            return new Student
            {
                Id = reader.GetInt32(0),
                LastName = reader.GetString(1),
                FirstName = reader.GetString(2),
                MiddleName = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                Group = reader.GetString(4),
                BirthDate = DateTime.Parse(reader.GetString(5)),
                Phone = reader.IsDBNull(6) ? string.Empty : reader.GetString(6),
                Address = reader.IsDBNull(7) ? string.Empty : reader.GetString(7),
                MotherLastName = reader.IsDBNull(8) ? string.Empty : reader.GetString(8),
                MotherFirstName = reader.IsDBNull(9) ? string.Empty : reader.GetString(9),
                MotherMiddleName = reader.IsDBNull(10) ? string.Empty : reader.GetString(10),
                MotherPhone = reader.IsDBNull(11) ? string.Empty : reader.GetString(11),
                MotherWorkplace = reader.IsDBNull(12) ? string.Empty : reader.GetString(12),
                FatherLastName = reader.IsDBNull(13) ? string.Empty : reader.GetString(13),
                FatherFirstName = reader.IsDBNull(14) ? string.Empty : reader.GetString(14),
                FatherMiddleName = reader.IsDBNull(15) ? string.Empty : reader.GetString(15),
                FatherPhone = reader.IsDBNull(16) ? string.Empty : reader.GetString(16),
                FatherWorkplace = reader.IsDBNull(17) ? string.Empty : reader.GetString(17),
                Notes = reader.IsDBNull(18) ? string.Empty : reader.GetString(18)
            };
        }

        public bool AddStudent(Student student)
        {
            try
            {
                using var connection = new SqliteConnection(_connectionString);
                connection.Open();

                string query = @"INSERT INTO Students (LastName, FirstName, MiddleName, [Group], BirthDate, Phone, Address,
                    MotherLastName, MotherFirstName, MotherMiddleName, MotherPhone, MotherWorkplace,
                    FatherLastName, FatherFirstName, FatherMiddleName, FatherPhone, FatherWorkplace, Notes)
                    VALUES (@lastName, @firstName, @middleName, @group, @birthDate, @phone, @address,
                    @motherLastName, @motherFirstName, @motherMiddleName, @motherPhone, @motherWorkplace,
                    @fatherLastName, @fatherFirstName, @fatherMiddleName, @fatherPhone, @fatherWorkplace, @notes)";

                using var command = new SqliteCommand(query, connection);
                AddStudentParameters(command, student);

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
                using var connection = new SqliteConnection(_connectionString);
                connection.Open();

                string query = @"UPDATE Students SET
                    LastName = @lastName, FirstName = @firstName, MiddleName = @middleName,
                    [Group] = @group, BirthDate = @birthDate, Phone = @phone, Address = @address,
                    MotherLastName = @motherLastName, MotherFirstName = @motherFirstName, MotherMiddleName = @motherMiddleName,
                    MotherPhone = @motherPhone, MotherWorkplace = @motherWorkplace,
                    FatherLastName = @fatherLastName, FatherFirstName = @fatherFirstName, FatherMiddleName = @fatherMiddleName,
                    FatherPhone = @fatherPhone, FatherWorkplace = @fatherWorkplace, Notes = @notes
                    WHERE Id = @id";

                using var command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue("@id", student.Id);
                AddStudentParameters(command, student);

                command.ExecuteNonQuery();
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void AddStudentParameters(SqliteCommand command, Student student)
        {
            command.Parameters.AddWithValue("@lastName", student.LastName);
            command.Parameters.AddWithValue("@firstName", student.FirstName);
            command.Parameters.AddWithValue("@middleName", student.MiddleName);
            command.Parameters.AddWithValue("@group", student.Group);
            command.Parameters.AddWithValue("@birthDate", student.BirthDate.ToString("yyyy-MM-dd"));
            command.Parameters.AddWithValue("@phone", student.Phone);
            command.Parameters.AddWithValue("@address", student.Address);
            command.Parameters.AddWithValue("@motherLastName", student.MotherLastName);
            command.Parameters.AddWithValue("@motherFirstName", student.MotherFirstName);
            command.Parameters.AddWithValue("@motherMiddleName", student.MotherMiddleName);
            command.Parameters.AddWithValue("@motherPhone", student.MotherPhone);
            command.Parameters.AddWithValue("@motherWorkplace", student.MotherWorkplace);
            command.Parameters.AddWithValue("@fatherLastName", student.FatherLastName);
            command.Parameters.AddWithValue("@fatherFirstName", student.FatherFirstName);
            command.Parameters.AddWithValue("@fatherMiddleName", student.FatherMiddleName);
            command.Parameters.AddWithValue("@fatherPhone", student.FatherPhone);
            command.Parameters.AddWithValue("@fatherWorkplace", student.FatherWorkplace);
            command.Parameters.AddWithValue("@notes", student.Notes);
        }

        public bool DeleteStudent(int id)
        {
            try
            {
                using var connection = new SqliteConnection(_connectionString);
                connection.Open();

                // Сначала удаляем связанные оценки
                string deleteGrades = "DELETE FROM Grades WHERE StudentId = @id";
                using (var cmd = new SqliteCommand(deleteGrades, connection))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }

                // Затем удаляем связанную посещаемость
                string deleteAttendance = "DELETE FROM Attendance WHERE StudentId = @id";
                using (var cmd = new SqliteCommand(deleteAttendance, connection))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }

                // Удаляем связанного пользователя (если есть)
                string deleteUser = "DELETE FROM Users WHERE StudentId = @id";
                using (var cmd = new SqliteCommand(deleteUser, connection))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }

                // Наконец удаляем самого студента
                string query = "DELETE FROM Students WHERE Id = @id";
                using var command = new SqliteCommand(query, connection);
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

            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            string query = @"SELECT g.*, (s.LastName || ' ' || s.FirstName || ' ' || COALESCE(s.MiddleName, '')) as StudentName, sub.Name
                           FROM Grades g
                           LEFT JOIN Students s ON g.StudentId = s.Id
                           LEFT JOIN Subjects sub ON g.SubjectId = sub.Id
                           WHERE g.StudentId = @studentId
                           ORDER BY g.Date DESC";
            using var command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@studentId", studentId);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                grades.Add(ReadGrade(reader));
            }

            return grades;
        }

        public List<Grade> GetAllGrades()
        {
            var grades = new List<Grade>();

            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            string query = @"SELECT g.*, (s.LastName || ' ' || s.FirstName || ' ' || COALESCE(s.MiddleName, '')) as StudentName, sub.Name
                           FROM Grades g
                           LEFT JOIN Students s ON g.StudentId = s.Id
                           LEFT JOIN Subjects sub ON g.SubjectId = sub.Id
                           ORDER BY g.Date DESC";
            using var command = new SqliteCommand(query, connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                grades.Add(ReadGrade(reader));
            }

            return grades;
        }

        private Grade ReadGrade(SqliteDataReader reader)
        {
            return new Grade
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
            };
        }

        public bool AddGrade(Grade grade)
        {
            try
            {
                using var connection = new SqliteConnection(_connectionString);
                connection.Open();

                string query = "INSERT INTO Grades (StudentId, SubjectId, GradeValue, Date, Topic, Notes) VALUES (@studentId, @subjectId, @gradeValue, @date, @topic, @notes)";
                using var command = new SqliteCommand(query, connection);
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
                using var connection = new SqliteConnection(_connectionString);
                connection.Open();

                string query = "DELETE FROM Grades WHERE Id = @id";
                using var command = new SqliteCommand(query, connection);
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

            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            string query = @"SELECT a.*, (s.LastName || ' ' || s.FirstName || ' ' || COALESCE(s.MiddleName, '')) as StudentName
                           FROM Attendance a
                           LEFT JOIN Students s ON a.StudentId = s.Id
                           WHERE a.StudentId = @studentId
                           ORDER BY a.Date DESC";
            using var command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@studentId", studentId);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                attendance.Add(ReadAttendance(reader));
            }

            return attendance;
        }

        public List<Attendance> GetAllAttendance()
        {
            var attendance = new List<Attendance>();

            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            string query = @"SELECT a.*, (s.LastName || ' ' || s.FirstName || ' ' || COALESCE(s.MiddleName, '')) as StudentName
                           FROM Attendance a
                           LEFT JOIN Students s ON a.StudentId = s.Id
                           ORDER BY a.Date DESC";
            using var command = new SqliteCommand(query, connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                attendance.Add(ReadAttendance(reader));
            }

            return attendance;
        }

        private Attendance ReadAttendance(SqliteDataReader reader)
        {
            return new Attendance
            {
                Id = reader.GetInt32(0),
                StudentId = reader.GetInt32(1),
                Date = DateTime.Parse(reader.GetString(2)),
                Status = reader.GetString(3),
                Reason = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                StudentName = reader.IsDBNull(5) ? null : reader.GetString(5)
            };
        }

        public bool AddAttendance(Attendance attendance)
        {
            try
            {
                using var connection = new SqliteConnection(_connectionString);
                connection.Open();

                string query = "INSERT INTO Attendance (StudentId, Date, Status, Reason) VALUES (@studentId, @date, @status, @reason)";
                using var command = new SqliteCommand(query, connection);
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
                using var connection = new SqliteConnection(_connectionString);
                connection.Open();

                string query = "DELETE FROM Attendance WHERE Id = @id";
                using var command = new SqliteCommand(query, connection);
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

            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            string query = "SELECT * FROM Subjects ORDER BY Name";
            using var command = new SqliteCommand(query, connection);
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
