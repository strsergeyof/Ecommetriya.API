using Ecommetriya.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Ecommetriya.API.Controllers
{
    public class AdvertsController(ILogger<OrdersController> logger, ApplicationDbContext context) : Controller
    {

        [Authorize]
        [HttpGet("/adverts")]
        public IActionResult Get()
        {
            var user = HttpContext.User;
            var id = user.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;

            //List<Store> stores = context.Cards.

            List<Store> stores = context.Stores
                .Include(x => x.CardWildberries)
     .Include(x => x.Adverts)
     .ThenInclude(x => x.AdvertsStatistics)
     .Where(x => x.UserId == Guid.Parse(id))
     .ToList();

            var list = stores.Select(x => new
            {
                // Копируем все свойства из x
                // Может потребоваться дополнительная настройка свойств, в зависимости от ваших требований
                Id = x.Id,
                StoreName = x.StoreName,
                // Добавляем обработку статистики рекламы
                Adverts = x.Adverts.Select(a => new
                {
                    // Копируем все свойства из a
                    // Может потребоваться дополнительная настройка свойств, в зависимости от ваших требований
                    Id = a.AdvertId,
                    Name = a.Name,
                    CreateTime = a.CreateTime,
                    EndTime = a.EndTime,
                    Status = a.Status,
                    Views = a.AdvertsStatistics.Sum(x => x.Views),
                    Clicks = a.AdvertsStatistics.Sum(x => x.Clicks),
                    Ctr = (a.AdvertsStatistics.Sum(x => x.Clicks) == 0 || a.AdvertsStatistics.Sum(x => x.Views) == 0) ? 0 : ((double)a.AdvertsStatistics.Sum(x => x.Clicks) / (double)a.AdvertsStatistics.Sum(x => x.Views)) * 100,
                    Atbs = a.AdvertsStatistics.Sum(x => x.Atbs),
                    Cpc = a.AdvertsStatistics.Sum(x => x.Cpc),
                    Cr = (a.AdvertsStatistics.Sum(x => x.Orders) == 0 || a.AdvertsStatistics.Sum(x => x.Clicks) == 0) ? 0 : ((double)a.AdvertsStatistics.Sum(x => x.Orders) / (double)a.AdvertsStatistics.Sum(x => x.Clicks)) * 100,
                    Orders = a.AdvertsStatistics.Sum(x => x.Orders),
                    Shks = a.AdvertsStatistics.Sum(x => x.Shks),
                    Sum = a.AdvertsStatistics.Sum(x => x.Sum),
                    Sum_price = a.AdvertsStatistics.Sum(x => x.Sum_price),
                    Nms = a.AdvertsStatistics
                    .GroupBy(stat => new { stat.AdvertId, stat.nmId })
                        .Select(group => new
                        {
                            nmId = group.Key.nmId,
                            Images = new[]
                            {
                               new{ Tm = "https://basket-12.wbbasket.ru/vol1778/part177868/177868341/images/tm/6.jpg" },
                                new{Tm = "https://basket-12.wbbasket.ru/vol1778/part177868/177868341/images/tm/5.jpg" },
                                new{Tm = "https://basket-12.wbbasket.ru/vol1778/part177868/177868341/images/tm/4.jpg" },
                               new{ Tm = "https://basket-12.wbbasket.ru/vol1778/part177868/177868341/images/tm/3.jpg" }
                            },                            Views = group.Sum(x => x.Views),
                            Clicks = group.Sum(x => x.Clicks),
                            Ctr = (group.Sum(x => x.Clicks) == 0 || group.Sum(x => x.Views) == 0) ? 0 : ((double)group.Sum(x => x.Clicks) / (double)group.Sum(x => x.Views)) * 100,
                            Atbs = group.Sum(x => x.Atbs),
                            Cpc = group.Sum(x => x.Cpc),
                            Cr = (group.Sum(x => x.Orders) == 0 || group.Sum(x => x.Clicks) == 0) ? 0 : ((double)group.Sum(x => x.Orders) / (double)group.Sum(x => x.Clicks)) * 100,
                            Orders = group.Sum(x => x.Orders),
                            Shks = group.Sum(x => x.Shks),
                            Sum = group.Sum(x => x.Sum),
                            Sum_price = group.Sum(x => x.Sum_price),
                            weekly_stats = group.GroupBy(a => CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(a.Date, CalendarWeekRule.FirstDay, DayOfWeek.Monday))
                            .Select(group2 => new
                            {
                                WeekOfYear = $"{GetLastMonday(group2.Min(x => x.Date)).ToString("dd.MM.yyyy")} - {GetLastMonday(group2.Min(x => x.Date)).AddDays(6).ToString("dd.MM.yyyy")}",
                                Views = group2.Sum(x => x.Views),
                                Clicks = group2.Sum(x => x.Clicks),
                                Ctr = (group2.Sum(x => x.Clicks) == 0 || group2.Sum(x => x.Views) == 0) ? 0 : ((double)group2.Sum(x => x.Clicks) / (double)group2.Sum(x => x.Views)) * 100,
                                Atbs = group2.Sum(x => x.Atbs),
                                Cpc = group2.Sum(x => x.Cpc),
                                Cr = (group2.Sum(x => x.Orders) == 0 || group2.Sum(x => x.Clicks) == 0) ? 0 : ((double)group2.Sum(x => x.Orders) / (double)group2.Sum(x => x.Clicks)) * 100,
                                Orders = group2.Sum(x => x.Orders),
                                Shks = group2.Sum(x => x.Shks),
                                Sum = group2.Sum(x => x.Sum),
                                Sum_price = group2.Sum(x => x.Sum_price)
                            }).ToList()
                                               
                        }).ToList()

                }).ToList()
            }).ToList();

            return Json(list);
        }

        public static DateTime GetLastMonday(DateTime date)
        {
            int daysUntilMonday = (int)date.DayOfWeek;
            DateTime _date = date;

            if (daysUntilMonday == 0)
                return date;
            else
            {
                for (int i = 1; i < 7; i++)
                {
                    DateTime lastMonday = date.AddDays(-i); // Вычитаем дни до понедельника из указанной даты

                    if (lastMonday.DayOfWeek == DayOfWeek.Monday)
                        _date = lastMonday;
                }
                
            }

            return _date;
        }

    }
}
