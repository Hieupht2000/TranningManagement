namespace TranningManagement.Model
{
    public class SchedulesDTO
    {
        public int schedule_id { get; set; }
        public int class_id { get; set; }
        public int teacher_id { get; set; }
        public DateOnly schedule_date { get; set; }
        public TimeOnly schedule_time { get; set; }
        public string location { get; set; }
    }
}
