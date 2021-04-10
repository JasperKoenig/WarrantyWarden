using System.Collections.Generic;
using System.Threading.Tasks;
using WarrantyWarden.Models;
using SQLite;
using System;

namespace WarrantyWarden.Data
{
    public class WarrantyDatabase
    {
        static SQLiteAsyncConnection Database;

        public static readonly AsyncLazy<WarrantyDatabase> Instance = new AsyncLazy<WarrantyDatabase>(async () =>
        {
            var instance = new WarrantyDatabase();
            CreateTableResult result = await Database.CreateTableAsync<Warranty>();
            return instance;
        });

        public WarrantyDatabase()
        {
            Database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
        }

        public Task<List<Warranty>> GetItemsAsync()
        {
            return Database.Table<Warranty>().ToListAsync();
        }

        public Task<List<Warranty>> QueryAsync(string query)
        {
            // SQL queries are also possible => "SELECT * FROM [Warranty] WHERE [Priority] = 0"
            return Database.QueryAsync<Warranty>(query);
        }

        public Task<Warranty> GetItemAsync(int id)
        {
            return Database.Table<Warranty>().Where(i => i.ID == id).FirstOrDefaultAsync();
        }

        public Task<int> SaveItemAsync(Warranty item)
        {
            if (item.ID != 0)
            {
                return Database.UpdateAsync(item);
            }
            else
            {
                return Database.InsertAsync(item);
            }
        }

        public Task<int> DeleteItemAsync(Warranty item)
        {
            return Database.DeleteAsync(item);
        }
    }
}
