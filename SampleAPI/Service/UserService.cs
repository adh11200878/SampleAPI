using Azure;
using Dapper;
using SampleAPI.Models;
using System.Data;

namespace SampleAPI.Service
{
    public class UserService
    {
        private readonly IDbConnection _db;
        public UserService(IDbConnection db)
        {
            _db = db;
        }

        // R：查全部
        public async Task<IEnumerable<UserModel>> GetAllAsync()
        {
            var sql = @"SELECT * 
                                    FROM Users 
                                    ORDER BY Id DESC";
            return await _db.QueryAsync<UserModel>(sql);
        }

        // R：查單筆
        public async Task<UserModel?> GetByIdAsync(int id)
        {
            var sql = @"SELECT * 
                                    FROM Users 
                                    WHERE Id = @Id";

            var parameters = new
            {
                Id = id
            };

            return await _db.QueryFirstOrDefaultAsync<UserModel>(sql, parameters);
        }

        // C：新增（含交易）
        public async Task<bool> CreateAsync(UserModel userModel)
        {
            var sql = @"INSERT INTO Users 
                                    (UserName, Email) VALUES 
                                    (@UserName, @Email);";

            if (_db.State != ConnectionState.Open)
            {
                _db.Open();
            }
            using var transaction = _db.BeginTransaction();
            try
            {
                //其實參數跟model名稱一樣可以用推斷的不需比對
                //var rows = await _db.ExecuteAsync(sql, user, transaction); 
                var parameters = new
                {
                    UserName = userModel.UserName,
                    Email = userModel.Email
                };
                var rows = await _db.ExecuteAsync(sql, parameters, transaction);
                transaction.Commit();
                return rows > 0;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        // U：更新（含交易）
        public async Task<bool> UpdateAsync(UserModel userModel)
        {
            var sql = @"UPDATE Users SET UserName = @UserName, Email = @Email WHERE Id = @Id";

            if (_db.State != ConnectionState.Open)
            {
                _db.Open();
            }
            using var transaction = _db.BeginTransaction();
            try
            {
                //var rows = await _db.ExecuteAsync(sql, user, transaction);
                var parameters = new
                {
                    Id = userModel.Id,
                    UserName = userModel.UserName,
                    Email = userModel.Email
                };
                var rows = await _db.ExecuteAsync(sql, parameters, transaction);
                transaction.Commit();
                return rows > 0;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        // D：刪除（含交易）
        public async Task<bool> DeleteAsync(int id)
        {
            var sql = @"DELETE FROM Users WHERE Id = @Id";

            if (_db.State != ConnectionState.Open)
            {
                _db.Open();
            }
            using var transaction = _db.BeginTransaction();
            try
            {
                var rows = await _db.ExecuteAsync(sql, new { Id = id }, transaction);
                transaction.Commit();
                return rows > 0;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

    }
}
