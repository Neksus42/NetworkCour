using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Page_Navigation_App.Services
{


    class ServerConnection
    {
        static private TcpClient _client;
        static public void ConnectToServerAsync()
        {

            try
            {
                _client = new TcpClient();
                _client.Connect("localhost", 8888);

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

            try
            {
                byte[] buffer = new byte[1024];
                StringBuilder completeMessage = new StringBuilder();
                NetworkStream stream = _client.GetStream();

                int bytesRead;
                while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    string chunk = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    completeMessage.Append(chunk);

                    // Проверяем наличие маркера конца сообщения
                    if (chunk.Contains("<END>"))
                    {
                        break;
                    }
                }

                // Удаляем маркер конца сообщения перед возвратом результата
                string fullMessage = completeMessage.ToString();
                return fullMessage.Replace("<END>", "");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
                return null;
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
                await stream.WriteAsync(dataBytes);


            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при отправке данных: {ex.Message}");
            }
        }
    }


}
