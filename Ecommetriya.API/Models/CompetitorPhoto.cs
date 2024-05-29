namespace Ecommetriya.API.Models
{
    /// <summary>
    /// Фотографии конкурента
    /// </summary>
    public class CompetitorPhoto
    {
        /// <summary>
        /// Индификатор
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Изображение товара
        /// </summary>
        public string? Url { get; set; }
    }
}