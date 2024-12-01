namespace TranningManagement.Model
{
    public class TuitionFeesDTO
    {
        public int payment_id { get; set; }
        public int student_id { get; set; }
        public int class_id { get; set; }
        public decimal total_amount { get; set; }
        public string payment_status { get; set; }
        public DateOnly due_date { get; set; }
        public string name { get; set; }
    }
}
