using Ecommetriya.API.Models;
using Ecommetriya.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Ecommetriya.API.Controllers
{
    public class СompetitorsController(ILogger<OrdersController> logger, ApplicationDbContext context) : Controller
    {

        [Authorize]
        [HttpGet("/competitors")]
        public IActionResult GetCompetitors()
        {
            var user = HttpContext.User;
            var id = user.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;

            if (string.IsNullOrEmpty(id)) return NotFound();

            List<Store> stores = context.Stores
                .Include(s => s.Сompetitors)
                .ThenInclude(x => x.Photos)
                .Include(s => s.Сompetitors)
                .ThenInclude(x => x.Statistics)
                .Where(x => x.UserId.ToString() == id).ToList();

            return Json(stores);
        }

        [Authorize]
        [HttpGet("/competitors/{guid}")]
        public async Task<IActionResult> GetCompetitorsId(Guid guid)
        {
            var user = HttpContext.User;
            var id = user.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;

            if (string.IsNullOrEmpty(id) || context.Competitors is null) return NotFound();

            var competitor = context?.Competitors.FindAsync(guid);

            if (competitor is null) return NotFound();

            return Json(competitor);
        }

        [Authorize]
        [HttpPost("/competitors")]
        public async Task<ActionResult<List<Competitor>>> AddCompetitors([FromBody]Competitor competitors)
        {
            try
            {
                var user = HttpContext.User;
                var id = user.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;

                //Есть ли пользователя нет в базе данных
                if (string.IsNullOrEmpty(id) || context.Competitors is null) return NotFound();

                List<Competitor> competitors_ = new List<Competitor>();
                competitors_.Add(competitors);

                //Если нигде не был указан правильный магазин, выводим ошибку запроса
                if (competitors_ == null || competitors_?.Count == 0) return BadRequest();

                //Получаем список конкурентов с данными
                List<Competitor> _competitor = await DataRepository.GetCompetitorsFromApi(competitors_);

                //Если не получили информацию по конкурентам, то выдаем ошибку запроса
                if (_competitor is null || _competitor?.Count() == 0) return BadRequest();

                //В противном случаем добавляем конкурентов в базу данных
                await context?.Competitors?.AddRangeAsync(_competitor);
                await context?.SaveChangesAsync();

                return Json(_competitor);
            }
            catch
            {
                return BadRequest();
            }
        }

        [Authorize]
        [HttpDelete("/competitors/{guid}")]
        public async Task<IActionResult> RemoveCompetitorsId(Guid guid)
        {
            var user = HttpContext.User;
            var id = user.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;

            if (string.IsNullOrEmpty(id) || context.Competitors is null) return BadRequest();

            var competitor = context?.Competitors.FindAsync(guid);

            if (competitor is null) return BadRequest();

            context?.Remove(competitor);
            await context?.SaveChangesAsync();

            return Ok();
        }
    }
}