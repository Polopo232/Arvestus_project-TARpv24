using SQLite;

namespace Arvestus_project_TARpv24.Models
{
    [Table("DailyMenus")]
    public class DailyMenu
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public int DishId { get; set; }

        public DateTime Date { get; set; }
    }
}