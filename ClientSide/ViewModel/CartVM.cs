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

namespace Page_Navigation_App.ViewModel
{
    class CartVM : Utilities.ViewModelBase
    {
        //CatalogBase _catalogBase = new CatalogBase();
        public EventNotification CurrentEventer;

        ObservableCollection<CartBase> _cartCollection = new ObservableCollection<CartBase>();
        public ObservableCollection<CartBase> CartCollection
        {
            get { return _cartCollection; }
            set { _cartCollection = value; OnPropertyChanged(); }
        }

     

        int _SelectedItemRow;

        public int SelectedItemRow
        {
            get { return _SelectedItemRow; }
            set => Set(ref _SelectedItemRow, value);

        }


        public ICommand DeletePosition { get; }

        private bool CanDeletePosition(object p) => true;

        private void OnDeletePosition(object p)
        {
            if(SelectedItemRow == -1) return;
            CartCollection.RemoveAt(SelectedItemRow);
           
        }

        public ICommand  ConfirmOrder { get; }

        private bool CanConfirmOrder(object p) => true;

        private async void OnConfirmOrder(object p)
        {
            if(CartCollection.Count() == 0) return;
            await GlobalSemaphore.ServerSemaphore.WaitAsync();
            await ServerConnection.SendDataAsync("5:" + JsonSerializer.Serialize<ObservableCollection<CartBase>>(CartCollection));

            string jsontocombo = await ServerConnection.GetDataAsync();
            GlobalSemaphore.ServerSemaphore.Release();
            CurrentEventer.Update.Invoke(this, new EventArgs());
            CartCollection.Clear();
           
        }
        int _totalprice;

        public int TotalPrice
        {
            get { return _totalprice; }
            set => Set(ref _totalprice, value);

        }
        public void refreshtotalprice(object sender, NotifyCollectionChangedEventArgs e)
        {
            int tempsum = 0;
            foreach (var item in CartCollection)
            {
                tempsum += item.position_price;
            }
            TotalPrice = tempsum;   
        }
        public CartVM()
        {
            DeletePosition = new RelayCommand(OnDeletePosition,CanDeletePosition);
            ConfirmOrder = new RelayCommand(OnConfirmOrder, CanConfirmOrder);
            CartCollection.CollectionChanged += refreshtotalprice;
        }
    }
}
