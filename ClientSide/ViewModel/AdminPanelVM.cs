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
            FillComboitemsManufacturers();
            FillComboitemsCategories();
        }
    }
}
