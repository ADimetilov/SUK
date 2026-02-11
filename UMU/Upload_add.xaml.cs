using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Windows.Devices.Sensors;
using Windows.Devices.Usb;
using static UMU.Cartridge;


namespace UMU
{
    /// <summary>
    /// Логика взаимодействия для Upload_add.xaml
    /// </summary>
    public partial class Upload_add : Window
    {
        public int value_pub = -1;
        public bool serial = false;
        public int is_dram = 0;
        public bool is_number = false;
        private int count_model;
        private string model_name;
        private cartridge_model currentmodel;
        public List<Cartridge_class> Cartridge_classs;
        public List<cartridge_model> list = new List<cartridge_model>();
        public class Post
        {
            public string model { get; set; }
            public int value { get; set; }
            public string adres { get; set; }
            public string serial { get; set; }
            public int dram { get; set; }
        }
        public class cartridge_model
        {
            public int cart_id;
            public string cart_name;
            public List<string> model_name = new List<string>();
        }
        public class template_item
        {
            public string number { get; set; }
            public string model { get; set; }
        }
        public Upload_add(MainWindow window)
        {
            InitializeComponent();
            if (window.DramButton.IsChecked == true) is_dram = 1;
            get_all_cartidge();
            Number.Focus();
        }
        public async void get_model_for_cart(List<cartridge_model> cartridge)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    foreach(cartridge_model cart in cartridge)
                    {
                        string reply = await client.GetStringAsync($"http://{Properties.Settings.Default.ip}:4433/model/get?id={cart.cart_id}");
                        byte[] JsonBytes = Encoding.UTF8.GetBytes(reply);
                        MemoryStream stream = new MemoryStream(JsonBytes);
                        {
                            DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(List<Model_class>));
                            List<Model_class> model_list = (List<Model_class>)jsonSerializer.ReadObject(stream);
                            foreach(Model_class model in model_list)
                            {
                                cart.model_name.Add(model.name);
                            }
                        }
                    }
                    
                }
            }
            catch (Exception Error)
            {
                MessageBox.Show(Error.ToString());
            }
        }

        public async void get_all_cartidge()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    string reply = await client.GetStringAsync($"http://{Properties.Settings.Default.ip}:4433/cart/all");
                    byte[] JsonBytes = Encoding.UTF8.GetBytes(reply);
                    MemoryStream stream = new MemoryStream(JsonBytes);
                    {
                        DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(List<Cartridge_class>));
                        Cartridge_classs = (List<Cartridge_class>)jsonSerializer.ReadObject(stream);
                        foreach (Cartridge_class cart in Cartridge_classs)
                        {
                            cartridge_model cartridge = new cartridge_model();
                            cartridge.cart_id = cart.id;
                            cartridge.cart_name = cart.name;
                            list.Add(cartridge);
                        }
                        get_model_for_cart(list);
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Не получилось соединиться с сервером");
            }
        }
        private async void Window_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter && is_number==false)
                {
                    if (value_pub == -1)
                    {
                        if (serial != true)
                        {
                            Message.Text = "Ожидаю серийный номер";
                            SerialBox.Focus();
                            SerialBox.Text = "";
                            serial = true;
                        }
                        else if (serial == true && !is_number)
                        {
                            try
                            {
                                int id = Convert.ToInt32(Number.Text.Substring(Number.Text.Length - 2, 2));
                                foreach (cartridge_model model in list)
                                {
                                    if (model.cart_id == id && model.model_name.Count > 1)
                                    {
                                        int count = 1;
                                        is_number = true;
                                        currentmodel = model;
                                        count_model = model.model_name.Count;
                                        foreach (string name in model.model_name)
                                        {
                                            template_item item = new template_item();
                                            item.number = count.ToString();
                                            item.model = name;
                                            NumberBox.Items.Add(item);
                                            count += 1;
                                        }
                                        Message.Text = "Ожидаю дополнительную клавишу";
                                        return;
                                    }
                                    else if (model.cart_id == id && model.model_name.Count == 1)
                                    {
                                        model_name = model.model_name[0];
                                    }
                                    else if (model.cart_id == id && model.model_name.Count < 1)
                                    {
                                        MessageBox.Show("Введен не верный код модели");
                                        Number.Focus();
                                    }
                                }
                                await Insert();
                            }
                            catch (Exception error)
                            {

                            }
                        }
                    }
                    else await Insert();
                }
                else if (is_number)
                {
                    int index = Convert.ToInt32(e.Key.ToString().ToUpper().Substring(1));
                    if (index <= count_model)
                    {
                        model_name = currentmodel.model_name[index-1];
                        is_number = false;
                        serial = false;
                        await Insert();
                    }
                }
                else if (e.Key == Key.Escape) this.Close();
            }
            catch (Exception error)
            {
                MessageBox.Show("Произошла ошибка!" + error.ToString());
            }

        }

        public async Task Insert()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    Post post = new Post()
                    {
                        model = model_name,
                        value = value_pub,
                        serial = SerialBox.Text,
                        adres = Properties.Settings.Default.adres,
                        dram = is_dram
                    };
                    JsonContent content = JsonContent.Create(post);
                    var response = await client.PostAsync($"http://{Properties.Settings.Default.ip}:4433/upload/add", content);
                    if (response.IsSuccessStatusCode)
                    {
                        var builder = new ToastContentBuilder()
                       .AddArgument("meetingId", 9813)
                       .AddText("Уведомление", hintMaxLines: 1)
                       .AddText("Изменения приняты!")
                       ;
                        builder.Show();
                    }
                    Number.Focus();
                }
            }
            catch (Exception error)
            {
                MessageBox.Show(error.ToString());

            }
        }

        private void Window_GotFocus(object sender, RoutedEventArgs e)
        {
        }

        private void Number_GotFocus(object sender, RoutedEventArgs e)
        {
            Number.Text = "";
            NumberBox.Items.Clear();
            Message.Text = "Ожидаю код";
            serial = false;
        }
    }
}
