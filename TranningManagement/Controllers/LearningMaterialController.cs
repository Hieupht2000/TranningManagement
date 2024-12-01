using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TranningManagement.Model;

namespace TranningManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LearningMaterialController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public LearningMaterialController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/learningmaterials
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LearningMaterialsDTO>>> GetLearningMaterials()
        {
            var materials = await _context.learning_materials
                .Select(m => new LearningMaterialsDTO
                {
                    material_id = m.material_id,
                    class_id = m.class_id,
                    material_name = m.material_name,
                    file_path = m.file_path
                }).ToListAsync();

            return Ok(materials);
        }

        // GET: api/learningmaterials/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<LearningMaterialsDTO>> GetLearningMaterial(int id)
        {
            var material = await _context.learning_materials.FindAsync(id);

            if (material == null)
            {
                return NotFound();
            }

            var materialDTO = new LearningMaterialsDTO
            {
                material_id = material.material_id,
                class_id = material.class_id,
                material_name = material.material_name,
                file_path = material.file_path
            };

            return Ok(materialDTO);
        }

        // POST: api/learningmaterials
        [HttpPost]
        public async Task<ActionResult<LearningMaterialsDTO>> PostLearningMaterial(LearningMaterialsDTO learningMaterialsDTO)
        {
            var material = new LearningMaterials
            {
                class_id = learningMaterialsDTO.class_id,
                material_name = learningMaterialsDTO.material_name,
                file_path = learningMaterialsDTO.file_path
            };

            _context.learning_materials.Add(material);
            await _context.SaveChangesAsync();

            learningMaterialsDTO.material_id = material.material_id; // Cập nhật ID

            return CreatedAtAction(nameof(GetLearningMaterial), new { id = material.material_id }, learningMaterialsDTO);
        }

        // PUT: api/learningmaterials/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLearningMaterial(int id, LearningMaterialsDTO learningMaterialsDTO)
        {
            if (id != learningMaterialsDTO.material_id)
            {
                return BadRequest();
            }

            var material = await _context.learning_materials.FindAsync(id);
            if (material == null)
            {
                return NotFound();
            }

            // Cập nhật các trường
            material.class_id = learningMaterialsDTO.class_id;
            material.material_name = learningMaterialsDTO.material_name;
            material.file_path = learningMaterialsDTO.file_path;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/learningmaterials/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLearningMaterial(int id)
        {
            var material = await _context.learning_materials.FindAsync(id);
            if (material == null)
            {
                return NotFound();
            }

            _context.learning_materials.Remove(material);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LearningMaterialExists(int id)
        {
            return _context.learning_materials.Any(m => m.material_id == id);
        }
    }
}
