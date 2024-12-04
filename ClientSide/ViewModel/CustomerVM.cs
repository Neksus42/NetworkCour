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
using Page_Navigation_App.Services;
namespace Page_Navigation_App.ViewModel
{
    class CustomerVM : Utilities.ViewModelBase
    {

        public EventNotification CurrentEventer;
        private CustomerBase _CustomerBaseModel;
        public CustomerBase CustomerBaseModel
        { get { return _CustomerBaseModel; } set => Set(ref _CustomerBaseModel, value); }

        
        public ObservableCollection<string> Comboitems
        {
            get { return CustomerBaseModel.Comboitems; }
            set { CustomerBaseModel.Comboitems = value; OnPropertyChanged(); }
        }
        public string Role
        {
            get { return CustomerBaseModel.Role; }
            set { CustomerBaseModel.Role = value;OnPropertyChanged(); }
        }
        public string CustomerName
        {
            get { return CustomerBaseModel.CustomerName; }
            set { CustomerBaseModel.CustomerName = value; OnPropertyChanged(); }
        }
        public int CustomerPhone
        {
            get { return CustomerBaseModel.CustomerPhone; }
            set { CustomerBaseModel.CustomerPhone = value; OnPropertyChanged(); }
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
      
        public ObservableCollection<CustomerOrderItems> Customer_Order_Items
        {
            get { return CustomerBaseModel.Customer_Order_Items; }
            set { CustomerBaseModel.Customer_Order_Items = value; OnPropertyChanged(); }
        }
        public async void ChangeList(string data)
        {
            
                //MessageBox.Show("jsontocombo");
                await ServerConnection.SendDataAsync($"3:{data}");
                //MessageBox.Show("jsontocombo2");
                string jsontocombo = await ServerConnection.GetDataAsync();
                //await ServerConnection.GetDataAsync().ConfigureAwait(false);
                //await Task.Run(() => ServerConnection.SendDataAsync("2:<>"));
                //MessageBox.Show("jsontocombo3");


            Application.Current.Dispatcher.Invoke(() =>
            {
                // Пример обновления Customers_List JsonSerializer.Deserialize<ObservableCollection<CustomerOrderItems>>(jsontocombo);
                Customer_Order_Items = JsonSerializer.Deserialize<ObservableCollection<CustomerOrderItems>>(jsontocombo);
            });
            Refreshitems();

        }
        public async void Fillcomboitems(object sender, EventArgs e)
        {
            await GlobalSemaphore.ServerSemaphore.WaitAsync();
            await Task.Run(() => ServerConnection.SendDataAsync("2:<>"));
            string jsontocombo = await Task.Run(() => ServerConnection.GetDataAsync());
            GlobalSemaphore.ServerSemaphore.Release();

            //MessageBox.Show(jsontocombo);
            Comboitems = JsonSerializer.Deserialize<ObservableCollection<string>>(jsontocombo);

        }

        public void Refreshitems()
        {
            Role = CustomerBaseModel.Role;
            CustomerName = CustomerBaseModel.CustomerName;
            CustomerPhone = CustomerBaseModel.CustomerPhone;
            Comboitems = CustomerBaseModel.Comboitems;
            Customer_Order_Items = CustomerBaseModel.Customer_Order_Items;
        }
        public ICommand GetComboItems { get; }

        private bool CanGetComboItems(object p) => true;

        private async void OnGetComboItems(object p)
        {
            await Task.Run(() => ServerConnection.SendDataAsync("2:<>"));
            string jsontocombo = await Task.Run(() => ServerConnection.GetDataAsync());

            Application.Current.Dispatcher.Invoke(() =>
            {
                //MessageBox.Show(jsontocombo);
                Comboitems = JsonSerializer.Deserialize<ObservableCollection<string>>(Convert.ToString(jsontocombo));
                
                
            });
        }

      


      
        public CustomerVM(EventNotification CurrentEventer)
        {
            
            GetComboItems = new RelayCommand(OnGetComboItems, CanGetComboItems);


            this.CurrentEventer = CurrentEventer;
            this.CurrentEventer.Update += Fillcomboitems;

            //MessageBox.Show("CustomerVM");
            //this.CustomerBaseModel = CustomerBase;


           Fillcomboitems(null,null);
        }
    }
}
