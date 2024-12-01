using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text.Json;
using TranningManagement.Model;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TranningManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BackupController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public BackupController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Backup/Export
        [HttpGet("export")]
        public async Task<IActionResult> ExportDatabase()
        {
            try
            {
                // Lấy dữ liệu từ tất cả các bảng
                var users = await _context.Users.ToListAsync();
                var students = await _context.Students.ToListAsync();
                var classes = await _context.Classes.ToListAsync();
                var studentClasses = await _context.student_classes.ToListAsync();
                var teachers = await _context.Teachers.ToListAsync();
                var schedules = await _context.schedules
                    .Select(s => new
                    {
                        s.schedule_id,
                        s.class_id,
                        schedule_time = s.schedule_time.ToString(), // Chuyển TimeSpan thành chuỗi
                        s.schedule_date
                    })
                    .ToListAsync();

                // Tạo một object tổng hợp tất cả dữ liệu
                var databaseBackup = new
                {
                    Users = users,
                    Students = students,
                    Classes = classes,
                    StudentClasses = studentClasses,
                    Teachers = teachers,
                    Schedules = schedules
                };

                // Tuần tự hóa object thành JSON
                var jsonBackup = JsonSerializer.Serialize(databaseBackup, new JsonSerializerOptions
                {
                    WriteIndented = true // Dễ đọc hơn
                });

                // Lưu JSON vào một tệp
                var backupFilePath = Path.Combine(Directory.GetCurrentDirectory(), "DatabaseBackup.json");
                await System.IO.File.WriteAllTextAsync(backupFilePath, jsonBackup);

                // Trả về file để tải xuống
                var fileBytes = await System.IO.File.ReadAllBytesAsync(backupFilePath);
                return File(fileBytes, "application/json", "DatabaseBackup.json");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while exporting the database: {ex.Message}");
            }
        }

        [HttpPost("export")]
        public async Task<IActionResult> ExportSelectedTables([FromBody] List<string> tablesToBackup)
        {
            try
            {
                // Kiểm tra danh sách các bảng được yêu cầu
                if (tablesToBackup == null || tablesToBackup.Count == 0)
                {
                    return BadRequest("No tables selected for backup.");
                }

                // Tạo một object tổng hợp dữ liệu từ các bảng đã chọn
                var databaseBackup = new Dictionary<string, object>();

                // Nếu người dùng yêu cầu tất cả các bảng
                if (tablesToBackup.Contains("all", StringComparer.OrdinalIgnoreCase))
                {
                    databaseBackup["Users"] = await _context.Users.ToListAsync();
                    databaseBackup["Students"] = await _context.Students.ToListAsync();
                    databaseBackup["Classes"] = await _context.Classes.ToListAsync();
                    databaseBackup["StudentClasses"] = await _context.student_classes.ToListAsync();
                    databaseBackup["Teachers"] = await _context.Teachers.ToListAsync();
                    databaseBackup["Schedules"] = await _context.schedules
                        .Select(s => new
                        {
                            s.schedule_id,
                            s.class_id,
                            schedule_time = s.schedule_time.ToString(), // Chuyển đổi TimeSpan thành chuỗi
                            s.schedule_date
                        })
                        .ToListAsync();
                }
                else
                {
                    // Sao lưu từng bảng dựa trên danh sách được yêu cầu
                    foreach (var table in tablesToBackup)
                    {
                        switch (table.ToLower())
                        {
                            case "users":
                                databaseBackup["Users"] = await _context.Users.ToListAsync();
                                break; 

                            case "students":
                                databaseBackup["Students"] = await _context.Students.ToListAsync();
                                break;
                            case "classes":
                                databaseBackup["Classes"] = await _context.Classes.ToListAsync();
                                break;
                            case "studentclasses":
                                databaseBackup["StudentClasses"] = await _context.student_classes.ToListAsync();
                                break;
                            case "teachers":
                                databaseBackup["Teachers"] = await _context.Teachers.ToListAsync();
                                break;
                            case "schedules":
                                databaseBackup["Schedules"] = await _context.schedules
                                    .Select(s => new
                                    {
                                        s.schedule_id,
                                        s.class_id,
                                        schedule_time = s.schedule_time.ToString(),
                                        s.schedule_date
                                    })
                                    .ToListAsync();
                                break;
                            default:
                                return BadRequest($"Table '{table}' is not recognized.");
                        }
                    }
                }
                // Tuần tự hóa object thành JSON
                var jsonBackup = JsonSerializer.Serialize(databaseBackup, new JsonSerializerOptions
                {
                    WriteIndented = true // Tạo JSON dễ đọc
                });

                // Lưu JSON vào một tệp
                var backupFilePath = Path.Combine(Directory.GetCurrentDirectory(), "DatabaseBackup.json");
                await System.IO.File.WriteAllTextAsync(backupFilePath, jsonBackup);

                // Trả về file để tải xuống
                var fileBytes = await System.IO.File.ReadAllBytesAsync(backupFilePath);
                return File(fileBytes, "application/json", "DatabaseBackup.json");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while exporting the database: {ex.Message}");
            }
        }

        // GET: api/Backup/Tables
        [HttpGet("tables")]
        public IActionResult GetTableNames()
        {
            try
            {
                var tableNames = _context.GetType()
                    .GetProperties()
                    .Where(p => p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>))
                    .Select(p => p.Name)
                    .ToList();

                // Return the list of table names
                return Ok(tableNames);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving the table names: {ex.Message}");
            }
        }

        private async Task AddOrUpdateEntitiesAsync<T>(IEnumerable<T> entities, DbSet<T> dbSet) where T : class
        {
            foreach (var entity in entities)
            {
                try
                {
                    // Kiểm tra khóa chính và cập nhật hoặc thêm mới.
                    var keyProperties = typeof(T).GetProperties().Where(p => p.GetCustomAttributes(typeof(KeyAttribute), false).Any()).ToList();
                    if (keyProperties.Count == 0)
                    {
                        throw new InvalidOperationException($"Entity type {typeof(T).Name} does not have a key property.");
                    }

                    var primaryKeyValues = keyProperties.Select(p => p.GetValue(entity)).ToArray();
                    var existingEntity = await dbSet.FindAsync(primaryKeyValues);

                    if (existingEntity != null)
                    {
                        // Nếu tồn tại, cập nhật dữ liệu
                        _context.Entry(existingEntity).CurrentValues.SetValues(entity);
                    }
                    else
                    {
                        // Nếu chưa tồn tại, thêm mới
                        dbSet.Add(entity);
                    }
                }
                catch (Exception ex)
                {
                    // Ghi lại lỗi nếu có
                    Console.WriteLine($"Error adding/updating entity: {typeof(T).Name}, Error: {ex.Message}");
                    throw;
                }
            }

            // Lưu thay đổi vào database
            await _context.SaveChangesAsync();
        }
        [HttpPost("import")]
        public async Task<IActionResult> ImportDatabase(IFormFile backupFile)
        {
            if (backupFile == null || backupFile.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            try
            {
                // Đọc nội dung file JSON
                string jsonContent;
                using (var reader = new StreamReader(backupFile.OpenReadStream()))
                {
                    jsonContent = await reader.ReadToEndAsync();
                }

                // Deserialize file JSON
                var databaseBackup = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(jsonContent);

                // Xử lý từng bảng
                foreach (var table in databaseBackup)
                {
                    switch (table.Key.ToLower())
                    {
                        case "users":
                            var users = JsonSerializer.Deserialize<List<User>>(table.Value.GetRawText());
                            if (users != null)
                                await AddOrUpdateEntitiesAsync(users, _context.Users);
                            break;
                        case "students":
                            var students = JsonSerializer.Deserialize<List<Student>>(table.Value.GetRawText());
                            if (students != null)
                                await AddOrUpdateEntitiesAsync(students, _context.Students);
                            break;
                        case "teachers":
                            var teachers = JsonSerializer.Deserialize<List<Teacher>>(table.Value.GetRawText());
                            if (teachers != null)
                                await AddOrUpdateEntitiesAsync(teachers, _context.Teachers);
                            break;
                        case "studentclasses":
                            var studentClasses = JsonSerializer.Deserialize<List<StudentClasses>>(table.Value.GetRawText());
                            if (studentClasses != null)
                                await AddOrUpdateEntitiesAsync(studentClasses, _context.student_classes);
                            break;
                        case "classes":
                            var classes = JsonSerializer.Deserialize<List<Classes>>(table.Value.GetRawText());
                            if (classes != null)
                                await AddOrUpdateEntitiesAsync(classes, _context.Classes);
                            break;
                        case "schedules":
                            var schedules = JsonSerializer.Deserialize<List<Schedules>>(table.Value.GetRawText());
                            if (schedules != null)
                            {
                                foreach (var schedule in schedules)
                                {
                                    try
                                    {
                                        // Kiểm tra xem schedule_time có phải là kiểu DateTime không
                                        if (DateTime.TryParse(schedule.schedule_time.ToString(), out DateTime dateTime))
                                        {
                                            // Nếu là DateTime hợp lệ, gán vào schedule_time
                                            schedule.schedule_time = dateTime;
                                        }
                                        else
                                        {
                                            // Trường hợp không thể chuyển đổi, thông báo lỗi ho
                                            return BadRequest($"Invalidd schedule_time format: {schedule.schedule_time}");
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        // Xử lý lỗi nếu có
                                        return StatusCode(500, $"Error processing schedule time: {ex.Message}");
                                    }
                                }

                                // Lưu các thay đổi vào database
                                await AddOrUpdateEntitiesAsync(schedules, _context.schedules);
                            }
                            break;
                        default:
                            return BadRequest($"Table '{table.Key}' is not recognized.");
                    }
                }

                return Ok("Database successfully imported.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while importing the database: {ex.Message}");
            }
        }


    }
}
