namespace Ecommetriya.API.Models
{
    /// <summary>
    /// Магазин
    /// </summary>
    public class Store
    {
        /// <summary>
        /// Индификатор
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Название магазина
        /// </summary>
        public string StoreName { get; set; }

        /// <summary>
        /// Токен
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Налог
        /// </summary>
        public double Tax { get; set; }

        /// <summary>
        /// Владелец магазина
        /// </summary>
        public Guid? UserId { get; set; }

        /// <summary>
        /// Заказы
        /// </summary>
        public virtual ICollection<Order> Orders { get; set; }

        /// <summary>
        /// Поставки
        /// </summary>
        public virtual ICollection<Income> Incomes { get; set; }

        /// <summary>
        /// Продажи
        /// </summary>
        public virtual ICollection<Sale> Sales { get; set; }

        /// <summary>
        /// Склад
        /// </summary>
        public virtual ICollection<Stock> Stocks { get; set; }

        /// <summary>
        /// Подробный отчет
        /// </summary>
        public virtual ICollection<ReportDetail> ReportDetails { get; set; }

        /// <summary>
        /// Лента товаров
        /// </summary>
        public virtual ICollection<Feed> CardFeeds { get; set; }

        /// <summary>
        /// Отправленные товары
        /// </summary>
        public virtual ICollection<CardDispatched> CardsDispatched { get; set; }

        /// <summary>
        /// Купленные товары
        /// </summary>
        public virtual ICollection<CardPurchased> CardsPurchased { get; set; }

        /// <summary>
        /// Возвращенные товары
        /// </summary>
        public virtual ICollection<CardReturn> CardsReturns { get; set; }

        /// <summary>
        /// Без статуса товары
        /// </summary>
        public virtual ICollection<CardNullStatus> CardNullStatuses { get; set; }

        /// <summary>
        /// Конкуренты
        /// </summary>
        public virtual ICollection<Competitor> Сompetitors { get; set; }

        /// <summary>
        /// Рекламная кампания
        /// </summary>
        public virtual ICollection<Advert> Adverts { get; set; }

        /// <summary>
        /// Товары
        /// </summary>
        public virtual List<Card> CardWildberries { get; set; } = new List<Card>();

    }
}