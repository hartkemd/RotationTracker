using DataAccessLibrary.Models;

namespace DataAccessLibrary.Data
{
    public interface ISqliteData
    {
        Task<List<EmployeeModel>> GetAllEmployeesAsync();
        Task CreateEmployeeAsync(string employeeName);
        Task DeleteEmployeeAsync(int id);
        Task CreateRotationAsync(FullRotationModel fullRotation);
        Task RecreateRotationOfEmployeesAsync(FullRotationModel fullRotation);
        Task<List<FullRotationModel>> GetAllRotationsAsync();
        Task<int> GetHighestIdFromRotationsAsync();
        Task UpdateRotationBasicInfoAsync(BasicRotationModel basicRotation);
        Task AdvanceRotationAsync(FullRotationModel fullRotation);
        Task ReverseRotationAsync(FullRotationModel fullRotation);
        Task DeleteRotationAsync(int id);
        Task<List<string>> ReadAllAdminsAsync();
        Task UpdateOnCalendarAsync(BasicRotationModel basicRotation, EmployeeModel employee, bool onCalendar);
        Task<List<CoverageReadModel>> ReadAllCoveragesAsync();
        Task<List<CoverageReadModel>> ReadCoveragesForRotationAsync(int rotationId);
        Task CreateCoverageAsync(CoverageModel coverage);
        Task SetCoverageInactiveAsync(int coverageId);
        Task DeleteCoverageAsync(int coverageId);
    }
}
