using Demo.Foundation.ProcessingEngine.Models;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace Demo.Foundation.ProcessingEngine.Import
{
    public class ExcelImportProcessor
    {
        public List<Customer> GetImportData(Stream fileStream)
        {
            return BuildObjectModel(ReadExcel(fileStream));
        }

        public List<PurchaseInvoice> GetImportProducts(Stream fileStream)
        {
            return BuildProductModel(ReadExcel(fileStream));
        }

        private List<Customer> BuildObjectModel(DataTable dataTable)
        {
            if (dataTable == null)
                throw new ArgumentNullException(nameof(dataTable));

            List<Customer> customers = new List<Customer>();

            var groupedData = dataTable.AsEnumerable().GroupBy(x => x.Field<string>("CustomerID"));
            foreach (IGrouping<string, DataRow> data in groupedData)
            {
                if (!string.IsNullOrEmpty(data.Key))
                {
                    int customerId = int.Parse(data.Key);
                    if (customerId <= 0)
                        continue;

                    Customer customer = new Customer
                    {
                        CustomerId = customerId,
                        Invoices = new List<PurchaseInvoice>(),
                    };

                    foreach (DataRow record in data.ToList())
                    {
                        int.TryParse(record["Quantity"].ToString(), out var quantity);
                        decimal.TryParse(record["UnitPrice"].ToString(), out var unitPrice);
                        DateTime.TryParse(record["InvoiceDate"].ToString(), out var dt);
                        int.TryParse(record["InvoiceNo"].ToString(), out var number);

                        if (number > 0 && quantity > 0)
                        {
                            customer.Invoices.Add(new PurchaseInvoice()
                            {
                                Quantity = quantity,
                                Country = record["Country"].ToString(),
                                Number = number,
                                StockCode = record["StockCode"].ToString(),
                                Price = unitPrice,
                                TimeStamp = dt.AddYears(7),
                                Currency = "EUR"
                            });
                        }


                    }

                    if (customer.Invoices.Any())
                        customers.Add(customer);
                }
            }

            return customers;
        }

        private List<PurchaseInvoice> BuildProductModel(DataTable dataTable)
        {
            if (dataTable == null)
                throw new ArgumentNullException(nameof(dataTable));

            List<PurchaseInvoice> products = new List<PurchaseInvoice>();

            var groupedData = dataTable.AsEnumerable().GroupBy(x => x.Field<string>("StockCode"));
            foreach (IGrouping<string, DataRow> data in groupedData)
            {
                if (!string.IsNullOrEmpty(data.Key))
                {
                    var found = false;
                    foreach (var record in data)
                    {
                        var description = record["Description"].ToString();
                        int.TryParse(record["Quantity"].ToString(), out var quantity);
                        decimal.TryParse(record["UnitPrice"].ToString(), out var unitPrice);
                        int.TryParse(record["InvoiceNo"].ToString(), out var number);



                        if (number > 0 && quantity > 0 && !string.IsNullOrEmpty(description))
                        {
                            found = true;
                            products.Add(new PurchaseInvoice
                            {
                                Quantity = quantity,
                                Number = number,
                                StockCode = data.Key,
                                Price = unitPrice,
                                Description = description
                            });
                        }

                        if(found) break;
                    }
                }
            }

            return products;
        }

        private DataTable ReadExcel(Stream stream)
        {
            DataTable dt = new DataTable();

            using (var excelPackage = new ExcelPackage(stream))
            {
                var ws = excelPackage.Workbook.Worksheets.First();

                foreach (var firstRowCell in ws.Cells[1, 1, 1, ws.Dimension.End.Column])
                {
                    dt.Columns.Add(firstRowCell.Text);
                }

                for (int rowNum = 2; rowNum <= ws.Dimension.End.Row; rowNum++)
                {
                    var wsRow = ws.Cells[rowNum, 1, rowNum, ws.Dimension.End.Column];
                    DataRow row = dt.Rows.Add();
                    foreach (var cell in wsRow)
                    {
                        row[cell.Start.Column - 1] = cell.Text;
                    }
                }
            }

            return dt;
        }
    }
}
