using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Globalization;
using TranningManagement.Model;

namespace TranningManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentClassesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public StudentClassesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/student_classes
        [HttpGet]
        public ActionResult<IEnumerable<StudentClassesDTO>> GetStudentClasses()
        {
            var studentClassesList = GetStudentClassesFiltered();
            return Ok(studentClassesList);
        }

        // Hàm con để lấy thông tin từ bảng student_classes với điều kiện
        private List<StudentInfoDTO> GetStudentClassesFiltered(int? studentId = null, int? classId = null, DateOnly? registrationDate = null)
        {
            // Lấy danh sách từ bảng student_classes
            var studentClassesList = _context.student_classes.AsNoTracking().ToList();

            // Tạo danh sách kết quả với thông tin học sinh
            var result = new List<StudentInfoDTO>();

            foreach (var sc in studentClassesList)
            {
                // Áp dụng các điều kiện lọc nếu được cung cấp
                bool isMatch = true;

                if (studentId.HasValue && sc.student_id != studentId.Value)
                {
                    isMatch = false;
                }

                if (classId.HasValue && sc.class_id != classId.Value)
                {
                    isMatch = false;
                }

                if (registrationDate.HasValue && DateOnly.FromDateTime(sc.Registrationdate) <= registrationDate.Value)
                {
                    isMatch = false;
                }

                // Nếu bản ghi thỏa tất cả các điều kiện, tiếp tục xử lý
                if (isMatch)
                {
                    // Truy xuất thông tin học sinh từ bảng Students
                    var student = _context.Students.AsNoTracking().FirstOrDefault(s => s.student_id == sc.student_id);

                    if (student != null)
                    {
                        // Kiểm tra và thêm vào danh sách nếu chưa có bản ghi trùng lặp
                        var studentInfoDTO = new StudentInfoDTO
                        {
                            student_id = sc.student_id,
                            class_id = sc.class_id,
                            Registrationdate = DateOnly.FromDateTime(sc.Registrationdate),
                            name = student.name,
                            phone_number = student.phone_number,
                            tuition_status = student.tuition_status
                        };

                        result.Add(studentInfoDTO);
                    }
                }
            }

            return result;
        }


        // Hàm con để xuất danh sách StudentInfoDTO ra file Excel
        private byte[] ExportToExcel(List<StudentInfoDTO> studentInfoList)
        {
            // Check if the list is empty
            if (studentInfoList == null || studentInfoList.Count == 0)
            {
                Console.WriteLine("No student information available to export.");
                return null; // Or handle accordingly
            }
            // Cài đặt context giấy phép của EPPlus
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage())
            {
                // Tạo một worksheet mới
                var worksheet = package.Workbook.Worksheets.Add("Student Info");

                // Đặt tiêu đề cho các cột
                worksheet.Cells[1, 1].Value = "Student ID";
                worksheet.Cells[1, 2].Value = "Class ID";
                worksheet.Cells[1, 3].Value = "Registration Date";
                worksheet.Cells[1, 4].Value = "Name";
                worksheet.Cells[1, 5].Value = "Phone Number";
                worksheet.Cells[1, 6].Value = "Tuition Status";

                // Đặt kiểu chữ đậm cho tiêu đề
                using (var range = worksheet.Cells[1, 1, 1, 6])
                {
                    range.Style.Font.Bold = true;
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                }

                // Điền dữ liệu từ danh sách studentInfoList vào các dòng của worksheet
                for (int i = 0; i < studentInfoList.Count; i++)
                {
                    var student = studentInfoList[i];
                    worksheet.Cells[i + 2, 1].Value = student.student_id;
                    worksheet.Cells[i + 2, 2].Value = student.class_id;
                    worksheet.Cells[i + 2, 3].Value = student.Registrationdate.ToString("MM/dd/yyyy");
                    worksheet.Cells[i + 2, 4].Value = student.name;
                    worksheet.Cells[i + 2, 5].Value = student.phone_number;
                    worksheet.Cells[i + 2, 6].Value = student.tuition_status;
                }

                // Tự động điều chỉnh kích thước cột
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                // Lưu file Excel vào MemoryStream và trả về dạng mảng byte
                using (var stream = new MemoryStream())
                {
                    package.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }

        // GET: api/student/classes/export
        [HttpGet("export")]
        public IActionResult ExportStudentClassesToExcel(int? studentId = null, int? classId = null, DateOnly? registrationDate = null)
        {
            var filteredStudentClasses = GetStudentClassesFiltered(studentId, classId, registrationDate);

            // Kiểm tra danh sách đã lọc có dữ liệu
            if (filteredStudentClasses == null || filteredStudentClasses.Count == 0)
            {
                return NotFound("No student classes found with the specified filters.");
            }

            var excelData = ExportToExcel(filteredStudentClasses);

            if (excelData == null || excelData.Length == 0)
            {
                return StatusCode(500, "Error exporting data to Excel.");
            }

            return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "StudentClasses.xlsx");
        }

        // GET: api/student_classes/5/10
        [HttpGet("{studentId}/{classId}")]
        public ActionResult<StudentInfoDTO> GetStudentClass(int studentId, int classId)
        {
            var studentClassDTO = GetStudentClassDTO(studentId, classId);

            if (studentClassDTO == null)
            {
                return NotFound();
            }

            return Ok(studentClassDTO);
        }

        // Hàm con để lấy thông tin một học sinh trong lớp
        private StudentInfoDTO GetStudentClassDTO(int studentId, int classId)
        {
            var studentClass = _context.student_classes
                .FirstOrDefault(sc => sc.student_id == studentId && sc.class_id == classId);

            if (studentClass == null)
            {
                return null;
            }

            var studentInfo = _context.Students
                .FirstOrDefault(s => s.student_id == studentId);

            if (studentInfo == null)
            {
                return null;
            }

            return new StudentInfoDTO
            {
                student_id = studentClass.student_id,
                class_id = studentClass.class_id,
                Registrationdate = DateOnly.FromDateTime(studentClass.Registrationdate),
                name = studentInfo.name,
                phone_number = studentInfo.phone_number,
                tuition_status = studentInfo.tuition_status
            };
        }


        // POST: api/studentclasses
        [HttpPost("add")]
        public ActionResult<StudentClassesDTO> AddStudentToClass(StudentClassesDTO addStudentToClassDTO)
        {
            // Kiểm tra nếu mã sinh viên có tồn tại
            var studentExists = _context.Students.Any(s => s.student_id == addStudentToClassDTO.student_id);
            if (!studentExists)
            {
                return BadRequest($"Student with ID {addStudentToClassDTO.student_id} does not exist.");
            }

            // Kiểm tra nếu mã lớp học có tồn tại
            var classExists = _context.Classes.Any(c => c.class_id == addStudentToClassDTO.class_id);
            if (!classExists)
            {
                return BadRequest($"Class with ID {addStudentToClassDTO.class_id} does not exist.");
            }

            // Kiểm tra nếu sinh viên đã được thêm vào lớp học này trước đó
            var studentInClass = _context.student_classes.Any(sc => sc.student_id == addStudentToClassDTO.student_id && sc.class_id == addStudentToClassDTO.class_id);
            if (studentInClass)
            {
                return Conflict($"Student with ID {addStudentToClassDTO.student_id} is already enrolled in class {addStudentToClassDTO.class_id}.");
            }

            var studentclass = new StudentClasses
            {
                student_id = addStudentToClassDTO.student_id,
                class_id = addStudentToClassDTO.class_id,
                Registrationdate = addStudentToClassDTO.Registrationdate.ToDateTime(TimeOnly.MinValue),
            };
            _context.student_classes.Add(studentclass);
            _context.SaveChanges();

            addStudentToClassDTO.class_id = studentclass.class_id;
            return CreatedAtAction(nameof(GetStudentClass), new { id = studentclass.class_id }, addStudentToClassDTO);
        }

        // POST: api/studentclasses/upload
        [HttpPost("upload")]
        public async Task<IActionResult> UploadStudentList(IFormFile file, int classId)
        {
            // Set EPPlus license context
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            var students = new List<StudentDTO>();

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets[0]; // Get the first worksheet

                    for (int row = 2; row <= worksheet.Dimension.End.Row; row++) // Start from row 2 to skip header
                    {
                        if (worksheet.Cells[row, 1].Text == null || worksheet.Cells[row, 2].Text == null)
                        {
                            continue; // Skip rows with null values in required fields
                        }

                        var student = new StudentDTO
                        {
                            student_id = int.TryParse(worksheet.Cells[row, 1].Text, out var studentId) ? studentId : 0,
                            user_id = int.TryParse(worksheet.Cells[row, 2].Text, out var userId) ? userId : 0,
                            dob = DateTime.TryParseExact(worksheet.Cells[row, 3].Text, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dob) ? dob : DateTime.MinValue,
                            phone_number = worksheet.Cells[row, 4]?.Text,
                            tuition_status = worksheet.Cells[row, 5]?.Text,
                            name = worksheet.Cells[row, 6]?.Text,
                            class_name = worksheet.Cells[row, 7]?.Text
                        };

                        // Skip student if ID or user ID parsing fails
                        if (student.student_id == 0 || student.user_id == 0)
                        {
                            continue;
                        }

                        // Check if the student exists in the database
                        var existingStudent = await _context.Students.FindAsync(student.student_id);
                        if (existingStudent != null)
                        {
                            students.Add(student); // Only add students who exist in the Students table
                        }
                    }
                }
            }

            // Retrieve the class to add students to
            var classEntity = await _context.Classes.FirstOrDefaultAsync(c => c.class_id == classId);
            if (classEntity == null)
            {
                Console.WriteLine($"Không tìm thấy lớp với ID {classId}.");
                return NotFound("Không tìm thấy lớp.");
            }

            // Check if the class has enough capacity
            var currentStudentCount = await _context.student_classes.CountAsync(sc => sc.class_id == classId);
            var remainingSeats = classEntity.max_students - currentStudentCount;

            if (remainingSeats <= 0)
            {
                return BadRequest("Class is full, please register for a different class.");
            }

            // Add only students who can fit in the remaining seats
            var studentsToBeAdded = students.Take(remainingSeats).ToList();
            foreach (var student in studentsToBeAdded)
            {
                // Add the student to the StudentClasses entity (the junction table)
                var studentClass = new StudentClasses
                {
                    student_id = student.student_id,
                    class_id = classId,
                    Registrationdate = DateTime.Now
                };

                _context.student_classes.Add(studentClass);
            }

            // Save changes to the database
            await _context.SaveChangesAsync();
            // Check if the class is full after adding students
            if (studentsToBeAdded.Count == 0)
            {
                return Ok(new { message = "Class is already full. No students were added.", TotalAdded = 0 });
            }
            else if (students.Count > studentsToBeAdded.Count)
            {
                return Ok(new { message = "Some students were added, but the class is now full.", TotalAdded = studentsToBeAdded.Count });
            }

            return Ok(new { message = "All students were added successfully.", TotalAdded = studentsToBeAdded.Count });
        }
    


        // PUT: api/student_classes/5/10
        [HttpPut("{studentId}/{classId}")]
        public IActionResult UpdateEnrollment(int studentId, int classId, StudentClassesDTO studentClassDTO)
        {
            if (studentId != studentClassDTO.student_id || classId != studentClassDTO.class_id)
            {
                return BadRequest();
            }

            var studentClass = _context.student_classes.FirstOrDefault(sc => sc.student_id == studentId && sc.class_id == classId);

            if (studentClass == null)
            {
                return NotFound();
            }

            studentClass.Registrationdate = studentClassDTO.Registrationdate.ToDateTime(TimeOnly.MaxValue);

            _context.SaveChanges();

            return NoContent();
        }

        // DELETE: api/student_classes/5/10
        [HttpDelete("{studentId}/{classId}")]
        public IActionResult DeleteEnrollment(int studentId, int classId)
        {
            var studentClass = _context.student_classes.FirstOrDefault(sc => sc.student_id == studentId && sc.class_id == classId);
            if (studentClass == null)
            {
                return NotFound();
            }

            _context.student_classes.Remove(studentClass);
            _context.SaveChanges();

            return NoContent();
        }

        private bool StudentClassesExists(int id)
        {
            return _context.student_classes.Any(y => y.student_id == id);
        }       
    }
}
