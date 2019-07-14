using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper.Contrib;
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
