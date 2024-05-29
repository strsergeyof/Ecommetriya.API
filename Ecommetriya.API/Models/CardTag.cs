using Newtonsoft.Json;

namespace Ecommetriya.API.Models
{
    /// <summary>
    /// Теги товара
    /// </summary>
    public class CardTag
    {
        /// <summary>
        /// Индификатор тега
        /// </summary>
        [JsonIgnore]
        public Guid Id { get; set; }

        /// <summary>
        /// Наименования тега
        /// </summary>
        public string? Name { get; set; }
    }
}