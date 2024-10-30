using Page_Navigation_App.Utilities;
using System;
using System.Windows;
using System.Windows.Input;
using Page_Navigation_App.Model;
using System.Text.Json;
using System.Collections.Generic;


namespace Page_Navigation_App.ViewModel
{
    class HomeVM : Utilities.ViewModelBase
    {

         static private NavigationVM Nvm;
         static private HomeVM instance;
         static private bool IsCreated = false;
         private int _number;
         private string _name;
         private string _viewmessage = "Text";
        
        private bool _isAuthorizationSuccessful = true;
        private bool _isadmin = false;
        public bool IsAuthorizationSuccessful
        {
            get => _isAuthorizationSuccessful;
            set => Set(ref _isAuthorizationSuccessful, value);
        }
    
        public bool IsAdmin { get { return _isadmin; } set =>Set(ref _isadmin, value);} 
        public int Number { get { return _number; } set => Set(ref _number, value); }
        public string Name { get { return _name; } set => Set(ref _name, value); }
        public string ViewMessage { get { return _viewmessage; } set => Set(ref _viewmessage, value); } 

       
      

        #region ChangeNumber
        public ICommand ChangeNumber {  get; }

        private bool CanChangeNumber(object p) => _number >= 0;

        private void OnChangeNumber(object p) 
        {
            if (p is null) return;
            Number += Convert.ToInt32(p);
        }
        #endregion
        

        #region SendMessageAuthorization
        public ICommand SendMessageAuthorization { get; }

        private bool CanSendMessageAuthorization(object p) => true;

        private async void OnSendMessageAuthorization(object p)
        {
            if (Name == "" || Number == 0)
            {
                ViewMessage = "Неверный формат имени или номера";
                return;
            }
            Customer customer = new Customer(Convert.ToString(Number), Name);
            await ServerConnection.SendDataAsync("1:"+JsonSerializer.Serialize<Customer>(customer));
            string Answer = await ServerConnection.GetDataAsync();
            string[] splitted = Answer.Split(':');
            string CodeAnswer = splitted[0];
            string Role = splitted[1];
            IsAdmin = Role == "1";
            MessageBox.Show(Answer);

            if (CodeAnswer == "1")
            {
                ViewMessage = "Успешная авторизация";
                
                
                IsAuthorizationSuccessful = false;
                IsAdmin = Convert.ToBoolean(int.Parse(Role));
                SettingCustomer();

                ChangingVisibilityParameters();
               
            } else if(CodeAnswer == "2")
            {
                ViewMessage = "Несовпадение имени и номера";
            } else if(CodeAnswer == "3")
            {
                
                ViewMessage = "Успешная регистрация";
                IsAuthorizationSuccessful = false;
                SettingCustomer();

                ChangingVisibilityParameters();
            }
            
        }
        #endregion
        private void SettingCustomer()
        {
            CustomerVM.GetInstance().CustomerName = Name;
            CustomerVM.GetInstance().CustomerPhone = Number;
            CustomerVM.GetInstance().Role = IsAdmin ? "Admin" : "Customer";
            //CustomerVM.GetInstance().Fillcomboitems();
            
        }
        private void ChangingVisibilityParameters()
        {
            Nvm.IsVisC = Visibility.Visible;

            Nvm.CurrentView = CustomerVM.GetInstance();


           //CustomerVM.GetInstance().Fillcomboitems();
            Nvm.IsEnabledCustomer = true;
            Nvm.IsVisHome = Visibility.Collapsed;
            Nvm.IsVisLogOut = Visibility.Visible;
        }
        public static HomeVM GetInstance(NavigationVM nvm)
        {
            Nvm = nvm;

            if(!IsCreated)
            {
                IsCreated = true;
                
                return instance = new HomeVM();
            }
            return instance;
        }
        public static HomeVM CreateInstance(NavigationVM nvm)
        {
            Nvm = nvm;
            IsCreated = true;
            return instance = new HomeVM();
        }

        public HomeVM()
        {
            SendMessageAuthorization = new RelayCommand(OnSendMessageAuthorization, CanSendMessageAuthorization);
            ChangeNumber = new RelayCommand(OnChangeNumber, CanChangeNumber);
            
            
            
        }
    }
}
