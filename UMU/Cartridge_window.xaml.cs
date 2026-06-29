using Microsoft.Office.Core;
using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
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
using Windows.Media.Protection.PlayReady;

namespace UMU
{
    /// <summary>
    /// Логика взаимодействия для Cartridge.xaml
    /// </summary>
    public partial class Cartridge: Window
    {
        public object selecteditem;
        public object selectedmodel;
        public class Cartridge_add
        {
            public string name { get; set; }
        }
        public class Model_add
        {
            public string name { get; set; }
        }
        public class Model_link
        {
            public int id_cart { get; set; }
            public List<int> id_model { get; set; }
        }
        public class Cartridge_edit
        {
            public int id { get; set; }
            public string name { get; set; }
        }
        public class Model_edit
        {
            public int id { get; set; }
            public string name { get; set; }
        }
        public class Model_class_box
        {
            public int id { get; set; }
            public string name { get; set; }
        }
        [DataContract]
        public class Model_class
        {
            [DataMember(Name = "id")]
            public int id { get; set; }
            [DataMember(Name = "model_id")]
            public int model_id { get; set; }
            [DataMember(Name = "name")]
            public string name { get; set; }
        }
        public Cartridge()
        {
            InitializeComponent();
            get_all_cartidge();
            
        }
        public async void get_all_model()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    string reply = await client.GetStringAsync($"http://{Properties.Settings.Default.ip}:4433/model/list");
                    byte[] JsonBytes = Encoding.UTF8.GetBytes(reply);
                    MemoryStream stream = new MemoryStream(JsonBytes);
                    {
                        DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(List<Model_class>));
                        List<Model_class> model_list = (List<Model_class>)jsonSerializer.ReadObject(stream);
                        Model_list.ItemsSource = model_list;
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Не получилось соединиться с сервером");
            }
        }
        public async void get_model_for_cart()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    string reply = await client.GetStringAsync($"http://{Properties.Settings.Default.ip}:4433/model/get?id={((Cartridge_class)LinkListCart.Items[0]).id}");
                    byte[] JsonBytes = Encoding.UTF8.GetBytes(reply);
                    MemoryStream stream = new MemoryStream(JsonBytes);
                    {
                        DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(List<Model_class>));
                        List<Model_class> model_list = (List<Model_class>)jsonSerializer.ReadObject(stream);
                        LinkListModel.Items.Clear();
                        foreach (Model_class model in model_list)
                        {
                            LinkListModel.Items.Add(model);
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
                        List<Cartridge_class> Cartridge_classs = (List<Cartridge_class>)jsonSerializer.ReadObject(stream);
                        cart_list.ItemsSource = Cartridge_classs;
                    }
                }
                get_all_model();
            }
            catch (Exception)
            {
                MessageBox.Show("Не получилось соединиться с сервером");
            }
        }

        private async void Delete_button_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы действительно хатите удалить этот картридж? Будут удалены также все связи с этим картриджем.", "Удаление картриджа", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
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
                                MessageBox.Show("Картридж успешно удален");
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

        private void LinkCartAdd_Click(object sender, RoutedEventArgs e)
        {
            if (LinkListCart.Items.Count < 1 && cart_list.SelectedIndex!=-1)
            {
                LinkListCart.Items.Add(cart_list.SelectedItem);
                get_model_for_cart();
            }
            else if (LinkListCart.Items.Count == 1)
            {
                MessageBox.Show("Нельзя добавлять более 1 картриджа в связи!","Ошибка добавления",MessageBoxButton.OK,MessageBoxImage.Error);
            }
        }

        private void LinkCartDel_Click(object sender, RoutedEventArgs e)
        {
            if (LinkListCart.SelectedIndex != -1)
            {
                LinkListCart.Items.Remove(LinkListCart.SelectedItem);
                LinkListModel.Items.Clear();
            }
        }

        private async void edit_model_Click(object sender, RoutedEventArgs e)
        {
            if (Model_list.SelectedIndex != -1)
            {
                try
                {
                    using (var client = new HttpClient())
                    {
                        Model_class model_selected = (Model_class)selectedmodel;
                        int id = Convert.ToInt32(model_selected.id);
                        Model_edit model = new Model_edit();
                        model.name = NameAdd_model_box.Text;
                        model.id = id;
                        JsonContent content = JsonContent.Create(model);
                        var response = await client.PutAsync($"http://{Properties.Settings.Default.ip}:4433/model/edit", content);
                        if (response.IsSuccessStatusCode)
                        {
                            var builder = new ToastContentBuilder()
                           .AddArgument("meetingId", 9813)
                           .AddText("Уведомление", hintMaxLines: 1)
                           .AddText("Модель изменена!")
                           ;
                            builder.Show();
                        }
                        get_all_model();
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
                MessageBox.Show("Выберите модель из списка");
            }
        }

        private void Model_list_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Model_list.SelectedIndex != -1)
            {
                selectedmodel = Model_list.SelectedItem;
                NameAdd_model_box.Text = ((Model_class)selectedmodel).name;
            }
        }

        private async void add_model_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    Model_add model = new Model_add();
                    model.name = NameAdd_model_box.Text;
                    JsonContent content = JsonContent.Create(model);
                    var response = await client.PostAsync($"http://{Properties.Settings.Default.ip}:4433/model/create", content);
                    if (response.IsSuccessStatusCode)
                    {
                        var builder = new ToastContentBuilder()
                       .AddArgument("meetingId", 9813)
                       .AddText("Уведомление", hintMaxLines: 1)
                       .AddText("Модель добавлена!")
                       ;
                        builder.Show();
                    }
                    get_all_model();
                    NameAdd_Box.Text = "";
                }
            }
            catch (Exception error)
            {
                MessageBox.Show(error.ToString());
            }
        }

        private async void delete_model_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы действительно хатите удалить модель? Будут удалены также все связи с этой моделью.","Удаление модели",MessageBoxButton.YesNo,MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                if (Model_list.SelectedIndex != -1)
                {
                    Model_class model = (Model_class)Model_list.SelectedItem;
                    try
                    {
                        using (var client = new HttpClient())
                        {
                            int id = Convert.ToInt32(model.id);
                            await client.DeleteAsync($"http://{Properties.Settings.Default.ip}:4433/model/delete?id={id}");
                            MessageBox.Show("Модель успешно удалена");
                            get_all_model();
                        }
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Не получилось соединиться с сервером");
                    }
                }
                else
                {
                    MessageBox.Show("Выберите модель из списка");
                }
            }
        }

        private void LinkModelAdd_Click(object sender, RoutedEventArgs e)
        {
            if (Model_list.SelectedIndex != -1)
            {
                LinkListModel.Items.Add(Model_list.SelectedItem);
            }
        }

        private void LinkModelDel_Click(object sender, RoutedEventArgs e)
        {
            if (LinkListModel.SelectedIndex > -1)
            {
                LinkListModel.Items.Remove(LinkListModel.SelectedItem);
            }
            else
            {
                MessageBox.Show(LinkListModel.SelectedIndex.ToString());
            }
        }

        private async Task DeleteAllLink(HttpClient client,int id)
        {
            await client.DeleteAsync($"http://{Properties.Settings.Default.ip}:4433/model/unlink?id={id}");
        }

        private async void SubmitChangeButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (LinkListModel.Items.Count >= 1)
                {
                    Model_link link = new Model_link();
                    link.id_model = new List<int>();
                    LinkListModel.Items.Refresh();
                    foreach (Model_class item in LinkListModel.Items)
                    {
                        link.id_model.Add(item.model_id);
                    }
                    link.id_cart = ((Cartridge_class)LinkListCart.Items[0]).id;
                    using (var client = new HttpClient())
                    {
                        JsonContent content = JsonContent.Create(link);
                        await DeleteAllLink(client, link.id_cart);
                        var response = await client.PostAsync($"http://{Properties.Settings.Default.ip}:4433/model/link", content);
                        if (response.IsSuccessStatusCode)
                        {
                            var builder = new ToastContentBuilder()
                           .AddArgument("meetingId", 9813)
                           .AddText("Уведомление", hintMaxLines: 1)
                           .AddText("Связано успешно!")
                           ;
                            builder.Show();
                        }
                        get_all_model();
                    }
                }
                else
                {
                    using (var client = new HttpClient())
                    {
                        int id_cart = ((Cartridge_class)LinkListCart.Items[0]).id;
                        await DeleteAllLink(client, id_cart);
                    }
                }
            }
            catch (Exception Error)
            {
                MessageBox.Show(Error.ToString());
            }
        }
    }
}
