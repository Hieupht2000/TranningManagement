namespace TranningManagement.Model
{
    public class Promotions
    {
        public int promotion_id { get; set; }
        public int class_id { get; set; }
        public decimal discount_percentage { get; set; }
        public DateTime start_date { get; set; }
        public DateTime end_date { get; set; }

    }
}
