using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TranningManagement.Model;

namespace TranningManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PromotionsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public PromotionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/promotions
        [HttpGet]
        public ActionResult<IEnumerable<PromotionsDTO>> GetPromotions()
        {
            var promotions = _context.promotions.Select(p => new PromotionsDTO
            {
                promotion_id = p.promotion_id,
                class_id = p.class_id,
                discount_percentage = p.discount_percentage,
                start_date = DateOnly.FromDateTime(p.start_date),
                end_date = DateOnly.FromDateTime(p.end_date)
            }).ToList();

            return Ok(promotions);
        }

        // GET: api/promotions/{id}
        [HttpGet("{id}")]
        public ActionResult<PromotionsDTO> GetPromotion(int id)
        {
            var promotion = _context.promotions.Find(id);

            if (promotion == null)
            {
                return NotFound();
            }

            var promotionDTO = new PromotionsDTO
            {
                promotion_id = promotion.promotion_id,
                class_id = promotion.class_id,
                discount_percentage = promotion.discount_percentage,
                start_date = DateOnly.FromDateTime(promotion.start_date),
                end_date = DateOnly.FromDateTime(promotion.end_date)
            };

            return Ok(promotionDTO);
        }

        // POST: api/promotions
        [HttpPost]
        public ActionResult<PromotionsDTO> CreatePromotion(PromotionsDTO promotionDTO)
        {
            var promotion = new Promotions
            {
                class_id = promotionDTO.class_id,
                discount_percentage = promotionDTO.discount_percentage,
                start_date = promotionDTO.start_date.ToDateTime(TimeOnly.MinValue),
                end_date = promotionDTO.end_date.ToDateTime(TimeOnly.MinValue)
            };

            _context.promotions.Add(promotion);
            _context.SaveChanges();

            promotionDTO.promotion_id = promotion.promotion_id;

            return CreatedAtAction(nameof(GetPromotion), new { id = promotion.promotion_id }, promotionDTO);
        }

        // PUT: api/promotions/{id}
        [HttpPut("{id}")]
        public IActionResult UpdatePromotion(int id, PromotionsDTO promotionDTO)
        {
            var promotion = _context.promotions.Find(id);

            if (promotion == null)
            {
                return NotFound();
            }

            promotion.class_id = promotionDTO.class_id;
            promotion.discount_percentage = promotionDTO.discount_percentage;
            promotion.start_date = promotionDTO.start_date.ToDateTime(TimeOnly.MinValue);
            promotion.end_date = promotionDTO.end_date.ToDateTime(TimeOnly.MinValue);

            _context.promotions.Update(promotion);
            _context.SaveChanges();

            return NoContent();
        }

        // DELETE: api/promotions/{id}
        [HttpDelete("{id}")]
        public IActionResult DeletePromotion(int id)
        {
            var promotion = _context.promotions.Find(id);

            if (promotion == null)
            {
                return NotFound();
            }

            _context.promotions.Remove(promotion);
            _context.SaveChanges();

            return NoContent();
        }
        private bool PromotionsExists(int id)
        {
            return _context.promotions.Any(p => p.promotion_id == id);
        }
    }
}
