using SQLite;

namespace Arvestus_project_TARpv24.Models
{
    [Table("Ratings")]
    public class Rating
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public int DishId { get; set; }

        public int Score { get; set; }

        public string Comment { get; set; }

        public string ImagePath { get; set; }
    }
}