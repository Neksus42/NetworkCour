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
    class CatalogVM : Utilities.ViewModelBase
    {
        CartVM _cartVM;
        public CartVM CartVM
        {
        get { return _cartVM; }
        set { _cartVM = value; }
        
        }
        CatalogBase _catalogBase = new CatalogBase();

        public CatalogBase CatalogBase
        {
            get { return _catalogBase; }
            set => Set(ref _catalogBase, value);
        }
        public ObservableCollection<CatalogItems> CatalogItems
        {
            get { return CatalogBase.CatalogItems; }
            set { CatalogBase.CatalogItems = value; OnPropertyChanged(); }
        }
        public ObservableCollection<string> ComboItems_catalog
        {
            get { return CatalogBase.Comboitems_Catalog; }
            set { CatalogBase.Comboitems_Catalog = value; OnPropertyChanged(); }
        }
        private int _Price = 0;
        public int Price
        {
            get { return _Price; }
            set => Set(ref _Price, value);
        }


        private int _selecteditemprice = 0;
        public int SelectedItemPrice
        {
            get { return _selecteditemprice; }
            set { _selecteditemprice = value; }
        }
        void SetItemPrice(string ComponentName)
        {
            foreach (var item in CatalogItems) {
                if (item.component_name == ComponentName)
                {
                    SelectedItemPrice = item.price;
                    break;
                }
            }
        }

        private int _counter = 0;
        public int Counter
        {
            get { return _counter; }
            set { Set(ref _counter, value); Price = Counter * SelectedItemPrice; }
        }
        private string _selectedItem;
        public string SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                SetItemPrice(SelectedItem);
                OnPropertyChanged();
                Price = Counter * SelectedItemPrice;
            }
        }


        public async void FillCatalog()
        {
            await GlobalSemaphore.ServerSemaphore.WaitAsync();
            await ServerConnection.SendDataAsync("4:<>");
            
            string jsontocombo = await ServerConnection.GetDataAsync();
            GlobalSemaphore.ServerSemaphore.Release();


            CatalogItems = JsonSerializer.Deserialize<ObservableCollection<CatalogItems>>(jsontocombo);


            foreach (CatalogItems CItems in CatalogItems)
            {
                ComboItems_catalog.Add(CItems.component_name);
            }

            CartVM.CatalogItems = CatalogItems;

        }
        int? _SelectedItemRow = null;

        public int? SelectedItemRow
        {
            get { return _SelectedItemRow; }
            set => Set(ref _SelectedItemRow, value);

        }


        public ICommand AddPosition { get; }

        private bool CanAddPosition(object p) => true;

        private void OnAddPosition(object p)
        {
            if(Counter == 0 || SelectedItemRow == null) return;
            CartBase additem = new CartBase();
            additem.component_name = CatalogItems[Convert.ToInt32(SelectedItemRow)].component_name;
            additem.category_name = CatalogItems[Convert.ToInt32(SelectedItemRow)].category_name;
            additem.manufacturer_name = CatalogItems[Convert.ToInt32(SelectedItemRow)].manufacturer_name;
            additem.quantity = Counter;
            additem.position_price = Price;

            CartVM.CartCollection.Add(additem);
        }
        public CatalogVM()
        {
            AddPosition = new RelayCommand(OnAddPosition, CanAddPosition);
          FillCatalog();
        }
    }
}
