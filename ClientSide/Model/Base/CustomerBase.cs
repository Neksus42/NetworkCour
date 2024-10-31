using Page_Navigation_App.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Page_Navigation_App.Model.Base
{
    internal class CustomerBase
    {
        static private bool IsCreated = false;
        static private CustomerBase instance = null;
        static private bool IsSecondCreated = false;
        //private readonly PageModel _pageModel;
        // static private bool issended = false;
        static private string _CustomerName;
        static private int _CustomerPhone;
        static private string _Role;
        public string Role
        {
            get { return _Role; }
            set { _Role = value;}
        }
        public string CustomerName
        {
            get { return _CustomerName; }
            set { _CustomerName = value;}
        }
        public int CustomerPhone
        {
            get { return _CustomerPhone; }
            set { _CustomerPhone = value; }
        }

        static ObservableCollection<Customer> _Customers_List = new ObservableCollection<Customer>
                {
                    new Customer { customer_id=6, customer_name="Apple", phone_number="54990" },
                    new Customer {customer_id=6, customer_name="Microsoft", phone_number="39990"},

                };
        public ObservableCollection<Customer> Customers_List
        {
            get { return _Customers_List; }
            set { _Customers_List = value;}
        }

        static private ObservableCollection<string> _comboitems = new ObservableCollection<string>();
        public ObservableCollection<string> Comboitems
        {
            get { return _comboitems; }
            set { _comboitems = value;}
        }
    }
}
