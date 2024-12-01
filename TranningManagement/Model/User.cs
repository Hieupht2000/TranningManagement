using System.ComponentModel.DataAnnotations;

namespace TranningManagement.Model
{
    public class User
    {
        [Key]
        public int user_id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string password_hash { get; set; }
        public string Role { get; set; } // 'student', 'teacher', 'staff', 'admin'
        public byte Status { get; set; } // 0 for inactive, 1 for active
    }
}
