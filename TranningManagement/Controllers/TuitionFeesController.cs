using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TranningManagement.Model;

namespace TranningManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TuitionFeesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public TuitionFeesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TuitionFeesDTO>>> GetAllTuitionFees()
        {
            var tuitionFees = await _context.tuition_fees
                .Select(tuition => new TuitionFeesDTO
                {
                    payment_id = tuition.payment_id,
                    student_id = tuition.student_id,
                    class_id = tuition.class_id,
                    total_amount = tuition.total_amount,
                    payment_status = tuition.payment_status,
                    due_date = DateOnly.FromDateTime(tuition.due_date),
                    name = tuition.name,
                })
                .ToListAsync();

            return Ok(tuitionFees); // Trả về danh sách các khoản học phí dưới dạng DTO
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TuitionFeesDTO>> GetTuitionFeeById(int id)
        {
            var tuitionFee = await _context.tuition_fees
                .Where(tuition => tuition.payment_id == id)
                .Select(tuition => new TuitionFeesDTO
                {
                    payment_id = tuition.payment_id,
                    student_id = tuition.student_id,
                    class_id = tuition.class_id,
                    total_amount = tuition.total_amount,
                    payment_status = tuition.payment_status,
                    due_date = DateOnly.FromDateTime(tuition.due_date),
                    name = tuition.name
                })
                .FirstOrDefaultAsync();

            if (tuitionFee == null)
            {
                return NotFound("Tuition fee not found."); // Trả về NotFound nếu không tìm thấy học phí
            }

            return Ok(tuitionFee); // Trả về thông tin học phí
        }

        [HttpPost]
        public async Task<ActionResult<TuitionFeesDTO>> CreateTuitionFee(TuitionFeesDTO dto)
        {
            var tuitionFee = new TuitionFees
            {
                student_id = dto.student_id,
                class_id = dto.class_id,
                total_amount = dto.total_amount,
                payment_status = dto.payment_status,
                due_date = dto.due_date.ToDateTime(new TimeOnly(0)), // Convert DateOnly to DateTime if needed
                name = dto.name
            };

            _context.tuition_fees.Add(tuitionFee);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAllTuitionFees), new { id = tuitionFee.payment_id }, dto);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTuitionFee(int id, TuitionFeesDTO dto)
        {
            if (id != dto.payment_id)
            {
                return BadRequest("Payment ID mismatch.");
            }

            var tuitionFee = await _context.tuition_fees.FindAsync(id);
            if (tuitionFee == null)
            {
                return NotFound("Tuition fee not found.");
            }

            // Cập nhật thông tin
            tuitionFee.student_id = dto.student_id;
            tuitionFee.class_id = dto.class_id;
            tuitionFee.total_amount = dto.total_amount;
            tuitionFee.payment_status = dto.payment_status;
            tuitionFee.due_date = dto.due_date.ToDateTime(new TimeOnly(0)); // Convert DateOnly to DateTime
            tuitionFee.name = dto.name;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.tuition_fees.Any(e => e.payment_id == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent(); // Trả về NoContent nếu thành công
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTuitionFee(int id)
        {
            var tuitionFee = await _context.tuition_fees.FindAsync(id);
            if (tuitionFee == null)
            {
                return NotFound("Tuition fee not found.");
            }

            _context.tuition_fees.Remove(tuitionFee);
            await _context.SaveChangesAsync();

            return NoContent(); // Trả về NoContent nếu xoá thành công
        }
        private bool TuitionFeeExists(int id)
        {
            return _context.tuition_fees.Any(f => f.payment_id == id);
        }
    }
}
