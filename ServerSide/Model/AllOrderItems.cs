using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSide.Model
{
    internal class AllOrderItems
    {
        public int order_item_id { get; set; }
        public int order_id { get; set; }
        public string component_name { get; set; }
        public int quantity { get; set; }
        public int price { get; set; }
        public string category_name { get; set; }
        public string manufacturer_name { get; set; }
    }
}
