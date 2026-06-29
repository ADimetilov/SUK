using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.Remoting.Channels;
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
    /// Логика взаимодействия для set_server.xaml
    /// </summary>
    public partial class set_server : Window
    {
        public set_server()
        {
            InitializeComponent();
            ip_box.Text = Properties.Settings.Default.ip;
            adres_box.Text = Properties.Settings.Default.adres;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync($"http://{ip_box.Text}:4433/status");
                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Соединение установлено");
                        Properties.Settings.Default.ip = ip_box.Text;
                        Properties.Settings.Default.Save();
                    }
                    else MessageBox.Show("Соединение не установлено");
                }
            }
            catch (Exception)
            {

                
            }
        }

        private void Сохранить_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.adres = adres_box.Text;
            Properties.Settings.Default.Save();
        }
    }
}
