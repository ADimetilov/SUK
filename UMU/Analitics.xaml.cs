using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
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
    /// Логика взаимодействия для Analitics.xaml
    /// </summary>
    public partial class Analitics : Window
    {
        public Analitics()
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
                        ModelComboBox.ItemsSource = Cartridge_classs;
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Не получилось соединиться с сервером");
            }
        }
        private async void GiveAnalitics_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    if (IsAllModelChecker.IsChecked == false)
                    {
                        if (ModelComboBox.SelectedIndex == -1)
                        {
                            MessageBox.Show("Выберите модель картриджа или поставьте галочку для общей статистики");
                            return;
                        }
                        try
                        {
                            DateStartPicker.SelectedDate.Value.Date.ToString("yyyy-MM-dd");
                            DateEndPicker.SelectedDate.Value.Date.ToString("yyyy-MM-dd");
                        }
                        catch
                        {
                            MessageBox.Show("Выбран некорректный период");
                            return;
                        }
                        string reply_get = await client.GetStringAsync($"http://{Properties.Settings.Default.ip}:4433/anal/get?id={ModelComboBox.SelectedValue}&date1={DateStartPicker.SelectedDate.Value.Date.ToString("yyyy-MM-dd")}&date2={DateEndPicker.SelectedDate.Value.Date.ToString("yyyy-MM-dd")}");
                        string reply_post = await client.GetStringAsync($"http://{Properties.Settings.Default.ip}:4433/anal/post?id={ModelComboBox.SelectedValue}&date1={DateStartPicker.SelectedDate.Value.Date.ToString("yyyy-MM-dd")}&date2={DateEndPicker.SelectedDate.Value.Date.ToString("yyyy-MM-dd")}");
                        get_count.Content = reply_get;
                        post_count.Content = reply_post;
                        count.Content = Convert.ToInt32(reply_get) - Convert.ToInt32(reply_post);
                    }
                    else
                    {
                        string reply_get = await client.GetStringAsync($"http://{Properties.Settings.Default.ip}:4433/anal/get?id=112200&date1={DateStartPicker.SelectedDate.Value.Date.ToString("yyyy-MM-dd")}&date2={DateEndPicker.SelectedDate.Value.Date.ToString("yyyy-MM-dd")}");
                        string reply_post = await client.GetStringAsync($"http://{Properties.Settings.Default.ip}:4433/anal/post?id=112200&date1={DateStartPicker.SelectedDate.Value.Date.ToString("yyyy-MM-dd")}&date2={DateEndPicker.SelectedDate.Value.Date.ToString("yyyy-MM-dd")}");
                        get_count.Content = reply_get;
                        post_count.Content = reply_post;
                        count.Content = Convert.ToInt32(reply_get) - Convert.ToInt32(reply_post);
                    }
                    
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Не получилось соединиться с сервером");
            }
        }
    }
}
