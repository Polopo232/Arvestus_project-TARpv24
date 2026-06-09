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

            // Seed initial data
            await SeedDataAsync();
        }

        private async Task SeedDataAsync()
        {
            // Check if data already exists
            var dishCount = await _database.Table<Dish>().CountAsync();
            if (dishCount > 0) return;

            // Insert sample dishes with Estonian names
            var dishes = new List<Dish>
            {
                new Dish { Name = "Borscht", Category = "Supid", Description = "Traditionline peetisupp hapukoorega", Allergens = "Hapukoor" },
                new Dish { Name = "Kartulisupp", Category = "Supid", Description = "Kreemine kartulisupp broilerilihaga", Allergens = "Gluteen, Piim" },
                new Dish { Name = "Sealiha praad", Category = "Praed", Description = "Ahjus küpsetatud sealiha kartuliga", Allergens = "" },
                new Dish { Name = "Kanarind grillil", Category = "Praed", Description = "Grillitud kanarind juurviljadega", Allergens = "" },
                new Dish { Name = "Lihapallid", Category = "Praed", Description = "Hakklihapallid koorekastme ja kartuliga", Allergens = "Gluteen, Piim" },
                new Dish { Name = "Pasta Bolognese", Category = "Pasta", Description = "Itaalia lihakastmega pasta", Allergens = "Gluteen" },
                new Dish { Name = "Pizza Margherita", Category = "Pasta", Description = "Traditsiooniline Itaalia pizza", Allergens = "Gluteen, Piim" },
                new Dish { Name = "Kala ja kartul", Category = "Praed", Description = "Ahjukala aurutatud juurviljadega", Allergens = "Kala" },
                new Dish { Name = "Salat Caesar", Category = "Salatid", Description = "Kanafilee, salatilehed, parmesan, krutoonid", Allergens = "Gluteen, Juust" },
                new Dish { Name = "Kaerahelbepuder", Category = "Hommikusöök", Description = "Kaerahelbepuder meega", Allergens = "Gluteen" },
                new Dish { Name = "Köögiviljasalat", Category = "Salatid", Description = "Köögiviljasalat hapukoorega", Allergens = "Hapukoor" }
            };

            foreach (var dish in dishes)
            {
                await _database.InsertAsync(dish);
            }

            // Insert some sample ratings
            var allDishes = await _database.Table<Dish>().ToListAsync();
            var random = new Random();
            foreach (var dish in allDishes)
            {
                // Add 2-7 random ratings per dish
                var ratingCount = random.Next(2, 8);
                for (int i = 0; i < ratingCount; i++)
                {
                    await _database.InsertAsync(new Rating
                    {
                        DishId = dish.Id,
                        Score = random.Next(1, 6),
                        Comment = $"Proovi kommentaar {i+1} {dish.Name} kohta",
                        Username = "Kasutaja"
                    });
                }
            }

            // Insert daily menu entries (for today and next 6 days)
            var today = DateTime.Today;
            for (int day = 0; day < 7; day++)
            {
                var date = today.AddDays(day);
                var dailyCount = random.Next(3, 6);
                var selectedIndices = new HashSet<int>();
                while (selectedIndices.Count < dailyCount && selectedIndices.Count < allDishes.Count)
                {
                    selectedIndices.Add(random.Next(0, allDishes.Count));
                }

                foreach (var idx in selectedIndices)
                {
                    await _database.InsertAsync(new DailyMenu
                    {
                        DishId = allDishes[idx].Id,
                        Date = date
                    });
                }
            }
        }

        public async Task<List<Dish>> GetDishesAsync()
        {
            await InitializeAsync();
            var dishes = await _database.Table<Dish>().ToListAsync();

            // Load ratings and calculate average for each dish
            foreach (var dish in dishes)
            {
                var ratings = await _database.Table<Rating>()
                    .Where(r => r.DishId == dish.Id)
                    .ToListAsync();

                if (ratings.Any())
                {
                    dish.AverageRating = Math.Round(ratings.Average(r => r.Score), 1);
                    dish.RatingCount = ratings.Count;
                }
                else
                {
                    dish.AverageRating = 0;
                    dish.RatingCount = 0;
                }
            }

            return dishes;
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

            // Handle potential schema issues - ensure Username is not null
            if (string.IsNullOrEmpty(rating.Username))
            {
                rating.Username = "Külaline";
            }

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

        public async Task<int> DeleteDailyMenuAsync(int dishId, DateTime date)
        {
            await InitializeAsync();
            var startOfDay = date.Date;
            var endOfDay = startOfDay.AddDays(1);
            var entries = await _database.Table<DailyMenu>()
                .Where(dm => dm.DishId == dishId && dm.Date >= startOfDay && dm.Date < endOfDay)
                .ToListAsync();
            int result = 0;
            foreach (var entry in entries)
            {
                result += await _database.DeleteAsync<DailyMenu>(entry.Id);
            }
            return result;
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