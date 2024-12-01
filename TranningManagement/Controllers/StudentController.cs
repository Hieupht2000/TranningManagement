using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TranningManagement.Model;

namespace TranningManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public StudentController(ApplicationDbContext context)
        {
            _context = context;
        }


        // GET: api/Student
        [HttpGet]
        public ActionResult<IEnumerable<StudentDTO>> GetStudents()
        {
            var students = _context.Students.Select(s => new StudentDTO
            {
                student_id = s.student_id,
                user_id = s.user_id,
                dob = s.dob,
                phone_number = s.phone_number,
                tuition_status = s.tuition_status,
                name = s.name,
                class_name = s.class_name,
            }).ToList();

            return Ok(students);
        }

        // GET: api/Student/5
        [HttpGet("{id}")]
        public ActionResult<StudentDTO> GetStudent(int id)
        {
            var student = _context.Students.Find(id);

            if (student == null)
            {
                return NotFound();
            }

            var studentDTO = new StudentDTO
            {
                student_id = student.student_id,
                user_id = student.user_id,
                dob = student.dob,
                phone_number = student.phone_number,
                tuition_status = student.tuition_status,
                name = student.name,
                class_name = student.class_name,
            };

            return Ok(studentDTO);
        }

        // POST: api/Student
        [HttpPost]
        public ActionResult<StudentDTO> CreateStudent(StudentDTO studentDTO)
        {
            var student = new Student
            {
                user_id = studentDTO.user_id,
                dob = studentDTO.dob,
                phone_number = studentDTO.phone_number,
                tuition_status = studentDTO.tuition_status,
                name = studentDTO.name,
                class_name=studentDTO.class_name,
            };

            _context.Students.Add(student);
            _context.SaveChanges();

            studentDTO.student_id = student.student_id;

            return CreatedAtAction(nameof(GetStudent), new { id = student.student_id }, studentDTO);
        }

        // PUT: api/Student/5
        [HttpPut("{id}")]
        public IActionResult UpdateStudent(int id, StudentDTO studentDTO)
        {
            if (id != studentDTO.student_id)
            {
                return BadRequest();
            }

            var student = _context.Students.Find(id);

            if (student == null)
            {
                return NotFound();
            }

            student.user_id = studentDTO.user_id;
            student.dob = studentDTO.dob;
            student.phone_number = studentDTO.phone_number;
            student.tuition_status = studentDTO.tuition_status;
            student.name = studentDTO.name;
            student.class_name = studentDTO.class_name;

            _context.SaveChanges();

            return NoContent();
        }

        // DELETE: api/Student/5
        [HttpDelete("{id}")]
        public IActionResult DeleteStudent(int id)
        {
            var student = _context.Students.Find(id);
            if (student == null)
            {
                return NotFound();
            }

            _context.Students.Remove(student);
            _context.SaveChanges();

            return NoContent();
        }

        private bool StudentExists(int id)
        {
            return _context.Students.Any(s => s.student_id == id);
        }
    }
}
