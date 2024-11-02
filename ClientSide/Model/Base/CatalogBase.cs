using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Page_Navigation_App.Model.Base
{
    class CatalogBase
    {
        ObservableCollection<CatalogItems> _CatalogItems;

        public ObservableCollection<CatalogItems> CatalogItems
        {
            get { return _CatalogItems; }
            set { _CatalogItems = value; }
        }

        static private ObservableCollection<string> _comboitems = new ObservableCollection<string>();
        public ObservableCollection<string> Comboitems_Catalog
        {
            get { return _comboitems; }
            set { _comboitems = value; }
        }
    }
}
