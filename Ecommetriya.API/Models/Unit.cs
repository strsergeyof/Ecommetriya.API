
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Ecommetriya.API.Models
{
    //unit
    public class Unit : INotifyPropertyChanged
    {
        private byte[] image;
        private string? tags;
        public long? nmID1;
        private string? promotionName;
        private string? sa_name;
        private double? costPrice;
        private int orderedToday;
        private double? avgCommissionPercent;
        private double? avgDeliveryRub;
        private double? tax;
        private double? priceBeforeDiscount;
        private double? discount;
        private double? priceAfterDiscount;
        private double? minPriceAfterSPP;
        private double? priceWithSPP;
        private double? rOS;
        private double? comingToPC;
        private double? netProfit;
        private double? planPriceBeforeDiscount;
        private double? planDiscount;
        private double? calcAvgCommissionPercent;
        private double? calcAvgDeliveryRub;
        private double? calcTax;
        private double? calcPriceBeforeDiscount;
        private double calcDiscount;
        private double? calcNetProfit;
        private bool sEO = false;
        private int? ostFBO;

        /// <summary>
        /// Порядковый номер
        /// </summary>
        public int Number { get; set; }

        /// <summary>
        /// Изображение
        /// </summary>
        public byte[] Image
        {
            get => image; set
            {
                image = value;
                OnPropertyChanged(nameof(Image));
            }
        }

        /// <summary>
        /// Ссылка на товар
        /// </summary>
        public string? Url => $"https://wb.ru/catalog/{nmID}/detail.aspx";

        public string? Tags
        {
            get => tags; set
            {
                tags = value;
                OnPropertyChanged(nameof(Tags));
            }
        }

        /// <summary>
        /// Артикул продавца
        /// </summary>
        public string? Sa_name
        {
            get => sa_name; set
            {
                sa_name = value;
                OnPropertyChanged(nameof(Sa_name));
            }
        }

        /// <summary>
        /// Артикул товара
        /// </summary>
        public long? nmID
        {
            get => nmID1; set
            {
                nmID1 = value;
                OnPropertyChanged(nameof(nmID));
                OnPropertyChanged(nameof(Url));
            }
        }

        /// <summary>
        /// Себестоимость
        /// </summary>
        public double? CostPrice
        {
            get => costPrice; set
            {
                costPrice = value;
                OnPropertyChanged(nameof(CostPrice));
            }
        }

        /// <summary>
        /// Название акции
        /// </summary>
        public string? PromotionName
        {
            get => promotionName; set
            {
                promotionName = value;
                OnPropertyChanged(nameof(PromotionName));
            }
        }

        /// <summary>
        /// Количество к поставке
        /// </summary>
        public int QuantityToBeDelivered { get; set; }

        /// <summary>
        /// Заказано сегодня
        /// </summary>
        public int OrderedToday
        {
            get => orderedToday; set
            {
                orderedToday = value;
                OnPropertyChanged(nameof(OrderedToday));
            }
        }

        public int? OstFBO
        {
            get => ostFBO; set
            {
                ostFBO = value;
                OnPropertyChanged(nameof(OstFBO));
            }
        }
        public bool SEO
        {
            get => sEO; set
            {
                sEO = value;
                OnPropertyChanged(nameof(SEO));
            }
        }
        public bool Income { get; set; } = false;
        public bool InRK { get; set; } = false;
        public bool OutRK { get; set; } = false;
        public bool Test { get; set; } = false;

        /// <summary>
        /// Средняя комиссия в процентах
        /// </summary>
        public double? AvgCommissionPercent
        {
            get => avgCommissionPercent; set
            {
                avgCommissionPercent = value;
                OnPropertyChanged(nameof(AvgCommissionPercent));

                OnPropertyChanged(nameof(ROS));
                OnPropertyChanged(nameof(NetProfit));
                OnPropertyChanged(nameof(ComingToPC));
            }
        }

        /// <summary>
        /// Средняя комиссия в процентах в калькуляторе
        /// </summary>
        public double? CalcAvgCommissionPercent
        {
            get => calcAvgCommissionPercent; set
            {
                calcAvgCommissionPercent = value;

                if (IsAvgCommissionPercent == false) AvgCommissionPercent = value;

                OnPropertyChanged(nameof(CalcAvgCommissionPercent));
                OnPropertyChanged(nameof(CalcAvgDeliveryRub));
                OnPropertyChanged(nameof(CalcTax));

                OnPropertyChanged(nameof(CalcPriceBeforeDiscount));
                OnPropertyChanged(nameof(CalcDiscount));
                OnPropertyChanged(nameof(СalcPriceAfterDiscount));

                OnPropertyChanged(nameof(CalcROS));
                OnPropertyChanged(nameof(CalcNetProfit));
                OnPropertyChanged(nameof(CalcComingToPC));
            }
        }

        /// <summary>
        /// Средняя стоимость логистики
        /// </summary>
        public double? AvgDeliveryRub
        {
            get => avgDeliveryRub; set
            {
                avgDeliveryRub = value;
                OnPropertyChanged(nameof(AvgDeliveryRub));

                SetTax(6);
                OnPropertyChanged(nameof(ROS));
                OnPropertyChanged(nameof(NetProfit));
                OnPropertyChanged(nameof(ComingToPC));
            }
        }

        /// <summary>
        /// Средняя стомисоть логистики в калькуляторе
        /// </summary>
        public double? CalcAvgDeliveryRub
        {
            get => calcAvgDeliveryRub; set
            {
                calcAvgDeliveryRub = value;

                if (IsAvgDeliveryRubt == false) AvgDeliveryRub = value;

                OnPropertyChanged(nameof(CalcAvgCommissionPercent));

                OnPropertyChanged(nameof(CalcROS));
                OnPropertyChanged(nameof(CalcNetProfit));
                OnPropertyChanged(nameof(CalcComingToPC));

            }
        }

        /// <summary>
        /// Налог
        /// </summary>
        public double? Tax
        {
            get => tax; set
            {
                tax = value;
                OnPropertyChanged(nameof(Tax));
            }
        }

        /// <summary>
        /// Налог в калькуляторе
        /// </summary>
        public double? CalcTax
        {
            get => calcTax; set
            {
                calcTax = value;

                OnPropertyChanged(nameof(CalcTax));

                OnPropertyChanged(nameof(CalcROS));
                OnPropertyChanged(nameof(CalcNetProfit));
                OnPropertyChanged(nameof(CalcComingToPC));
            }
        }

        /// <summary>
        /// Фактическая цена до
        /// </summary>
        public double? PriceBeforeDiscount
        {
            get => priceBeforeDiscount; set
            {
                priceBeforeDiscount = value;
                OnPropertyChanged(nameof(PriceBeforeDiscount));
            }
        }

        /// <summary>
        /// Плановая цена до
        /// </summary>
        public double? PlanPriceBeforeDiscount
        {
            get => planPriceBeforeDiscount; set
            {
                planPriceBeforeDiscount = value;
                OnPropertyChanged(nameof(PlanPriceBeforeDiscount));
            }
        }

        /// <summary>
        /// Цена до скидки в калькуляторе
        /// </summary>
        public double? CalcPriceBeforeDiscount
        {
            get => calcPriceBeforeDiscount; set
            {
                calcPriceBeforeDiscount = value;

                OnPropertyChanged(nameof(CalcPriceBeforeDiscount));
                OnPropertyChanged(nameof(CalcDiscount));
                OnPropertyChanged(nameof(СalcPriceAfterDiscount));

                SetCalcTax(6);
            }
        }

        /// <summary>
        /// Фактическая скидка
        /// </summary>
        public double? Discount
        {
            get => discount; set
            {
                discount = value;
                OnPropertyChanged(nameof(Discount));
            }
        }

        /// <summary>
        /// Плановая скидка
        /// </summary>
        public double? PlanDiscount
        {
            get => planDiscount; set
            {
                planDiscount = value;
                OnPropertyChanged(nameof(PlanDiscount));
            }
        }

        /// <summary>
        /// Скидка в калькуляторе
        /// </summary>
        public double CalcDiscount
        {
            get => calcDiscount; set
            {
                calcDiscount = value;

                OnPropertyChanged(nameof(CalcDiscount));
                OnPropertyChanged(nameof(СalcPriceAfterDiscount));
                SetCalcTax(6);
                OnPropertyChanged(nameof(CalcTax));
            }
        }

        /// <summary>
        /// Фактическая цена после
        /// </summary>
        public double? PriceAfterDiscount
        {
            get => priceAfterDiscount; set
            {
                priceAfterDiscount = value;
                OnPropertyChanged(nameof(PriceAfterDiscount));
                OnPropertyChanged(nameof(ComingToPC));
            }
        }

        /// <summary>
        /// Плановая цена после скидки
        /// </summary>
        public double? PlanPriceAfterDiscount => PlanDiscount * PlanPriceBeforeDiscount / 100;

        /// <summary>
        /// Цена после скидки в калькуляторе
        /// </summary>
        public double? СalcPriceAfterDiscount => CalcPriceBeforeDiscount - (CalcDiscount * CalcPriceBeforeDiscount / 100);

        /// <summary>
        /// Минимальная цена после SPP
        /// </summary>
        public double? MinPriceAfterSPP
        {
            get => minPriceAfterSPP; set
            {
                minPriceAfterSPP = value;
                OnPropertyChanged(nameof(MinPriceAfterSPP));
            }
        }

        /// <summary>
        /// Цена с SPP
        /// </summary>
        public double? PriceWithSPP
        {
            get => priceWithSPP; set
            {
                priceWithSPP = value;
                OnPropertyChanged(nameof(PriceWithSPP));
            }
        }

        /// <summary>
        /// Плановая цена с SPP
        /// </summary>
        public double? PlanPriceWithSPP => PriceWithSPP;

        /// <summary>
        /// ROS
        /// </summary>
        public double? ROS => (NetProfit / PriceAfterDiscount) * 100;

        /// <summary>
        /// Плановый ROS
        /// </summary>
        public double? PlanROS => (PlanNetProfit / PlanPriceAfterDiscount) * 100;

        /// <summary>
        /// ROS в калькуляторе
        /// </summary>
        public double? CalcROS => (CalcNetProfit / СalcPriceAfterDiscount) * 100;

        /// <summary>
        /// Придет на РС
        /// </summary>
        public double? ComingToPC => PriceAfterDiscount - AvgDeliveryRub - ((AvgCommissionPercent * PriceAfterDiscount) / 100);

        /// <summary>
        /// Планово придет на РС
        /// </summary>
        public double? PlanComingToPC => PlanPriceAfterDiscount - AvgDeliveryRub - ((AvgCommissionPercent * PlanPriceAfterDiscount) / 100);

        /// <summary>
        /// придет на РС в калькуляторе
        /// </summary>
        public double? CalcComingToPC => СalcPriceAfterDiscount - CalcAvgDeliveryRub - ((CalcAvgCommissionPercent * СalcPriceAfterDiscount) / 100);

        /// <summary>
        /// Чистая прибыль
        /// </summary>
        public double? NetProfit
        {
            get => netProfit; set
            {
                netProfit = value;
                OnPropertyChanged(nameof(NetProfit));
            }
        }

        /// <summary>
        /// Плановая чистая прибыль
        /// </summary>
        public double? PlanNetProfit { get; set; }

        /// <summary>
        /// Чистая прибыль в калькуляторе
        /// </summary>
        public double? CalcNetProfit
        {
            get => calcNetProfit; set
            {
                calcNetProfit = value;
                OnPropertyChanged(nameof(CalcNetProfit));
            }
        }

        public bool IsAvgCommissionPercent { get; set; }
        public bool IsAvgDeliveryRubt { get; set; }

        public void SetTax(double? tax)
        {
            Tax = (tax * PriceAfterDiscount) / 100;
            NetProfit = ComingToPC - CostPrice - ((tax * PriceAfterDiscount) / 100);

            OnPropertyChanged(nameof(Tax));
            OnPropertyChanged(nameof(NetProfit));
            OnPropertyChanged(nameof(ROS));
        }

        public void SetCalcTax(double? tax)
        {
            CalcTax = (tax * СalcPriceAfterDiscount) / 100;
            CalcNetProfit = CalcComingToPC - CostPrice - ((tax * СalcPriceAfterDiscount) / 100);

            OnPropertyChanged(nameof(CalcTax));
            OnPropertyChanged(nameof(CalcNetProfit));
            OnPropertyChanged(nameof(CalcROS));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
