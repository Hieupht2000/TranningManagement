using System.ComponentModel.DataAnnotations;

namespace TranningManagement.Model
{
    public class StudentClasses
    {
        [Key]
        public int student_id {  get; set; }
        public int class_id { get; set; }
        public DateTime Registrationdate { get; set; }
    }
}
