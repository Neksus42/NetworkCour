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
        ObservableCollection<string> MainDates = new ObservableCollection<string> { };


        ObservableCollection<string> _Dates ;
        public ObservableCollection<string> Dates
        {

            get { return MainDates; }
            set => Set(ref MainDates, value);
        }
        ObservableCollection<string> _Dates2 ;
        public ObservableCollection<string> Dates2
        {

            get { return _Dates2; }
            set => Set(ref _Dates2, value);
        }
        int? _SelectedIndexDate = null;

        public int? SelectedIndexDate
        {
            get { return _SelectedIndexDate; }
            set => Set(ref _SelectedIndexDate, value);

        }
        int? _SelectedIndexDate2 = null;

        public int? SelectedIndexDate2
        {
            get { return _SelectedIndexDate2; }
            set => Set(ref _SelectedIndexDate2, value);

        }
        private async void SaveReportToExcel(string filePath)
        {
            string Answer = string.Empty;
            await ServerConnection.SendDataAsync("14:<>");
             Answer = await ServerConnection.GetDataAsync();
     
            ReportOrders = JsonSerializer.Deserialize<ObservableCollection<ReportPanelBase>>(Answer);
            await ServerConnection.SendDataAsync("17:<>");
            Answer = await ServerConnection.GetDataAsync();
     
            OrderItems = JsonSerializer.Deserialize<ObservableCollection<AllOrderItems>>(Answer);

            using (var workbook = new XLWorkbook())
            {
                
                var worksheet = workbook.Worksheets.Add("Orders Report");

              
                worksheet.Cell(1, 1).Value = "Order ID";
                worksheet.Cell(1, 2).Value = "Order Date";
                worksheet.Cell(1, 3).Value = "Total Amount";
                worksheet.Cell(1, 4).Value = "Customer ID";
                worksheet.Cell(1, 5).Value = "Customer Name";
                worksheet.Columns().AdjustToContents();

          
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

                worksheet.Cell(1, 15).Value = "Category";
                worksheet.Cell(1, 16).Value = "Total Sum";
                await ServerConnection.SendDataAsync($"18:{MainDates[Convert.ToInt32(SelectedIndexDate)]}:{MainDates[Convert.ToInt32(SelectedIndexDate2)]}");
                Answer = await ServerConnection.GetDataAsync();
         
                List<CategoryPriceSumForPeriod> CategoryPrice = JsonSerializer.Deserialize<List<CategoryPriceSumForPeriod>>(Answer);
                for (int i = 0; i < CategoryPrice.Count; i++)
                {
                    var CategoryCurr = CategoryPrice[i];
                    worksheet.Cell(i + 2, 15).Value = CategoryCurr.category;
                    worksheet.Cell(i + 2, 16).Value = CategoryCurr.total_price;
                    
                }
                worksheet.Cell(2, 17).Value = $"From: {MainDates[Convert.ToInt32(SelectedIndexDate)]} to: {MainDates[Convert.ToInt32(SelectedIndexDate2)]}";

                worksheet.Cell(1, 19).Value = "Manufacturer";
                worksheet.Cell(1, 20).Value = "Total Sum";
                await ServerConnection.SendDataAsync($"19:<>");
                Answer = await ServerConnection.GetDataAsync();

                List<ManufacturerPriceSum> ManufacturerPrice = JsonSerializer.Deserialize<List<ManufacturerPriceSum>>(Answer);
                for (int i = 0; i < ManufacturerPrice.Count; i++)
                {
                    var ManufacturerCurr = ManufacturerPrice[i];
                    worksheet.Cell(i + 2, 19).Value = ManufacturerCurr.manufacturer;
                    worksheet.Cell(i + 2, 20).Value = ManufacturerCurr.total_price;

                }



                worksheet.Cell(1, 22).Value = "Month";
                worksheet.Cell(1, 23).Value = "Total Sum";


                await ServerConnection.SendDataAsync($"20:<>");
                Answer = await ServerConnection.GetDataAsync();

                List<SumPriceByMonths> MonthPrice = JsonSerializer.Deserialize<List<SumPriceByMonths>>(Answer);
                for (int i = 0; i < MonthPrice.Count; i++)
                {
                    var MonthCurr = MonthPrice[i];
                    worksheet.Cell(i + 2, 22).Value = MonthCurr.month;
                    worksheet.Cell(i + 2, 23).Value = MonthCurr.total_sum;

                }



                worksheet.Columns().AdjustToContents();
                workbook.SaveAs(filePath);
            }
        }
        static void FillDates(ObservableCollection<string> collection)
        {
          
            DateTime now = DateTime.Now;
            int currentYear = now.Year;
            int currentMonth = now.Month;

       
            for (int year = 2024; year <= currentYear; year++)
            {
                int startMonth = (year == 2024) ? 1 : 1;
                int endMonth = (year == currentYear) ? currentMonth : 12;

                for (int month = startMonth; month <= endMonth; month++)
                {
                    string date = $"{year}-{month:D2}"; 
                    collection.Add(date);
                }
            }
        }

        public ICommand MakeReport { get; }

        private bool CanMakeReport(object p) => true;

        private async void OnMakeReport(object p)
        {
            if(SelectedIndexDate2 < SelectedIndexDate || SelectedIndexDate == null || SelectedIndexDate2 == null) return;
            SaveReportToExcel("ReportOrders.xlsx");


        }

        public ReportsVM()
        {

            MakeReport = new RelayCommand(OnMakeReport, CanMakeReport);
            FillDates(MainDates);
                _Dates = MainDates;
            _Dates2 = MainDates;
        
        
        }








    }
}
