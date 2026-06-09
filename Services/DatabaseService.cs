using SQLite;
using Arvestus_project_TARpv24.Models;

namespace Arvestus_project_TARpv24.Services
{
    public class DatabaseService
    {
        private SQLiteAsyncConnection _database;
        private readonly string _dbPath;

        public DatabaseService(string dbPath)
        {
            _dbPath = dbPath;
        }

        private async Task InitializeAsync()
        {
            if (_database != null) return;

            _database = new SQLiteAsyncConnection(_dbPath);
            await _database.CreateTableAsync<Dish>();
            await _database.CreateTableAsync<Rating>();
        }

        public async Task<List<Dish>> GetDishesAsync()
        {
            await InitializeAsync();
            return await _database.Table<Dish>().ToListAsync();
        }

        public async Task<int> SaveDishAsync(Dish dish)
        {
            await InitializeAsync();
            if (dish.Id != 0)
            {
                return await _database.UpdateAsync(dish);
            }
            else
            {
                return await _database.InsertAsync(dish);
            }
        }

        public async Task<List<Rating>> GetRatingsForDishAsync(int dishId)
        {
            await InitializeAsync();
            return await _database.Table<Rating>().Where(r => r.DishId == dishId).ToListAsync();
        }

        public async Task<int> SaveRatingAsync(Rating rating)
        {
            await InitializeAsync();
            return await _database.InsertAsync(rating);
        }
    }
}