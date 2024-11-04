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

namespace Page_Navigation_App.ViewModel
{
    class CartVM : Utilities.ViewModelBase
    {
        CatalogBase _catalogBase = new CatalogBase();
        

        ObservableCollection<CartBase> _cartCollection = new ObservableCollection<CartBase>();
        public ObservableCollection<CartBase> CartCollection
        {
            get { return _cartCollection; }
            set { _cartCollection = value; OnPropertyChanged(); }
        }

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

        public CartVM()
        {
            DeletePosition = new RelayCommand(OnDeletePosition,CanDeletePosition);


        }
    }
}
