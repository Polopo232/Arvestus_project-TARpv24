using SQLite;

namespace Arvestus_project_TARpv24.Models
{
    [SQLite.Table("Dishes")]
    public class Dish
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [SQLite.MaxLength(100), NotNull]
        public string Name { get; set; }

        public string Description { get; set; }

        [SQLite.MaxLength(255)]
        public string Allergens { get; set; }

        [SQLite.MaxLength(50)]
        public string Category { get; set; }

        public string ImagePath { get; set; }

        [Ignore]
        public double AverageRating { get; set; }

        [Ignore]
        public int RatingCount { get; set; }
    }
}