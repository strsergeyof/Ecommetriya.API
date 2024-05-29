using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using OfficeOpenXml;
using System.Windows.Controls;
using System.Windows;
using System.Net.Http.Json;
using System.Windows.Input;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Collections.Concurrent;
namespace Ecommetriya.Model
{
    public class Methods
    {
        private static readonly HttpClient client = new HttpClient();
        private static readonly string ApiUrlTovars = "https://api-seller.ozon.ru/v2/product/list";
        private static readonly string ApiUrlOrdersFBS = "https://api-seller.ozon.ru/v2/posting/fbs/list";
        private static readonly string ApiUrlOrdersFBO = "https://api-seller.ozon.ru/v2/posting/fbo/list";
        private static readonly string ApiUrlWarehouse = "https://api-seller.ozon.ru/v2/posting/fbo/warehouse/list";
        private static readonly string ApiUrlProductList = "https://api-seller.ozon.ru/v2/product/info/list";
        private static readonly string ApiUrlTransaction = "https://api-seller.ozon.ru/v3/finance/transaction/list";

        //Остатки
        public static async Task<List<StockInfo>> GetOzonStockInfoAsync(string _ApiKey, string _ClientID)
        {
            try
            {
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("Client-Id", _ClientID);
                client.DefaultRequestHeaders.Add("Api-Key", _ApiKey);
                var body = new
                {
                    filter = new { visibility = "ALL" },
                    limit = 1000
                };
                var jsonBody = JsonConvert.SerializeObject(body);
                var content = new StringContent(jsonBody, System.Text.Encoding.UTF8, "application/json");
                var response = await client.PostAsync("https://api-seller.ozon.ru/v3/product/info/stocks", content);
                response.EnsureSuccessStatusCode();
                var responseContent = await response.Content.ReadAsStringAsync();
                var ozonStockResponse = JsonConvert.DeserializeObject<OzonStockResponse>(responseContent);

                if (ozonStockResponse?.result?.items != null)
                {
                    List<StockInfo> stockInfoList = new List<StockInfo>();
                    foreach (var item in ozonStockResponse.result.items)
                    {
                        StockInfo stockInfo = new StockInfo
                        {
                            OfferId = item.offer_id,
                            Name = item.name,
                            Fbo = item.stocks.FirstOrDefault(s => s.type == "fbo")?.present - item.stocks.FirstOrDefault(s => s.type == "fbo")?.reserved,
                            Fbs = item.stocks.FirstOrDefault(s => s.type == "fbs")?.present - item.stocks.FirstOrDefault(s => s.type == "fbs")?.reserved
                        };
                        stockInfoList.Add(stockInfo);
                    }
                    return stockInfoList;
                }
                else
                {
                    return new List<StockInfo>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении остатков: {ex.Message}");
                return null;
            }
        }
        //Склады
        public static async Task<List<Warehouse>> GetOzonWarehouseInfoAsync(string _ApiKey, string _ClientID)
        {
            try
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("Client-Id", _ClientID);
                client.DefaultRequestHeaders.Add("Api-Key", _ApiKey);
                var response = await client.GetAsync(ApiUrlWarehouse);
                response.EnsureSuccessStatusCode();
                var responseContent = await response.Content.ReadAsStringAsync();
                var ozonWarehouseResponse = JsonConvert.DeserializeObject<OzonWarehouseResponse>(responseContent);
                if (ozonWarehouseResponse?.result != null)
                {
                    return ozonWarehouseResponse.result;
                }
                else
                {
                    return new List<Warehouse>();
                }
            }
            catch (Exception ex)
            {
                return new List<Warehouse>();
            }
        }
        // Заказы FBO
        public static async Task<List<List<object>>> LoadOzonZakFBO(string _ApiKey, string _ClientID)
        {
            var dateStart = DateTime.Now.AddDays(-28);
            var dateEnd = DateTime.Now;
            var body = new
            {
                filter = new
                {
                    since = dateStart.ToString("yyyy-MM-ddTHH:mm:ss.000Z"),
                    to = dateEnd.ToString("yyyy-MM-ddTHH:mm:ss.000Z")
                },
                limit = 1000
            };
            var response = await SendRequest("https://api-seller.ozon.ru/v2/posting/fbo/list", body, _ApiKey, _ClientID);
            var data = JsonConvert.DeserializeObject<OzonResponse>(response);
            var orders = new List<List<object>>();
            foreach (var order in data.result)
            {
                foreach (var product in order.products)
                {
                    orders.Add(new List<object>
            {
                order.order_id,
                order.order_number,
                order.posting_number,
                order.status,
                order.created_at,
                product.sku,
                product.name,
                product.offer_id,
                product.quantity
            });
                }
            }
            return orders;
        }

        // Заказы FBS
        public static async Task<List<List<object>>> LoadOzonZakFBS(string _ApiKey, string _ClientID)
        {
            var dateStart = DateTime.Now.AddDays(-28);
            var dateEnd = DateTime.Now;
            var body = new
            {
                filter = new
                {
                    since = dateStart.ToString("yyyy-MM-ddTHH:mm:ss.000Z"),
                    to = dateEnd.ToString("yyyy-MM-ddTHH:mm:ss.000Z")
                },
                limit = 1000
            };
            var response = await SendRequest("https://api-seller.ozon.ru/v3/posting/fbs/list", body, _ApiKey, _ClientID);
            var data = JsonConvert.DeserializeObject<OzonResponseFBS>(response);
            var orders = new List<List<object>>();
            foreach (var order in data.result.postings)
            {
                foreach (var product in order.products)
                {
                    orders.Add(new List<object>
            {
                order.order_id,
                order.order_number,
                order.posting_number,
                order.status,
                order.in_process_at,
                product.sku,
                product.name,
                product.offer_id,
                product.quantity
            });
                }
            }
            return orders;
        }
        static async Task<string> SendRequest(string url, object body, string _ApiKey, string _ClientID)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Client-Id", _ClientID);
                client.DefaultRequestHeaders.Add("Api-Key", _ApiKey);

                var content = new StringContent(JsonConvert.SerializeObject(body), System.Text.Encoding.UTF8, "application/json");
                var response = await client.PostAsync(url, content);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
        }
        //Товары
        public static async Task<List<ProductInfo>> GetProductInfoAsync(List<string> skuList, string _ApiKey, string _ClientID)
        {
            try
            {
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("Client-Id", _ClientID);
                client.DefaultRequestHeaders.Add("Api-Key", _ApiKey);
                var body = new { offer_id = skuList };
                var jsonBody = JsonConvert.SerializeObject(body);
                var content = new StringContent(jsonBody, System.Text.Encoding.UTF8, "application/json");
                var response = await client.PostAsync(ApiUrlProductList, content);
                response.EnsureSuccessStatusCode();
                var responseContent = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<ProductListResponse>(responseContent);

                if (data?.result?.items != null)
                {
                    return data.result.items;
                }
                else
                {
                    return new List<ProductInfo>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении информации о товарах: {ex.Message}");
                return null;
            }
        }
        // Отчёт
        public static async Task<List<List<object>>> LoadOzonTransaction(string _ApiKey, string _ClientID)
        {
            var dateStart = DateTime.Now.AddDays(-28);
            var dateEnd = DateTime.Now;

            var filterRequest = new FilterTransactionRequest
            {
                date = new DateRange
                {
                    from = dateStart.ToString("yyyy-MM-ddTHH:mm:ss.000Z"),
                    to = dateEnd.ToString("yyyy-MM-ddTHH:mm:ss.000Z")
                },
                operation_type = new List<string>(),
                posting_number = "",
                transaction_type = "all"
            };

            var body = new TransactionRequestBody
            {
                filter = filterRequest,
                page = 1,
                page_size = 1000
            };

            var response = await SendRequest(ApiUrlTransaction, body, _ApiKey, _ClientID);
            var data = JsonConvert.DeserializeObject<OzonTransactionResponse>(response);

            var transactions = new List<List<object>>();
            transactions.AddRange(ParseTransactions(data.result.operations));

            var totalPages = data.result.page_count;

            for (var page = 2; page <= totalPages; page++)
            {
                body.page = page;
                response = await SendRequest(ApiUrlTransaction, body, _ApiKey, _ClientID);
                data = JsonConvert.DeserializeObject<OzonTransactionResponse>(response);
                transactions.AddRange(ParseTransactions(data.result.operations));
            }

            return transactions;
        }
        static List<List<object>> ParseTransactions(List<Operation> operations)
        {
            var transactions = new List<List<object>>();
            foreach (var operation in operations)
            {
                var servicesTotal = operation.services.Count > 0 ? operation.services.Sum(s => s.price) : 0;
                var itemName = operation.items.Count > 0 ? string.Join(" , ", operation.items.Select(i => i.name)) : "Прочее";
                var itemSKU = operation.items.Count > 0 ? string.Join(" , ", operation.items.Select(i => i.sku)) : "Прочее";
                var transaction = new List<object>
                {
                    operation.operation_date,
                    operation.operation_type_name,
                    operation.posting.delivery_schema,
                    operation.posting.posting_number,
                    itemSKU,
                    itemName,
                    operation.accruals_for_sale,
                    operation.sale_commission,
                    servicesTotal,
                    operation.amount
                };
                transactions.Add(transaction);
            }
            return transactions;
        }

        public static void WriteToExcel(
            List<StockInfo> stocks,
            List<Warehouse> warehouses,
            List<List<object>> ordersFBO,
            List<List<object>> ordersFBS,
            List<string> headersFBO,
            List<string> headersFBS,
            List<ProductInfo> products,
            List<List<object>> transactions,
            List<string> headersTransactions)
            {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage())
            {
                // Лист Stock
                var worksheetStock = package.Workbook.Worksheets.Add("Stock");
                worksheetStock.Cells[1, 1].Value = "№";
                worksheetStock.Cells[1, 2].Value = "Название товара";
                worksheetStock.Cells[1, 3].Value = "Остаток FBO";
                worksheetStock.Cells[1, 4].Value = "Остаток FBS";
                int rowStock = 2;
                int numStock = 1;
                foreach (var stock in stocks)
                {
                    worksheetStock.Cells[rowStock, 1].Value = numStock++;
                    worksheetStock.Cells[rowStock, 2].Value = stock.OfferId;
                    worksheetStock.Cells[rowStock, 3].Value = stock.Fbo;
                    worksheetStock.Cells[rowStock, 4].Value = stock.Fbs;
                    rowStock++;
                }

                // Лист Warehouses
                var worksheetWarehouses = package.Workbook.Worksheets.Add("Warehouses");
                worksheetWarehouses.Cells[1, 1].Value = "ID";
                worksheetWarehouses.Cells[1, 2].Value = "Name";
                worksheetWarehouses.Cells[1, 3].Value = "Type";
                worksheetWarehouses.Cells[1, 4].Value = "Address";
                int rowWarehouses = 2;
                foreach (var warehouse in warehouses)
                {
                    worksheetWarehouses.Cells[rowWarehouses, 1].Value = warehouse.id;
                    worksheetWarehouses.Cells[rowWarehouses, 2].Value = warehouse.name;
                    worksheetWarehouses.Cells[rowWarehouses, 3].Value = warehouse.type;
                    worksheetWarehouses.Cells[rowWarehouses, 4].Value = warehouse.address;
                    rowWarehouses++;
                }

                // Лист Заказы FBO
                var worksheetFBO = package.Workbook.Worksheets.Add("Заказы FBO");
                for (int col = 1; col <= headersFBO.Count; col++)
                {
                    worksheetFBO.Cells[1, col].Value = headersFBO[col - 1];
                }
                for (int row = 2; row <= ordersFBO.Count + 1; row++)
                {
                    for (int col = 1; col <= ordersFBO[row - 2].Count; col++)
                    {
                        worksheetFBO.Cells[row, col].Value = ordersFBO[row - 2][col - 1];
                    }
                }

                // Лист Заказы FBS
                var worksheetFBS = package.Workbook.Worksheets.Add("Заказы FBS");
                for (int col = 1; col <= headersFBS.Count; col++)
                {
                    worksheetFBS.Cells[1, col].Value = headersFBS[col - 1];
                }
                for (int row = 2; row <= ordersFBS.Count + 1; row++)
                {
                    for (int col = 1; col <= ordersFBS[row - 2].Count; col++)
                    {
                        worksheetFBS.Cells[row, col].Value = ordersFBS[row - 2][col - 1];
                    }
                }

                // Лист Товары
                var worksheetProducts = package.Workbook.Worksheets.Add("Товары");
                var headersProducts = new List<string>
                {
                    "Index", "OfferId", "SKU", "FbsId", "FboId", "Barcode", "Primary Image", "Name",
                    "Old Price", "Price", "Marketing Price", "Recommended Price", "Index Price",
                    "Created At", "Updated At", "Status State Description", "Stocks Coming",
                    "Stocks Present", "Stocks Reserved", "Status State Name"
                };
                for (int col = 1; col <= headersProducts.Count; col++)
                {
                    worksheetProducts.Cells[1, col].Value = headersProducts[col - 1];
                }
                int rowProducts = 2;
                foreach (var product in products)
                {
                    worksheetProducts.Cells[rowProducts, 1].Value = rowProducts - 1;
                    worksheetProducts.Cells[rowProducts, 2].Value = product.offer_id;
                    worksheetProducts.Cells[rowProducts, 3].Value = product.sku;
                    worksheetProducts.Cells[rowProducts, 4].Value = product.fbs_sku;
                    worksheetProducts.Cells[rowProducts, 5].Value = product.fbo_sku;
                    worksheetProducts.Cells[rowProducts, 6].Value = product.barcode;
                    worksheetProducts.Cells[rowProducts, 7].Value = product.primary_image;
                    worksheetProducts.Cells[rowProducts, 8].Value = product.name;
                    worksheetProducts.Cells[rowProducts, 9].Value = product.old_price;
                    worksheetProducts.Cells[rowProducts, 10].Value = product.price;
                    worksheetProducts.Cells[rowProducts, 11].Value = product.marketing_price;
                    worksheetProducts.Cells[rowProducts, 12].Value = product.recommended_price;
                    worksheetProducts.Cells[rowProducts, 13].Value = product.price_index;
                    worksheetProducts.Cells[rowProducts, 14].Value = product.created_at;
                    worksheetProducts.Cells[rowProducts, 15].Value = product.updated_at;
                    worksheetProducts.Cells[rowProducts, 16].Value = product.status.state_description;
                    worksheetProducts.Cells[rowProducts, 17].Value = product.stocks.coming;
                    worksheetProducts.Cells[rowProducts, 18].Value = product.stocks.present;
                    worksheetProducts.Cells[rowProducts, 19].Value = product.stocks.reserved;
                    worksheetProducts.Cells[rowProducts, 20].Value = product.status.state_name;
                    rowProducts++;
                }

                // Лист Отчет
                var worksheetReport = package.Workbook.Worksheets.Add("Отчет");
                for (int col = 1; col <= headersTransactions.Count; col++)
                {
                    worksheetReport.Cells[1, col].Value = headersTransactions[col - 1];
                }
                for (int row = 2; row <= transactions.Count + 1; row++)
                {
                    for (int col = 1; col <= transactions[row - 2].Count; col++)
                    {
                        worksheetReport.Cells[row, col].Value = transactions[row - 2][col - 1];
                    }
                }
                FileInfo file = new FileInfo($"Статистика_OZON_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.xlsx");
                package.SaveAs(file);
            }
        }
    }
    
}
