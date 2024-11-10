using Page_Navigation_App.Model.Base;
using Page_Navigation_App.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Page_Navigation_App.Utilities;
using System.Text.Json;
using Page_Navigation_App.Services;
using System.Collections.Specialized;
using ControlzEx;
using System.Windows;
using System.Windows.Controls;

namespace Page_Navigation_App.ViewModel
{
    class AdminPanelVM : Utilities.ViewModelBase
    {
        ObservableCollection<Object> _DataGridCollection = new ObservableCollection<Object>();
        public ObservableCollection<Object> DataGridCollection
        {
            get { return _DataGridCollection; }
            set { _DataGridCollection = value; OnPropertyChanged(); }
        }
        
        
        string _manufacturername;
        public string ManufacturerName
        { get { return _manufacturername; } set { _manufacturername = value; OnPropertyChanged(); } }

        string _categoryname;
        public string CategoryName
        { get { return _categoryname; } set { _categoryname = value; OnPropertyChanged(); } }

        ObservableCollection<string> _ComboItemsManufacturers = new ObservableCollection<string>();

        public ObservableCollection<string> ComboItemsManufacturers
        {
            get { return _ComboItemsManufacturers; }
            set { _ComboItemsManufacturers = value; OnPropertyChanged(); }
        }

        ObservableCollection<string> _ComboItemsCategories = new ObservableCollection<string>();

        public ObservableCollection<string> ComboItemsCategories
        {
            get { return _ComboItemsCategories; }
            set { _ComboItemsCategories = value; OnPropertyChanged(); }
        }
        int? _SelectedCategoryItemRow = null;

        public int? SelectedCategoryItemRow
        {
            get { return _SelectedCategoryItemRow; }
            set => Set(ref _SelectedCategoryItemRow, value);

        }
        int? _SelectedManufacturerItemRow = null;

        public int? SelectedManufacturerItemRow
        {
            get { return _SelectedManufacturerItemRow; }
            set => Set(ref _SelectedManufacturerItemRow, value);

        }
        string _ComponentName;
        public string ComponentName
        {
            get { return _ComponentName; } set { _ComponentName = value; OnPropertyChanged(); }
        }

        int? _ComponentPrice;

        public int? ComponentPrice
        { get { return _ComponentPrice; } set { _ComponentPrice = value; OnPropertyChanged(); } }

        int? _SelectedIndexManufacturer;
        public int? SelectedIndexManufacturer
        {
            get { return _SelectedIndexManufacturer; }
            set {  Set(ref _SelectedIndexManufacturer, value); }
        }
        int? _SelectedIndexCategory;
        public int? SelectedIndexCategory
        {
            get { return _SelectedIndexCategory; }
            set { Set(ref _SelectedIndexCategory, value); }
        }
        int? _SelectedIndexDataGrid;
        public int? SelectedIndexDataGrid
        {
            get { return _SelectedIndexDataGrid; }
            set { Set(ref _SelectedIndexDataGrid, value); }
        }
        Visibility _Visibility = Visibility.Hidden;
        public Visibility VisibilityForOrderItemsButton
        {
            get => _Visibility;
            set => Set(ref _Visibility, value);

        }

        #region AddComponent

        public ICommand SendNewComponent { get; }

        private bool CanSendNewComponent(object p) => true;

        private async void OnSendNewComponent(object p)
        {
            if(ComponentName == "" || ComponentPrice == null) return;
            var CurrentComponent = new Component();
            CurrentComponent.component_name = ComponentName;
            CurrentComponent.price = Convert.ToInt32(ComponentPrice);
            CurrentComponent.manufacturer_name = ComboItemsManufacturers[Convert.ToInt32(SelectedIndexManufacturer)];
            CurrentComponent.category_name = ComboItemsCategories[Convert.ToInt32(SelectedIndexCategory)];
            await ServerConnection.SendDataAsync("12:" + JsonSerializer.Serialize<Component>(CurrentComponent));
            string Answer = await ServerConnection.GetDataAsync();
        }



        #endregion
        #region SendCategory_Manufacturer
        public ICommand SendManufacturer { get; }

        private bool CanSendManufacturer(object p) => true;

        private async void OnSendManufacturer(object p)
        {
            if (ManufacturerName == "") return;
            await ServerConnection.SendDataAsync("6:" + ManufacturerName);
            string Answer = await ServerConnection.GetDataAsync();
            ComboItemsManufacturers.Add(ManufacturerName);
        }

        public ICommand SendCategory { get; }

        private bool CanSendCategory(object p) => true;

        private async void OnSendCategory(object p)
        {
            if (CategoryName == "") return;
            await ServerConnection.SendDataAsync("7:" + CategoryName);
            string Answer = await ServerConnection.GetDataAsync();
            ComboItemsCategories.Add(CategoryName);
        }
        #endregion

        #region ShowForDataGrid
        public ICommand ShowAllOrders { get; }

        private bool CanShowAllOrders(object p) => true;

        private async void OnShowAllOrders(object p)
        {
            await ServerConnection.SendDataAsync("14:<>");
            string Answer = await ServerConnection.GetDataAsync();
            //MessageBox.Show(Answer);
            List<AllOrders> Orders = JsonSerializer.Deserialize<List<AllOrders>>(Answer);


            DataGridCollection = new ObservableCollection<object>(Orders);
            VisibilityForOrderItemsButton = Visibility.Visible;
        }
        
        public ICommand ShowAllCustomers { get; }

        private bool CanShowAllCustomers(object p) => true;

        private async void OnShowAllCustomers(object p)
        {
            await ServerConnection.SendDataAsync("13:<>");
            string Answer = await ServerConnection.GetDataAsync();
            //MessageBox.Show(Answer);
            List<AllCustomers> customers = JsonSerializer.Deserialize<List<AllCustomers>>(Answer);


            DataGridCollection = new ObservableCollection<object>(customers);
         
            
            VisibilityForOrderItemsButton = Visibility.Hidden;
            //MessageBox.Show(DataGridCollection.GetType().Name);
        }
        public ICommand ShowSelectedOrder { get; }

        private bool CanShowSelectedOrder(object p) => true;

        private async void OnShowSelectedOrder(object p)
        {
            if (SelectedIndexDataGrid == null || SelectedIndexDataGrid == -1) return;
            AllOrders temp = DataGridCollection[Convert.ToInt32(SelectedIndexDataGrid)] as AllOrders;
            await ServerConnection.SendDataAsync("15:"+temp.order_id);
            string Answer = await ServerConnection.GetDataAsync();
            //MessageBox.Show(Answer);
            List<Selectedorder> SelectedOrder = JsonSerializer.Deserialize<List<Selectedorder>>(Answer);


            DataGridCollection = new ObservableCollection<object>(SelectedOrder);
            VisibilityForOrderItemsButton = Visibility.Hidden;
        }


        #endregion


        #region DeleteCategory_Manufacturer
        public ICommand DeleteManufacturer { get; }

        private bool CanDeleteManufacturer(object p) => true;

        private async void OnDeleteManufacturer(object p)
        {
            if (SelectedManufacturerItemRow == null) return;
            await ServerConnection.SendDataAsync("10:" + ComboItemsManufacturers[Convert.ToInt32(SelectedManufacturerItemRow)]);
            string Answer = await ServerConnection.GetDataAsync();
            ComboItemsManufacturers.RemoveAt(Convert.ToInt32(SelectedManufacturerItemRow));
            SelectedManufacturerItemRow = null;
        }

        public ICommand DeleteCategory { get; }

        private bool CanDeleteCategory(object p) => true;

        private async void OnDeleteCategory(object p)
        {
            if (SelectedCategoryItemRow == null) return;
            await ServerConnection.SendDataAsync("11:" + ComboItemsCategories[Convert.ToInt32(SelectedCategoryItemRow)]);
            string Answer = await ServerConnection.GetDataAsync();
            ComboItemsCategories.RemoveAt(Convert.ToInt32(SelectedCategoryItemRow));
            SelectedCategoryItemRow = null;
        }
        #endregion

        #region DeleteRow

        public ICommand DeleteRowFromDataGrid { get; }

        private bool CanDeleteRowFromDataGrid(object p) => true;

        private async void OnDeleteRowFromDataGrid(object p)
        {
            if (SelectedIndexDataGrid == null || SelectedIndexDataGrid == -1) return;

            var selectedItem = DataGridCollection[Convert.ToInt32(SelectedIndexDataGrid)];
            if (selectedItem == null) return;

            string classType = selectedItem.GetType().Name;
            int id = selectedItem switch
            {
                AllCustomers customer => customer.customer_id,
                AllOrders order => order.order_id,
                Selectedorder orderItem => orderItem.order_item_id,
                _ => -1
            };

            if (id == -1) return;

            await ServerConnection.SendDataAsync($"16:{classType}:{id}");
            string Answer = await ServerConnection.GetDataAsync();
            DataGridCollection.RemoveAt(Convert.ToInt32(SelectedIndexDataGrid));
        }
        #endregion
        #region Fills
        public async void FillComboitemsManufacturers()
        {
            await GlobalSemaphore.ServerSemaphore.WaitAsync();
            await Task.Run(() => ServerConnection.SendDataAsync("8:<>"));
            string jsontocombo = await Task.Run(() => ServerConnection.GetDataAsync());
            GlobalSemaphore.ServerSemaphore.Release();

            //MessageBox.Show(jsontocombo);
            ComboItemsManufacturers = JsonSerializer.Deserialize<ObservableCollection<string>>(jsontocombo);

        }
        public async void FillComboitemsCategories()
        {
            await GlobalSemaphore.ServerSemaphore.WaitAsync();
            await Task.Run(() => ServerConnection.SendDataAsync("9:<>"));
            string jsontocombo = await Task.Run(() => ServerConnection.GetDataAsync());
            GlobalSemaphore.ServerSemaphore.Release();

            //MessageBox.Show(jsontocombo);
            ComboItemsCategories = JsonSerializer.Deserialize<ObservableCollection<string>>(jsontocombo);

        }
        #endregion
        public AdminPanelVM()
        {
            SendManufacturer = new RelayCommand(OnSendManufacturer,CanSendManufacturer);
            SendCategory = new RelayCommand(OnSendCategory, CanSendCategory);
            DeleteManufacturer = new RelayCommand(OnDeleteManufacturer, CanDeleteManufacturer);
            DeleteCategory = new RelayCommand(OnDeleteCategory, CanDeleteCategory);
            SendNewComponent = new RelayCommand(OnSendNewComponent, CanSendNewComponent);
            ShowAllOrders = new RelayCommand(OnShowAllOrders, CanShowAllOrders);
            ShowAllCustomers = new RelayCommand(OnShowAllCustomers, CanShowAllCustomers);
            ShowSelectedOrder = new RelayCommand(OnShowSelectedOrder, CanShowSelectedOrder);
            DeleteRowFromDataGrid = new RelayCommand(OnDeleteRowFromDataGrid, CanDeleteRowFromDataGrid);
            FillComboitemsManufacturers();
            FillComboitemsCategories();
        }
    }

}
