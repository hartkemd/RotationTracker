using DataAccessLibrary.Models;

namespace DataAccessLibrary.Data
{
    public interface ISqliteData
    {
        List<EmployeeModel> GetAllEmployees();
        void CreateEmployee(string employeeName);
        void DeleteEmployee(int id);
        void CreateRotation(FullRotationModel fullRotation);
        void RecreateRotationOfEmployees(FullRotationModel fullRotation);
        List<FullRotationModel> GetAllRotations();
        int GetHighestIdFromRotations();
        void UpdateRotationBasicInfo(BasicRotationModel basicRotation);
        void AdvanceRotation(FullRotationModel fullRotation);
        void ReverseRotation(FullRotationModel fullRotation);
        void DeleteRotation(int id);
        List<string> ReadAllAdmins();
        void UpdateOnCalendar(BasicRotationModel basicRotation, EmployeeModel employee, bool onCalendar);
    }
}
