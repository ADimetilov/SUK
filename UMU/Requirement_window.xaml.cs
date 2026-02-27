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
using Windows.Networking.BackgroundTransfer;
using static UMU.Cartridge;
namespace UMU
{
    [DataContract]
    public class Requirement
    {
        [DataMember(Name = "id")]
        public int id { get; set; }
        [DataMember(Name ="name")]
        public string name { get; set; }
        [DataMember(Name = "score")]
        public int score { get; set; }
    }

    public class Requirement_create
    {
        public int id_model { get; set; }
        public int score { get; set; }
    }
    public class Requirement_edit
    {
        public int id { get; set; }
        public int score { get; set; }
    }
    /// <summary>
    /// Логика взаимодействия для Requirement_window.xaml
    /// </summary>
    public partial class Requirement_window : Window
    {
        private int id_model_selected;
        public async Task get_all_modell()
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

        public async void set_new_requirement(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    Requirement_create requirement = new Requirement_create
                    {
                        id_model = Convert.ToInt32(id_model_selected),
                        score = Convert.ToInt32(ScoreBox.Text)
                    };
                    JsonContent content = JsonContent.Create(requirement);
                    var response = await client.PostAsync($"http://{Properties.Settings.Default.ip}:4433/requirement/add", content);
                    await get_all_modell();
                    await get_all_requirement();
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Не получилось соединиться с сервером");
            }
        }

        public async Task get_all_requirement()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    string reply = await client.GetStringAsync($"http://{Properties.Settings.Default.ip}:4433/requirement/all");
                    byte[] JsonBytes = Encoding.UTF8.GetBytes(reply);
                    MemoryStream stream = new MemoryStream(JsonBytes);
                    {
                        DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(List<Requirement>));
                        List<Requirement> requirement_list = (List<Requirement>)jsonSerializer.ReadObject(stream);
                        RequirementGrid.ItemsSource = requirement_list;
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Не получилось соединиться с сервером");
            }
        }

        public Requirement_window()
        {
            InitializeComponent();
            getapilist();
        }

        public async void getapilist()
        {
            await get_all_modell();
            await get_all_requirement();
        }

        private void Model_list_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Model_list.SelectedIndex != -1)
            {
                Model_class model = new Model_class();
                model = (Model_class)Model_list.SelectedItem;
                ModelBlock.Text = model.name;
                id_model_selected = Convert.ToInt32(model.id);
            }
        }

        private async void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            int id = ((Requirement)RequirementGrid.SelectedItem).id;
            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.DeleteAsync($"http://{Properties.Settings.Default.ip}:4433/requirement/delete?id={id}");
                    await get_all_modell();
                    await get_all_requirement();
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Не получилось соединиться с сервером");
            }
        }

        private async void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            if (RequirementGrid.SelectedIndex != -1)
            {
                try
                {

                    Requirement_edit requirement = new Requirement_edit
                    {
                        id = ((Requirement)RequirementGrid.SelectedItem).id,
                        score = ((Requirement)RequirementGrid.SelectedItem).score
                    };
                    JsonContent content = JsonContent.Create(requirement);
                    using (var client = new HttpClient())
                    {
                        var response = await client.PutAsync($"http://{Properties.Settings.Default.ip}:4433/requirement/edit", content);
                        await get_all_modell();
                        await get_all_requirement();
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Не получилось соединиться с сервером");
                }
            }
        }

        private async void RequirementGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            
            if (RequirementGrid.SelectedIndex != -1)
            {
                Requirement requirement = e.Row.Item as Requirement;
                try
                {

                    Requirement_edit requirement_edit = new Requirement_edit
                    {
                        id = requirement.id,
                        score = requirement.score
                    };
                    JsonContent content = JsonContent.Create(requirement);
                    using (var client = new HttpClient())
                    {
                        var response = await client.PutAsync($"http://{Properties.Settings.Default.ip}:4433/requirement/edit", content);
                        await get_all_modell();
                        await get_all_requirement();
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Не получилось соединиться с сервером");
                }
            }
        }
    }
}
