namespace TranningManagement.Model
{
    public class PromotionsDTO
    {
        public int promotion_id { get; set; }
        public int class_id { get; set; }
        public decimal discount_percentage { get; set; }
        public DateOnly start_date { get; set; }
        public DateOnly end_date { get; set; }
    }
}
