using Microsoft.Office.Core;
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

namespace UMU
{
    /// <summary>
    /// Логика взаимодействия для Cartridge.xaml
    /// </summary>
    public partial class Cartridge: Window
    {
        public object selecteditem;
        public class Cartridge_add
        {
            public string name { get; set; }
        }
        public class Cartridge_edit
        {
            public int id { get; set; }
            public string name { get; set; }
        }
        public Cartridge()
        {
            InitializeComponent();
            get_all_cartidge();
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
                        List<Cartridge_class> Cartridge_classs = (List<Cartridge_class>)jsonSerializer.ReadObject(stream);
                        cart_list.ItemsSource = Cartridge_classs;
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Не получилось соединиться с сервером");
            }
        }

        private async void Delete_button_Click(object sender, RoutedEventArgs e)
        {
            if (cart_list.SelectedIndex != -1)
            {
                Cartridge_class cartridge = (Cartridge_class)cart_list.SelectedItem;
                try
                {
                    using (var client = new HttpClient())
                    {
                        int id = Convert.ToInt32(cartridge.id);
                        await client.DeleteAsync($"http://{Properties.Settings.Default.ip}:4433/cart/del?id={id}");
                        MessageBox.Show("Имя успешно удалено");
                        get_all_cartidge();
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Не получилось соединиться с сервером");
                }
            }
            else
            {
                MessageBox.Show("Выберите модель картриджжа из списка");
            }
            
        }

        private async void Add_button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    Cartridge_add cartridge = new Cartridge_add();
                    cartridge.name = NameAdd_Box.Text;
                    JsonContent content = JsonContent.Create(cartridge);
                    var response = await client.PostAsync($"http://{Properties.Settings.Default.ip}:4433/cart/add", content);
                    if (response.IsSuccessStatusCode)
                    {
                        var builder = new ToastContentBuilder()
                       .AddArgument("meetingId", 9813)
                       .AddText("Уведомление", hintMaxLines: 1)
                       .AddText("Картридж добавлен!")
                       ;
                        builder.Show();
                    }
                    get_all_cartidge();
                    NameAdd_Box.Text = "";
                }
            }
            catch (Exception error)
            {
                MessageBox.Show(error.ToString());

            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (cart_list.SelectedIndex != -1)
            {
                try
                {
                    using (var client = new HttpClient())
                    {
                        Cartridge_class cartridge_selected = (Cartridge_class)selecteditem;
                        int id = Convert.ToInt32(cartridge_selected.id);
                        Cartridge_edit cartridge = new Cartridge_edit();
                        cartridge.name = NameAdd_Box.Text;
                        cartridge.id = id;
                        JsonContent content = JsonContent.Create(cartridge);
                        var response = await client.PutAsync($"http://{Properties.Settings.Default.ip}:4433/cart/edit", content);
                        if (response.IsSuccessStatusCode)
                        {
                            var builder = new ToastContentBuilder()
                           .AddArgument("meetingId", 9813)
                           .AddText("Уведомление", hintMaxLines: 1)
                           .AddText("Картридж изменен!")
                           ;
                            builder.Show();
                        }
                        get_all_cartidge();
                        NameAdd_Box.Text = "";
                    }
                }
                catch (Exception error)
                {
                    MessageBox.Show(error.ToString());

                }
            }
            else
            {
                MessageBox.Show("Выберите модель картриджжа из списка");
            }
        }

        private void cart_list_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cart_list.SelectedIndex != -1)
            {
                selecteditem = cart_list.SelectedItem;
                NameAdd_Box.Text = ((Cartridge_class)selecteditem).name;
            }
            
        }
    }
}
