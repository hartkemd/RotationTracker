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

        public void CreateRotation(FullRotationModel rotation)
        {
            string sql = "INSERT INTO Rotations (RotationName, RotationRecurrence, NextDateTimeRotationAdvances, Notes) " +
                            "VALUES(@RotationName, @RecurrenceInterval, @NextDateTimeRotationAdvances, @Notes);";

            _db.SaveData(sql, new { RotationName = rotation.BasicInfo.RotationName,
                                    RecurrenceInterval = rotation.BasicInfo.RotationRecurrence,
                                    NextDateTimeRotationAdvances = rotation.BasicInfo.NextDateTimeRotationAdvances,
                                    Notes = rotation.BasicInfo.Notes},
                                    connectionStringName);

            sql = "SELECT Id FROM Rotations ORDER BY Id DESC LIMIT 1;"; // select the id of the row just inserted

            int rotationId = _db.LoadData<int, dynamic>(sql, new { }, connectionStringName).First();

            foreach (var employee in rotation.RotationOfEmployees)
            {
                sql = "INSERT INTO RotationEmployees (RotationId, EmployeeId) " +
                        "VALUES(@RotationId, @EmployeeId);";

                _db.SaveData(sql, new { RotationId = rotationId, EmployeeId = employee.Id }, connectionStringName);
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
                        "WHERE r.Id = @Id;";

                fullRotation.RotationOfEmployees = _db.LoadData<EmployeeModel, dynamic>(sql, new { Id = id }, connectionStringName);

                output.Add(fullRotation);
            }

            return output;
        }

        public void DeleteRotation(int id)
        {
            string sql = "DELETE FROM Rotations WHERE Id = @Id; " +
                            "DELETE FROM RotationEmployees WHERE RotationId = @Id;";

            _db.SaveData(sql, new { Id = id }, connectionStringName);
        }
    }
}
