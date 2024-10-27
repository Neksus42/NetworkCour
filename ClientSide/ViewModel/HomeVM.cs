﻿using Page_Navigation_App.Utilities;
using System;
using System.Windows;
using System.Windows.Input;
using Page_Navigation_App.Model;
using System.Text.Json;


namespace Page_Navigation_App.ViewModel
{
    class HomeVM : Utilities.ViewModelBase
    {
        static private int _number;
        static private string _name;
        private Visibility _isvisible = Visibility.Collapsed;
        public Visibility IsVisible
        {
            get { return _isvisible; }
            set => Set(ref _isvisible, value);
        }
        public int Number { get { return _number; } set => Set(ref _number, value); }
        public string Name { get { return _name; } set => Set(ref _name, value); }

       
      

        #region ChangeNumber
        public ICommand ChangeNumber {  get; }

        private bool CanChangeNumber(object p) => _number >= 0;

        private void OnChangeNumber(object p) 
        {
            if (p is null) return;
            Number += Convert.ToInt32(p);
        }
        #endregion
        #region ChangeVisibility
        public ICommand ChangeVisibility { get; }

        private bool CanChangeVisibility(object p) => true;

        private void OnChangeVisibility(object p)
        {
            IsVisible = Visibility.Visible;
        }
        #endregion

        #region SendMessageAuthorization
        public ICommand SendMessageAuthorization { get; }

        private bool CanSendMessageAuthorization(object p) => true;

        private void OnSendMessageAuthorization(object p)
        {
            Customer customer = new Customer(Convert.ToString(Number), Name);
            MainWindow.SendDataAsync("1:"+JsonSerializer.Serialize<Customer>(customer));
        }
        #endregion


        public HomeVM()
        {
            SendMessageAuthorization = new RelayCommand(OnSendMessageAuthorization, CanSendMessageAuthorization);
            ChangeNumber = new RelayCommand(OnChangeNumber, CanChangeNumber);
            ChangeVisibility = new RelayCommand(OnChangeVisibility, CanChangeVisibility);
        }
    }
}
