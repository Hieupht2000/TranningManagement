namespace TranningManagement.Model
{
    public class ClassesDTO
    {
        public int class_id { get; set; }
        public string class_name { get; set; }
        public string description { get; set; }
        public int teacher_id { get; set; }
        public DateOnly start_date { get; set; }
        public DateOnly end_date { get; set; }
        public decimal tuition { get; set; }
        public int max_students { get; set; }
        public string teacher_name { get; set; }
        public string classstatus { get; set; }
    }
}
