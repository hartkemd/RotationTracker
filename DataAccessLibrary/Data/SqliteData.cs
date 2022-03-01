using DataAccessLibrary.Databases;
using DataAccessLibrary.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        public async Task<List<EmployeeModel>> GetAllEmployeesAsync()
        {
            string sql = "SELECT Id, FullName FROM Employees ORDER BY FullName;";

            return await _db.LoadDataAsync<EmployeeModel, dynamic>(sql, new { }, connectionStringName);
        }

        public async Task CreateEmployeeAsync(string employeeName)
        {
            string sql = "INSERT INTO Employees (FullName) VALUES(@FullName);";

            await _db.SaveDataAsync(sql, new { FullName = employeeName }, connectionStringName);
        }

        public async Task DeleteEmployeeAsync(int id)
        {
            string sql = "DELETE FROM Employees WHERE Id = @Id;";

            await _db.SaveDataAsync(sql, new { Id = id }, connectionStringName);
        }

        public async Task CreateRotationAsync(FullRotationModel fullRotation)
        {
            string sql = "INSERT INTO Rotations (RotationName, RotationRecurrence, NextDateTimeRotationAdvances, AdvanceAutomatically, Notes) " +
                            "VALUES(@RotationName, @RecurrenceInterval, @NextDateTimeRotationAdvances, @AdvanceAutomatically, @Notes);";

            await _db.SaveDataAsync(sql, new { RotationName = fullRotation.BasicInfo.RotationName,
                                    RecurrenceInterval = fullRotation.BasicInfo.RotationRecurrence,
                                    NextDateTimeRotationAdvances = fullRotation.BasicInfo.NextDateTimeRotationAdvances,
                                    AdvanceAutomatically = fullRotation.BasicInfo.AdvanceAutomatically,
                                    Notes = fullRotation.BasicInfo.Notes},
                                    connectionStringName);

            sql = "SELECT Id FROM Rotations ORDER BY Id DESC LIMIT 1;"; // select the id of the row just inserted

            int rotationId = (await _db.LoadDataAsync<int, dynamic>(sql, new { }, connectionStringName)).First();

            for (int i = 0; i < fullRotation.RotationOfEmployees.Count; i++)
            {
                sql = "INSERT INTO RotationEmployees (RotationId, EmployeeId, Position, OnCalendar) " +
                        "VALUES(@RotationId, @EmployeeId, @Position, 0);";

                await _db.SaveDataAsync(sql, new { RotationId = rotationId,
                                        EmployeeId = fullRotation.RotationOfEmployees[i].Id,
                                        Position = i },
                                        connectionStringName);
            }
        }

        public async Task RecreateRotationOfEmployeesAsync(FullRotationModel fullRotation)
        {
            int rotationId = fullRotation.BasicInfo.Id;

            string sql = "DELETE FROM RotationEmployees WHERE RotationId = @RotationId;";

            await _db.SaveDataAsync(sql, new { RotationId = rotationId }, connectionStringName);

            int position = 0;
            foreach (var employee in fullRotation.RotationOfEmployees)
            {
                sql = "INSERT INTO RotationEmployees(RotationId, EmployeeId, Position, OnCalendar) " +
                        "VALUES(@RotationId, @EmployeeId, @Position, 0);";

                await _db.SaveDataAsync(sql, new { RotationId = rotationId,
                                        EmployeeId = employee.Id,
                                        Position = position },
                                        connectionStringName);

                position++;
            }
        }

        public async Task<List<FullRotationModel>> GetAllRotationsAsync()
        {
            List<FullRotationModel> output = new();

            string sql = "SELECT Id FROM Rotations;";

            List<int> rotationIds = await _db.LoadDataAsync<int, dynamic>(sql, new { }, connectionStringName);

            foreach (var id in rotationIds)
            {
                FullRotationModel fullRotation = new();

                sql = "SELECT * FROM Rotations WHERE Id = @Id;";

                fullRotation.BasicInfo = (await _db.LoadDataAsync<BasicRotationModel, dynamic>(sql, new { Id = id }, connectionStringName)).First();

                sql = "SELECT e.*, re.OnCalendar FROM Employees e " +
                        "INNER JOIN RotationEmployees re ON e.Id = re.EmployeeId " +
                        "INNER JOIN Rotations r ON re.RotationId = r.Id " +
                        "WHERE r.Id = @Id ORDER BY re.Position;";

                fullRotation.RotationOfEmployees = new ObservableCollection<EmployeeModel>(
                    await _db.LoadDataAsync<EmployeeModel, dynamic>(sql, new { Id = id }, connectionStringName));

                output.Add(fullRotation);
            }

            return output;
        }

        public async Task<int> GetHighestIdFromRotationsAsync()
        {
            int output;
            
            string sql = "SELECT Id FROM Rotations ORDER BY Id DESC LIMIT 1;";

            output = (await _db.LoadDataAsync<int, dynamic>(sql, new { }, connectionStringName)).First();

            return output;
        }

        public async Task UpdateRotationBasicInfoAsync(BasicRotationModel basicRotation)
        {
            string sql = "UPDATE Rotations SET RotationName = @RotationName, RotationRecurrence = @RotationRecurrence, " +
                            "NextDateTimeRotationAdvances = @NextDateTimeRotationAdvances, " +
                            "AdvanceAutomatically = @AdvanceAutomatically, Notes = @Notes, OutlookCategory = @OutlookCategory WHERE Id = @Id;";

            await _db.SaveDataAsync(sql, new { basicRotation.RotationName,
                                    basicRotation.RotationRecurrence,
                                    basicRotation.NextDateTimeRotationAdvances,
                                    basicRotation.AdvanceAutomatically,
                                    basicRotation.Notes,
                                    basicRotation.OutlookCategory,
                                    basicRotation.Id },
                                    connectionStringName);
        }

        public async Task AdvanceRotationAsync(FullRotationModel fullRotation)
        {
            int rotationId = fullRotation.BasicInfo.Id;

            string sql = "SELECT EmployeeId FROM RotationEmployees WHERE RotationId = @RotationId AND Position = 0;";

            int employeeId = (await _db.LoadDataAsync<int, dynamic>(sql, new { RotationId = rotationId }, connectionStringName)).First();

            sql = "UPDATE RotationEmployees SET Position = " +
                    "(SELECT count(*) FROM RotationEmployees WHERE RotationId = @RotationId) " +
                    "WHERE EmployeeId = @EmployeeId AND RotationId = @RotationId; " +
                    "UPDATE RotationEmployees SET Position = Position - 1 WHERE RotationId = @RotationId;";

            await _db.SaveDataAsync(sql, new { RotationId = rotationId, EmployeeId = employeeId }, connectionStringName);
        }

        public async Task ReverseRotationAsync(FullRotationModel fullRotation) // moves the rotation back one position in the list
        {
            int rotationId = fullRotation.BasicInfo.Id;
            
            // select the id of the last employee in the rotation
            string sql = "SELECT EmployeeId FROM RotationEmployees WHERE RotationId = @RotationId AND Position = " +
                            "(SELECT Position FROM RotationEmployees WHERE RotationId = @RotationId ORDER BY Position DESC LIMIT 1);";

            int employeeId = (await _db.LoadDataAsync<int, dynamic>(sql, new { RotationId = rotationId }, connectionStringName)).First();

            sql = "UPDATE RotationEmployees SET Position = -1 WHERE RotationId = @RotationId AND EmployeeId = @EmployeeId; " +
                    "UPDATE RotationEmployees SET Position = Position + 1 WHERE RotationId = @RotationId;";

            await _db.SaveDataAsync(sql, new { EmployeeId = employeeId, RotationId = rotationId }, connectionStringName);
        }

        public async Task DeleteRotationAsync(int id)
        {
            string sql = "DELETE FROM Rotations WHERE Id = @Id; " +
                            "DELETE FROM RotationEmployees WHERE RotationId = @Id;";

            await _db.SaveDataAsync(sql, new { Id = id }, connectionStringName);
        }

        public async Task<List<string>> ReadAllAdminsAsync()
        {
            string sql = "SELECT UserName FROM Admins;";

            return await _db.LoadDataAsync<string, dynamic>(sql, new { }, connectionStringName);
        }

        public async Task UpdateOnCalendarAsync(BasicRotationModel basicRotation, EmployeeModel employee, bool onCalendar)
        {
            string sql = "UPDATE RotationEmployees SET OnCalendar = @OnCalendar " +
                            "WHERE RotationId = @RotationId AND EmployeeId = @EmployeeId;";

            await _db.SaveDataAsync(sql, new { OnCalendar = onCalendar,
                                    RotationId = basicRotation.Id,
                                    EmployeeId = employee.Id }, connectionStringName);
        }

        public async Task<List<CoverageReadModel>> ReadAllCoveragesAsync()
        {
            string sql = "SELECT c.Id, r.RotationName, a.FullName AS EmployeeCovering, b.FullName AS EmployeeCovered, c.StartDate, c.EndDate " +
                            "FROM Coverages c " +
                            "INNER JOIN Rotations r ON c.RotationId = r.Id " +
                            "INNER JOIN Employees a ON c.EmployeeIdOfCovering = a.Id " +
                            "INNER JOIN Employees b ON c.EmployeeIdOfCovered = b.Id " +
                            "WHERE IsActive = 1;";

            return await _db.LoadDataAsync<CoverageReadModel, dynamic>(sql, new { }, connectionStringName);
        }

        public async Task<List<CoverageReadModel>> ReadCoveragesForRotationAsync(int rotationId)
        {
            string sql = "SELECT c.Id, r.RotationName, a.FullName AS EmployeeCovering, b.FullName AS EmployeeCovered, c.StartDate, c.EndDate " +
                            "FROM Coverages c " +
                            "INNER JOIN Rotations r ON c.RotationId = r.Id " +
                            "INNER JOIN Employees a ON c.EmployeeIdOfCovering = a.Id " +
                            "INNER JOIN Employees b ON c.EmployeeIdOfCovered = b.Id " +
                            "WHERE c.RotationId = @RotationId AND IsActive = 1;";

            return await _db.LoadDataAsync<CoverageReadModel, dynamic>(sql, new { RotationId = rotationId }, connectionStringName);
        }

        public async Task CreateCoverageAsync(CoverageModel coverage)
        {
            string sql = "INSERT INTO Coverages (RotationId, EmployeeIdOfCovering, EmployeeIdOfCovered, StartDate, EndDate) " +
                            "VALUES (@RotationId, @EmployeeIdOfCovering, @EmployeeIdOfCovered, @StartDate, @EndDate)";

            await _db.SaveDataAsync(sql, new { coverage.RotationId,
                                         coverage.EmployeeIdOfCovering,
                                         coverage.EmployeeIdOfCovered,
                                         coverage.StartDate,
                                         coverage.EndDate}, connectionStringName);
        }

        public async Task SetCoverageInactiveAsync(int coverageId)
        {
            string sql = "UPDATE Coverages SET IsActive = 0 WHERE Id = @Id;";

            await _db.SaveDataAsync(sql, new { Id = coverageId }, connectionStringName);
        }

        public async Task DeleteCoverageAsync(int coverageId)
        {
            string sql = "DELETE FROM Coverages WHERE Id = @Id;";

            await _db.SaveDataAsync(sql, new { Id = coverageId }, connectionStringName);
        }
    }
}
