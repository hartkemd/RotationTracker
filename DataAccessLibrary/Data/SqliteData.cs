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

        public BasicRotationModel GetRotationById(int id) // needs work
        {
            string sql = "SELECT * FROM Rotations WHERE Id = @Id;";

            return _db.LoadData<BasicRotationModel, dynamic>(sql, new { Id = id }, connectionStringName).First();
        }
    }
}
