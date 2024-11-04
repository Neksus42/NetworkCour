using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Page_Navigation_App.Model.Base
{
    class CartBase
    {
        public string component_name { get; set; }
        public int position_price { get; set; }

        public string category_name { get; set; }
        public string manufacturer_name { get; set; }

        public int quantity { get; set; }
    }
}
