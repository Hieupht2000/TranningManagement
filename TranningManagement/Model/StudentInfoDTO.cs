namespace TranningManagement.Model
{
    public class StudentInfoDTO
    {
        public int student_id { get; set; }
        public int class_id { get; set; }
        public DateOnly Registrationdate { get; set; }
        public string name { get; set; }
        public string phone_number { get; set; }
        public string tuition_status { get; set; }
    }
}
