using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TranningManagement.Model;

namespace TranningManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LearningMaterialUploadController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        
        public LearningMaterialUploadController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST: api/uploadfile/{classId}
        [HttpPost("{classId}")]
        public async Task<IActionResult> UploadFile(int classId, IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            // Tạo thư mục lưu trữ nếu chưa có
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "learning_materials");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            // Đường dẫn đầy đủ đến file được lưu trữ
            var filePath = Path.Combine(folderPath, file.FileName);

            // Lưu file vào thư mục
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Tạo đối tượng LearningMaterial và lưu vào database
            var learningMaterial = new LearningMaterials
            {
                class_id = classId,
                material_name = Path.GetFileNameWithoutExtension(file.FileName),
                file_path = filePath // Lưu đường dẫn file
            };

            _context.learning_materials.Add(learningMaterial);
            await _context.SaveChangesAsync();

            // Trả về kết quả
            return Ok(new { message = "File uploaded successfully", file_path = filePath });
        }

    }
}
