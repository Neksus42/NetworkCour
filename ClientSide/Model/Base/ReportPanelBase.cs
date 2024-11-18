using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Page_Navigation_App.Model.Base
{
    internal class ReportPanelBase
    {
        public int order_id { get; set; }
        public string order_date { get; set; }
        public int total_amount { get; set; }
        public int customer_id { get; set; }
        public string customer_name { get; set; }
    }
    class AllOrderItems
    {

        public int order_item_id { get; set; }
        public int order_id { get; set; }
        public string component_name { get; set; }
        public int quantity { get; set; }
        public int price { get; set; }
        public string category_name { get; set; }
        public string manufacturer_name { get; set; }
    }
    class CategoryPriceSumForPeriod
    {
        public int total_price { get; set; }
        public string category { get; set; }
    }
    class ManufacturerPriceSum
    {
        public int total_price { get; set; }
        public string manufacturer { get; set; }
    }
    class SumPriceByMonths
    {
        public int total_sum {  get; set; }
        public string month {  get; set; }
    }
}
