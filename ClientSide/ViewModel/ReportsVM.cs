using Page_Navigation_App.Model.Base;
using Page_Navigation_App.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;
using ClosedXML.Excel;
namespace Page_Navigation_App.ViewModel
{
    internal class ReportsVM : ViewModelBase
    {
        ObservableCollection<ReportPanelBase> _reports;
        public ObservableCollection<ReportPanelBase> ReportOrders
        {

            get { return _reports; }
            set => Set(ref _reports, value);
        }

        ObservableCollection<AllOrderItems> _order_items;
        public ObservableCollection<AllOrderItems> OrderItems
        {

            get { return _order_items; }
            set => Set(ref _order_items, value);
        }
        private async void SaveReportToExcel(string filePath)
        {
            string Answer = string.Empty;
            await ServerConnection.SendDataAsync("14:<>");
             Answer = await ServerConnection.GetDataAsync();
            //MessageBox.Show(Answer);
            ReportOrders = JsonSerializer.Deserialize<ObservableCollection<ReportPanelBase>>(Answer);
            await ServerConnection.SendDataAsync("17:<>");
            Answer = await ServerConnection.GetDataAsync();
            //MessageBox.Show(Answer);
            OrderItems = JsonSerializer.Deserialize<ObservableCollection<AllOrderItems>>(Answer);

            using (var workbook = new XLWorkbook())
            {
                // Добавляем рабочий лист
                var worksheet = workbook.Worksheets.Add("Orders Report");

                // Заполняем заголовки столбцов
                worksheet.Cell(1, 1).Value = "Order ID";
                worksheet.Cell(1, 2).Value = "Order Date";
                worksheet.Cell(1, 3).Value = "Total Amount";
                worksheet.Cell(1, 4).Value = "Customer ID";
                worksheet.Cell(1, 5).Value = "Customer Name";
                worksheet.Columns().AdjustToContents();

                // Заполняем строки данными
                for (int i = 0; i < ReportOrders.Count; i++)
                {
                    var order = ReportOrders[i];
                    worksheet.Cell(i + 2, 1).Value = order.order_id;
                    worksheet.Cell(i + 2, 2).Value = order.order_date;
                    worksheet.Cell(i + 2, 3).Value = order.total_amount;
                    worksheet.Cell(i + 2, 4).Value = order.customer_id;
                    worksheet.Cell(i + 2, 5).Value = order.customer_name;
                }

                worksheet.Cell(1, 7).Value = "Order item id";
                worksheet.Cell(1, 8).Value = "Order id";
                worksheet.Cell(1, 9).Value = "Component name";
                worksheet.Cell(1, 10).Value = "Quantity";
                worksheet.Cell(1, 11).Value = "Price";
                worksheet.Cell(1, 12).Value = "Category name";
                worksheet.Cell(1, 13).Value = "Manufacturer name";
                worksheet.Columns().AdjustToContents();
                // Заполняем строки данными
                for (int i = 0; i < OrderItems.Count; i++)
                {
                    var order = OrderItems[i];
                    worksheet.Cell(i + 2, 7).Value = order.order_item_id;
                    worksheet.Cell(i + 2, 8).Value = order.order_id;
                    worksheet.Cell(i + 2, 9).Value = order.component_name;
                    worksheet.Cell(i + 2, 10).Value = order.quantity;
                    worksheet.Cell(i + 2, 11).Value = order.price;
                    worksheet.Cell(i +2,12).Value = order.category_name;
                    worksheet.Cell(i+2,13).Value = order.manufacturer_name;
                }
                worksheet.Range(1, 1, 1, 5).SetAutoFilter();
                worksheet.Range("G1:M1").SetAutoFilter();
                worksheet.Columns().AdjustToContents();

                // Сохраняем Excel файл
                workbook.SaveAs(filePath);
            }
        }


        public ICommand MakeReport { get; }

        private bool CanMakeReport(object p) => true;

        private async void OnMakeReport(object p)
        {
            SaveReportToExcel("ReportOrders.xlsx");


        }

        public ReportsVM()
        {

            MakeReport = new RelayCommand(OnMakeReport, CanMakeReport);
        
        
        
        }








    }
}
