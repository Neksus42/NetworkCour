using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Page_Navigation_App.ViewModel
{
    class MainViewModel : Utilities.ViewModelBase
    {
        public NavigationVM NavigationVM { get; }
        public HomeVM HomeVM { get; }


        public MainViewModel() 
        {
            NavigationVM = new NavigationVM();
            HomeVM = new HomeVM();
        }
    }
}
