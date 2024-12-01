using System.ComponentModel.DataAnnotations;

namespace TranningManagement.Model
{
    public class Student
    {
        [Key]
        public int student_id { get; set; }
        public int user_id { get; set; }
        public DateTime dob { get; set; }
        public string phone_number { get; set; }
        public string tuition_status { get; set; } 
        public string name { get; set; }
        public string class_name { get; set; }
    }
}
