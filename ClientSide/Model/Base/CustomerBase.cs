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
        static ObservableCollection<CustomerOrderItems> _Customer_Order_Items;
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

        
        public ObservableCollection<CustomerOrderItems> Customer_Order_Items
        {
            get { return _Customer_Order_Items; }
            set { _Customer_Order_Items = value;}
        }

        static private ObservableCollection<string> _comboitems = new ObservableCollection<string>();
        public ObservableCollection<string> Comboitems
        {
            get { return _comboitems; }
            set { _comboitems = value;}
        }
    }
}
