namespace Ecommetriya.API.Models
{
    /// <summary>
    /// Конкуренты
    /// </summary>
    public class Competitor
    {
        /// <summary>
        /// Индификатор
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Артикул товара
        /// </summary>
        public long nmId { get; set; }

        /// <summary>
        /// Категория
        /// </summary>
        public string? Category { get; set; }

        /// <summary>
        /// Наименование
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Брэнд
        /// </summary>
        public string? Brand { get; set; }

        /// <summary>
        /// Коллекция фотографий
        /// </summary>
        public virtual ICollection<CompetitorPhoto>? Photos { get; set; }

        /// <summary>
        /// Статистика
        /// </summary>
        public virtual ICollection<CompetitorStatistic>? Statistics { get; set; }

        /// <summary>
        /// Магазин
        /// </summary>
        public Guid StoreId { get; set; }
    }
}
