using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
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
using Windows.Graphics.Printing.PrintTicket;
namespace UMU
{
    [DataContract]
    public class cartridge_log
    {
        [DataMember(Name ="id")]
        public int id { get; set; }
        [DataMember(Name ="model")]
        public string Модель { get; set; }
        [DataMember(Name ="serial")]
        public string Серия { get; set; }
        [DataMember(Name="adres")]
        public string Адрес { get; set; }
        [DataMember(Name ="date")]
        public string Дата { get; set; }
    }
    /// <summary>
    /// Логика взаимодействия для Upload.xaml
    /// </summary>
    public partial class Upload : Window
    {
        
        public Upload()
        {
            InitializeComponent();
            get_all_upload();
        }
        public List<cartridge_log> cartridges;
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            await get_all_upload();
        }

        private async Task get_all_upload()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    string reply = await client.GetStringAsync($"http://{Properties.Settings.Default.ip}:4433/upload/all");
                    byte[] JsonBytes = Encoding.UTF8.GetBytes(reply);
                    MemoryStream stream = new MemoryStream(JsonBytes);
                    {
                        DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(List<cartridge_log>));
                        cartridges = (List<cartridge_log>)jsonSerializer.ReadObject(stream);
                        Table_Uploads.ItemsSource = cartridges;
                        Cost.Content = cartridges.Count;
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Не получилось соединиться с сервером");
            }
        }


        private async void Button_Click_2(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MessageBox.Show("Вы действительно хотите очистить список уходящих картриджей?","Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    using (var client = new HttpClient())
                    {
                        var response = await client.DeleteAsync($"http://{Properties.Settings.Default.ip}:4433/upload/del");
                        if (response.IsSuccessStatusCode)
                        {
                            var builder = new ToastContentBuilder()
                           .AddArgument("meetingId", 9813)
                           .AddText("Уведомление", hintMaxLines: 1)
                           .AddText("База очищена!")
                           ;
                            builder.Show();
                            Button_Click(sender, e);
                        }
                    }
                }
            }
            catch (Exception error)
            {
                MessageBox.Show(error.ToString());

            }
        }

        private async void Window_KeyDown(object sender, KeyEventArgs e)
        {
            
        }

        private async void Table_Uploads_KeyDown(object sender, KeyEventArgs e)
        {
            
        }

        private async void Table_Uploads_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                if (Table_Uploads.SelectedItem != null)
                {
                    cartridge_log cartridge_ = (cartridge_log)Table_Uploads.SelectedItem;
                    try
                    {
                        using (var client = new HttpClient())
                        {
                            var response = await client.DeleteAsync($"http://{Properties.Settings.Default.ip}:4433/upload/delid?id={cartridge_.id}");
                            if (response.IsSuccessStatusCode)
                            {
                                var builder = new ToastContentBuilder()
                               .AddArgument("meetingId", 9813)
                               .AddText("Уведомление", hintMaxLines: 1)
                               .AddText("Запись удалена!")
                               ;
                                builder.Show();
                            }
                            Button_Click(sender, e);
                        }
                    }
                    catch (Exception error)
                    {
                        MessageBox.Show(error.ToString());

                    }
                }
            }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            CreateExcel excel = new CreateExcel();
            excel.SetTitle();
            excel.SetContent(cartridges);
            excel.SetLastContribution();
            excel.SaveTable();
        }
    }
}
