using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Page_Navigation_App.Model;

namespace Page_Navigation_App.ViewModel
{
    class CustomerVM : Utilities.ViewModelBase
    {
        static private bool IsCreated = false;
        static private CustomerVM instance;
        private readonly PageModel _pageModel;



        public int CustomerID
        {
            get { return _pageModel.CustomerCount; }
            set { _pageModel.CustomerCount = value; OnPropertyChanged(); }
        }

        public static CustomerVM GetInstance()
        {
           
            if (!IsCreated)
            {
                IsCreated = true;

                return instance = new CustomerVM();
            }
            return instance;
        }

        public CustomerVM()
        {
            _pageModel = new PageModel();
            CustomerID = 100528;
        }
    }
}
