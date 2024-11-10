using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSide
{
    internal class Selectedorder
    {
        public int order_item_id { get; set; }
        public int order_id { get; set; }
        public string component_name { get; set; }
        public int quantity { get; set; }
        public int price { get; set; }
    }
}
