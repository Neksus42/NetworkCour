using Page_Navigation_App.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net.Sockets;
using System.ComponentModel;
using Page_Navigation_App.Services;

namespace Page_Navigation_App.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

       
        public MainWindow()
        {
            ServerConnection.ConnectToServerAsync();
            InitializeComponent();
            
            


        }



        private void Window_Close(object sender, CancelEventArgs e)
        {
            ServerConnection.Disconnect();
            
        }

        private void CloseApp_Click(object sender, RoutedEventArgs e)
        {
            ServerConnection.Disconnect();
            Close();
        }
        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }
    }
}
