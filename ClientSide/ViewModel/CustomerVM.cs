using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Page_Navigation_App.Model;
using Page_Navigation_App.Utilities;
using NLog;
using System.Windows.Controls;
using System.Threading;
using Page_Navigation_App.Model.Base;
using System.Data;
namespace Page_Navigation_App.ViewModel
{
    class CustomerVM : Utilities.ViewModelBase
    {
        private CustomerBase _CustomerBaseModel;
        public CustomerBase CustomerBaseModel
        {
            get { return _CustomerBaseModel; } set => Set(ref _CustomerBaseModel, value); }

        private ObservableCollection<string> _comboitems;
        public ObservableCollection<string> Comboitems
        {
            get { return _comboitems; }
            set => Set(ref _comboitems, value);
        }
        static private string _CustomerName;
        static private int _CustomerPhone;
        static private string _Role;
        public string Role
        {
            get { return _Role; }
            set { _Role = value;OnPropertyChanged(); }
        }
        public string CustomerName
        {
            get { return _CustomerName; }
            set { _CustomerName = value; }
        }
        public int CustomerPhone
        {
            get { return _CustomerPhone; }
            set { _CustomerPhone = value; }
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
            string jsontocombo;
            async Task RunAsyncTask()
            {
                MessageBox.Show("jsontocombo");
                await ServerConnection.SendDataAsync("2:<>");
                MessageBox.Show("jsontocombo2");
                string jsontocombo = await ServerConnection.GetDataAsync();
                //await ServerConnection.GetDataAsync().ConfigureAwait(false);
                //await Task.Run(() => ServerConnection.SendDataAsync("2:<>"));
                MessageBox.Show("jsontocombo3");
            }

            // Запуск
            await RunAsyncTask();




            Application.Current.Dispatcher.Invoke(() =>
            {
                // Пример обновления Customers_List
                CustomerBaseModel.Customers_List = new ObservableCollection<Customer>
        {
            new Customer { customer_id=6, customer_name="Apple", phone_number="54990" }
        };
            });

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

        public void Refreshitems()
        {
            Role = CustomerBaseModel.Role;
        }
        public ICommand GetComboItems { get; }

        private bool CanGetComboItems(object p) => true;

        private async void OnGetComboItems(object p)
        {
            await Task.Run(() => ServerConnection.SendDataAsync("2:<>"));
            string jsontocombo = await Task.Run(() => ServerConnection.GetDataAsync());

            Application.Current.Dispatcher.Invoke(() =>
            {
                MessageBox.Show(jsontocombo);
                CustomerBaseModel.Comboitems = JsonSerializer.Deserialize<ObservableCollection<string>>(Convert.ToString(jsontocombo));
                Comboitems = CustomerBaseModel.Comboitems;
                
            });
        }

        public ICommand Check { get; }

        private bool CanCheck(object p) => true;

        private async void OnCheck(object p)
        {
            await Task.Run(() => ServerConnection.SendDataAsync("2:<>"));
            string jsontocombo = await Task.Run(() => ServerConnection.GetDataAsync());
            //OnPropertyChanged(nameof(CustomerBaseModel.Comboitems));
        }


        private void FillItms()
        {

        }

        //public static CustomerVM GetInstance()
        //{

        //    if (!IsCreated)
        //    {
        //        IsCreated = true;
        //        instance = new CustomerVM();
                
        //        return instance;
        //    }
            
        //    return instance;
        //}
        public CustomerVM()
        {
            
            GetComboItems = new RelayCommand(OnGetComboItems, CanGetComboItems);
            Check = new RelayCommand(OnCheck, CanCheck);
            //MessageBox.Show("CustomerVM");
            //this.CustomerBaseModel = CustomerBase;
            

            Fillcomboitems();
        }
    }
}
