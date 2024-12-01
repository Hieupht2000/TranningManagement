using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TranningManagement.Model;

namespace TranningManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SchedulesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public SchedulesController(ApplicationDbContext context)
        {
            _context = context;
        }
        // GET: api/schedules
        [HttpGet]
        public ActionResult<IEnumerable<SchedulesDTO>> GetSchedules()
        {
            var schedules = _context.schedules.Select(s => new SchedulesDTO
            {
                schedule_id = s.schedule_id,
                class_id = s.class_id,
                teacher_id = s.teacher_id,
                schedule_date = DateOnly.FromDateTime(s.schedule_date),
                schedule_time = TimeOnly.FromDateTime(s.schedule_date),  // extracting time part
                location = s.location
            }).ToList();

            return Ok(schedules);
        }

        // GET: api/schedules/{id}
        [HttpGet("{id}")]
        public ActionResult<SchedulesDTO> GetSchedule(int id)
        {
            var schedule = _context.schedules.Find(id);

            if (schedule == null)
            {
                return NotFound();
            }

            var scheduleDTO = new SchedulesDTO
            {
                schedule_id = schedule.schedule_id,
                class_id = schedule.class_id,
                teacher_id = schedule.teacher_id,
                schedule_date = DateOnly.FromDateTime(schedule.schedule_date),
                schedule_time = TimeOnly.FromDateTime(schedule.schedule_date),
                location = schedule.location
            };

            return Ok(scheduleDTO);
        }

        // POST: api/schedules
        [HttpPost]
        public ActionResult<SchedulesDTO> CreateSchedule(SchedulesDTO scheduleDTO)
        {
            var combinedDateTime = scheduleDTO.schedule_date.ToDateTime(scheduleDTO.schedule_time);
            var schedule = new Schedules
            {
                class_id = scheduleDTO.class_id,
                teacher_id = scheduleDTO.teacher_id,
                schedule_date = combinedDateTime,  
                location = scheduleDTO.location
            };

            _context.schedules.Add(schedule);
            _context.SaveChanges();

            scheduleDTO.schedule_id = schedule.schedule_id;

            return CreatedAtAction(nameof(GetSchedule), new { id = schedule.schedule_id }, scheduleDTO);
        }

        // PUT: api/schedules/{id}
        [HttpPut("{id}")]
        public IActionResult UpdateSchedule(int id, SchedulesDTO scheduleDTO)
        {
            var schedule = _context.schedules.Find(id);

            if (schedule == null)
            {
                return NotFound();
            }
            var combinedDateTime = scheduleDTO.schedule_date.ToDateTime(scheduleDTO.schedule_time);
            schedule.class_id = scheduleDTO.class_id;
            schedule.teacher_id = scheduleDTO.teacher_id;
            schedule.schedule_date = combinedDateTime;
            schedule.location = scheduleDTO.location;

            _context.schedules.Update(schedule);
            _context.SaveChanges();

            return NoContent();
        }

        // DELETE: api/schedules/{id}
        [HttpDelete("{id}")]
        public IActionResult DeleteSchedule(int id)
        {
            var schedule = _context.schedules.Find(id);

            if (schedule == null)
            {
                return NotFound();
            }

            _context.schedules.Remove(schedule);
            _context.SaveChanges();

            return NoContent();
        }
        private bool ScheduleExists(int id)
        {
            return _context.schedules.Any(l => l.schedule_id == id);
        }
    }
}
