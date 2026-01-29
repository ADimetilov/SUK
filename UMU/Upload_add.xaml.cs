using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
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
using Windows.Devices.Usb;


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
        public class Post
        {
            public int id { get; set; }
            public int value { get; set; }
            public string adres { get; set; }
            public string serial { get; set; }
            public int dram { get; set; }
        }
        public Upload_add(MainWindow window)
        {
            InitializeComponent();
            if (window.DramButton.IsChecked == true) is_dram = 1;
            Number.Focus();
        }
        private async void Window_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter)
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
                        else
                        {
                            serial = false;
                            await Insert();
                        }
                    }
                    else await Insert();

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
                        id = Convert.ToInt32(Number.Text.Substring(Number.Text.Length - 2, 2)),
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
            Message.Text = "Ожидаю код";
        }
    }
}
