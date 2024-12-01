namespace TranningManagement.Model
{
    public class Schedules
    {
        public int schedule_id {  get; set; }
        public int class_id { get; set; }
        public int teacher_id { get; set; }
        public DateTime schedule_date {  get; set; }
        public DateTime schedule_time { get; set; }
        public string location { get; set; }
    }
}
