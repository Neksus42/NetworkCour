
using MySql.Data.MySqlClient;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;


namespace ServerSide
{

    public class Customer
    {
        public int customer_id { get; set; }
        public string phone_number { get; set; }
        public string customer_name { get; set; }
        private string role { get; set; }


    }
    public class CustomerOrderItems
    {
        public string component_name { get; set; }
        public string category_name { get; set; }
        public string manufacturer_name { get; set; }
        public int quantity { get; set; }
        public int price { get; set; }

    }

    public class getCode
    { 
    static public int getIntFromMessageJson(ref string JsonMessage)
        {

            string intpart = string.Empty;

            foreach (char line in JsonMessage)
            {
                if (line == ':')
                    break;
                intpart += line;
                
            }

            return Convert.ToInt32(intpart);
        }
    
    
    }
    public class TcpServer
    {
        static private string connectionString = "Server=localhost;Database=networkbase;User ID=root;Password=1111;";
        static private TcpListener listener;
        static private MySqlConnection connection;
        static AsyncLocal<Customer> localCustomer = new AsyncLocal<Customer>();
        static private string CodeHandler(ref string jsonstring)
        {
            string[] subarr = jsonstring.Split(':', 2);
            int Code = Convert.ToInt32(subarr[0]);
            Console.WriteLine($"Код задачи: {Code}");
            string JsonData = subarr[1];
            string stringtoreturn = string.Empty;

            switch (Code)
            {
                case 1:
                    {
                        stringtoreturn = Authorization(JsonData);
                        break;
                    }
                case 2:
                    {
                        stringtoreturn = SendDataClientOrders();
                        break;
                    }
                case 3:
                    {
                        stringtoreturn = SendOrderItemsforClient(JsonData);
                        break;
                    }

            }


            return stringtoreturn;
        }
        static private string SendOrderItemsforClient(string date)
        {
            string stringresult = string.Empty;
            string query = $"SELECT " +

                
                $"cmp.component_name, cat.category_name, m.manufacturer_name, oi.quantity, oi.price " +
                $"FROM " +
                $"orders o " +
                $"JOIN " +
                $"order_items oi ON o.order_id = oi.order_id " +
                $"JOIN " +
                $"components cmp ON oi.component_id = cmp.component_id " +
                $"JOIN " +
                $"categories cat ON cmp.category_id = cat.category_id " +
                $"JOIN " +
                $"manufacturers m ON cmp.manufacturer_id = m.manufacturer_id " +
                $"WHERE " +
                $"o.order_date = '{date}';";
            Console.WriteLine(query);
            using (var command = new MySqlCommand(query, connection))
            {
                Console.WriteLine($"Позиции заказа клиента {localCustomer.Value.customer_name}");
                using (var reader = command.ExecuteReader())
                {
                    List<CustomerOrderItems> AllOrderItems = [];
                    while (reader.Read())
                    {
                        CustomerOrderItems customerOrderItems = new CustomerOrderItems();
                        customerOrderItems.component_name = reader.GetString("component_name");
                        customerOrderItems.category_name = reader.GetString("category_name");
                        customerOrderItems.manufacturer_name = reader.GetString("manufacturer_name");
                        customerOrderItems.quantity = reader.GetInt32("quantity");
                        customerOrderItems.price = reader.GetInt32("price");
                        AllOrderItems.Add(customerOrderItems);
                    }
                    stringresult = JsonSerializer.Serialize<List<CustomerOrderItems>>(AllOrderItems);
                }
            }
            return stringresult;
        }

        static private string SendDataClientOrders()
        {
            string stringresult = string.Empty;

            string query = $"SELECT * FROM networkbase.orders where customer_id = {localCustomer.Value.customer_id};";
            Console.WriteLine(query);
            using (var command = new MySqlCommand(query, connection))
            {
                Console.WriteLine($"Заказы клиента {localCustomer.Value.customer_name}");
                using (var reader = command.ExecuteReader())
                {
                    List<string> AllOrders = new List<string>();
                    while(reader.Read())
                    {
                        DateTime orderDate = reader.GetDateTime("order_date");
                        AllOrders.Add(orderDate.ToString("yyyy-MM-dd HH:mm:ss"));
                    }
                    stringresult = JsonSerializer.Serialize<List<string>>(AllOrders);
                }
            }
            return stringresult;
        }
        static private string Authorization(string Data)
        {
            string stringresult = string.Empty;
            string role = string.Empty;
            localCustomer.Value = JsonSerializer.Deserialize<Customer>(Data);
            
            string query = "SELECT * FROM networkbase.customers where phone_number ="+ localCustomer.Value.phone_number+";";
            using (var command = new MySqlCommand(query, connection))
            {
                Console.WriteLine($"Client: {localCustomer.Value.customer_name} Phone: {localCustomer.Value.phone_number}");
                using (var reader = command.ExecuteReader())
                {
                    
                    if (reader.Read())
                    {
                        role = reader.GetString("role");
                        if (reader.GetString("customer_name") == localCustomer.Value.customer_name)
                        {
                            stringresult = "1"; // Успешная авторизация
                            Console.WriteLine($"Клиент: {localCustomer.Value.customer_name}");
                            localCustomer.Value.customer_id = reader.GetInt32("customer_id");
                        }
                        else
                        {
                            stringresult = "2"; // Несовпадение имени и номера
                        }
                    }
                    else // Клиента нет в БД => Регистрация
                    {
                        reader.Close();
                        string queryAddCustomer = "INSERT INTO `networkbase`.`customers` (`phone_number`, `customer_name`) VALUES (@phone_number, @customer_name);";
                        using (var addCommand = new MySqlCommand(queryAddCustomer, connection))
                        {
                            addCommand.Parameters.AddWithValue("@phone_number", localCustomer.Value.phone_number);
                            addCommand.Parameters.AddWithValue("@customer_name", localCustomer.Value.customer_name);
                            addCommand.ExecuteNonQuery();
                        }
                        stringresult = "3"; // Регистрация успешна
                        query = "SELECT * FROM networkbase.customers where phone_number =" + localCustomer.Value.phone_number + ";";
                        using (var addCommand = new MySqlCommand(query, connection))
                        {
                            using (var secondreader = command.ExecuteReader())
                            {
                                secondreader.Read();
                                localCustomer.Value.customer_id = secondreader.GetInt32("customer_id");
                            }

                        }
                    }
                    stringresult += role == "admin" ? ":1" : ":0";

                }
            }








            return stringresult;
        }
        public static void StartServerAsync(int port)
        {
            listener = new TcpListener(IPAddress.IPv6Any, port);
            listener.Server.DualMode = true;
            listener.Start();

            Console.WriteLine("Сервер запущен, ожидание подключения...");
            try
            {
                connection = new MySqlConnection(connectionString);
                connection.Open();
                Console.WriteLine("Соединение с SQL сервером открыто");
            }
            catch (Exception ex) {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }
        public static async Task SendMessageOverTcpAsync(TcpClient client, string message,NetworkStream stream)
        {
          
            //var stream = client.GetStream();

            // Отправляем JSON-байты
            await  stream.WriteAsync(Encoding.UTF8.GetBytes(message));
            Console.WriteLine("Сообщение отправлено.");
        }
        public static async Task RecieveConnection(TcpClient tcpClient)
        {
            
            try {Console.WriteLine("Подключен: " + tcpClient.Client.RemoteEndPoint);
            var stream = tcpClient.GetStream();
            while (true) // делаем цикл бесконечным
            {
                byte[] buffer = new byte[1024];
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

                if (bytesRead == 0)
                {
                    Console.WriteLine("Клиент отключился.");
                    break; // выходим из цикла, если клиент отключился
                }

                string receivedJson = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Console.WriteLine("Полученный JSON: " + receivedJson + "\n");
                    
                    string response = CodeHandler(ref receivedJson);
                    //await SendMessageOverTcpAsync(tcpClient, response,stream);

                    await stream.WriteAsync(Encoding.UTF8.GetBytes(response));
                    Console.WriteLine($"Сообщение отправлено.{response}");
                }

            tcpClient.Close();
            Console.WriteLine("Клиент отключен");
                
            }catch (Exception ex) { Console.WriteLine(ex.ToString()); }


        }

        public static async void WaitConnection()
        {
            
            while (true)
            {
                var tcpclient = await listener.AcceptTcpClientAsync();
                

                new Thread(async () => {
                    localCustomer.Value = new Customer();
                    await RecieveConnection(tcpclient);
                    }).Start();
            }
        }
    }



    internal class Program
    {

        public static void Main()
        {
            TcpServer.StartServerAsync(8888);

           
                new Thread(() => TcpServer.WaitConnection()).Start();
            while (true) { }

            



        }


    }
}

