using Ecommetriya.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommetriya.API.Controllers
{
    public class FeedController(ILogger<OrdersController> logger, ApplicationDbContext context) : Controller
    {

        [Authorize]
        [HttpGet("/feed")]
        public IActionResult Feed()
        {
            var user = HttpContext.User;
            var id = user.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;

            List<Store> stores = context.Stores
                .Include(x => x.CardFeeds)
                .Where(x => x.UserId.ToString() == id).ToList();

            return Json(stores);
        }

        [Authorize]
        [HttpGet("/feed/dispatched")]
        public IActionResult Dispatched()
        {
            var user = HttpContext.User;
            var id = user.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;

            List<Store> stores = context.Stores
                .Include(x => x.CardsDispatched)
                .Where(x => x.UserId.ToString() == id).ToList();

            return Json(stores);
        }

        [Authorize]
        [HttpGet("/feed/purchased")]
        public IActionResult Purchased()
        {
            var user = HttpContext.User;
            var id = user.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;

            List<Store> stores = context.Stores
                .Include(x => x.CardsPurchased)
                .Where(x => x.UserId.ToString() == id).ToList();

            return Json(stores);
        }

        [Authorize]
        [HttpGet("/feed/returns")]
        public IActionResult Returns()
        {
            var user = HttpContext.User;
            var id = user.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;

            List<Store> stores = context.Stores
                .Include(x => x.CardsReturns)
                .Where(x => x.UserId.ToString() == id).ToList();

            return Json(stores);
        }

        [Authorize]
        [HttpGet("/feed/nullstatus")]
        public IActionResult NullStatuses()
        {
            var user = HttpContext.User;
            var id = user.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;

            List<Store> stores = context.Stores
                .Include(x => x.CardNullStatuses)
                .Where(x => x.UserId.ToString() == id).ToList();

            return Json(stores);
        }
    }
}
