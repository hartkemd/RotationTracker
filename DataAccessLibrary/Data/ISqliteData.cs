using DataAccessLibrary.Models;

namespace DataAccessLibrary.Data
{
    public interface ISqliteData
    {
        List<EmployeeModel> GetAllEmployees();
        void CreateEmployee(string employeeName);
        void DeleteEmployee(int id);
        BasicRotationModel GetRotationById(int id);
    }
}
