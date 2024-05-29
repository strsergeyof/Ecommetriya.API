﻿namespace Ecommetriya.API.Models
{
    public class Income
    {
        private string? number1;
        private string? warehouseName1;

        /// <summary>
        /// Индификатор поставки
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Номер поставки
        /// </summary>
        public int? IncomeId { get; set; }

        /// <summary>
        /// Номер УПД
        /// </summary>
        public string? Number
        {
            get => number1; set
            {
                number1 = string.IsNullOrWhiteSpace(value) ? null : value;
            }
        }

        /// <summary>
        /// Дата поступления.
        /// Если часовой пояс не указан, то берется Московское время UTC+3.
        /// </summary>
        public DateTime? Date { get; set; }

        /// <summary>
        /// Дата и время обновления информации в сервисе.
        /// Это поле соответствует параметру dateFrom в запросе.
        /// Если часовой пояс не указан, то берется Московское время UTC+3.
        /// </summary>
        public DateTime? LastChangeDate { get; set; }

        /// <summary>
        /// Артикул продавца
        /// </summary>
        public string? SupplierArticle { get; set; }

        /// <summary>
        /// Размер товара (пример S, M, L, XL, 42, 42-43)
        /// </summary>
        public string? TechSize { get; set; }

        /// <summary>
        /// Бар-код
        /// </summary>
        public string? Barcode { get; set; }

        /// <summary>
        /// Количество
        /// </summary>
        public int? Quantity { get; set; }

        /// <summary>
        /// Цена из УПД
        /// </summary>
        public decimal? TotalPrice { get; set; }

        /// <summary>
        /// Дата принятия (закрытия) в WB. Если часовой пояс не указан, то берется Московское время UTC+3
        /// </summary>
        public DateTime? DateClose { get; set; }

        /// <summary>
        /// Название склада
        /// </summary>
        public string? WarehouseName
        {
            get => warehouseName1; set
            {
                warehouseName1 = string.IsNullOrWhiteSpace(value) ? null : value;
            }
        }

        /// <summary>
        /// Артикул WB
        /// </summary>
        public int? NmId { get; set; }

        /// <summary>
        /// Value: "Принято"
        /// Текущий статус поставки
        /// </summary>
        public string? Status { get; set; }

        /// <summary>
        /// Индификатор магазина
        /// </summary>
        public Guid StoreId { get; set; }
    }
}
