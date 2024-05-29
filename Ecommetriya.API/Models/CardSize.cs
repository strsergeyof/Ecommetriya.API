namespace Ecommetriya.API.Models
{
    /// <summary>
    /// Размеры товара
    /// </summary>
    public class CardSize
    {
        /// <summary>
        /// Индификатор размера
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Размер товара
        /// </summary>
        public string? TechSize { get; set; }
    }
}