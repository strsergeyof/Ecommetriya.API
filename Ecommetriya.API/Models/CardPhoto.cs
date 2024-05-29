namespace Ecommetriya.API.Models
{
    /// <summary>
    /// Ссылки на фотографии
    /// </summary>
    public class CardPhoto
    {
        /// <summary>
        /// Индификатор товара
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Большое изображение товара
        /// </summary>
        public string? Big { get; set; }

        /// <summary>
        /// Маленькое изображение товара
        /// </summary>
        public string? Tm { get; set; }
    }
}