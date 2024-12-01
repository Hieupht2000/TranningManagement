using System.ComponentModel.DataAnnotations;

namespace TranningManagement.Model
{
    public class Teacher
    {
        [Key]
        public int teacher_id { get; set; }
        public int user_id {  get; set; }
        public string specialty { get; set; }
        public string name { get; set; }
    }
}
