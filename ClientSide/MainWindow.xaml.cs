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

namespace Page_Navigation_App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        static private TcpClient _client;
        public MainWindow()
        {
           
            InitializeComponent();
            ConnectToServerAsync();

        }
        private async void ConnectToServerAsync()
        {
            try
            {
                _client = new TcpClient(); // Инициализируем TcpClient
                await _client.ConnectAsync("localhost", 8888);
                
            }
            catch (SocketException ex)
            {
                MessageBox.Show($"Ошибка подключения: {ex.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}");
            }
        }
        static public async Task SendDataAsync(string data)
        {
            if (_client == null || !_client.Connected)
            {
                MessageBox.Show("Не подключено к серверу.");
                return;
            }

            try
            {
                NetworkStream stream = _client.GetStream();
                byte[] dataBytes = Encoding.UTF8.GetBytes(data);
                await stream.WriteAsync(dataBytes, 0, dataBytes.Length);
                
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при отправке данных: {ex.Message}");
            }
        }



        private void CloseApp_Click(object sender, RoutedEventArgs e)
        {
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
