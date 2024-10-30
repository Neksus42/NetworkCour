using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Page_Navigation_App.Utilities;
using System.Windows.Input;
using System.Windows;

namespace Page_Navigation_App.ViewModel
{
    class NavigationVM : ViewModelBase
    {
        private object _currentView;
        public object CurrentView
        {
            get { return _currentView; }
            set { _currentView = value; OnPropertyChanged(); }
        }
        private bool _iscustomerenabled;

        public bool IsEnabledCustomer
        { get { return _iscustomerenabled; } set => Set(ref _iscustomerenabled, value); }

        private bool _ishomerenabled = true;
        public bool IsEnabledHome
        { get { return _ishomerenabled; } set => Set(ref _ishomerenabled, value); }

        static private Visibility _isvisLogOut = Visibility.Hidden; public Visibility IsVisLogOut { get {  return _isvisLogOut; } set { Set(ref _isvisLogOut, value); } }

        static private Visibility _isvisforcustomer = Visibility.Collapsed;
        public Visibility IsVisC
        {   
            get { return _isvisforcustomer; }
            set => Set(ref _isvisforcustomer, value);
         }
        static private Visibility _isvishome = Visibility.Visible;

        public Visibility IsVisHome
        {
            get { return _isvishome; }
            set => Set(ref _isvishome, value);
        }

        public ICommand HomeCommand { get; set; }
        public ICommand CustomersCommand { get; set; }
        public ICommand ProductsCommand { get; set; }
        public ICommand OrdersCommand { get; set; }
        public ICommand TransactionsCommand { get; set; }
        public ICommand ShipmentsCommand { get; set; }
        public ICommand SettingsCommand { get; set; }

        public ICommand LogOutCommand { get;}
        private bool CanLogOutCommand(object p) => true;

        private void OnLogOutCommand(object p)
        {
            IsEnabledHome = true;
            IsVisC = Visibility.Hidden;
            IsVisHome = Visibility.Visible;
            IsVisLogOut = Visibility.Hidden;
            CurrentView = HomeVM.CreateInstance(this);

        }

        private void Home(object obj) => CurrentView = HomeVM.GetInstance(this);
        //private void Customer(object obj) => CurrentView = CustomerVM.GetInstance();
        private void Product(object obj) => CurrentView = new ProductVM();
        private void Order(object obj) => CurrentView = new OrderVM();


        public NavigationVM()
        {
            HomeCommand = new RelayCommand(Home);
            CustomersCommand = new RelayCommand(Product);
            ProductsCommand = new RelayCommand(Product);
            OrdersCommand = new RelayCommand(Order);
            LogOutCommand = new RelayCommand(OnLogOutCommand, CanLogOutCommand);

            // Startup Page
            CurrentView = HomeVM.GetInstance(this);
        }











        }
}
