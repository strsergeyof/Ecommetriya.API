using Ecommetriya.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommetriya.API.Controllers
{
    public class OrdersController(ILogger<OrdersController> logger, ApplicationDbContext context) : Controller
    {

        [Authorize]
        [HttpGet("/orders")]
        public IActionResult Get()
        {
            var user = HttpContext.User;
            var id = user.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;

            List<Store> stores = context.Stores
                .Include(x => x.Orders)
                .Where(x => x.UserId.ToString() == id).ToList();

            return Json(stores);
        }
    }
}
