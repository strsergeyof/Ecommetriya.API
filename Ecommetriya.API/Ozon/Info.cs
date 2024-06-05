namespace Ecommetriya.Model
{
    public class StockInfo
    {
        public string OfferId { get; set; }
        public string Name { get; set; }
        public int? Fbo { get; set; }
        public int? Fbs { get; set; }
    }

    public class OzonStockResponse
    {
        public ResultData result { get; set; }
    }

    public class ResultData
    {
        public List<StockItem> items { get; set; }
    }

    public class StockItem
    {
        public string offer_id { get; set; }
        public string name { get; set; }
        public List<Stock> stocks { get; set; }
    }

    public class Stock
    {
        public string type { get; set; }
        public int present { get; set; }
        public int reserved { get; set; }
    }

    public class Warehouse //склады
    {
        public string id { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public string address { get; set; }
    }
    public class OzonWarehouseResponse
    {
        public List<Warehouse> result { get; set; }
    }

    class OzonResponse
    {
        public List<Order> result { get; set; }
    }

    class Order
    {
        public string order_id { get; set; }
        public string order_number { get; set; }
        public string posting_number { get; set; }
        public string status { get; set; }
        public string created_at { get; set; }
        public List<Product> products { get; set; }
    }

    class Product
    {
        public string sku { get; set; }
        public string name { get; set; }
        public string offer_id { get; set; }
        public int quantity { get; set; }
    }

    class OzonResponseFBS
    {
        public ResultFBS result { get; set; }
    }

    class ResultFBS
    {
        public List<OrderFBS> postings { get; set; }
    }

    class OrderFBS
    {
        public string order_id { get; set; }
        public string order_number { get; set; }
        public string posting_number { get; set; }
        public string status { get; set; }
        public string in_process_at { get; set; }
        public List<Product> products { get; set; }
    }
    public class ProductListResponse
    {
        public ProductListResult result { get; set; }
    }

    public class ProductListResult
    {
        public List<ProductInfo> items { get; set; }
    }

    public class ProductInfo
    {
        public string offer_id { get; set; }
        public string sku { get; set; }
        public string fbs_sku { get; set; }
        public string fbo_sku { get; set; }
        public string barcode { get; set; }
        public string primary_image { get; set; }
        public string name { get; set; }
        public string old_price { get; set; }
        public string price { get; set; }
        public string marketing_price { get; set; }
        public string recommended_price { get; set; }
        public string price_index { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public ProductStatus status { get; set; }
        public ProductStock stocks { get; set; }
    }
    public class ProductStatus
    {
        public string state_description { get; set; }
        public string state_name { get; set; }
    }

    public class ProductStock
    {
        public int coming { get; set; }
        public int present { get; set; }
        public int reserved { get; set; }
    }
    public class OzonTransactionResponse
    {
        public Result result { get; set; }
    }
    public class Result
    {
        public List<Operation> operations { get; set; }
        public int page_count { get; set; }
    }
    public class Operation
    {
        public DateTime operation_date { get; set; }
        public string operation_type_name { get; set; }
        public Posting posting { get; set; }
        public List<Service> services { get; set; }
        public List<Item> items { get; set; }
        public decimal accruals_for_sale { get; set; }
        public decimal sale_commission { get; set; }
        public decimal amount { get; set; }
    }
    public class Posting
    {
        public string delivery_schema { get; set; }
        public string posting_number { get; set; }
    }
    public class Service
    {
        public decimal price { get; set; }
    }
    public class Item
    {
        public string name { get; set; }
        public string sku { get; set; }
    }
    public class TransactionRequestBody
    {
        public FilterTransactionRequest filter { get; set; }
        public int page { get; set; }
        public int page_size { get; set; }
    }
    public class FilterTransactionRequest
    {
        public DateRange date { get; set; }
        public List<string> operation_type { get; set; }
        public string posting_number { get; set; }
        public string transaction_type { get; set; }
    }
    public class DateRange
    {
        public string from { get; set; }
        public string to { get; set; }
    }
}
