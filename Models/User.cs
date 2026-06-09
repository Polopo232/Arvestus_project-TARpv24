using SQLite;

namespace Arvestus_project_TARpv24.Models
{
    [Table("Users")]
    public class User
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Unique, NotNull]
        public string Username { get; set; }

        [NotNull]
        public string Password { get; set; }

        [NotNull]
        public string Role { get; set; }
    }
}