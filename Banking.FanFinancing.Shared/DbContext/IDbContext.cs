using Banking.FanFinancing.Shared.Models;
using System.Data;
using static Dapper.SqlMapper;

namespace Banking.FanFinancing.Shared.DbContext
{
    public interface IDbContext
    {
        Task<ApiUrl> GetURL(string ProcessingCode);

        Task<T> GetSingleAsync<T>(string sp, object? parms = null, CommandType commandType = CommandType.StoredProcedure);

        Task<List<T>> GetListAsync<T>(string sp, object? parms = null, CommandType commandType = CommandType.StoredProcedure);

        Task<List<object>> GetMultipleSelectsAsync(string sql, object? parameters = null, params Func<GridReader, object>[] readerFuncs);

        Task<bool> ExecuteAsync(string sp, object? parms = null, CommandType commandType = CommandType.StoredProcedure);

    }
}
