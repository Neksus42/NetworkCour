using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Page_Navigation_App.Model;
using Page_Navigation_App.Utilities;

namespace Page_Navigation_App.ViewModel
{
    class CustomerVM : Utilities.ViewModelBase
    {
        static private bool IsCreated = false;
        static private CustomerVM instance = null;
        //private readonly PageModel _pageModel;
     // static private bool issended = false;
        static private string _CustomerName;
        static private int _CustomerPhone;
        static private string _Role;
        public string Role
        {
            get { return _Role; }
            set { _Role = value; OnPropertyChanged(); }
        }
        public string CustomerName
        {
            get { return _CustomerName; }
            set { _CustomerName = value; OnPropertyChanged(); }
        }
        public int CustomerPhone
        {
            get { return _CustomerPhone; }
            set { _CustomerPhone = value; OnPropertyChanged(); }
        }
        
        static ObservableCollection<Customer> _Customers_List = new ObservableCollection<Customer>
                {
                    new Customer { customer_id=6, customer_name="Apple", phone_number="54990" },
                    new Customer {customer_id=6, customer_name="Microsoft", phone_number="39990"},
    
                };
        public ObservableCollection<Customer> Customers_List
        {
            get { return _Customers_List; }
            set { _Customers_List = value; OnPropertyChanged(); }
        }

       static private ObservableCollection<string> _comboitems = new ObservableCollection<string>();
        public ObservableCollection<string> Comboitems
        {
            get { return _comboitems; }
            set { _comboitems = value; OnPropertyChanged(); }
        }


        private string _selectedItem;
        public string SelectedItem
        {
            get => _selectedItem;
            set
            {
                MessageBox.Show("Selected");
                
                _selectedItem = value;
                ChangeList(_selectedItem);
                OnPropertyChanged(); // Вызовите метод уведомления об изменениях
                

            }
        }
        public async void ChangeList(string data)
        {
            await ServerConnection.SendDataAsync("3:<>"); //Запрос на получение дат всех заказов клиента
            string jsontocombo = await ServerConnection.GetDataAsync();
            Customers_List = new ObservableCollection<Customer>
            {
                new Customer { customer_id=6, customer_name="Apple", phone_number="54990" }
            };

        }
        public async void Fillcomboitems()
        {
            ////JsonSerializer.Serialize<Customer>(customer)
            //await ServerConnection.SendDataAsync("2:<>"); //Запрос на получение дат всех заказов клиента
            //string jsontocombo = await ServerConnection.GetDataAsync();
            //MessageBox.Show($"ComboList{jsontocombo}");
            ////JsonSerializer.Deserialize<List<string>>(jsontocombo)
            //Comboitems = new List<string>()
            //{ 
            //"her","gan"

            //};

            await Task.Run(()=> OnGetComboItems(null));
            //OnGetComboItems(null);
            //Task.Delay(2000);
            //Comboitems = new List<string>()
            //{
            //"her","gan","gdfgdfgdf", "gdfgdf"

            //};


        }


        public ICommand GetComboItems { get; }

        private bool CanGetComboItems(object p) => true;

        private async void OnGetComboItems(object p)
        {
            //JsonSerializer.Serialize<Customer>(customer)
            await ServerConnection.SendDataAsync("2:<>"); //Запрос на получение дат всех заказов клиента
            string jsontocombo = await ServerConnection.GetDataAsync();
            MessageBox.Show($"ComboList{jsontocombo}");
            //JsonSerializer.Deserialize<List<string>>(jsontocombo)
            
            Comboitems = await Task.Run(() => JsonSerializer.Deserialize<ObservableCollection<string>>(jsontocombo));

        }






        public static CustomerVM GetInstance()
        {

            if (!IsCreated)
            {
                IsCreated = true;
                instance = new CustomerVM();
                
                return instance;
            }
            
            return instance;
        }
        public CustomerVM()
        {
            GetComboItems = new RelayCommand(OnGetComboItems, CanGetComboItems);
            //MessageBox.Show("CustomerVM");
            Fillcomboitems();
        }
    }
}
