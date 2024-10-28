﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Page_Navigation_App.Model
{
    class Customer
    {
        public int customer_id { get; set; }
        public string phone_number { get; set; }
        public string customer_name { get; set; }
        private string role {  get; set; }

        public Customer(int customer_id, string phone_number, string customer_name,string role) 
        {
        this.customer_id = customer_id;
            this.phone_number = phone_number;
            this.customer_name = customer_name;
            this.role = role;
        }
        public Customer(string phone_number, string customer_name)
        {
            this.phone_number=phone_number;
            this.customer_name=customer_name;
        }


    }
}
