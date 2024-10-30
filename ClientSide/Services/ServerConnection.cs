using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Page_Navigation_App
{


    class ServerConnection
    {
        static private TcpClient _client;
        static public async void ConnectToServerAsync()
        {
          
                try
                {
                    _client = new TcpClient();
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
        static public void Disconnect()
        {
            _client.Close();
        }
        static public async Task<string> GetDataAsync()
        {
            byte[] buffer = new byte[1024];
            int bytesRead = await _client.GetStream().ReadAsync(buffer);
            return Encoding.UTF8.GetString(buffer, 0, bytesRead);
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
                await stream.WriteAsync(dataBytes);  
                

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при отправке данных: {ex.Message}");
            }
        }
    }

    
}
