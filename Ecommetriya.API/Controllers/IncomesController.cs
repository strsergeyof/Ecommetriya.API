using Ecommetriya.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommetriya.API.Controllers
{
    public class IncomesController(ILogger<OrdersController> logger, ApplicationDbContext context) : Controller
    {

        [Authorize]
        [HttpGet("/incomes")]
        public IActionResult Get()
        {
            var user = HttpContext.User;
            var id = user.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;

            List<Store> stores = context.Stores
                .Include(x => x.Incomes)
                .Where(x => x.UserId.ToString() == id).ToList();

            return Json(stores);
        }
    }
}
