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
            await _database.CreateTableAsync<DailyMenu>();
            await _database.CreateTableAsync<User>();

            var adminExists = await _database.Table<User>().Where(u => u.Username == "admin").FirstOrDefaultAsync();
            if (adminExists == null)
            {
                await _database.InsertAsync(new User
                {
                    Username = "admin",
                    Password = "admin",
                    Role = "Admin"
                });
            }
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

        public async Task<List<Dish>> GetDailyMenuDishesAsync(DateTime date)
        {
            await InitializeAsync();
            var startOfDay = date.Date;
            var endOfDay = startOfDay.AddDays(1);

            var dailyEntries = await _database.Table<DailyMenu>()
                .Where(dm => dm.Date >= startOfDay && dm.Date < endOfDay)
                .ToListAsync();

            var dishIds = dailyEntries.Select(e => e.DishId).ToList();
            var allDishes = await GetDishesAsync();
            return allDishes.Where(d => dishIds.Contains(d.Id)).ToList();
        }

        public async Task<int> SaveDailyMenuAsync(DailyMenu dailyMenu)
        {
            await InitializeAsync();
            return await _database.InsertAsync(dailyMenu);
        }

        public async Task<User> LoginAsync(string username, string password)
        {
            await InitializeAsync();
            return await _database.Table<User>()
                .Where(u => u.Username == username && u.Password == password)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> RegisterUserAsync(string username, string password)
        {
            await InitializeAsync();
            var exists = await _database.Table<User>().Where(u => u.Username == username).FirstOrDefaultAsync();
            if (exists != null) return false;

            var user = new User
            {
                Username = username,
                Password = password,
                Role = "User"
            };
            await _database.InsertAsync(user);
            return true;
        }
    }
}