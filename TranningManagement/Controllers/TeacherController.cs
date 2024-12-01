using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TranningManagement.Model;

namespace TranningManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeacherController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TeacherController (ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Teacher
        [HttpGet]
        public ActionResult<IEnumerable<TeacherDTO>> GetTeachers()
        {
            var teachers = _context.Teachers.Select(t => new TeacherDTO
            {
                teacher_id = t.teacher_id,
                user_id = t.user_id,
                specialty = t.specialty,
                name = t.name,
            }).ToList();

            return Ok(teachers);
        }

        // GET: api/Teacher/5
        [HttpGet("{id}")]
        public ActionResult<TeacherDTO> GetTeacher(int id)
        {
            var teacher = _context.Teachers.Find(id);

            if (teacher == null)
            {
                return NotFound();
            }

            var teacherDTO = new TeacherDTO
            {
                teacher_id = teacher.teacher_id,
                user_id = teacher.user_id,
                specialty = teacher.specialty,
                name= teacher.name,
            };

            return Ok(teacherDTO);
        }

        // POST: api/Teacher
        [HttpPost]
        public ActionResult<TeacherDTO> CreateTeacher(TeacherDTO teacherDTO)
        {
            var teacher = new Teacher
            {
                user_id = teacherDTO.user_id,
                specialty = teacherDTO.specialty,
                name = teacherDTO.name,
            };

            _context.Teachers.Add(teacher);
            _context.SaveChanges();

            teacherDTO.teacher_id = teacher.teacher_id;

            return CreatedAtAction(nameof(GetTeacher), new { id = teacher.teacher_id }, teacherDTO);
        }

        // PUT: api/Teacher/5
        [HttpPut("{id}")]
        public IActionResult UpdateTeacher(int id, TeacherDTO teacherDTO)
        {
            if (id != teacherDTO.teacher_id)
            {
                return BadRequest();
            }

            var teacher = _context.Teachers.Find(id);

            if (teacher == null)
            {
                return NotFound();
            }

            teacher.user_id = teacherDTO.user_id;
            teacher.specialty = teacherDTO.specialty;
            teacher.name = teacherDTO.name;

            _context.SaveChanges();

            return NoContent();
        }

        // DELETE: api/Teacher/5
        [HttpDelete("{id}")]
        public IActionResult DeleteTeacher(int id)
        {
            var teacher = _context.Teachers.Find(id);
            if (teacher == null)
            {
                return NotFound();
            }

            _context.Teachers.Remove(teacher);
            _context.SaveChanges();

            return NoContent();
        }

        private bool TeacherExists(int id)
        {
            return _context.Teachers.Any(t => t.teacher_id == id);
        }
    }
}
