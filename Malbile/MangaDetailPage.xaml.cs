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
    public partial class MangaDetailPage : PhoneApplicationPage
    {
        public static String URL = "http://myanimelist.net/api/";

        public List<String> m_Status = new List<String>() { "Reading", "Completed", "On-Hold", "Dropped", "Plan To Read" };
        public List<String> Status
        {
            get
            {
                return m_Status;
            }
        }

        public MangaDetailPage()
        {
            InitializeComponent();
            listPickerStatus.ItemsSource = Status;
        }
        public Manga MangaModel { get; set; }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (NavigationContext.QueryString.ContainsKey("id"))
            {
                string id = NavigationContext.QueryString["id"];
                MangaModel = App.ViewModel.GetMangaById(long.Parse(id));

                EnableBarButton("SAVE");
            }
            else
            {
                var ma = App.ViewModel.GetMangaById(App.MangaSearch.ID);
                if (ma != null)
                {
                    MangaModel = ma;
                    EnableBarButton("SAVE");
                }
                else
                {
                    MangaModel = App.MangaSearch;
                    EnableBarButton("ADD");
                }
            }
            DataContext = MangaModel;

            SetPickerStatus();
            ratingScore.Value = this.MangaModel.MyScore;
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
            switch (MangaModel.MyStatus)
            {
                case 1: listPickerStatus.SelectedItem = "Reading"; break;
                case 2: listPickerStatus.SelectedItem = "Completed"; break;
                case 3: listPickerStatus.SelectedItem = "On-Hold"; break;
                case 4: listPickerStatus.SelectedItem = "Dropped"; break;
                case 6: listPickerStatus.SelectedItem = "Plan To Read"; break;
                default: listPickerStatus.SelectedItem = "Reading"; break;
            }
        }

        public int GetPickerStatus()
        {
            switch (listPickerStatus.SelectedItem.ToString())
            {
                case "Reading": return 1;
                case "Completed": return 2;
                case "On-Hold": return 3;
                case "Dropped": return 4;
                case "Plan To Read": return 6;
                default: return 1;
            }
        }

        private void btnPlusChapter_Click(object sender, RoutedEventArgs e)
        {
            MangaModel.MyReadChapters++;
        }

        private void btnMinusChapter_Click(object sender, RoutedEventArgs e)
        {
            if (MangaModel.MyReadChapters > 0)
                MangaModel.MyReadChapters--;
        }

        private void btnPlusVolumes_Click(object sender, RoutedEventArgs e)
        {
            MangaModel.MyReadVolumes++;
        }

        private void btnMinusVolumes_Click(object sender, RoutedEventArgs e)
        {
            if (MangaModel.MyReadVolumes > 0)
                MangaModel.MyReadVolumes--;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            this.MangaModel.MyStatus = GetPickerStatus();
            this.MangaModel.MyScore = (int)ratingScore.Value;
            this.MangaModel.LastUpdate = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;

            var result = RequestUpdateManga();
        }

        public async Task RequestAddManga()
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
                        <chapter>{0}</chapter>
                        <volume>{1}</volume>
                        <status>{2}</status>
                        <score>{3}</score>
                        <downloaded_chapters></downloaded_chapters>
	                    <times_reread></times_reread>
	                    <reread_value></reread_value>
	                    <date_start></date_start>
	                    <date_finish></date_finish>
	                    <priority></priority>
	                    <enable_discussion></enable_discussion>
	                    <enable_rereading></enable_rereading>
	                    <comments></comments>
	                    <scan_group></scan_group>
	                    <tags></tags>
	                    <retail_volumes></retail_volumes>
                    </entry>", this.MangaModel.MyReadChapters.ToString(), this.MangaModel.MyReadVolumes.ToString(), this.MangaModel.MyStatus.ToString(), this.MangaModel.MyScore.ToString());

                    using (var handler = new HttpClientHandler { Credentials = new NetworkCredential(username, password) })
                    {
                        using (var client = new HttpClient(handler))
                        {
                            client.BaseAddress = new Uri(URL);
                            var content = new FormUrlEncodedContent(new[] { new KeyValuePair<string, string>("data", xml) });
                            var response = await client.PostAsync("mangalist/add/" + this.MangaModel.ID.ToString() + ".xml", content);
                        }
                    }

                    using (var db = new DBContext())
                    {
                        db.Mangas.InsertOnSubmit(this.MangaModel);
                        db.SubmitChanges();
                    }

                    App.ViewModel.InsertMangaCollection(this.MangaModel);

                    EnableBarButton("SAVE");

                    MessageBoxResult result = MessageBox.Show("Successful Insert!", "Success!", MessageBoxButton.OK);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Source);
                MessageBoxResult result = MessageBox.Show("Ocorreu um problema para adicionar o Manga.", "Erro!", MessageBoxButton.OK);
            }
            LoadingBar.IsEnabled = false;
            LoadingBar.Visibility = Visibility.Collapsed;
        }

        public async Task RequestUpdateManga()
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
                            <chapter>{0}</chapter>
                            <volume>{1}</volume>
                            <status>{2}</status>
                            <score>{3}</score>
                        </entry>", new String[] { this.MangaModel.MyReadChapters.ToString(), this.MangaModel.MyReadVolumes.ToString(), this.MangaModel.MyStatus.ToString(), this.MangaModel.MyScore.ToString() });


                    using (var handler = new HttpClientHandler { Credentials = new NetworkCredential(username, password) })
                    {
                        using (var client = new HttpClient(handler))
                        {
                            client.BaseAddress = new Uri(URL);
                            var content = new FormUrlEncodedContent(new[] { new KeyValuePair<string, string>("data", xml) });
                            var response = await client.PostAsync("mangalist/update/" + this.MangaModel.ID.ToString() + ".xml", content);
                        }
                    }
                }

                using (var db = new DBContext())
                {
                    var manga = db.Mangas.FirstOrDefault(a => a.ID == this.MangaModel.ID);
                    manga.MyReadChapters = this.MangaModel.MyReadChapters;
                    manga.MyReadVolumes = this.MangaModel.MyReadVolumes;
                    manga.MyStatus = this.MangaModel.MyStatus;
                    manga.MyScore = this.MangaModel.MyScore;
                    manga.LastUpdate = this.MangaModel.LastUpdate;

                    db.SubmitChanges();
                }

                App.ViewModel.UpdateMangaCollection(this.MangaModel);

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

        public async Task RequestDeleteManga()
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
                        <chapter>{0}</chapter>
                        <volume>{1}</volume>
                        <status>{2}</status>
                        <score>{3}</score>
                        <downloaded_chapters></downloaded_chapters>
	                    <times_reread></times_reread>
	                    <reread_value></reread_value>
	                    <date_start></date_start>
	                    <date_finish></date_finish>
	                    <priority></priority>
	                    <enable_discussion></enable_discussion>
	                    <enable_rereading></enable_rereading>
	                    <comments></comments>
	                    <scan_group></scan_group>
	                    <tags></tags>
	                    <retail_volumes></retail_volumes>
                    </entry>", this.MangaModel.MyReadChapters.ToString(), this.MangaModel.MyReadVolumes.ToString(), this.MangaModel.MyStatus.ToString(), this.MangaModel.MyScore.ToString());

                    using (var handler = new HttpClientHandler { Credentials = new NetworkCredential(username, password) })
                    {
                        using (var client = new HttpClient(handler))
                        {
                            client.BaseAddress = new Uri(URL);
                            var content = new FormUrlEncodedContent(new[] { new KeyValuePair<string, string>("data", xml) });
                            var response = await client.PostAsync("mangalist/delete/" + this.MangaModel.ID.ToString() + ".xml", content);
                        }
                    }

                    App.ViewModel.RemoveMangaCollection(this.MangaModel);

                    using (var db = new DBContext())
                    {
                        var itemDelete = from i in db.Mangas where i.ID == this.MangaModel.ID select i;
                        foreach (Manga item in itemDelete)
                        {
                            db.Mangas.DeleteOnSubmit(item);
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
                MessageBoxResult result = MessageBox.Show("Ocorreu um problema para excluir o Manga.", "Erro!", MessageBoxButton.OK);
            }
            LoadingBar.IsEnabled = false;
            LoadingBar.Visibility = Visibility.Collapsed;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            this.MangaModel.MyStatus = GetPickerStatus();
            this.MangaModel.MyScore = (int)ratingScore.Value;
            this.MangaModel.LastUpdate = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;

            var result = RequestAddManga();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            var result = RequestDeleteManga();
        }
    }
}