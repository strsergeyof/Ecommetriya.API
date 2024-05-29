using Ecommetriya.API.Models;
using Ecommetriya.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Data;

namespace Ecommetriya.API.Controllers
{
    public class AccountController(IJwtProvider jwtProvider, ApplicationDbContext context, IDataRepository dataRepository) : Controller
    {
        /// <summary>
        /// Получение токена для работы с API
        /// </summary>
        /// <param name="username">Логин</param>
        /// <param name="password">Пароль</param>
        /// <returns>Токен</returns>
        [HttpGet("/account/authentication")]
        public IActionResult Token(string username, string password)
        {
            var user = context.Users.FirstOrDefault(x => x.Login == username && x.Password == password);

            if (user is null)
                return StatusCode(401, new { errorText = "Authorization error." });

            var Token = jwtProvider.Generate(user.Login, user.Id.ToString());
            return Ok(Token);
        }

        /// <summary>
        /// Личный кабинет
        /// </summary>
        /// <returns>Информация по личному кабинету</returns>
        [Authorize]
        [HttpGet("/stores")]
        public IActionResult Get()
        {
            var _user = HttpContext.User;
            var id = _user.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
            var user = context.Users
                .Include(u => u.Stores)
                .FirstOrDefault(x => x.Id.ToString() == id);

            var result = user.Stores.Select(x => new
            {
                Id = x.Id,
                StoreName = x.StoreName,
                Tax = x.Tax,
                Token = x.Token
            });

            return Json(result);
        }

        /// <summary>
        /// Добавление нового магазина
        /// </summary>
        /// <param name="username">Логин</param>
        /// <param name="password">Пароль</param>
        /// <param name="role">Роль</param>
        /// <returns>Статус выполнения запроса</returns>
        [Authorize]
        [HttpPost("/account/stores")]
        public async Task<IActionResult> AddStoreAsync(string stores)
        {
            var user = HttpContext.User;
            var login = user.Claims.FirstOrDefault(c => c.Type == "Login")?.Value;
            var id = user.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
            var userId = context.Users.FirstOrDefault(x => x.Id.ToString() == id && x.Login == login)?.Id;

            if (user is null)
                return StatusCode(401, new { errorText = "Authorization error." });

            try
            {
                var jsonArray = JArray.Parse(stores);

                List<Store> Stores = jsonArray.Select(x => new Store()
                {
                    StoreName = (string)x["StoreName"],
                    Token = (string)x["Token"],
                    Tax = (double)x["Tax"],
                    UserId = userId
                }).ToList();
                
             var result = await dataRepository.AddStores(Stores);

                if (result)
                    return Ok();
                else
                    return BadRequest();
            }
            catch
            {
                return BadRequest();
            }
            
        }

        /// <summary>
        /// Удаление магазина
        /// </summary>
        /// <returns>Статус выполнения запроса</returns>
        [HttpDelete("/account/stores")]
        public async Task<IActionResult> RemoveStoreAsync(Guid guid)
        {
            var user = HttpContext.User;
            var login = user.Claims.FirstOrDefault(c => c.Type == "Login")?.Value;
            var id = user.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
            var userId = context.Users.FirstOrDefault(x => x.Id.ToString() == id && x.Login == login)?.Id;
            Store store = context.Stores.FirstOrDefault(x => x.Id == guid);

            if (user is null)
                return StatusCode(401, new { errorText = "Authorization error." });

            if (store is null) return BadRequest();

            context.Stores.Remove(store);
            await context?.SaveChangesAsync();
            int rowsAffected = await context.SaveChangesAsync();

            if (rowsAffected > 0)
                return Ok("Store remove successfully!");
            else
                return BadRequest();
        }

        /// <summary>
        /// Добавление нового клиента
        /// </summary>
        /// <param name="username">Логин</param>
        /// <param name="password">Пароль</param>
        /// <param name="role">Роль</param>
        /// <returns>Статус выполнения запроса</returns>
        [HttpPost("/account")]
        public async Task<IActionResult> AddUserAsync(string username, string password, Role role)
        {
            context.Users.Add(new User()
            {
                Login = username,
                Password = password,
                Role = role
            });

            int rowsAffected = await context.SaveChangesAsync();

            if (rowsAffected > 0)
                return Ok("User added successfully!");
            else
                return BadRequest("Failed to add user.");
        }

    }
}