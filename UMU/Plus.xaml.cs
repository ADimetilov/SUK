using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
using Microsoft.Toolkit.Uwp.Notifications;
using System.Net.Http.Json;
namespace UMU
{
    /// <summary>
    /// Логика взаимодействия для Plus.xaml
    /// </summary>
    public partial class Plus : Window
    {
        public int value_pub;
        public MainWindow prev_window;
        public class Post
        {
            public int id { get; set; }
            public int value { get; set; }
        }
        public Plus(int value, MainWindow mainWindow)
        {
            prev_window = mainWindow;
            InitializeComponent();
            Number.Focus();
            if (value == 1) title.Content = "ПРИЁМ";
            else title.Content = "ОТПРАВКА";
            value_pub = value;
        }

        private async void Window_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter)
                {
                    await Insert();
                }
                else if (e.Key == Key.Escape) this.Close();
            }
            catch (Exception error)
            {
                MessageBox.Show("Произошла ошибка!"+error.ToString());
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
                        id = Convert.ToInt32(Number.Text.Substring(Number.Text.Length-2,2)),
                        value = value_pub
                    };
                    JsonContent content = JsonContent.Create(post);
                    var response = await client.PostAsync($"http://{Properties.Settings.Default.ip}:4433/cart/change", content);
                    if (response.IsSuccessStatusCode)
                    {
                         var builder = new ToastContentBuilder()
                        .AddArgument("meetingId", 9813)
                        .AddText("Уведомление", hintMaxLines: 1)
                        .AddText("Успешно отправлено!")
                        ;
                        builder.Show();
                        prev_window.get_all_cartidge();
                    }
                }
            }
            catch (Exception error)
            {
                MessageBox.Show(error.ToString());

            }
        }

        private void Window_GotFocus(object sender, RoutedEventArgs e)
        {
            Number.Focus();
            Number.Text = "";
        }
    }
}
