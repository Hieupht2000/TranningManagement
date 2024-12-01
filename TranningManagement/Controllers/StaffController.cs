using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TranningManagement.Model;

namespace TranningManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StaffController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public StaffController (ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Staff
        [HttpGet]
        public ActionResult<IEnumerable<StaffDTO>> GetStaff()
        {
            var staffList = _context.Staffs.Select(s => new StaffDTO
            {
                staff_id = s.staff_id,
                user_id = s.user_id,
                position = s.position
            }).ToList();

            return Ok(staffList);
        }

        // GET: api/Staff/5
        [HttpGet("{id}")]
        public ActionResult<StaffDTO> GetStaffById(int id)
        {
            var staff = _context.Staffs.Find(id);

            if (staff == null)
            {
                return NotFound();
            }

            var staffDTO = new StaffDTO
            {
                staff_id = staff.staff_id,
                user_id = staff.user_id,
                position = staff.position
            };

            return Ok(staffDTO);
        }

        // POST: api/Staff
        [HttpPost]
        public ActionResult<StaffDTO> CreateStaff(StaffDTO staffDTO)
        {
            var staff = new Staff
            {
                user_id = staffDTO.user_id,
                position = staffDTO.position
            };

            _context.Staffs.Add(staff);
            _context.SaveChanges();

            staffDTO.staff_id = staff.staff_id;

            return CreatedAtAction(nameof(GetStaffById), new { id = staff.staff_id }, staffDTO);
        }

        // PUT: api/Staff/5
        [HttpPut("{id}")]
        public IActionResult UpdateStaff(int id, StaffDTO staffDTO)
        {
            if (id != staffDTO.staff_id)
            {
                return BadRequest();
            }

            var staff = _context.Staffs.Find(id);

            if (staff == null)
            {
                return NotFound();
            }

            staff.user_id = staffDTO.user_id;
            staff.position = staffDTO.position;

            _context.SaveChanges();

            return NoContent();
        }

        // DELETE: api/Staff/5
        [HttpDelete("{id}")]
        public IActionResult DeleteStaff(int id)
        {
            var staff = _context.Staffs.Find(id);
            if (staff == null)
            {
                return NotFound();
            }

            _context.Staffs.Remove(staff);
            _context.SaveChanges();

            return NoContent();
        }
        private bool StaffExists(int id)
        {
            return _context.Staffs.Any(f => f.staff_id == id);
        }
    }
}
