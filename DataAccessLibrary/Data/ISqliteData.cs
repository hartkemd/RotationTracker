using DataAccessLibrary.Models;

namespace DataAccessLibrary.Data
{
    public interface ISqliteData
    {
        List<EmployeeModel> GetAllEmployees();
        void CreateEmployee(string employeeName);
        void DeleteEmployee(int id);
        void CreateRotation(FullRotationModel rotation);
        List<FullRotationModel> GetAllRotations();
        void DeleteRotation(int id);
    }
}
