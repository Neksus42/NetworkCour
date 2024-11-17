using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSide.Model
{
    internal class Component
    {
        public string component_name { get; set; }
        public int price { get; set; }

        public string category_name { get; set; }
        public string manufacturer_name { get; set; }
    }
}
