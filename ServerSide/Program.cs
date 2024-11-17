
using MySql.Data.MySqlClient;
using ServerSide.Model;
using System.Net;
using System.Net.Sockets;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace ServerSide
{




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
                case 4:
                    {stringtoreturn = SendCatalogItemsforClient();
                        break;
                    }
                case 5:
                    {stringtoreturn = AddCustomerOrder(JsonData);
                        break;
                    }
                case 6:
                    {stringtoreturn = InsertManufacturer(JsonData);
                        break;
                    }
                case 7:
                 {   stringtoreturn = InsertCategory(JsonData);
                        break;
                    }
                    case 8:
                    {
                        stringtoreturn = SendManufacturers();
                            break;
                    }
                    case 9:
                    {
                        stringtoreturn = SendCategories();
                        break;

                    }
                case 10:
                    {
                        stringtoreturn = DeleteManufacturer(JsonData);
                        break;

                    }
                    case 11:
                    {
                        stringtoreturn = DeleteCategory(JsonData);
                        break;
                    }
                case 12:
                    {
                        stringtoreturn = AddComponent(JsonData);
                        break;
                    }
                case 13:
                    {
                    stringtoreturn = SendAllCustomers();
                        break;
                    }
                case 14:
                    {
                        stringtoreturn = SendAllOrders();
                        break;

                    }
                    case 15:
                    {
                        stringtoreturn = SendOrder_items_byID(JsonData);
                        break;
                    }
                    case 16:
                    {
                        stringtoreturn = DeleteRowFromAdminDataGrid(JsonData);
                        break ;
                    }
                case 17:
                    {
                        stringtoreturn = SendAllOrder_items();
                        break;
                    }
                case 18:
                    {
                        stringtoreturn = SendCategorySumPriceForPeriod(JsonData);
                        break;
                    }

            }


            return stringtoreturn;
        }
        static private string SendCategorySumPriceForPeriod(string Data)
        {
            string[] subarr = Data.Split(':', 2);
            string query = @$"
                    SELECT 
                        c.category_name AS category,
                        SUM(comp.price * oi.quantity) AS total_amount
                    FROM 
                        orders o
                    JOIN 
                        order_items oi ON o.order_id = oi.order_id
                    JOIN 
                        components comp ON oi.component_id = comp.component_id
                    JOIN 
                        categories c ON comp.category_id = c.category_id
                    WHERE 
                        DATE_FORMAT(o.order_date, '%Y-%m') BETWEEN '{subarr[0]}' AND '{subarr[1]}'
                    GROUP BY 
                        c.category_name
                    ORDER BY 
                        total_amount DESC;";
            string jsontoreturn;
            using (var command = new MySqlCommand(query, connection))
            {
                Console.WriteLine($"Сумма заказов за период по категориям для {localCustomer.Value.customer_name}");
                List<CategoryPriceSumForPeriod> CategorySum = new List<CategoryPriceSumForPeriod>();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        CategorySum.Add(new CategoryPriceSumForPeriod
                        {
                            total_price = reader.GetInt32("total_amount"),
                            category = reader.GetString("category")

                        });



                    }


                }
                jsontoreturn = JsonSerializer.Serialize<List<CategoryPriceSumForPeriod>>(CategorySum);
                Console.WriteLine(jsontoreturn);
                return jsontoreturn;
            }
        }
        static private string SendAllOrder_items()
        {
            string query = @"
                    SELECT 
                        oi.order_item_id, 
                        oi.order_id, 
                        c.component_name, 
                        
                        oi.quantity, 
                        oi.price,
                         cat.category_name, 
                        m.manufacturer_name 
                    FROM networkbase.order_items oi
                    JOIN networkbase.components c ON oi.component_id = c.component_id
                    JOIN networkbase.categories cat ON c.category_id = cat.category_id
                    JOIN networkbase.manufacturers m ON c.manufacturer_id = m.manufacturer_id;";

            string jsontoreturn;
            using (var command = new MySqlCommand(query, connection))
            {
                Console.WriteLine($"Все содержимое всех заказов клиента для {localCustomer.Value.customer_name}");
                List<AllOrderItems> OrderItems = new List<AllOrderItems>();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        OrderItems.Add(new AllOrderItems
                        {
                            order_item_id = reader.GetInt32("order_item_id"),
                            order_id = reader.GetInt32("order_id"),
                            component_name = reader.GetString("component_name"),
                            quantity = reader.GetInt32("quantity"),
                            price = reader.GetInt32("price"),
                            category_name = reader.GetString("category_name"),
                            manufacturer_name = reader.GetString("manufacturer_name")

                        });



                    }


                }
                jsontoreturn = JsonSerializer.Serialize<List<AllOrderItems>>(OrderItems);
                Console.WriteLine(jsontoreturn);
                return jsontoreturn;
            }

        }
        static private string DeleteRowFromAdminDataGrid(string Data)
        {
            string[] subarr = Data.Split(':', 2);
            string ClassName = subarr[0];
            int idRowtoDelete = Convert.ToInt32(subarr[1]);
            Console.WriteLine($"Все удаление позиции для {localCustomer.Value.customer_name}");
            string query = "DELETE FROM `networkbase`.`{0}` WHERE (`{1}` = '{2}');";
            if(ClassName == "AllCustomers")
            {
                query = string.Format(query,"customers", "customer_id",idRowtoDelete);

            } else if(ClassName == "AllOrders")
            {
                query = string.Format(query, "orders", "order_id", idRowtoDelete);

            }else if(ClassName == "Selectedorder")
            {
                query = string.Format(query, "order_items", "order_item_id", idRowtoDelete);

            }
            Console.WriteLine(query);
            new MySqlCommand(query,connection).ExecuteNonQuery();
            return "<>";
        }
        static private string SendOrder_items_byID(string orderID)
        {
            string query = $@"
        SELECT oi.order_item_id, oi.order_id, c.component_name, oi.quantity, oi.price
        FROM networkbase.order_items oi
        JOIN networkbase.components c ON oi.component_id = c.component_id
        WHERE oi.order_id = {orderID};";
            string jsontoreturn;
            using (var command = new MySqlCommand(query, connection))
            {
                Console.WriteLine($"Все содержимое заказа клиента для {localCustomer.Value.customer_name}");
                List<Selectedorder> OrderItems = new List<Selectedorder>();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        OrderItems.Add(new Selectedorder
                        {
                            order_item_id = reader.GetInt32("order_item_id"),
                            order_id = reader.GetInt32("order_id"),
                            component_name = reader.GetString("component_name"),
                            quantity = reader.GetInt32("quantity"),
                            price = reader.GetInt32("price")


                        });



                    }


                }
                jsontoreturn = JsonSerializer.Serialize<List<Selectedorder>>(OrderItems);
                Console.WriteLine(jsontoreturn);
                return jsontoreturn;
            }

        }
        static private string SendAllOrders()
        {
            string query = "SELECT a.order_id, a.order_date, a.total_amount, a.customer_id, b.customer_name " +
                            "from orders a " +
                            "join customers b ON a.customer_id = b.customer_id; ";
            string jsontoreturn;
            using (var command = new MySqlCommand(query, connection))
            {
                Console.WriteLine($"Все заказы клиеетов для {localCustomer.Value.customer_name}");
                List<AllOrders> Orders = new List<AllOrders>();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Orders.Add(new AllOrders
                        {
                            order_id = reader.GetInt32("order_id"),
                            order_date = Convert.ToString(reader.GetDateTime("order_date")),
                            total_amount = reader.GetInt32("total_amount"),
                            customer_id = reader.GetInt32("customer_id"),
                            customer_name = reader.GetString("customer_name")


                        });



                    }


                }
                jsontoreturn = JsonSerializer.Serialize<List<AllOrders>>(Orders);
                return jsontoreturn;
            }


        }
        static private string SendAllCustomers()
        {
        string query = "SELECT * FROM networkbase.customers;";
            string jsontoreturn;
            using (var command = new MySqlCommand(query, connection))
            {
                Console.WriteLine($"Все клиенты для {localCustomer.Value.customer_name}");
                List<AllCustomers> customers = new List<AllCustomers>();
                using (var reader = command.ExecuteReader())
                { 
                while(reader.Read())
                    {
                        customers.Add(new AllCustomers
                        {
                            customer_id = reader.GetInt32("customer_id"),
                            phone_number = reader.GetString("phone_number"),
                            customer_name = reader.GetString("customer_name"),
                            role = reader.GetString("role")


                        });



                    }
                
                
                }
                jsontoreturn = JsonSerializer.Serialize<List<AllCustomers>>(customers); 
                return jsontoreturn;
            }


        }
        static private string AddComponent(string Data)
        {
            Component component = JsonSerializer.Deserialize<Component>(Data);
            string query = @"
        INSERT INTO components (component_name, price, category_id, manufacturer_id)
        VALUES (@ComponentName, @Price, 
                (SELECT category_id FROM categories WHERE category_name = @CategoryName),
                (SELECT manufacturer_id FROM manufacturers WHERE manufacturer_name = @ManufacturerName)
               );";

            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@ComponentName", component.component_name);
                command.Parameters.AddWithValue("@Price", component.price);
                command.Parameters.AddWithValue("@CategoryName", component.category_name);
                command.Parameters.AddWithValue("@ManufacturerName", component.manufacturer_name);

                // Выводим строку запроса с подставленными значениями
                string formattedQuery = query
                    .Replace("@ComponentName", $"'{component.component_name}'")
                    .Replace("@Price", component.price.ToString())
                    .Replace("@CategoryName", $"'{component.category_name}'")
                    .Replace("@ManufacturerName", $"'{component.manufacturer_name}'");

                Console.WriteLine(formattedQuery);  // Выводим строку в Console
                command.ExecuteNonQuery();
            }
            return "<>";
        }
        static private string DeleteCategory(string Data)
        {

            string query = "DELETE FROM `networkbase`.`categories` WHERE `category_name` = @categoryname;";

            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@categoryname", Data);

                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    Console.WriteLine($"Удалена категория {Data}");
                }
                else
                {
                    Console.WriteLine($"Категория {Data} не найден.");
                }
            }
            return "<>";
        }
        static private string DeleteManufacturer(string Data)
        {

            string query = "DELETE FROM `networkbase`.`manufacturers` WHERE `manufacturer_name` = @ManufacturerName;";

            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@ManufacturerName", Data);

                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    Console.WriteLine($"Удален производитель {Data}");
                }
                else
                {
                    Console.WriteLine($"Производитель {Data} не найден.");
                }
            }
            return "<>";
        }
        static private string SendCategories()
        {
            string stringresult = string.Empty;
            string query = "SELECT * FROM networkbase.categories;";
            using (var command = new MySqlCommand(query, connection))
            {
                Console.WriteLine($"Все категории для {localCustomer.Value.customer_name}");
                List<string> AllCategories = new List<string>();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        AllCategories.Add(reader.GetString("category_name"));

                    }
                    stringresult = JsonSerializer.Serialize<List<string>>(AllCategories);

                }

            }
            return stringresult;
        }
        static private string SendManufacturers()
        {
            string stringresult = string.Empty;
            string query = "SELECT * FROM networkbase.manufacturers;";
            using(var command = new MySqlCommand(query, connection))
            {
                Console.WriteLine($"Все производители для {localCustomer.Value.customer_name}");
                List<string> AllManufacturers = new List<string>(); 
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        AllManufacturers.Add(reader.GetString("manufacturer_name"));
                    
                    }
                    stringresult = JsonSerializer.Serialize<List<string>>(AllManufacturers);

                }

            }
            return stringresult;
        }
        static private string InsertCategory(string Data)
        {
            string query = "INSERT INTO `networkbase`.`categories` (`category_name`) VALUES (@category_name);";
            using (MySqlCommand cmd = new MySqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@category_name", Data);
                cmd.ExecuteNonQuery();
            }
            Console.WriteLine($"Добавлена категория: {Data}");
            return "<>";
        }
        static private string InsertManufacturer(string Data)
        {
            string query = "INSERT INTO `networkbase`.`manufacturers` (`manufacturer_name`) VALUES (@manufacturerName);";
            using (MySqlCommand cmd = new MySqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@manufacturerName", Data);
                cmd.ExecuteNonQuery();
            }
            Console.WriteLine($"Добавлен производитель: {Data}");
            return "<>";
        }

        static private string AddCustomerOrder(string JsonData)
        {
            
            List<CartBase> cart = JsonSerializer.Deserialize<List<CartBase>>(JsonData);
            int totalsum = 0;
            foreach (CartBase itemcart in cart)
            {
                totalsum += itemcart.position_price; 
            }

            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    
                    string orderQuery = "INSERT INTO orders (customer_id, order_date, total_amount) VALUES (@customer_id, NOW(), @total_amount);";
                    using (var orderCommand = new MySqlCommand(orderQuery, connection, transaction))
                    {
                        orderCommand.Parameters.AddWithValue("@customer_id", localCustomer.Value.customer_id);
                        orderCommand.Parameters.AddWithValue("@total_amount", totalsum);
                        orderCommand.ExecuteNonQuery();
                    }

            
                    int orderId = Convert.ToInt32(new MySqlCommand("SELECT LAST_INSERT_ID();", connection, transaction).ExecuteScalar());

                   
                    string orderItemsQuery = "INSERT INTO order_items (order_id, component_id, quantity, price) VALUES ";

                    List<string> valuesList = new List<string>();
                    foreach (var item in cart)
                    {
                        
                        string componentQuery = "SELECT component_id FROM components WHERE component_name = @component_name LIMIT 1;";
                        int componentId;
                        using (var componentCommand = new MySqlCommand(componentQuery, connection, transaction))
                        {
                            componentCommand.Parameters.AddWithValue("@component_name", item.component_name);
                            var result = componentCommand.ExecuteScalar();
                            if (result == null)
                            {
                                throw new Exception("Компонент не найден: " + item.component_name);
                            }
                            componentId = Convert.ToInt32(result);
                        }

                        
                        valuesList.Add($"({orderId}, {componentId}, {item.quantity}, {item.position_price})");
                    }

                    
                    orderItemsQuery += string.Join(", ", valuesList) + ";";
                    Console.WriteLine(orderItemsQuery);
                    
                    using (var orderItemsCommand = new MySqlCommand(orderItemsQuery, connection, transaction))
                    {
                        orderItemsCommand.ExecuteNonQuery();
                    }

                   
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                  
                    transaction.Rollback();
                    Console.WriteLine("Ошибка при добавлении заказа: " + ex.Message);
                }
            }
            return "AddedOrder";
        }
        static private string SendCatalogItemsforClient()
        {
            string stringresult = string.Empty;

            string query = $"SELECT " +
                            $"c.component_name, c.price, cat.category_name, m.manufacturer_name " +
                            $"FROM " +
                            $"components c " +
                            $"JOIN " +
                            $"categories cat ON c.category_id = cat.category_id " +
                            $"JOIN " +
                            $"manufacturers m ON c.manufacturer_id = m.manufacturer_id;";
            Console.WriteLine(query);
            using (var command = new MySqlCommand(query, connection))
            {
                Console.WriteLine($"Состав каталога для {localCustomer.Value.customer_name}");
                using (var reader = command.ExecuteReader())
                {
                    List<CatalogItems> AllOrders = new List<CatalogItems>();
                    while (reader.Read())
                    {
                        CatalogItems OneItem = new CatalogItems();
                        OneItem.component_name = reader.GetString("component_name");
                        OneItem.category_name = reader.GetString("category_name");
                        
                        OneItem.price = reader.GetInt32("price");
                        OneItem.manufacturer_name = reader.GetString("manufacturer_name");


                        AllOrders.Add(OneItem);
                    }
                    stringresult = JsonSerializer.Serialize<List<CatalogItems>>(AllOrders);
                }
            }
            return stringresult;
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
                Console.WriteLine("Полученный JSON: " + receivedJson);
                    
                    string response = CodeHandler(ref receivedJson);
                    //await SendMessageOverTcpAsync(tcpClient, response,stream);
                    byte[] data = Encoding.UTF8.GetBytes(response + "<END>");
                    await stream.WriteAsync(data, 0, data.Length);
                    Console.WriteLine($"Сообщение отправлено.{response}\n");
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

