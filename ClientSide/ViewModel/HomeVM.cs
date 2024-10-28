using Page_Navigation_App.Utilities;
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
        static private string _viewmessage = "Text";
        static private Visibility _isvisible = Visibility.Collapsed;
        static private bool _isAuthorizationSuccessful = true;
        static private bool _isadmin = false;
        public bool IsAuthorizationSuccessful
        {
            get => _isAuthorizationSuccessful;
            set => Set(ref _isAuthorizationSuccessful, value);
        }
        public Visibility IsVisible
        {
            get { return _isvisible; }
            set => Set(ref _isvisible, value);
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

        private async void OnSendMessageAuthorization(object p)
        {
            Customer customer = new Customer(Convert.ToString(Number), Name);
            await ServerConnection.SendDataAsync("1:"+JsonSerializer.Serialize<Customer>(customer));
            string Answer = await ServerConnection.GetDataAsync();
            string[] splitted = Answer.Split(':');
            string CodeAnswer = splitted[0];
            string Role = splitted[1];

            MessageBox.Show(Answer);
            if (Answer == "1")
            {
                ViewMessage = "Успешная авторизация";
                
                OnChangeVisibility(null);
                IsAuthorizationSuccessful = false;
                IsAdmin = Convert.ToBoolean(int.Parse(Role));
            } else if(Answer == "2")
            {
                ViewMessage = "Несовпадение имени и номера";
            } else if(Answer == "3")
            {
                OnChangeVisibility(null);
                ViewMessage = "Успешная регистрация";
                IsAuthorizationSuccessful = false;
            }

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
