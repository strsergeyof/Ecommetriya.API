using Ecommetriya.API.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Telegram.Bot;

namespace Ecommetriya.API.Services
{
    public class DataRepository(ApplicationDbContext context, ITelegramBotClient telegramBot) : IDataRepository
    {

        #region Поставки

        /// <summary>
        /// Загрузка поставок
        /// </summary>
        /// <returns></returns>
        public async Task LoadIncomes()
        {
            try
            {
                var stores = context.Stores.ToList();

                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                foreach (var store in stores)
                {
                    DateTime? lastOrder = context?.Incomes?.Where(x => x.StoreId == store.Id)?.OrderBy(x => x.Date)?.LastOrDefault()?.Date;
                    var incomes = await FetchIncomesFromApi(store, lastOrder);

                    if (incomes.Count > 0)
                    {
                        await context.Incomes.AddRangeAsync(incomes);
                        int rowsAffected = await context.SaveChangesAsync();
                    }
                }

                stopwatch.Stop();

                await telegramBot.SendTextMessageAsync("740755376", $"Поставки / Время загрузки данных: {stopwatch.Elapsed}!");
            }
            catch (Exception ex)
            {
                await telegramBot.SendTextMessageAsync("740755376", $"Поставки / Произошла ошибка: {ex.Message.ToString()}!");
            }

        }

        /// <summary>
        /// Список поставок
        /// </summary>
        /// <param name="store">Магазин</param>
        /// <param name="lastIncome">Последняя поставка</param>
        /// <returns></returns>
        public async Task<List<Income>> FetchIncomesFromApi(Store store, DateTime? lastIncome)
        {
            string dateFrom = lastIncome is null ? "?dateFrom=2023-01-29" : "?dateFrom=" + lastIncome.Value.ToString("yyyy-MM-ddTHH:mm:ss");
            List<Income> Incomes = new List<Income>();

            using (var httpClient = new HttpClient())
            {

                var apiUrl = $"https://statistics-api.wildberries.ru/api/v1/supplier/incomes{dateFrom}";

                try
                {
                    var requestMessage = new HttpRequestMessage(HttpMethod.Get, apiUrl);
                    requestMessage.Headers.Add("contentType", "application/json");
                    requestMessage.Headers.Add("Authorization", store.Token);

                    HttpResponseMessage response = await httpClient.SendAsync(requestMessage);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseContent = await response.Content.ReadAsStringAsync();

                        if (lastIncome is not null)
                            Incomes.AddRange(JsonConvert.DeserializeObject<List<Income>>(responseContent)?.Where(x => x.Date > lastIncome)?.ToList());
                        else
                            Incomes.AddRange(JsonConvert.DeserializeObject<List<Income>>(responseContent)?.ToList());

                        foreach (var income in Incomes)
                            income.StoreId = store.Id;

                        return Incomes;
                    }
                }
                catch (Exception ex)
                {
                    await telegramBot.SendTextMessageAsync("740755376", $"Поставки / Произошла ошибка: {ex.Message.ToString()}!");
                }
            }

            return Incomes;
        }
        #endregion

        #region Склад

        /// <summary>
        /// Загрузка склада
        /// </summary>
        /// <returns></returns>
        public async Task LoadStocks()
        {
            try
            {
                var stores = context.Stores.ToList();

                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                foreach (var store in stores)
                {
                    List<Stock> Stocks = context?.Stocks?.Where(x => x.StoreId == store.Id).ToList();
                    List<Stock> stocks = await FetchStocksFromApi(store);

                    if (stocks.Count > 0)
                    {
                        context?.Stocks.RemoveRange(Stocks);
                        await context.SaveChangesAsync();

                        await context.Stocks.AddRangeAsync(stocks);
                        await context.SaveChangesAsync();
                    }
                }

                stopwatch.Stop();
                await telegramBot.SendTextMessageAsync("740755376", $"Склад / Время загрузки данных: {stopwatch.Elapsed}!");

            }
            catch (Exception ex)
            {
                await telegramBot.SendTextMessageAsync("740755376", $"Склад / Произошла ошибка: {ex.Message.ToString()}!");
            }
        }

        /// <summary>
        ///  Остатки товаров на складах WB. Данные обновляются раз в 30 минут.
        /// Сервис статистики не хранит историю остатков товаров, поэтому получить данные о них можно только в режиме "на текущий момент".
        ///  Максимум 1 запрос в минуту 
        /// </summary>
        /// <param name="store">Магазин</param>
        /// <returns></returns>
        public async Task<List<Stock>> FetchStocksFromApi(Store store)
        {
            string dateFrom = "?dateFrom=2023-01-29";
            List<Stock> stocks = new List<Stock>();

            using (var httpClient = new HttpClient())
            {

                var apiUrl = $"https://statistics-api.wildberries.ru/api/v1/supplier/stocks{dateFrom}";

                try
                {
                    var requestMessage = new HttpRequestMessage(HttpMethod.Get, apiUrl);
                    requestMessage.Headers.Add("contentType", "application/json");
                    requestMessage.Headers.Add("Authorization", store.Token);

                    HttpResponseMessage response = await httpClient.SendAsync(requestMessage);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseContent = await response.Content.ReadAsStringAsync();

                        stocks.AddRange(JsonConvert.DeserializeObject<List<Stock>>(responseContent)?.ToList());

                        foreach (var stock in stocks)
                            stock.StoreId = store.Id;

                        return stocks;
                    }
                }
                catch (Exception ex)
                {
                    await telegramBot.SendTextMessageAsync("740755376", $"Склад / Произошла ошибка: {ex.Message.ToString()}!");
                }
            }

            return stocks;
        }
        #endregion

        #region Заказы

        /// <summary>
        /// Загрузка заказов
        /// </summary>
        /// <returns></returns>
        public async Task LoadOrders()
        {
            try
            {
                var stores = context.Stores
          .ToList();

                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                foreach (var store in stores)
                {
                    DateTime? lastOrder = context?.Orders?.Where(x => x.StoreId == store.Id)?
                        .OrderBy(x => x.Date)?
                        .LastOrDefault()
                        ?.Date;

                    var orders = await FetchOrdersFromApi(store, lastOrder);

                    if (orders.Count > 0)
                    {
                        await context.Orders.AddRangeAsync(orders);
                        await context.SaveChangesAsync();
                    }
                }

                stopwatch.Stop();
                await telegramBot.SendTextMessageAsync("740755376", $"Заказы / Время загрузки данных: {stopwatch.Elapsed}!");
            }
            catch (Exception ex)
            {
                await telegramBot.SendTextMessageAsync("740755376", $"Заказы / Произошла ошибка: {ex.Message.ToString()}!");
            }

        }

        /// <summary>
        /// Заказы
        /// Важно: гарантируется хранение данных по заказам не более 90 дней от даты заказа.
        /// Данные обновляются раз в 30 минут.
        /// Точное время обновления информации в сервисе можно увидеть в поле lastChangeDate.
        /// Для идентификации товаров из одного заказа, а также продаж по ним, следует использовать поле gNumber (строки с одинаковым значением этого поля относятся к одному заказу) и номер уникальной позиции в заказе odid (rid).
        /// </summary>
        /// <param name="store">Магазин</param>
        /// <param name="lastOrder">Последний заказ</param>
        /// <returns></returns>

        public async Task<List<Order>> FetchOrdersFromApi(Store store, DateTime? lastOrder)
        {
            string dateFrom = lastOrder is null ? "?dateFrom=2023-01-29" : "?dateFrom=" + lastOrder.Value.ToString("yyyy-MM-ddTHH:mm:ss");
            List<Order> orders = new List<Order>();

            using (var httpClient = new HttpClient())
            {

                var apiUrl = $"https://statistics-api.wildberries.ru/api/v1/supplier/orders{dateFrom}";

                try
                {
                    var requestMessage = new HttpRequestMessage(HttpMethod.Get, apiUrl);
                    requestMessage.Headers.Add("contentType", "application/json");
                    requestMessage.Headers.Add("Authorization", store.Token);

                    HttpResponseMessage response = await httpClient.SendAsync(requestMessage);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseContent = await response.Content.ReadAsStringAsync();

                        if (lastOrder is not null)
                            orders.AddRange(JsonConvert.DeserializeObject<List<Order>>(responseContent)?.Where(x => x.Date > lastOrder)?.ToList());
                        else
                            orders.AddRange(JsonConvert.DeserializeObject<List<Order>>(responseContent)?.ToList());

                        foreach (var order in orders)
                            order.StoreId = store.Id;

                        return orders;
                    }
                }
                catch (Exception ex)
                {
                    await telegramBot.SendTextMessageAsync("740755376", $"Заказы / Произошла ошибка: {ex.Message.ToString()}!");
                }
            }

            return orders;
        }
        #endregion

        #region Продажи

        /// <summary>
        /// Загрузка продаж
        /// </summary>
        /// <returns></returns>
        public async Task LoadSales()
        {
            try
            {
                var stores = context.Stores.ToList();

                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                foreach (var store in stores)
                {
                    DateTime? lastSale = context?.Sales?.Where(x => x.StoreId == store.Id)?.OrderBy(x => x.Date)?.LastOrDefault()?.Date;

                    var sales = await FetchSalesFromApi(store, lastSale);

                    if (sales.Count > 0)
                    {
                        await context.Sales.AddRangeAsync(sales);
                        await context.SaveChangesAsync();
                    }
                }

                stopwatch.Stop();
                await telegramBot.SendTextMessageAsync("740755376", $"Продажи / Время загрузки данных: {stopwatch.Elapsed}!");
            }
            catch (Exception ex)
            {
                await telegramBot.SendTextMessageAsync("740755376", $"Продажи / Произошла ошибка: {ex.Message.ToString()}!");
            }
        }

        /// <summary>
        ///  Продажи и возвраты.
        ///  Гарантируется хранение данных не более 90 дней от даты продажи.
        ///  Данные обновляются раз в 30 минут.
        ///  Для идентификации заказа следует использовать поле srid.
        ///  1 строка = 1 продажа/возврат = 1 единица товара.
        ///  Максимум 1 запрос в минуту
        /// </summary>
        /// <param name="store"></param>
        /// <param name="lastOrder"></param>
        /// <returns></returns>
        public async Task<List<Sale>> FetchSalesFromApi(Store store, DateTime? lastSale)
        {
            string dateFrom = lastSale is null ? "?dateFrom=2023-01-29" : "?dateFrom=" + lastSale.Value.ToString("yyyy-MM-ddTHH:mm:ss");
            List<Sale> sales = new List<Sale>();

            using (var httpClient = new HttpClient())
            {

                var apiUrl = $"https://statistics-api.wildberries.ru/api/v1/supplier/sales{dateFrom}";

                try
                {
                    var requestMessage = new HttpRequestMessage(HttpMethod.Get, apiUrl);
                    requestMessage.Headers.Add("contentType", "application/json");
                    requestMessage.Headers.Add("Authorization", store.Token);

                    HttpResponseMessage response = await httpClient.SendAsync(requestMessage);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseContent = await response.Content.ReadAsStringAsync();

                        if (lastSale is not null)
                            sales.AddRange(JsonConvert.DeserializeObject<List<Sale>>(responseContent)?.Where(x => x.Date > lastSale)?.ToList());
                        else
                            sales.AddRange(JsonConvert.DeserializeObject<List<Sale>>(responseContent)?.ToList());

                        foreach (var order in sales)
                            order.StoreId = store.Id;

                        return sales;
                    }
                }
                catch (Exception ex)
                {
                    await telegramBot.SendTextMessageAsync("740755376", $"Продажи / Произошла ошибка: {ex.Message.ToString()}!");
                }
            }

            return sales;
        }
        #endregion

        #region Отчет

        /// <summary>
        /// Получение подробного отчета
        /// </summary>
        /// <returns></returns>
        public async Task LoadReportDetails()
        {
            try
            {
                var stores = context.Stores.ToList();

                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                foreach (var store in stores)
                {
                    DateTime? lastDate = context?.ReportDetails?.Where(x => x.StoreId == store.Id)?.OrderBy(x => x.Date_to)?.LastOrDefault()?.Date_to;
                    var reportDetails = await FetchReportDetailsFromApi(store, lastDate);

                    if (reportDetails.Count > 0)
                    {
                        await context.ReportDetails.AddRangeAsync(reportDetails);
                        await context.SaveChangesAsync();
                    }
                }

                stopwatch.Stop();
                await telegramBot.SendTextMessageAsync("740755376", $"Отчет / Время загрузки данных: {stopwatch.Elapsed.Minutes}!");
            }
            catch (Exception ex)
            {
                await telegramBot.SendTextMessageAsync("740755376", $"Отчет / Произошла ошибка: {ex.Message.ToString()}!");
            }

        }

        /// <summary>
        /// Отчет о продажах по реализации.
        /// Отчет о продажах по реализации.
        /// В отчете доступны данные за последние 3 месяца.
        /// В случае отсутствия данных за указанный период метод вернет null.
        /// Технический перерыв в работе метода: каждый понедельник с 3:00 до 16:00.
        /// </summary>
        /// <param name="store">Магазин</param>
        /// <param name="lastDate">Последняя дата обновления</param>
        /// <returns></returns>
        private async Task<List<ReportDetail>> FetchReportDetailsFromApi(Store store, DateTime? lastDate)
        {
            string date = lastDate is null ? $"?dateFrom=2024-01-29&rrdid=0&dateTo={DateTime.Now.Date.ToString("yyyy-MM-dd")}" : $"?dateFrom={lastDate.Value.ToString("yyyy-MM-dd")}&rrdid=0&dateTo={DateTime.Now.Date.ToString("yyyy-MM-dd")}";
            List<ReportDetail> reportDetails = new List<ReportDetail>();

            try
            {
                var apiUrl = $"https://statistics-api.wildberries.ru/api/v5/supplier/reportDetailByPeriod{date}";

                using (var httpClient = new HttpClient())
                {
                    var requestMessage = new HttpRequestMessage(HttpMethod.Get, apiUrl);
                    requestMessage.Headers.Add("contentType", "application/json");
                    requestMessage.Headers.Add("Authorization", store.Token);

                    HttpResponseMessage response = await httpClient.SendAsync(requestMessage);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseContent = await response.Content.ReadAsStringAsync();

                        var list = JsonConvert.DeserializeObject<List<ReportDetail>>(responseContent);

                        if (list is not null)
                            reportDetails.AddRange(list);

                        foreach (var rd in reportDetails)
                            rd.StoreId = store.Id;
                    }
                }
            }
            catch (Exception ex)
            {
                await telegramBot.SendTextMessageAsync("740755376", $"Отчет / Произошла ошибка: {ex.Message.ToString()}!");
            }

            return reportDetails;
        }
        #endregion

        #region Конкуренты

        /// <summary>
        /// Загрузка данных по конкурентам
        /// </summary>
        /// <returns></returns>
        public async Task LoadCompetitors()
        {

            try
            {
                var stores = context?.Stores.ToList();

                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                foreach (var store in stores)
                {
                    var result = await GetCompetitorsFromApi(store);

                    if (result is not null)
                    {
                        await context?.CompetitorsStatistics.AddRangeAsync(result);
                        await context?.SaveChangesAsync();
                    }
                }

                stopwatch.Stop();
                await telegramBot.SendTextMessageAsync("740755376", $"Конкуренты / Время загрузки данных: {stopwatch.Elapsed}");
            }
            catch (Exception ex)
            {
                await telegramBot.SendTextMessageAsync("740755376", $"Конкуренты / Произошла ошибка: {ex.Message.ToString()}");
            }

        }

        /// <summary>
        /// Загрузка конкурентов по магазину
        /// </summary>
        /// <param name="store">Магазин</param>
        /// <returns></returns>
        public async Task<List<CompetitorStatistic>> GetCompetitorsFromApi(Store store)
        {
            List<Competitor> competitors = context.Competitors.Where(x => x.StoreId == store.Id)
                .Include(x => x.Statistics)
                .Where(c => !c.Statistics.Any(s => s.Date == DateTime.Today.Date))
                .ToList();

            List<CompetitorStatistic> CompetitorStatistics = new List<CompetitorStatistic>();

            using (var httpClient = new HttpClient())
            {

                var apiUrl = $"https://card.wb.ru/cards/detail?nm={string.Join(';', competitors.Select(x => x.nmId))}&dest=-1216601,-115136,-421732,123585595";

                try
                {
                    HttpResponseMessage response = await httpClient.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseContent = await response.Content.ReadAsStringAsync();
                        JsonDocument jsonDoc = JsonDocument.Parse(responseContent);
                        JsonElement root = jsonDoc.RootElement;
                        var products = root.GetProperty("data").GetProperty("products").ToString();
                        var jsonArray = JArray.Parse(products);

                        var firstObject = jsonArray.First;

                        if (firstObject is null) return CompetitorStatistics;

                        foreach (var row in jsonArray)
                        {
                            var sizesArray = firstObject["sizes"] as JArray;
                            var firstSize = sizesArray.First;
                            var stocksArray = firstSize["stocks"] as JArray;
                            var firstStockQty = (int)stocksArray.First["qty"];

                            CompetitorStatistics.Add(new CompetitorStatistic()
                            {
                                Date = DateTime.Now.Date,
                                SalePrice = (int)row["salePriceU"] / 100,
                                InStock = firstStockQty,
                                CompetitorId = competitors.FirstOrDefault(x => x.nmId == (int)row["id"])!.Id
                            });
                        }
                        return CompetitorStatistics;
                    }
                }
                catch (Exception ex)
                {
                    await telegramBot.SendTextMessageAsync("740755376", $"Конкуренты / Произошла ошибка: {ex.Message.ToString()}");
                }
            }

            return CompetitorStatistics;
        }

        /// <summary>
        /// Добавление новых конкурентов
        /// </summary>
        /// <param name="competitors"></param>
        /// <returns></returns>
        public static async Task<List<Competitor>> GetCompetitorsFromApi(List<Competitor> competitors)
        {
            List<Competitor> Competitors = new List<Competitor>();

            using (var httpClient = new HttpClient())
            {

                var apiUrl = $"https://card.wb.ru/cards/detail?nm={string.Join(';', competitors.Select(x => x.nmId))}&dest=-1216601,-115136,-421732,123585595";

                try
                {
                    HttpResponseMessage response = await httpClient.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseContent = await response.Content.ReadAsStringAsync();
                        JsonDocument jsonDoc = JsonDocument.Parse(responseContent);
                        JsonElement root = jsonDoc.RootElement;
                        var products = root.GetProperty("data").GetProperty("products").ToString();
                        var jsonArray = JArray.Parse(products);

                        var firstObject = jsonArray.First;

                        if (firstObject is null) return Competitors;

                        foreach (var row in jsonArray)
                        {
                            var sizesArray = firstObject["sizes"] as JArray;
                            var firstSize = sizesArray.First;
                            var stocksArray = firstSize["stocks"] as JArray;
                            var firstStockQty = (int)stocksArray.First["qty"];

                            Competitors.Add(new Competitor()
                            {

                                nmId = (int)row["id"],
                                Name = (string)row["name"],
                                Brand = (string)row["brand"],
                                Category = competitors.FirstOrDefault(y => y.nmId == (int)row["id"])!.Category,
                                StoreId = competitors.FirstOrDefault(y => y.nmId == (int)row["id"])!.StoreId,
                                Photos = new List<CompetitorPhoto>()
                                {
                                    new CompetitorPhoto()
                                    {
                                        Url = GetWbImageUrl((int)row["id"], 1)
                                    },
                                    new CompetitorPhoto()
                                    {
                                        Url = GetWbImageUrl((int)row["id"], 2)
                                    },
                                    new CompetitorPhoto()
                                    {
                                        Url = GetWbImageUrl((int)row["id"], 3)
                                    },
                                    new CompetitorPhoto()
                                    {
                                        Url = GetWbImageUrl((int)row["id"], 4)
                                    },
                                    new CompetitorPhoto()
                                    {
                                        Url = GetWbImageUrl((int)row["id"], 5)
                                    },
                                },
                                Statistics = new List<CompetitorStatistic>()
                                {
                                    new CompetitorStatistic()
                                    {
                                        Date = DateTime.Now.Date,
                                        SalePrice = (int)row["salePriceU"] / 100,
                                        InStock = firstStockQty
                                    }
                                }
                            });
                        }
                        return Competitors;
                    }
                    else
                    {
                    }
                }
                catch (HttpRequestException e)
                {
                    return Competitors;
                }
            }

            return Competitors;
        }
        #endregion

        #region Рекламные кампании

        /// <summary>
        /// Загрузка рекламных кампаний
        /// </summary>
        /// <returns></returns>
        public async Task LoadAdverts()
        {
            try
            {
                //Получаем список магазинов на сервере
                List<Store> stores = context.Stores.ToList();

                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                foreach (var store in stores)
                {
                    //Получение список рекламных кампаний по магазину У валбериса
                    var advertsList = await GetAdvertIdAsync(store);

                    //Полчение список рекламных кампаний в базе данных по магазину
                    List<Advert> adverts = context?.Adverts?.Where(x => x.StoreId == store.Id)?.ToList();

                    //Проверяем есть уникальные рекламные кампании загруженные
                    var uniqueAdverts = advertsList
                        .Where(a => !adverts.Any(x => x.AdvertId == a.AdvertId))
                        .Distinct()
                        .ToList();

                    //Если есть уникальные новые рекламные кампании, то добавляем их в базу данных
                    if (uniqueAdverts.Count > 0)
                    {
                        //Привязываем к какому магазину они относятся
                        foreach (var row in uniqueAdverts)
                            row.StoreId = store.Id;

                        //Добавляем новые рекламные кампании и сохраняем в базу данных
                        await context.Adverts.AddRangeAsync(uniqueAdverts);
                        await context.SaveChangesAsync();
                    }

                    //Загружаем обновленный список рекламных кампаний со статистикой
                    List<Advert> advertslist = context.Adverts
                        .Where(x => store.Id == x.StoreId)
                        .Include(z => z.AdvertsStatistics)
                        .ToList();

                    //Определяем минимальную дату создания рекламной кампнии
                    DateTime? MinDateOfAdvert = advertslist?.Min(x => x.CreateTime).Date;

                    //Есть ли данные статистики за вчерашний день
                    bool Yesterday = advertslist.Any(x => x.AdvertsStatistics.Any(s => s.Date.ToString("yyyy-MM-dd") == DateTime.Today.Date.AddDays(-1).ToString("yyyy-MM-dd")));

                    //Есть ли данные статистики вообще
                    bool YesData = advertslist.Any(x => x.AdvertsStatistics.Count > 0);

                    //Статистика по рекламным кампаниям
                    List<AdvertStatistic> advertStatistics = new List<AdvertStatistic>();

                    //Если уже были загружены данные, то пропускаем магазин
                    if (Yesterday) continue;

                    //Считаем сколько иттераций по запросу данных надо совершить
                    int count = (int)Math.Ceiling((double)advertslist.Count / 100);

                    for (int i = 0; i < count; i++)
                    {
                        //Загружаем список статистики
                        List<AdvertStatistic> _advertslist = await GetFullStatAsync(
                            //Передаем токен
                            store.Token,
                            //Пропускаем уже полученную статистику
                            advertslist.Skip(i * 100)
                            //Выбираем первые 100 по которым еще не получена статистика
                            .Take(100)
                            //Преобразовываем в лист
                            .ToList(),
                            //Определяем начальную дату
                            YesData ? DateTime.Now.Date.AddDays(-1) : MinDateOfAdvert.Value,
                            //Указываем конечную дату
                            DateTime.Now.Date.AddDays(-1));

                        //Если список статистики не равен нал или больше 0, то опредлеяем артикул поставщика
                        if (_advertslist is not null || _advertslist.Count > 0)
                        {
                            foreach (var row in _advertslist)
                                row.Sa_name = context?.Cards?.FirstOrDefault(y => y.NmID == row.nmId)?.VendorCode ??
                                    context?.ReportDetails?.FirstOrDefault(y => y.Nm_id == row.nmId)?.Sa_name ??
                                    context?.Stocks?.FirstOrDefault(y => y.NmId == row.nmId)?.SupplierArticle ??
                                    context?.Orders.FirstOrDefault(y => y.NmId == row.nmId)?.SupplierArticle;

                            //Добавляем в общий список статистики
                            advertStatistics.AddRange(_advertslist);

                            //Делаем паузу в 1 минуту, если не перебрали весь список рекламных кампаний
                            if (i != count - 1)
                                await Task.Delay(TimeSpan.FromMinutes(1));
                        }
                    }

                    //Подсчет статистики по рекламным кампаниям в общем
                    foreach (var advert in advertslist)
                        advert.AdvertsStatistics.AddRange(advertStatistics.Where(x => x.AdvertId == advert.Id).ToList());

                    if(advertslist.Count > 0)
                    //Обновляем статистику
                    context.Adverts.UpdateRange(advertslist);
                    await context?.SaveChangesAsync();
                }
                stopwatch.Stop();
                await telegramBot.SendTextMessageAsync("740755376", $"Рекламные кампании / Время загрузки данных: {stopwatch.Elapsed}!");
            }
            catch (Exception ex)
            {
                await telegramBot.SendTextMessageAsync("740755376", $"Рекламные кампании / Произошла ошибка: {ex.Message.ToString()}!");
            }
        }

        /// <summary>
        /// Получение статистики по рекламным кампаниям
        /// </summary>
        /// <param name="token">Токен</param>
        /// <param name="adverts">Список рекламных кампаний</param>
        /// <param name="dateFrom">Дата начала</param>
        /// <param name="dateTo">Дата окончания</param>
        /// <returns></returns>
        private async Task<List<AdvertStatistic>> GetFullStatAsync(string token, List<Advert> adverts, DateTime dateFrom, DateTime dateTo)
        {
            List<AdvertStatistic> advertStatistics = new List<AdvertStatistic>();
            var cards = context?.Cards.Where(x => x.StoreId == adverts!.FirstOrDefault().StoreId);

            try
            {
                string apiUrl = "https://advert-api.wb.ru/adv/v2/fullstats";

                var Array = adverts.Select(x =>
                new
                {
                    id = x.AdvertId,
                    interval = new
                    {
                        begin = DateTime.Today.ToString("yyyy-MM-dd"),
                        end = DateTime.Today.ToString("yyyy-MM-dd")
                    }
                }).ToArray();

                var js = System.Text.Json.JsonSerializer.Serialize(Array);

                var jsonContent = new StringContent(js, Encoding.UTF8, "application/json");

                using (HttpClient httpClient = new HttpClient())
                {
                    var hdr = new HttpRequestMessage(HttpMethod.Post, apiUrl);
                    hdr.Headers.Add("contentType", "application/json");
                    hdr.Content = jsonContent;
                    hdr.Headers.Add("Authorization", token);


                    HttpResponseMessage response = await httpClient.SendAsync(hdr);


                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var jsonArray = JArray.Parse(content);

                        foreach (var advert in jsonArray)
                        {
                            var days = advert["days"] as JArray;
                            var _advertId = (long)advert["advertId"];

                            foreach (var day in days)
                            {
                                var apps = day["apps"] as JArray;
                                var _day = (DateTime)day["date"];

                                foreach (var app in apps)
                                {
                                    var nms = app["nm"] as JArray;

                                    foreach (var nm in nms)
                                    {
                                        AdvertStatistic advertStatistic = new AdvertStatistic()
                                        {
                                            Date = _day,
                                            AdvertId = adverts.FirstOrDefault(x => x.AdvertId == _advertId).Id,
                                            nmId = (long)nm["nmId"],
                                            Name = (string)nm["name"],
                                            Views = (int)nm["views"],
                                            Clicks = (int)nm["clicks"],
                                            Ctr = (double)nm["ctr"],
                                            Atbs = (int)nm["atbs"],
                                            Cpc = (double)nm["cpc"],
                                            Cr = (double)nm["cr"],
                                            Orders = (int)nm["orders"],
                                            Shks = (int)nm["shks"],
                                            Sum = (double)nm["sum"],
                                            Sum_price = (double)nm["sum_price"],
                                            Sa_name = cards.FirstOrDefault(x => x.NmID == _advertId)?.VendorCode
                                        };

                                        advertStatistics.Add(advertStatistic);
                                    }
                                }
                            }

                        }

                    }

                    var uniqueElements = advertStatistics.GroupBy(x => new { x.Date, x.nmId, x.AdvertId })
                       .Select(group => new AdvertStatistic()
                       {
                           Date = group.Key.Date,
                           AdvertId = group.Key.AdvertId,
                           nmId = group.Key.nmId,
                           Name = group.First().Name,
                           Views = group.Sum(x => x.Views),
                           Clicks = group.Sum(x => x.Clicks),
                           Ctr = group.Sum(x => x.Clicks).Value == 0 || group.Sum(x => x.Views).Value == 0 ? 0 : ((double)group.Sum(x => x.Clicks).Value / (double)group.Sum(x => x.Views).Value) * 100,
                           Atbs = group.Sum(x => x.Atbs),
                           Cpc = group.Sum(x => x.Cpc),
                           Cr = group.Sum(x => x.Orders).Value == 0 || group.Sum(x => x.Clicks).Value == 0 ? 0 : ((double)group.Sum(x => x.Orders).Value / (double)group.Sum(x => x.Clicks).Value) * 100,
                           Orders = group.Sum(x => x.Orders),
                           Shks = group.Sum(x => x.Shks),
                           Sum = group.Sum(x => x.Sum),
                           Sum_price = group.Sum(x => x.Sum_price),
                           Sa_name = group.First().Sa_name
                       }).ToList();

                    return uniqueElements;
                }
            }
            catch (Exception ex)
            {
                return new List<AdvertStatistic>();
            }
        }

        public async Task<List<Advert>> GetAdvertIdAsync(Store store)
        {
            List<int> Adverts = new List<int>();
            List<Advert> AdvertsList = new List<Advert>();
            try
            {
                var servUrl = "https://advert-api.wb.ru/adv/v1/promotion/count";

                var request = new HttpRequestMessage(HttpMethod.Get, servUrl);
                request.Headers.Add("Authorization", store.Token);


                using (HttpClient httpClient = new HttpClient())
                {
                    var response = await httpClient.SendAsync(request);
                    response.EnsureSuccessStatusCode();

                    var responseString = await response.Content.ReadAsStringAsync();

                    var jsonObject = JObject.Parse(responseString);
                    var adverts = jsonObject["adverts"] as JArray;
                    var list = adverts.Where(x => (int)x["status"] == 7 || (int)x["status"] == 9 || (int)x["status"] == 11).ToList();

                    foreach (var ladv in list)
                    {
                        var advert_list = ladv["advert_list"] as JArray;

                        foreach (var adv in advert_list)
                            Adverts.Add((int)adv["advertId"]);
                    }

                    int count = (int)Math.Ceiling((double)Adverts.Count / 50);

                    for (int i = 0; i < count; i++)
                    {
                        var data = Adverts.Skip(i * 50).Take(50).ToList();
                        List<Advert> adverts1 = await GetRkName(store, data);

                        if (adverts1.Count() > 0)
                            AdvertsList.AddRange(adverts1);
                    }

                    return AdvertsList;
                }

            }
            catch (Exception ex)
            {
                return AdvertsList;
            }
        }

        public async Task<List<Advert>> GetRkName(Store store, List<int> adverts)
        {
            List<Advert> advertsList = new List<Advert>();

            var servUrl = "https://advert-api.wb.ru/adv/v1/promotion/adverts";

            var bodyzap = adverts.ToArray();

            var jsonContent = new StringContent(System.Text.Json.JsonSerializer.Serialize(bodyzap), Encoding.UTF8, "application/json");

            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    var hdr = new HttpRequestMessage(HttpMethod.Post, servUrl);
                    hdr.Headers.Add("contentType", "application/json");
                    hdr.Headers.Add("maxRedirects", "20");
                    hdr.Content = jsonContent;
                    hdr.Headers.Add("Authorization", store.Token);


                    HttpResponseMessage response = await httpClient.SendAsync(hdr);


                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var jsonArray = JArray.Parse(content);
                        List<Advert> _advertsList = jsonArray.Select(x => new Advert()
                        {
                            AdvertId = (int)x["advertId"],
                            Status = (int)x["status"],
                            Name = (string)x["name"],
                            CreateTime = (DateTime)x["createTime"],
                            EndTime = (DateTime)x["endTime"]
                        }).ToList();

                        if (_advertsList.Count() > 0)
                            advertsList.AddRange(_advertsList);

                        await Task.Delay(TimeSpan.FromSeconds(1));

                        return advertsList;

                    }
                    return advertsList;
                }

            }
            catch (Exception)
            {
                return advertsList;
            }
        }
        #endregion

        #region Лента товаров

        /// <summary>
        /// Лента товаров 
        /// </summary>
        /// <returns></returns>
        public async Task ExecuteCardsFeeds()
        {
            try
            {
                var stores = context.Stores.ToList();

                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                await telegramBot.SendTextMessageAsync("740755376", $"Лента товаров / Загрузка данных!");
                foreach (var store in stores)
                {

                    var cardsFeeds = await DataAnalysisForCardsFeedsAsync(store);

                    var _cardFeeds = context?.Feeds?.Where(x => x.StoreId == store.Id).ToList();

                    if (cardsFeeds?.Count > 0)
                    {
                        context?.Feeds?.RemoveRange(_cardFeeds);
                        await context.SaveChangesAsync();

                        await context.Feeds.AddRangeAsync(cardsFeeds);
                        await context.SaveChangesAsync();
                    }
                }

                stopwatch.Stop();
                await telegramBot.SendTextMessageAsync("740755376", $"Лента товаров / Время загрузки данных: {stopwatch.Elapsed}!");
            }
            catch (Exception ex)
            {
                await telegramBot.SendTextMessageAsync("740755376", $"Лента товаров / Произошла ошибка: {ex.Message.ToString()}!");
            }

        }

        /// <summary>
        /// Анализ данных для ленты новостей
        /// </summary>
        /// <param name="store">Магазин</param>
        /// <returns></returns>
        private async Task<List<Feed>> DataAnalysisForCardsFeedsAsync(Store store)
        {

            try
            {
                var reportDetails = context?.ReportDetails?.Where(x => x.StoreId == store.Id).ToList();

                var cards = context?.Cards
                    .Include(x => x.Photos)?
                    .Include(X => X.Sizes)?
                    .Include(X => X.Tags)?
                    .Where(x => x.StoreId == store.Id)
                    .ToList();

                var orders = context?.Orders?.Where(x => x.StoreId == store.Id).ToList();
                var stocks = context?.Stocks?.Where(x => x.StoreId == store.Id).ToList();
                var incomes = context?.Incomes?.Where(x => x.StoreId == store.Id).ToList();

                List<Feed> cardFeeds = new List<Feed>();

                //Список уникальных баркодов из отчета
                List<Feed>? barcodes = reportDetails?.Where(x => !string.IsNullOrEmpty(x.Barcode))
                    .GroupBy(x => x.Barcode)
                    .Select(group => new Feed
                    {
                        Barcode = group.FirstOrDefault().Barcode,
                        Url = $"https://wb.ru/catalog/{group.FirstOrDefault().Nm_id}/detail.aspx",
                        NmId = group.First().Nm_id,
                    })
                    .ToList();

                //Список уникальных баркодов из склада
                List<Feed>? barcodesStoks = stocks?.Where(x => !string.IsNullOrEmpty(x.Barcode))
                    .GroupBy(x => x.Barcode)
                    .Select(group => new Feed
                    {
                        Barcode = group.FirstOrDefault().Barcode,
                        Url = $"https://wb.ru/catalog/{group.FirstOrDefault().NmId}/detail.aspx",
                        NmId = group.First().NmId,
                    })
                    .ToList();

                //Список уникальных баркодов в заказах
                List<Feed>? barcodesOrders = orders?.Where(x => !string.IsNullOrEmpty(x.Barcode))
                    .GroupBy(x => x.Barcode)
                    .Select(group => new Feed
                    {
                        Barcode = group.FirstOrDefault().Barcode,
                        Url = $"https://wb.ru/catalog/{group.FirstOrDefault().NmId}/detail.aspx",
                        NmId = group.First().NmId,
                    })
                    .ToList();

                //Объединение 3 коллекций с уникальными баркодами
                barcodes.AddRange(barcodesStoks);
                barcodes.AddRange(barcodesOrders);

                //Отправленные товары
                var cardsDispatched = await FetchCardDispatchedAsync(store);

                //Купленные товары
                var cardsPurchased = await FetchCardsPurchasedAsync(store);

                //Возвращенные товары
                var cardsReturn = await FetchCardsReturnsAsync(store);

                //Без статуса товары
                var cardsNullStatus = await FilterCardsNullStatusAsync(cardsDispatched, store);


                //Получение уникальных баркодов товаров
                List<Feed> UniqProducts = barcodes.Where(x => !string.IsNullOrEmpty(x.Barcode))
                                           .GroupBy(x => x.Barcode)
                                           .Select(x => x.First())
                                           .Select(x => new Feed()
                                           {
                                               Barcode = x.Barcode,
                                               NmId = x.NmId,
                                               Url = x.Url,

                                               Sa_name = cards?.FirstOrDefault(y => y.NmID == x.NmId)?.VendorCode ??
                                                         reportDetails?.FirstOrDefault(y => y.Barcode == x.Barcode)?.Sa_name ??
                                                         stocks?.FirstOrDefault(y => y.NmId == x.NmId)?.SupplierArticle ??
                                                         orders.FirstOrDefault(y => y.NmId == x.NmId)?.SupplierArticle,

                                               Ts_name = reportDetails?.FirstOrDefault(y => y.Barcode == x.Barcode)?.Ts_name ??
                                                         orders?.FirstOrDefault(y => y.Barcode == x.Barcode)?.TechSize ??
                                                         stocks?.FirstOrDefault(y => y.NmId == x.NmId)?.TechSize,


                                               Brand_name = cards?.FirstOrDefault(y => y.NmID == x.NmId)?.Brand ??
                                                            reportDetails?.FirstOrDefault(y => y.Barcode == x.Barcode)?.Brand_name ??
                                                            stocks?.FirstOrDefault(y => y.NmId == x.NmId)?.Brand ??
                                                             orders.FirstOrDefault(y => y.NmId == x.NmId)?.Brand,

                                               Subject = cards?.FirstOrDefault(y => y.NmID == x.NmId)?.SubjectName ??
                                                        reportDetails?.FirstOrDefault(y => y.Barcode == x.Barcode)?.Subject_name ??
                                                        stocks?.FirstOrDefault(y => y.NmId == x.NmId)?.Subject ??
                                                        orders.FirstOrDefault(y => y.NmId == x.NmId)?.Subject,

                                               Category = orders?.FirstOrDefault(y => y.NmId == x.NmId)?.Category ??
                                                          stocks?.FirstOrDefault(y => y.NmId == x.NmId)?.Category,

                                               Image = cards?.FirstOrDefault(y => y.NmID == x.NmId)?.Photos?.FirstOrDefault()?.Tm ??
                                                       GetWbImageUrl(x.NmId),

                                               InStock = stocks?.FirstOrDefault(y => y.Barcode == x.Barcode)?.QuantityFull.Value ?? 0,

                                               Tags = cards?.FirstOrDefault(y => y.NmID == x.NmId)?.Tags.ToList().Count() > 0 ? string.Join(',', cards?.FirstOrDefault(y => y.NmID == x.NmId)?.Tags.Select(x => x.Name)) : string.Empty,

                                               Commision = reportDetails?.Where(y => y.Barcode == x.Barcode).Sum(x => x.Commission),
                                               Logistics = reportDetails.Where(y => y.Barcode == x.Barcode).Sum(x => x.Logistics),

                                               QuantityOfSupplies = incomes?.Where(y => y.Barcode == x.Barcode).Sum(x => x.Quantity),
                                               DateTimeQuantityOfSupplies = incomes?.Where(y => y.Barcode == x.Barcode).Max(x => x.Date),

                                               //Заказы
                                               OrderedCount = orders?.Where(y => y.Barcode == x.Barcode).Count(),
                                               OrderSummaPrice = orders?.Where(y => y.Barcode == x.Barcode).Sum(x => x.TotalPriceDiscount.Value),

                                               //Отмены
                                               CancelCount = orders?.Where(y => y.Barcode == x.Barcode && y.IsCancel).Count(),
                                               CancelSummaPrice = orders?.Where(y => y.Barcode == x.Barcode && y.IsCancel).Where(x => x.TotalPriceDiscount.HasValue).Sum(x => x.TotalPriceDiscount.Value),

                                               //Отправлены
                                               DispatchCount = cardsDispatched?.Where(y => y.Barcode == x.Barcode).Count(),
                                               DispatchSummaPrice = cardsDispatched?.Where(y => y.Barcode == x.Barcode).Where(x => x.TotalPriceDiscount.HasValue).Sum(x => x.TotalPriceDiscount.Value),

                                               //Выкуплены
                                               PurchasedCount = cardsPurchased?.Where(у => у.Barcode == x.Barcode)?.Count(),
                                               PurchasedSummaPrice = cardsPurchased?.Where(у => у.Barcode == x.Barcode)?.Where(x => x.TotalPriceDiscount.HasValue).Sum(x => x.TotalPriceDiscount.Value),

                                               //Возвращены
                                               ReturnCount = cardsReturn.Where(y => y.Barcode == x.Barcode).Count(),
                                               ReturnSummaPrice = cardsReturn.Where(y => y.Barcode == x.Barcode)?.Where(x => x.TotalPriceDiscount.HasValue).Sum(x => x.TotalPriceDiscount.Value),

                                               //Без статуса
                                               WitchStatusCount = cardsReturn.Where(y => y.Barcode == x.Barcode).Count(),

                                               StoreId = store.Id
                                           }).ToList();

                cardFeeds.AddRange(UniqProducts);

                return cardFeeds;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        /// <summary>
        /// Отправленные
        /// </summary>
        /// <returns></returns>
        private async Task<List<CardDispatched>> FetchCardDispatchedAsync(Store store)
        {
            var result = await Task.Run(async () =>
            {
                try
                {

                    var cards = context?.Cards
                           .Where(x => x.StoreId == store.Id)
                           .Include(x => x.Photos)?
                           .Include(X => X.Sizes)?
                           .Include(X => X.Tags)?.ToList();

                    var orders = context?.Orders
                        .Where(x => x.StoreId == store.Id && !string.IsNullOrEmpty(x.Srid) && !x.IsCancel).ToList();

                    var dispatchedOrders = orders
                    ?.Select(x => new CardDispatched
                    {
                        Srid = x.Srid,
                        Url = $"https://wb.ru/catalog/{x.NmId}/detail.aspx",
                        Image = cards?.FirstOrDefault(y => y.NmID == x.NmId)?.Photos?.FirstOrDefault()?.Tm ?? GetWbImageUrl(x.NmId),
                        Order_dt = x.Date,
                        Barcode = x.Barcode,
                        NmId = x.NmId,
                        Sa_name = x.SupplierArticle,
                        Ts_name = x.TechSize,
                        TotalPriceDiscount = x.TotalPriceDiscount,
                        StoreId = store.Id
                    });

                    return dispatchedOrders.ToList();
                }
                catch (Exception ex)
                {

                    throw;
                }

            });

            var cardsDispatched = context?.CardsDispatched?.Where(x => x.StoreId == store.Id).ToList();
            context?.CardsDispatched?.RemoveRange(cardsDispatched);
            await context?.SaveChangesAsync();

            await context?.CardsDispatched?.AddRangeAsync(result);
            await context?.SaveChangesAsync();

            return result;
        }

        /// <summary>
        /// Купленные товары
        /// </summary>
        /// <param name="store">Магазин</param>
        /// <returns></returns>
        private async Task<List<CardPurchased>> FetchCardsPurchasedAsync(Store store)
        {
            var result = await Task.Run(async () =>
            {
                try
                {

                    var list = context?.ReportDetails?.Where(x => x.Doc_type_name != null && x.Doc_type_name.ToLower() == "продажа" && x.StoreId == store.Id).ToList();

                    var cards = context?.Cards
                        .Include(x => x.Photos)?
                        .Include(X => X.Sizes)?
                        .Include(X => X.Tags)?
                        .Where(x => x.StoreId == store.Id).ToList();

                    var orders = context?.Orders
                        .Where(x => x.StoreId == store.Id).ToList();

                    var result = orders
                     .Where(x => x.IsOrdered && !x.IsCancel)
                     .Where(x => list.Any(y => y.Srid == x.Srid))
                     .Select(x => new CardPurchased
                     {

                         Url = $"https://wb.ru/catalog/{x.NmId}/detail.aspx",

                         Image = cards?.FirstOrDefault(y => y.NmID == x.NmId)?.Photos?.FirstOrDefault()?.Tm ?? GetWbImageUrl(x.NmId),

                         Sale_dt = list?.FirstOrDefault(y => y.Srid == x.Srid)?.Sale_dt,
                         Order_dt = x.Date,

                         Barcode = x.Barcode,
                         NmId = x.NmId,
                         Sa_name = x.SupplierArticle,

                         Ts_name = x.TechSize,
                         TotalPriceDiscount = x.TotalPriceDiscount,
                         StoreId = store.Id
                     });

                    return result.ToList();
                }
                catch (Exception ex)
                {

                    throw;
                }

            });

            var cardsPurchased = context?.CardsPurchaed?.Where(x => x.StoreId == store.Id).ToList();
            context?.CardsPurchaed?.RemoveRange(cardsPurchased);
            await context?.SaveChangesAsync();

            await context?.CardsPurchaed?.AddRangeAsync(result);
            await context?.SaveChangesAsync();

            return result;
        }

        /// <summary>
        /// Возвращенные товары
        /// </summary>
        /// <returns></returns>
        private async Task<List<CardReturn>> FetchCardsReturnsAsync(Store store)
        {
            var result = await Task.Run(async () =>
            {
                try
                {


                    var list = context?.ReportDetails?.Where(x => x.Doc_type_name != null && x.Doc_type_name.ToLower() == "возврат" && x.StoreId == store.Id).ToList();

                    var cards = context?.Cards
                           .Include(x => x.Photos)?
                           .Include(X => X.Sizes)?
                           .Include(X => X.Tags)?
                           .Where(x => x.StoreId == store.Id).ToList();

                    var orders = context?.Orders
                        .Where(x => x.StoreId == store.Id).ToList();

                    var _cardsReturns = orders
                    ?.Where(x => x.IsOrdered && !x.IsCancel)
                ?.Where(x => list.Any(y => y.Srid == x.Srid))
                    ?.Select(x => new CardReturn
                    {
                        Url = $"https://wb.ru/catalog/{x.NmId}/detail.aspx",

                        Image = cards?.FirstOrDefault(y => y.NmID == x.NmId)?.Photos?.FirstOrDefault()?.Tm ?? GetWbImageUrl(x.NmId),

                        Order_dt = x.Date,

                        Barcode = x.Barcode,
                        NmId = x.NmId,
                        Sa_name = x.SupplierArticle,

                        Ts_name = x.TechSize,
                        TotalPriceDiscount = x.TotalPriceDiscount,
                        StoreId = store.Id
                    });

                    return _cardsReturns.ToList();
                }
                catch (Exception ex)
                {

                    throw;
                }

            });

            var cardsReturns = context?.CardsReturns?.Where(x => x.StoreId == store.Id).ToList();
            context?.CardsReturns?.RemoveRange(cardsReturns);
            await context?.SaveChangesAsync();

            await context?.CardsReturns?.AddRangeAsync(result);
            await context?.SaveChangesAsync();

            return result;
        }

        /// <summary>
        /// Без статуса товары
        /// </summary>
        /// <returns></returns>
        private async Task<List<CardNullStatus>> FilterCardsNullStatusAsync(List<CardDispatched> cardsDispatched, Store store)
        {
            var result = await Task.Run(async () =>
            {
                try
                {


                    List<CardNullStatus> nullStatusOrders = new List<CardNullStatus>();

                    foreach (var cd in cardsDispatched)
                    {
                        var dateFrom = cd.Order_dt.Value;
                        var dateTo = cd.Order_dt.Value.AddMonths(1);

                        var card = context?.ReportDetails?.Where(x => x.Order_dt >= dateFrom && x.Order_dt <= dateTo && x.StoreId == store.Id);

                        if (card is null || card?.FirstOrDefault(x => x.Srid == cd.Srid) is null) continue;

                        nullStatusOrders.Add(new CardNullStatus()
                        {
                            Srid = cd.Srid,
                            Url = cd.Url,

                            Image = cd.Image,

                            Order_dt = cd.Order_dt,

                            Barcode = cd.Barcode,
                            NmId = cd.NmId,
                            Sa_name = cd.Sa_name,

                            Ts_name = cd.Ts_name,
                            TotalPriceDiscount = cd.TotalPriceDiscount,
                            StoreId = cd.StoreId

                        });
                    }

                    return nullStatusOrders;
                }
                catch (Exception ex)
                {

                    throw;
                }

            });

            var cardsNullStatus = context?.CardsNullStatus?.Where(x => x.StoreId == store.Id).ToList();
            context?.CardsNullStatus?.RemoveRange(cardsNullStatus);
            await context?.SaveChangesAsync();

            context?.CardsNullStatus?.AddRangeAsync(result);
            await context?.SaveChangesAsync();

            return result;
        }

        /// <summary>
        /// Получение URL изображения товара из поддомена ВБ по артикулу
        /// </summary>
        /// <param name="wbArticle">Артикул</param>
        /// <returns>URL</returns>
        public static string GetWbImageUrl(long? wbArticle, int number = 1)
        {

            int count4 = Convert.ToInt32(wbArticle.ToString().Substring(0, wbArticle.ToString().Length - 5));
            int count6 = Convert.ToInt32(wbArticle.ToString().Substring(0, wbArticle.ToString().Length - 3));

            string num = count4 switch
            {
                int n when n >= 0 && n <= 143 => "01",
                int n when n >= 144 && n <= 287 => "02",
                int n when n >= 288 && n <= 431 => "03",
                int n when n >= 432 && n <= 719 => "04",
                int n when n >= 720 && n <= 1007 => "05",
                int n when n >= 1008 && n <= 1061 => "06",
                int n when n >= 1062 && n <= 1115 => "07",
                int n when n >= 1116 && n <= 1169 => "08",
                int n when n >= 1170 && n <= 1313 => "09",
                int n when n >= 1314 && n <= 1601 => "10",
                int n when n >= 1602 && n <= 1655 => "11",
                int n when n >= 1656 && n <= 1918 => "12",
                int n when n >= 1919 && n <= 2045 => "13",
                int n when n >= 2045 && n <= 2200 => "14",
                _ => "15"
            };

            var url = "https://basket-#id#.wb.ru/vol#count4#/part#count6#/#article#/images/tm/#number#.jpg".Replace("#id#", num)
                .Replace("wb.ru", num == "15" ? "wbbasket.ru" : "wb.ru")
                                           .Replace("#count6#", count6.ToString())
                                           .Replace("#count4#", count4.ToString())
                                           .Replace("#article#", wbArticle.ToString())
                                           .Replace("#number#", "1");

            return url;
        }
        #endregion

        #region Карточки WB

        /// <summary>
        /// Загрузка карточек Wildberries
        /// </summary>
        /// <returns></returns>
        public async Task CardsWildberries()
        {
            try
            {
                Stopwatch stopwatch = new Stopwatch();
                var stores = context.Stores.ToList();

                List<Card> cardWildberriesOld = new List<Card>();
                List<Card> cardWildberriesNew = new List<Card>();

                stopwatch.Start();
                foreach (var store in stores)
                {
                    //Старые карточки
                    cardWildberriesOld.AddRange(context.Cards.Where(x => x.StoreId == store.Id)?.ToList());

                    //Новые карточки
                    cardWildberriesNew.AddRange(await FetchCardWildberriesFromApi(store));
                }

                context.Cards.RemoveRange(cardWildberriesOld);
                await context.Cards.AddRangeAsync(cardWildberriesNew);
                await context.SaveChangesAsync();
                stopwatch.Stop();
                await telegramBot.SendTextMessageAsync("740755376", $"Карточки Wildberries / Время загрузки данных: {stopwatch.Elapsed}!");
            }
            catch (Exception ex)
            {
                await telegramBot.SendTextMessageAsync("740755376", $"Карточки Wildberries / Произошла ошибка: {ex.Message.ToString()}!");
            }

        }



        /// <summary>
        /// Получение товаров
        /// </summary>
        /// <param name="store">Магазин</param>
        /// <returns>Список товаров</returns>
        public async Task<List<Card>> FetchCardWildberriesFromApi(Store store)
        {
            string updatedAt = string.Empty;
            int? nmID = null;

            var bodyzap = new
            {
                settings = new
                {
                    cursor = new { limit = 100 },
                    filter = new { withPhoto = -1 }
                }
            };

            var bodyzap2 = new
            {
                settings = new
                {
                    cursor = new
                    {
                        limit = 100,
                        updatedAt = updatedAt,
                        nmID = nmID
                    },

                    filter = new
                    {
                        withPhoto = -1
                    }
                }
            };

            List<Card> productWbs = new List<Card>();

            bool IsNextPage = false;

            do
            {
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        var apiUrl = "https://suppliers-api.wildberries.ru/content/v2/get/cards/list";

                        object _content = !IsNextPage ? bodyzap : bodyzap2;

                        var jsonContent = new StringContent(System.Text.Json.JsonSerializer.Serialize(_content), Encoding.UTF8, "application/json");
                        var hdr = new HttpRequestMessage(HttpMethod.Post, apiUrl);
                        hdr.Headers.Add("contentType", "application/json");
                        hdr.Headers.Add("maxRedirects", "20");
                        hdr.Content = jsonContent;
                        hdr.Headers.Add("Authorization", store.Token);


                        HttpResponseMessage response = await client.SendAsync(hdr);

                        if (response.IsSuccessStatusCode)
                        {
                            var content = await response.Content.ReadAsStringAsync();

                            JsonDocument jsonDoc = JsonDocument.Parse(content);
                            JsonElement root = jsonDoc.RootElement;
                            var cards = root.GetProperty("cards");

                            var jsonObject = JObject.Parse(content);
                            var cursor = jsonObject["cursor"] as JObject;
                            updatedAt = (string)cursor["updatedAt"];
                            nmID = (int)cursor["nmID"];
                            int total = (int)cursor["total"];

                            List<Card> _cardWildberries = JsonConvert.DeserializeObject<List<Card>>(cards.ToString());
                            productWbs.AddRange(_cardWildberries);

                            foreach (var cw in productWbs)
                                cw.StoreId = store.Id;

                            if (total < 100)
                                IsNextPage = false;
                            else
                                IsNextPage = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    await telegramBot.SendTextMessageAsync("740755376", $"Карточки Wildberries / Произошла ошибка: {ex.Message.ToString()}!");
                }
            }

            while (IsNextPage);

            return productWbs;
        }
        #endregion

        /// <summary>
        /// Добавление новых магазинов
        /// </summary>
        /// <param name="stores"></param>
        /// <returns></returns>
        public async Task<bool> AddStores(List<Store> stores)
        {
            try
            {
                foreach (var _store in stores)
                {
                    var result = await FetchCardWildberriesFromApi(_store);

                    if (result.Count > 0)
                        //Новые карточки
                        _store.CardWildberries.AddRange(result);

                    await context.Stores.AddAsync(_store);
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {

                throw;
            }

            return true;

        }
    }
}
