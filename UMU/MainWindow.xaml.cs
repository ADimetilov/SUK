using System;
using System.Collections.Generic;
using System.IO.Ports;
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
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;
using System.Net.Http;
using System.IO;
using System.Threading;

namespace UMU
{
    [DataContract]
    public class Cartridge
    {
        [DataMember(Name="id")]
        public int id { get; set; }
        [DataMember(Name ="name")]
        public string name { get; set; }
        [DataMember(Name = "value")]
        public int value { get; set; }
    }
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public bool cancel = false;
        public MainWindow()
        {
            InitializeComponent();
            get_all_cartidge();
        }
        public async void get_all_cartidge()
        {
            try
            {
                using (var client = new HttpClient()){
                    string reply = await client.GetStringAsync($"http://{Properties.Settings.Default.ip}:4433/cart/all");
                    byte[] JsonBytes = Encoding.UTF8.GetBytes(reply);
                    MemoryStream stream = new MemoryStream(JsonBytes);
                    {
                        DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(List<Cartridge>));
                        List<Cartridge> cartridges = (List<Cartridge>)jsonSerializer.ReadObject(stream);
                        cart_list.ItemsSource = cartridges;
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Не получилось соединиться с сервером");
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Plus plus = new Plus(1,this);
            plus.Show();
            get_all_cartidge();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                cancel = true;
                MessageBox.Show("Esc");
            }
            if (e.Key == Key.Enter)
            {
                MessageBox.Show("Enter");
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Plus plus = new Plus(-1,this);
            plus.ShowDialog();
            get_all_cartidge();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            set_server server = new set_server();
            server.ShowDialog();
            get_all_cartidge();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            get_all_cartidge();
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {

        }

    }
}
