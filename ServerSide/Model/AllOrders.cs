using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSide.Model
{
    internal class AllOrders
    {
        public int order_id { get; set; }
        public string order_date { get; set; }
        public int total_amount { get; set; }
        public int customer_id { get; set; }
        public string customer_name { get; set; }
    }
}
