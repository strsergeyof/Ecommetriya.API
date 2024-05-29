namespace Ecommetriya.API.Models
{
    /// <summary>
    /// Статистика конкурента
    /// </summary>
    public class CompetitorStatistic
    {

        /// <summary>
        /// Индификатор
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Дата
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Цена
        /// </summary>
        public int SalePrice { get; set; }

        /// <summary>
        /// Остатки
        /// </summary>
        public int InStock { get; set; }

        /// <summary>
        /// Конкурента индификатор
        /// </summary>
        public virtual Guid CompetitorId { get; set; }
    }
}
