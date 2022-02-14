using DataAccessLibrary.Databases;
using DataAccessLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.Data
{
    public class SqliteData : ISqliteData
    {
        private readonly ISqliteDataAccess _db;
        private const string connectionStringName = "Default";

        public SqliteData(ISqliteDataAccess db)
        {
            _db = db;
        }

        public List<EmployeeModel> GetAllEmployees()
        {
            string sql = "SELECT Id, FullName FROM Employees ORDER BY FullName;";

            return _db.LoadData<EmployeeModel, dynamic>(sql, new { }, connectionStringName);
        }

        public void CreateEmployee(string employeeName)
        {
            string sql = "INSERT INTO Employees (FullName) VALUES(@FullName);";

            _db.SaveData(sql, new { FullName = employeeName }, connectionStringName);
        }

        public void DeleteEmployee(int id)
        {
            string sql = "DELETE FROM Employees WHERE Id = @Id;";

            _db.SaveData(sql, new { Id = id }, connectionStringName);
        }

        public void CreateRotation(FullRotationModel fullRotation)
        {
            string sql = "INSERT INTO Rotations (RotationName, RotationRecurrence, NextDateTimeRotationAdvances, AdvanceAutomatically, Notes) " +
                            "VALUES(@RotationName, @RecurrenceInterval, @NextDateTimeRotationAdvances, @AdvanceAutomatically, @Notes);";

            _db.SaveData(sql, new { RotationName = fullRotation.BasicInfo.RotationName,
                                    RecurrenceInterval = fullRotation.BasicInfo.RotationRecurrence,
                                    NextDateTimeRotationAdvances = fullRotation.BasicInfo.NextDateTimeRotationAdvances,
                                    AdvanceAutomatically = fullRotation.BasicInfo.AdvanceAutomatically,
                                    Notes = fullRotation.BasicInfo.Notes},
                                    connectionStringName);

            sql = "SELECT Id FROM Rotations ORDER BY Id DESC LIMIT 1;"; // select the id of the row just inserted

            int rotationId = _db.LoadData<int, dynamic>(sql, new { }, connectionStringName).First();

            for (int i = 0; i < fullRotation.RotationOfEmployees.Count; i++)
            {
                sql = "INSERT INTO RotationEmployees (RotationId, EmployeeId, Position) " +
                        "VALUES(@RotationId, @EmployeeId, @Position);";

                _db.SaveData(sql, new { RotationId = rotationId,
                                        EmployeeId = fullRotation.RotationOfEmployees[i].Id,
                                        Position = i },
                                        connectionStringName);
            }

            sql = "INSERT INTO CalendarEvents (RotationId, EmployeeId, NextStartDateTime, NextEndDateTime, OnCalendar) " +
                    "VALUES (@RotationId, @EmployeeId, @StartDateTime, @EndDateTime, 0);";

            foreach (var employee in fullRotation.RotationOfEmployees)
            {
                _db.SaveData(sql, new { RotationId = rotationId,
                                        EmployeeId = employee.Id,
                                        StartDateTime = employee.NextStartDateTime,
                                        EndDateTime = employee.NextEndDateTime },
                                        connectionStringName);
            }
        }

        public void RecreateRotationOfEmployees(FullRotationModel fullRotation)
        {
            int rotationId = fullRotation.BasicInfo.Id;

            string sql = "DELETE FROM RotationEmployees WHERE RotationId = @RotationId;";

            _db.SaveData(sql, new { RotationId = rotationId }, connectionStringName);

            int position = 0;
            foreach (var employee in fullRotation.RotationOfEmployees)
            {
                sql = "INSERT INTO RotationEmployees(RotationId, EmployeeId, Position) " +
                        "VALUES(@RotationId, @EmployeeId, @Position);";

                _db.SaveData(sql, new { RotationId = rotationId,
                                        EmployeeId = employee.Id,
                                        Position = position },
                                        connectionStringName);

                position++;
            }
        }

        public List<FullRotationModel> GetAllRotations()
        {
            List<FullRotationModel> output = new();

            string sql = "SELECT Id FROM Rotations;";

            List<int> rotationIds = _db.LoadData<int, dynamic>(sql, new { }, connectionStringName);

            foreach (var id in rotationIds)
            {
                FullRotationModel fullRotation = new();

                sql = "SELECT * FROM Rotations WHERE Id = @Id;";

                fullRotation.BasicInfo = _db.LoadData<BasicRotationModel, dynamic>(sql, new { Id = id }, connectionStringName).First();

                sql = "SELECT e.* FROM Employees e " +
                        "INNER JOIN RotationEmployees re ON e.Id = re.EmployeeId " +
                        "INNER JOIN Rotations r ON re.RotationId = r.Id " +
                        "WHERE r.Id = @Id " +
                        "ORDER BY re.Position;";

                fullRotation.RotationOfEmployees = _db.LoadData<EmployeeModel, dynamic>(sql, new { Id = id }, connectionStringName);

                output.Add(fullRotation);
            }

            return output;
        }

        public int GetHighestIdFromRotations()
        {
            int output;
            
            string sql = "SELECT Id FROM Rotations ORDER BY Id DESC LIMIT 1;";

            output = _db.LoadData<int, dynamic>(sql, new { }, connectionStringName).First();

            return output;
        }

        public void UpdateRotationBasicInfo(BasicRotationModel basicRotation)
        {
            string sql = "UPDATE Rotations SET RotationName = @RotationName, RotationRecurrence = @RotationRecurrence, " +
                            "NextDateTimeRotationAdvances = @NextDateTimeRotationAdvances, " +
                            "AdvanceAutomatically = @AdvanceAutomatically, Notes = @Notes WHERE Id = @Id;";

            _db.SaveData(sql, new { basicRotation.RotationName,
                                    basicRotation.RotationRecurrence,
                                    basicRotation.NextDateTimeRotationAdvances,
                                    basicRotation.AdvanceAutomatically,
                                    basicRotation.Notes,
                                    basicRotation.Id },
                                    connectionStringName);
        }

        public void AdvanceRotation(FullRotationModel fullRotation)
        {
            int rotationId = fullRotation.BasicInfo.Id;

            string sql = "SELECT EmployeeId FROM RotationEmployees WHERE RotationId = @RotationId AND Position = 0;";

            int employeeId = _db.LoadData<int, dynamic>(sql, new { RotationId = rotationId }, connectionStringName).First();

            sql = "UPDATE RotationEmployees SET Position = " +
                    "(SELECT count(*) FROM RotationEmployees WHERE RotationId = @RotationId) " +
                    "WHERE EmployeeId = @EmployeeId AND RotationId = @RotationId; " +
                    "UPDATE RotationEmployees SET Position = Position - 1 WHERE RotationId = @RotationId;";

            _db.SaveData(sql, new { RotationId = rotationId, EmployeeId = employeeId }, connectionStringName);
        }

        public void ReverseRotation(FullRotationModel fullRotation) // moves the rotation back one position in the list
        {
            int rotationId = fullRotation.BasicInfo.Id;
            
            // select the id of the last employee in the rotation
            string sql = "SELECT EmployeeId FROM RotationEmployees WHERE RotationId = @RotationId AND Position = " +
                            "(SELECT Position FROM RotationEmployees WHERE RotationId = @RotationId ORDER BY Position DESC LIMIT 1);";

            int employeeId = _db.LoadData<int, dynamic>(sql, new { RotationId = rotationId }, connectionStringName).First();

            sql = "UPDATE RotationEmployees SET Position = -1 WHERE RotationId = @RotationId AND EmployeeId = @EmployeeId; " +
                    "UPDATE RotationEmployees SET Position = Position + 1 WHERE RotationId = @RotationId;";

            _db.SaveData(sql, new { EmployeeId = employeeId, RotationId = rotationId }, connectionStringName);
        }

        public void DeleteRotation(int id)
        {
            string sql = "DELETE FROM Rotations WHERE Id = @Id; " +
                            "DELETE FROM RotationEmployees WHERE RotationId = @Id;";

            _db.SaveData(sql, new { Id = id }, connectionStringName);
        }

        public List<string> ReadAllAdmins()
        {
            List<string> output = new ();

            string sql = "SELECT UserName FROM Admins;";

            output = _db.LoadData<string, dynamic>(sql, new { }, connectionStringName);

            return output;
        }
    }
}
