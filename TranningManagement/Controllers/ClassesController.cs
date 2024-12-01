using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TranningManagement.Model;

namespace TranningManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClassesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public ClassesController(ApplicationDbContext context) 
        {
            _context = context;
        }

        // GET: api/Classes
        [HttpGet]
        public ActionResult<IEnumerable<ClassesDTO>> GetClasses()
        {
            var classesList = _context.Classes.Select(c => new ClassesDTO
            {
                class_id = c.class_id,
                class_name = c.class_name,
                description = c.description,
                teacher_id = c.teacher_id,
                start_date = c.start_date,
                end_date = c.end_date,
                tuition = c.tuition,
                max_students = c.max_students,
                teacher_name = c.teacher_name,
                classstatus = c.classstatus 
            }).ToList();

            return Ok(classesList);
        }

        // GET: api/Classes/5
        [HttpGet("{id}")]
        public ActionResult<ClassesDTO> GetClassById(int id)
        {
            var classEntity = _context.Classes.Find(id);

            if (classEntity == null)
            {
                return NotFound();
            }

            var classDTO = new ClassesDTO
            {
                class_id = classEntity.class_id,
                class_name = classEntity.class_name,
                description = classEntity.description,
                teacher_id = classEntity.teacher_id,
                start_date = classEntity.start_date,
                end_date = classEntity.end_date,
                tuition = classEntity.tuition,
                max_students = classEntity.max_students,
                teacher_name = classEntity.teacher_name,
                classstatus = classEntity.classstatus ?? "Not Assigned"
            };

            return Ok(classDTO);
        }

        // POST: api/Classes
        [HttpPost]
        public ActionResult<ClassesDTO> CreateClass(ClassesDTO classDTO)
        {
            var classEntity = new Classes
            {
                class_name = classDTO.class_name,
                description = classDTO.description,
                teacher_id = classDTO.teacher_id,
                start_date = classDTO.start_date,
                end_date = classDTO.end_date,
                tuition = classDTO.tuition,
                max_students = classDTO.max_students,
                teacher_name = classDTO.teacher_name,
                classstatus = classDTO.classstatus ?? "Pending"

            };

            _context.Classes.Add(classEntity);
            _context.SaveChanges();

            classDTO.class_id = classEntity.class_id;

            return CreatedAtAction(nameof(GetClassById), new { id = classEntity.class_id }, classDTO);
        }

        // PUT: api/Classes/5
        [HttpPut("{id}")]
        public IActionResult UpdateClass(int id, ClassesDTO classDTO)
        {
            if (id != classDTO.class_id)
            {
                return BadRequest();
            }

            var classEntity = _context.Classes.Find(id);

            if (classEntity == null)
            {
                return NotFound();
            }

            classEntity.class_name = classDTO.class_name;
            classEntity.description = classDTO.description;
            classEntity.teacher_id = classDTO.teacher_id;
            classEntity.start_date = classDTO.start_date;
            classEntity.end_date = classDTO.end_date;
            classEntity.tuition = classDTO.tuition;
            classEntity.max_students = classDTO.max_students;
            classEntity.teacher_name = classDTO.teacher_name;
            classEntity.classstatus = classDTO.classstatus;

            _context.SaveChanges();

            return NoContent();
        }

        // DELETE: api/Classes/5
        [HttpDelete("{id}")]
        public IActionResult DeleteClass(int id)
        {
            var classEntity = _context.Classes.Find(id);
            if (classEntity == null)
            {
                return NotFound();
            }

            _context.Classes.Remove(classEntity);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpGet("{id}/details")]
        public ActionResult<ClassDetailsDTO> GetClassDetails(int id)
        {
            // Lấy thông tin lớp học
            var classEntity = _context.Classes
                .Where(c => c.class_id == id)
                .Select(c => new
                {
                    c.class_id,
                    c.class_name,
                    c.description,
                    c.teacher_id,
                    c.start_date,
                    c.end_date,
                    c.tuition,
                    c.max_students,
                    c.classstatus,
                    TeacherName = c.teacher_name,
                })
                .FirstOrDefault();

            if (classEntity == null)
            {
                return NotFound("Class not found.");
            }
            //viết api xuất toàn bộ db đồng bộ bằng file json một cái dum bakup từng bảng backup 
            // Lấy danh sách học sinh
            var studentClasses = _context.student_classes
            .Where(sc => sc.class_id == id)
            .ToList();

            var students = new List<StudentDTO>();

            foreach (var sc in studentClasses)
            {
                var student = _context.Students.Where(st => st.student_id == sc.student_id).FirstOrDefault();
                students.Add(new StudentDTO
                {
                    student_id = student.student_id,
                    user_id = student.user_id,
                    dob = student.dob,
                    phone_number = student.phone_number,
                    tuition_status = student.tuition_status,
                    name = student.name,
                    class_name = student.class_name,
                    
                });
            }

            // Lấy lịch học
            var schedules = _context.schedules
                .Where(s => s.class_id == id)
                .Select(s => new SchedulesDTO
                {
                    schedule_id = s.schedule_id,
                    class_id = s.class_id,
                    teacher_id = s.teacher_id,
                    schedule_date = DateOnly.FromDateTime(s.schedule_date),
                    schedule_time = TimeOnly.FromDateTime(s.schedule_date),
                    location = s.location,
                })
                .ToList();

            // Lấy tài liệu học tập
            var learningMaterials = _context.learning_materials
                .Where(m => m.class_id == id)
                .Select(m => new LearningMaterialsDTO
                {
                    material_id = m.material_id,
                    class_id = m.class_id,
                    material_name = m.material_name,
                    file_path = m.file_path
                })
                .ToList();

            // Kết hợp tất cả thông tin vào DTO
            var classDetails = new ClassDetailsDTO
            {
                ClassId = classEntity.class_id,
                ClassName = classEntity.class_name,
                Description = classEntity.description,
                TeacherName = classEntity.TeacherName,
                Students = students,
                Schedules = schedules,
                LearningMaterials = learningMaterials
            };

            return Ok(classDetails);
        }

        private bool ClassesExists(int id)
        {
            return _context.Classes.Any(e => e.class_id == id);
        }
    }
}
