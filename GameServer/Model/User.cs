using Dapper.Contrib.Extensions;

namespace GameServer.Model
{
    [Table("user")]
    public class User
    {
        [Key]
        public long Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
    }
}
