
namespace DataAccessLibrary.Databases
{
    public interface ISqliteDataAccess
    {
        Task<List<T>> LoadDataAsync<T, U>(string sqlStatement, U parameters, string connectionStringName);
        Task SaveDataAsync<T>(string sqlStatement, T parameters, string connectionStringName);
    }
}
