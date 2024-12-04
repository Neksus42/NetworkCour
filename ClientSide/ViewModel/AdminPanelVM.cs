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
        EventNotification CurrentEventer;
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
        /// <summary>
        /// /////////////////////////
        /// </summary>



        #region ElementsVisibility
        Visibility _ManufacturerEditingElementsVisibility = Visibility.Hidden;
        public Visibility ManufacturerEditingElementsVisibility
        {
            get => _ManufacturerEditingElementsVisibility;
            set => Set(ref _ManufacturerEditingElementsVisibility, value);

        }
        Visibility _ManufacturerDelitingElementsVisibility = Visibility.Visible;
        public Visibility ManufacturerDelitingElementsVisibility
        {
            get => _ManufacturerDelitingElementsVisibility;
            set => Set(ref _ManufacturerDelitingElementsVisibility, value);

        }

        Visibility _CategoryEditingElementsVisibility = Visibility.Hidden;
        public Visibility CategoryEditingElementsVisibility
        {
            get => _CategoryEditingElementsVisibility;
            set => Set(ref _CategoryEditingElementsVisibility, value);

        }
        Visibility _CategoryDelitingElementsVisibility = Visibility.Visible;
        public Visibility CategoryDelitingElementsVisibility
        {
            get => _CategoryDelitingElementsVisibility;
            set => Set(ref _CategoryDelitingElementsVisibility, value);

        }


        #endregion



        #region ChangeManufacturerElementsVisibility
        private double _manufacturerDelitingElementsOpacity = 1.0;
        public double ManufacturerDelitingElementsOpacity
        {
            get => _manufacturerDelitingElementsOpacity;
            set
            {
                if (_manufacturerDelitingElementsOpacity != value)
                {
                    _manufacturerDelitingElementsOpacity = value;
                    OnPropertyChanged(nameof(ManufacturerDelitingElementsOpacity));
                }
            }
        }
        private double _manufacturerEditingElementsOpacity = 0;
        public double ManufacturerEditingElementsOpacity
        {
            get => _manufacturerEditingElementsOpacity;
            set
            {
                if (_manufacturerEditingElementsOpacity != value)
                {
                    _manufacturerEditingElementsOpacity = value;
                    OnPropertyChanged(nameof(ManufacturerEditingElementsOpacity));
                }
            }
        }


        public ICommand ChangeManufacturerElementsVisibility { get; }
        bool ManufacturerExecutable = true;
        private bool CanChangeManufacturerElementsVisibility(object p) => true;

        private async void OnChangeManufacturerElementsVisibility(object p)
        {
            if (!ManufacturerExecutable) return;
            ManufacturerExecutable = false;
            if (ManufacturerDelitingElementsVisibility == Visibility.Visible)
            {
            
                for (double opacity = 1.0; opacity >= 0; opacity -= 0.1)
                {
                    ManufacturerDelitingElementsOpacity = opacity;
                    await Task.Delay(30); 
                }

                
                ManufacturerDelitingElementsVisibility = Visibility.Hidden;
                ManufacturerDelitingElementsOpacity = 0;

                
                ManufacturerEditingElementsVisibility = Visibility.Visible;
                ManufacturerEditingElementsOpacity = 0;

         
                for (double opacity = 0; opacity <= 1.0; opacity += 0.1)
                {
                    ManufacturerEditingElementsOpacity = opacity;
                    await Task.Delay(30);
                }
            }
            else if (ManufacturerEditingElementsVisibility == Visibility.Visible)
            {
                
                for (double opacity = 1.0; opacity >= 0; opacity -= 0.1)
                {
                    ManufacturerEditingElementsOpacity = opacity;
                    await Task.Delay(30); 
                }

          
                ManufacturerEditingElementsVisibility = Visibility.Hidden;
                ManufacturerEditingElementsOpacity = 0;

                
                ManufacturerDelitingElementsVisibility = Visibility.Visible;
                ManufacturerDelitingElementsOpacity = 0;

            
                for (double opacity = 0; opacity <= 1.0; opacity += 0.1)
                {
                    ManufacturerDelitingElementsOpacity = opacity;
                    await Task.Delay(30);
                }
            }
            ManufacturerExecutable = true;
        }
        #endregion
        #region ChangeCategoryElementsVisibility
        private double _CategoryDelitingElementsOpacity = 1.0;
        public double CategoryDelitingElementsOpacity
        {
            get => _CategoryDelitingElementsOpacity;
            set
            {
                if (_CategoryDelitingElementsOpacity != value)
                {
                    _CategoryDelitingElementsOpacity = value;
                    OnPropertyChanged(nameof(CategoryDelitingElementsOpacity));
                }
            }
        }
        private double _CategoryEditingElementsOpacity = 0;
        public double CategoryEditingElementsOpacity
        {
            get => _CategoryEditingElementsOpacity;
            set
            {
                if (_CategoryEditingElementsOpacity != value)
                {
                    _CategoryEditingElementsOpacity = value;
                    OnPropertyChanged(nameof(CategoryEditingElementsOpacity));
                }
            }
        }


        public ICommand ChangeCategoryElementsVisibility { get; }
        bool CategoryExecutable = true;
        private bool CanChangeCategoryElementsVisibility(object p) => true;

        private async void OnChangeCategoryElementsVisibility(object p)
        {
            if (!CategoryExecutable) return;

            CategoryExecutable = false;
            if (CategoryDelitingElementsVisibility == Visibility.Visible)
            {

                for (double opacity = 1.0; opacity >= 0; opacity -= 0.1)
                {
                    CategoryDelitingElementsOpacity = opacity;
                    await Task.Delay(30);
                }


                CategoryDelitingElementsVisibility = Visibility.Hidden;
                CategoryDelitingElementsOpacity = 0;


                CategoryEditingElementsVisibility = Visibility.Visible;
                CategoryEditingElementsOpacity = 0;


                for (double opacity = 0; opacity <= 1.0; opacity += 0.1)
                {
                    CategoryEditingElementsOpacity = opacity;
                    await Task.Delay(30);
                }
            }
            else if (CategoryEditingElementsVisibility == Visibility.Visible)
            {

                for (double opacity = 1.0; opacity >= 0; opacity -= 0.1)
                {
                    CategoryEditingElementsOpacity = opacity;
                    await Task.Delay(30);
                }


                CategoryEditingElementsVisibility = Visibility.Hidden;
                CategoryEditingElementsOpacity = 0;


                CategoryDelitingElementsVisibility = Visibility.Visible;
                CategoryDelitingElementsOpacity = 0;


                for (double opacity = 0; opacity <= 1.0; opacity += 0.1)
                {
                    CategoryDelitingElementsOpacity = opacity;
                    await Task.Delay(30);
                }
            }
            CategoryExecutable = true;
        }

        #endregion

        #region ChangeManufacturer
        private string _UpdateManufacturerstring;
        public string UpdateManufacturerstring
        {
            get => _UpdateManufacturerstring;
            set => Set(ref _UpdateManufacturerstring, value);

        }
        public ICommand UpdateManufacturer { get; }

        private bool CanUpdateManufacturer(object p) => true;

        private async void OnUpdateManufacturer(object p)
        {
            if(UpdateManufacturerstring == "" || SelectedManufacturerItemRow == null) return;
            await ServerConnection.SendDataAsync("21:" + ComboItemsManufacturers[Convert.ToInt32(SelectedManufacturerItemRow)]+":"+ UpdateManufacturerstring);
            string Answer = await ServerConnection.GetDataAsync();


            ComboItemsManufacturers[Convert.ToInt32(SelectedManufacturerItemRow)] = UpdateManufacturerstring;
            SelectedManufacturerItemRow = null;
            UpdateManufacturerstring = "";

        }
        #endregion
        #region ChangeCategory
        private string _UpdateCategorystring;
        public string UpdateCategorystring
        {
            get => _UpdateCategorystring;
            set => Set(ref _UpdateCategorystring, value);

        }
        public ICommand UpdateCategory { get; }

        private bool CanUpdateCategory(object p) => true;

        private async void OnUpdateCategory(object p)
        {
            if (UpdateCategorystring == "" || SelectedCategoryItemRow == null) return;
            await ServerConnection.SendDataAsync("22:" + ComboItemsCategories[Convert.ToInt32(SelectedCategoryItemRow)] + ":" + UpdateCategorystring);
            string Answer = await ServerConnection.GetDataAsync();


            ComboItemsCategories[Convert.ToInt32(SelectedCategoryItemRow)] = UpdateCategorystring;
            SelectedCategoryItemRow = null;
            UpdateCategorystring = "";

        }
        #endregion


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
            CurrentEventer.UpdateCatalog.Invoke(this, new EventArgs());
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
            ManufacturerName = "";
        }

        public ICommand SendCategory { get; }

        private bool CanSendCategory(object p) => true;

        private async void OnSendCategory(object p)
        {
            if (CategoryName == "") return;
            await ServerConnection.SendDataAsync("7:" + CategoryName);
            string Answer = await ServerConnection.GetDataAsync();
            ComboItemsCategories.Add(CategoryName);
            CategoryName = "";
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
        public ICommand ShowCatalog { get; }

        private bool CanShowCatalog(object p) => true;

        private async void OnShowCatalog(object p)
        {
            await ServerConnection.SendDataAsync("4:<>");

            string Answer = await ServerConnection.GetDataAsync();
            //MessageBox.Show(Answer);
            List<CatalogItems> Orders = JsonSerializer.Deserialize<List<CatalogItems>>(Answer);


            DataGridCollection = new ObservableCollection<object>(Orders);
            VisibilityForOrderItemsButton = Visibility.Hidden;
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
            string catalogname = string.Empty;
            int id = selectedItem switch
            {
                AllCustomers customer => customer.customer_id,
                AllOrders order => order.order_id,
                Selectedorder orderItem => orderItem.order_item_id,
                CatalogItems component when component.component_name != null =>
                    int.TryParse(component.component_name, out var parsedId) ? parsedId : -1,
                _ => -1
            };

            
            if (selectedItem is CatalogItems catalog)
            {
                catalogname = catalog.component_name;
                await ServerConnection.SendDataAsync($"16:{classType}:{catalogname}");
                id = -1; 
            }

            if (id == -1 && string.IsNullOrEmpty(catalogname)) return;
            if(!(id == -1))
            await ServerConnection.SendDataAsync($"16:{classType}:{id}");

            string Answer = await ServerConnection.GetDataAsync();
            DataGridCollection.RemoveAt(Convert.ToInt32(SelectedIndexDataGrid));
            CurrentEventer.UpdateCatalog.Invoke(this, new EventArgs());
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
        public AdminPanelVM(EventNotification CurrentEventer)

        {

            this.CurrentEventer = CurrentEventer;
            SendManufacturer = new RelayCommand(OnSendManufacturer,CanSendManufacturer);
            SendCategory = new RelayCommand(OnSendCategory, CanSendCategory);
            DeleteManufacturer = new RelayCommand(OnDeleteManufacturer, CanDeleteManufacturer);
            DeleteCategory = new RelayCommand(OnDeleteCategory, CanDeleteCategory);
            SendNewComponent = new RelayCommand(OnSendNewComponent, CanSendNewComponent);
            ShowAllOrders = new RelayCommand(OnShowAllOrders, CanShowAllOrders);
            ShowAllCustomers = new RelayCommand(OnShowAllCustomers, CanShowAllCustomers);
            ShowSelectedOrder = new RelayCommand(OnShowSelectedOrder, CanShowSelectedOrder);
            ShowCatalog = new RelayCommand(OnShowCatalog, CanShowCatalog);
            DeleteRowFromDataGrid = new RelayCommand(OnDeleteRowFromDataGrid, CanDeleteRowFromDataGrid);
            ChangeManufacturerElementsVisibility = new RelayCommand(OnChangeManufacturerElementsVisibility, CanChangeManufacturerElementsVisibility);
            ChangeCategoryElementsVisibility = new RelayCommand(OnChangeCategoryElementsVisibility, CanChangeCategoryElementsVisibility);
            UpdateManufacturer = new RelayCommand(OnUpdateManufacturer, CanUpdateManufacturer);
            UpdateCategory = new RelayCommand(OnUpdateCategory, CanUpdateCategory);
            FillComboitemsManufacturers();
            FillComboitemsCategories();
        }
    }

}
