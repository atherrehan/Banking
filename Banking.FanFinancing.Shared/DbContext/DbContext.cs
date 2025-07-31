using Banking.FanFinancing.Shared.Enums;
using Banking.FanFinancing.Shared.Models;
using Banking.FanFinancing.Shared.Services.Interface;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using System.Data;
using static Dapper.SqlMapper;

namespace Banking.FanFinancing.Shared.DbContext
{
    public class DbContext : IDbContext
    {
        private readonly string _connectionString;
        private readonly ICacheService _cacheService;
        private readonly CacheTimeConfig? _cacheTimeConfig;

        public DbContext(IOptions<DatabaseConnection> connection, ICacheService cacheService)
        {
            _connectionString = connection.Value.AlBarakaAppConnectionString;
            _cacheService = cacheService;
        }

        public async Task<ApiUrl> GetURL(string ProcessingCode)
        {
            ApiUrl url = new();
            _cacheService.TryGet(DBCacheKeys.UrlCache.ToString(), out List<ApiUrl> esbURL);
            if (esbURL is null)
            {
                using var _db = await CreateConnectionAsync();
                var Data = await _db.QueryAsync<ApiUrl>("usp_get_all_urls", commandType: CommandType.StoredProcedure);
                esbURL = Data.ToList();
                if (esbURL is not null)
                {
                    _cacheService.Set(DBCacheKeys.UrlCache.ToString(), esbURL, _cacheTimeConfig?.UrlAbsoluteExpirationTime ?? 30, _cacheTimeConfig?.UrlSlidingExpiration ?? 30);
                }
            }
            url = esbURL?.FirstOrDefault(row => row.ProcessingCode == ProcessingCode) ?? new ApiUrl();
            return url;
        }

        public async Task<T> GetSingleAsync<T>(string sp, object? parms = null, CommandType commandType = CommandType.StoredProcedure)
        {
            using var _db = await CreateConnectionAsync();
            var result = await _db.QueryFirstOrDefaultAsync<T>(sp, parms, commandType: commandType);
            return result!;
        }

        public async Task<List<T>> GetListAsync<T>(string sp, object? parms = null, CommandType commandType = CommandType.StoredProcedure)
        {
            using var _db = await CreateConnectionAsync();
            var Data = await _db.QueryAsync<T>(sp, parms, commandType: commandType);
            return Data.ToList();
        }

        public async Task<List<object>> GetMultipleSelectsAsync(string sql, object? parameters = null, params Func<GridReader, object>[] readerFuncs)
        {

            var results = new List<object>();
            using var _db = await CreateConnectionAsync();
            using var Result = await _db.QueryMultipleAsync(sql, parameters, commandType: CommandType.StoredProcedure);
            foreach (var readerFunc in readerFuncs)
            {
                results.Add(readerFunc(Result));
            }
            return results;
        }

        public async Task<bool> ExecuteAsync(string sp, object? parms = null, CommandType commandType = CommandType.StoredProcedure)
        {
            using var _db = await CreateConnectionAsync();
            return await _db.ExecuteAsync(sp, parms, commandType: commandType) >= 0;
        }

        private async Task<IDbConnection> CreateConnectionAsync()
        {
            var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            return connection;
        }
    }
}
