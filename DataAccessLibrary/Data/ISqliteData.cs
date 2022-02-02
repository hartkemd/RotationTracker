using DataAccessLibrary.Models;

namespace DataAccessLibrary.Data
{
    public interface ISqliteData
    {
        List<EmployeeModel> GetAllEmployees();
        void CreateEmployee(string employeeName);
        void DeleteEmployee(int id);
        void CreateRotation(FullRotationModel rotation);
        void RecreateRotationOfEmployees(FullRotationModel fullRotation);
        List<FullRotationModel> GetAllRotations();
        void UpdateRotationBasicInfo(BasicRotationModel basicRotation);
        void AdvanceRotation(FullRotationModel fullRotation);
        void DeleteRotation(int id);
        List<string> ReadAllAdmins();
    }
}
