using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Malbile.Common;
using Malbile.Model;
using Malbile.Context;
using System.Xml.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Net.Http;

namespace Malbile
{
    public partial class AnimeDetailPage : PhoneApplicationPage
    {

        public static String URL = "http://myanimelist.net/api/";

        public List<String> m_Status = new List<String>() { "Watching", "Completed", "On-Hold", "Dropped", "Plan To Watch" };
        public List<String> Status
        {
            get
            {
                return m_Status;
            }
        }

        public AnimeDetailPage()
        {
            InitializeComponent();

            listPickerStatus.ItemsSource = Status;
        }

        public Anime AnimeModel { get; set; }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (NavigationContext.QueryString.ContainsKey("id"))
            {
                string id = NavigationContext.QueryString["id"];
                AnimeModel = App.ViewModel.GetAnimeById(long.Parse(id));

                EnableBarButton("SAVE");
            }
            else
            {
                var an = App.ViewModel.GetAnimeById(App.AnimeSearch.ID);
                if (an != null)
                {
                    AnimeModel = an;
                    EnableBarButton("SAVE");
                }
                else
                {
                    AnimeModel = App.AnimeSearch;
                    EnableBarButton("ADD");
                }
            }
            DataContext = AnimeModel;

            SetPickerStatus();
            ratingScore.Value = this.AnimeModel.MyScore;
        }

        public void EnableBarButton(string type)
        {
            var btnAdd = (ApplicationBarIconButton)this.ApplicationBar.Buttons[0];
            var btnSave = (ApplicationBarIconButton)this.ApplicationBar.Buttons[1];
            var btnDelete = (ApplicationBarIconButton)this.ApplicationBar.Buttons[2];

            if (type.Equals("SAVE"))
            {
                btnAdd.IsEnabled = false;
                btnSave.IsEnabled = true;
                btnDelete.IsEnabled = true;
            }
            else
            {
                btnAdd.IsEnabled = true;
                btnSave.IsEnabled = false;
                btnDelete.IsEnabled = false;
            }
        }

        public void SetPickerStatus()
        {
            switch (AnimeModel.MyStatus)
            {
                case 1: listPickerStatus.SelectedItem = "Watching"; break;
                case 2: listPickerStatus.SelectedItem = "Completed"; break;
                case 3: listPickerStatus.SelectedItem = "On-Hold"; break;
                case 4: listPickerStatus.SelectedItem = "Dropped"; break;
                case 6: listPickerStatus.SelectedItem = "Plan To Watch"; break;
                default: listPickerStatus.SelectedItem = "Watching"; break;
            }
        }

        public int GetPickerStatus()
        {
            switch (listPickerStatus.SelectedItem.ToString())
            {
                case "Watching": return 1;
                case "Completed": return 2;
                case "On-Hold": return 3;
                case "Dropped": return 4;
                case "Plan To Watch": return 6;
                default: return 1;
            }
        }

        private void btnPlusEpisode_Click(object sender, RoutedEventArgs e)
        {
            this.AnimeModel.MyWatchedEpisodes++;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            this.AnimeModel.MyStatus = GetPickerStatus();
            this.AnimeModel.MyScore = (int)ratingScore.Value;
            this.AnimeModel.LastUpdate = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;

            var result = RequestUpdateAnime();
        }

        public async Task RequestAddAnime()
        {
            string username, password;
            LoadingBar.IsEnabled = true;
            LoadingBar.Visibility = Visibility.Visible;
            try
            {
                if (AppContext.TryGetSetting<string>("xUser", out username) && AppContext.TryGetSetting<string>("pUser", out password))
                {
                    string xml;
                    xml = string.Format("<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                    @"<entry>
                        <episode>{0}</episode>
                        <status>{1}</status>
                        <score>{2}</score>
                        <downloaded_episodes></downloaded_episodes>
                        <storage_type></storage_type>
                        <storage_value></storage_value>
                        <times_rewatched></times_rewatched>
                        <rewatch_value></rewatch_value>
                        <date_start></date_start>
                        <date_finish></date_finish>
                        <priority></priority>
                        <enable_discussion></enable_discussion>
                        <enable_rewatching></enable_rewatching>
                        <comments></comments>
                        <fansub_group></fansub_group>
                        <tags></tags>
                    </entry>", this.AnimeModel.MyWatchedEpisodes.ToString(), this.AnimeModel.MyStatus.ToString(), this.AnimeModel.MyScore.ToString());

                    using (var handler = new HttpClientHandler { Credentials = new NetworkCredential(username, password) })
                    {
                        using (var client = new HttpClient(handler))
                        {
                            client.BaseAddress = new Uri(URL);
                            var content = new FormUrlEncodedContent(new[] { new KeyValuePair<string, string>("data", xml) });
                            var response = await client.PostAsync("animelist/add/" + this.AnimeModel.ID.ToString() + ".xml", content);
                        }
                    }

                    using (var db = new DBContext())
                    {
                        db.Animes.InsertOnSubmit(this.AnimeModel);
                        db.SubmitChanges();
                    }

                    App.ViewModel.InsertAnimeCollection(this.AnimeModel);

                    EnableBarButton("SAVE");

                    MessageBoxResult result = MessageBox.Show("Successful Insert!", "Success!", MessageBoxButton.OK);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Source);
                MessageBoxResult result = MessageBox.Show("Ocorreu um problema para adicionar o Anime.", "Erro!", MessageBoxButton.OK);
            }
            LoadingBar.IsEnabled = false;
            LoadingBar.Visibility = Visibility.Collapsed;
        }

        public async Task RequestUpdateAnime()
        {
            string username, password;
            LoadingBar.IsEnabled = true;
            LoadingBar.Visibility = Visibility.Visible;
            try
            {
                if (AppContext.TryGetSetting<string>("xUser", out username) && AppContext.TryGetSetting<string>("pUser", out password))
                {
                    string xml;
                    xml = string.Format("<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                        @"<entry>
                            <episode>{0}</episode>
                            <status>{1}</status>
                            <score>{2}</score>
                        </entry>", new String[] { this.AnimeModel.MyWatchedEpisodes.ToString(), this.AnimeModel.MyStatus.ToString(), this.AnimeModel.MyScore.ToString() });


                    using (var handler = new HttpClientHandler { Credentials = new NetworkCredential(username, password) })
                    {
                        using (var client = new HttpClient(handler))
                        {
                            client.BaseAddress = new Uri(URL);
                            var content = new FormUrlEncodedContent(new[] { new KeyValuePair<string, string>("data", xml) });
                            var response = await client.PostAsync("animelist/update/" + this.AnimeModel.ID.ToString() + ".xml", content);
                        }
                    }
                }

                using (var db = new DBContext())
                {
                    var anime = db.Animes.FirstOrDefault(a => a.ID == this.AnimeModel.ID);
                    anime.MyWatchedEpisodes = this.AnimeModel.MyWatchedEpisodes;
                    anime.MyStatus = this.AnimeModel.MyStatus;
                    anime.MyScore = this.AnimeModel.MyScore;
                    anime.LastUpdate = this.AnimeModel.LastUpdate;

                    db.SubmitChanges();
                }

                App.ViewModel.UpdateAnimeCollection(this.AnimeModel);

                MessageBoxResult result = MessageBox.Show("Successful Update!", "Success!", MessageBoxButton.OK);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Source);
                MessageBoxResult result = MessageBox.Show("Ocorreu um problema durante a atualização.", "Erro!", MessageBoxButton.OK);
            }
            LoadingBar.IsEnabled = false;
            LoadingBar.Visibility = Visibility.Collapsed;
        }

        public async Task RequestDeleteAnime()
        {
            string username, password;
            LoadingBar.IsEnabled = true;
            LoadingBar.Visibility = Visibility.Visible;
            try
            {
                if (AppContext.TryGetSetting<string>("xUser", out username) && AppContext.TryGetSetting<string>("pUser", out password))
                {
                    string xml;
                    xml = string.Format("<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                    @"<entry>
                        <episode>{0}</episode>
                        <status>{1}</status>
                        <score>{2}</score>
                        <downloaded_episodes></downloaded_episodes>
                        <storage_type></storage_type>
                        <storage_value></storage_value>
                        <times_rewatched></times_rewatched>
                        <rewatch_value></rewatch_value>
                        <date_start></date_start>
                        <date_finish></date_finish>
                        <priority></priority>
                        <enable_discussion></enable_discussion>
                        <enable_rewatching></enable_rewatching>
                        <comments></comments>
                        <fansub_group></fansub_group>
                        <tags></tags>
                    </entry>", this.AnimeModel.MyWatchedEpisodes.ToString(), this.AnimeModel.MyStatus.ToString(), this.AnimeModel.MyScore.ToString());

                    using (var handler = new HttpClientHandler { Credentials = new NetworkCredential(username, password) })
                    {
                        using (var client = new HttpClient(handler))
                        {
                            client.BaseAddress = new Uri(URL);
                            var content = new FormUrlEncodedContent(new[] { new KeyValuePair<string, string>("data", xml) });
                            var response = await client.PostAsync("animelist/delete/" + this.AnimeModel.ID.ToString() + ".xml", content);
                        }
                    }
                    
                    App.ViewModel.InsertAnimeCollection(this.AnimeModel);

                    using (var db = new DBContext())
                    {
                        var itemDelete = from i in db.Animes where i.ID == this.AnimeModel.ID select i;
                        foreach (Anime item in itemDelete)
                        {
                            db.Animes.DeleteOnSubmit(item);
                        }
                        db.SubmitChanges();
                    }

                    EnableBarButton("ADD");

                    MessageBoxResult result = MessageBox.Show("Successful Delete!", "Success!", MessageBoxButton.OK);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Source);
                MessageBoxResult result = MessageBox.Show("Ocorreu um problema para excluir o Anime.", "Erro!", MessageBoxButton.OK);
            }
            LoadingBar.IsEnabled = false;
            LoadingBar.Visibility = Visibility.Collapsed;
        }

        private void btnMinusEpisode_Click(object sender, RoutedEventArgs e)
        {
            if (this.AnimeModel.MyWatchedEpisodes > 0)
                this.AnimeModel.MyWatchedEpisodes--;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            this.AnimeModel.MyStatus = GetPickerStatus();
            this.AnimeModel.MyScore = (int)ratingScore.Value;
            this.AnimeModel.LastUpdate = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;

            var result = RequestAddAnime();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            var result = RequestDeleteAnime();
        }
    }
}