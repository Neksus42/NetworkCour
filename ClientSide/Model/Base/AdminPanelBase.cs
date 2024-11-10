using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Page_Navigation_App.Model.Base
{
    class Component
    {
        public string component_name { get; set; }
        public int price { get; set; }

        public string category_name { get; set; }
        public string manufacturer_name { get; set; }
    }

    public interface AdminGridBase
    { }
    class AllOrders 
    {
        public int order_id { get; set; }
        public string order_date { get; set; }
        public int total_amount { get; set; }
        public int customer_id { get; set; }
        public string customer_name { get; set; }
    }
    class Selectedorder 
    {
        public int order_item_id { get; set; }
        public int order_id { get; set; }
        public string component_name { get;set; }
        public int quantity { get; set; }
        public int price { get; set; }
    }
    class AllCustomers
    {
        public int customer_id { get; set; }
        public string phone_number { get; set; }
        public string customer_name { get; set; }
        public string role {  get; set; }
    }
}
