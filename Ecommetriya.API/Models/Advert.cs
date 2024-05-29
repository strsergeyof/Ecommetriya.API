﻿namespace Ecommetriya.API.Models
{
    /// <summary>
    /// Рекламная кампания
    /// </summary>
    public class Advert
    {
        private DateTime? endTime;

        /// <summary>
        /// Индификатор
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Номер рекламной кампании
        /// </summary>
        public int AdvertId { get; set; }

        /// <summary>
        /// Наименование рекламной кампании
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Дата создания кампании
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// Дата завершения кампании
        /// </summary>
        public DateTime? EndTime
        {
            get => endTime;
            set
            {
                if (value.Value.ToString("yyyy-MM-dd") == "2100-01-01")
                    endTime = null;
                else
                    endTime = value;
            }
        }

        /// <summary>
        /// Статус рекламной кампании
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Строковый статус рекламной кампании
        /// </summary>
        public string StatusString => Status == 7 ? "Кампания завершена" :
            (Status == 9 ? "Идут показы" : "Кампания на паузе");

        /// <summary>
        /// Показы
        /// </summary>
        public int? Views => AdvertsStatistics.Sum(x => x.Views);

        /// <summary>
        /// Клики
        /// </summary>
        public int? Clicks => AdvertsStatistics.Sum(x => x.Clicks);

        /// <summary>
        /// Показатель кликабельности, отношение числа кликов к количеству показов, %
        /// </summary>
        public double? Ctr => AdvertsStatistics.Sum(x => x.Clicks).Value == 0 || AdvertsStatistics.Sum(x => x.Views).Value == 0 ? 0 : ((double)AdvertsStatistics.Sum(x => x.Clicks).Value / (double)AdvertsStatistics.Sum(x => x.Views).Value) * 100;

        /// <summary>
        /// Средняя стоимость клика, ₽
        /// </summary>
        public double? Cpc => AdvertsStatistics.Average(x => x.Cpc);

        /// <summary>
        /// Затраты, ₽
        /// </summary>
        public double? Sum => AdvertsStatistics.Sum(x => x.Sum);

        /// <summary>
        /// Количество добавлений товаров в корзину
        /// </summary>
        public int? Atbs => AdvertsStatistics.Sum(x => x.Atbs);

        /// <summary>
        /// Количество заказов
        /// </summary>
        public int? Orders => AdvertsStatistics.Sum(x => x.Orders);

        /// <summary>
        /// CR(conversion rate) — отношение количества заказов к общему количеству посещений кампании
        /// </summary>
        public double? Cr => AdvertsStatistics.Sum(x => x.Orders).Value == 0 || AdvertsStatistics.Sum(x => x.Clicks).Value == 0 ? 0 : ((double)AdvertsStatistics.Sum(x => x.Orders).Value / (double)AdvertsStatistics.Sum(x => x.Clicks).Value) * 100;

        /// <summary>
        /// Количество заказанных товаров, шт
        /// </summary>
        public int? Shks => AdvertsStatistics.Sum(x => x.Shks);

        /// <summary>
        /// Заказов на сумму, ₽
        /// </summary>
        public double? Sum_price => AdvertsStatistics.Sum(x => x.Sum_price);

        /// <summary>
        /// Статистика рекламной кампании по дням
        /// </summary>
        public virtual List<AdvertStatistic> AdvertsStatistics { get; set; }

        /// <summary>
        /// Магазин
        /// </summary>
        public Guid StoreId { get; set; }
    }
}
