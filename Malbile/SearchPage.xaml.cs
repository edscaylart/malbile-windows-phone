using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Threading.Tasks;
using Malbile.Context;
using System.Xml.Linq;
using System.Collections.ObjectModel;
using Malbile.Model;
using System.Xml;

namespace Malbile
{
    public partial class SearchPage : PhoneApplicationPage
    {
        public ObservableCollection<Anime> Animes { get; private set; }
        public ObservableCollection<Manga> Mangas { get; private set; }

        public SearchPage()
        {
            InitializeComponent();
            this.Animes = new ObservableCollection<Anime>();
            this.Mangas = new ObservableCollection<Manga>();
            
            listSearchAnime.DataContext = this.Animes;
            listSearchManga.DataContext = this.Mangas;
        }
        
        public void searchAnime(string searchTerm)
        {
            searchTerm = searchTerm.Replace(" ", "%20");
            string url = "http://myanimelist.net/api/anime/search.xml?q=" + searchTerm;
            string username, password;

            if (AppContext.TryGetSetting<string>("xUser", out username) && AppContext.TryGetSetting<string>("pUser", out password))
            {
                WebClient client = new WebClient();
                String credentials = Convert.ToBase64String(System.Text.Encoding.Unicode.GetBytes(username + ":" + password));
                client.Credentials = new NetworkCredential(username, password);
                client.Headers[HttpRequestHeader.UserAgent] = "api-team-692e8861471e4de2fd84f6d91d1175c0";

                client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(anime_DownloadStringCompleted);
                client.DownloadStringAsync(new Uri(url));
            }
        }

        public void searchManga(string searchTerm)
        {
            searchTerm = searchTerm.Replace(" ", "%20");
            string url = "http://myanimelist.net/api/manga/search.xml?q=" + searchTerm;
            string username, password;

            if (AppContext.TryGetSetting<string>("xUser", out username) && AppContext.TryGetSetting<string>("pUser", out password))
            {
                WebClient client = new WebClient();
                String credentials = Convert.ToBase64String(System.Text.Encoding.Unicode.GetBytes(username + ":" + password));
                client.Credentials = new NetworkCredential(username, password);
                client.Headers[HttpRequestHeader.UserAgent] = "api-team-692e8861471e4de2fd84f6d91d1175c0";

                client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(manga_DownloadStringCompleted);
                client.DownloadStringAsync(new Uri(url));
            }
        }

        void anime_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                //XDocument xdoc = XDocument.Parse(e.Result.Replace("<?xml version=\"1.0\" encoding=\"utf-8\"?>", "<?xml version=\"1.0\" encoding=\"utf-8\"?><!DOCTYPE documentElement[<!ENTITY Alpha \"&#913;\"><!ENTITY ndash \"&#8211;\"><!ENTITY mdash \"&#8212;\">]>"));
                string xml = e.Result.Replace("&", "?");
                XDocument xdoc = XDocument.Parse(xml);

                foreach (XElement xanime in xdoc.Root.Elements("entry"))
                {
                    var item = new Anime()
                    {
                        ID = (long)xanime.Element("id"),
                        Title = (string)xanime.Element("title"),
                        Type = GetAnimeTypeByName((string)xanime.Element("type")),
                        Episodes = (int)xanime.Element("episodes"),
                        Status = GetAnimeStatusByName((string)xanime.Element("status")),
                        Image = (string)xanime.Element("image"),
                        MyWatchedEpisodes = 0,
                        MyScore = 0,
                        MyStatus = 6
                    };

                    this.Animes.Add(item);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Source);
                MessageBoxResult result = MessageBox.Show("Erro durante a busca.", "Erro!", MessageBoxButton.OK);
            }
            LoadingBar.IsEnabled = false;
            LoadingBar.Visibility = Visibility.Collapsed;
        }
        void manga_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                //XDocument xdoc = XDocument.Parse(e.Result.Replace("<?xml version=\"1.0\" encoding=\"utf-8\"?>", "<?xml version=\"1.0\" encoding=\"utf-8\"?><!DOCTYPE documentElement[<!ENTITY Alpha \"&#913;\"><!ENTITY ndash \"&#8211;\"><!ENTITY mdash \"&#8212;\">]>"));
                string xml = e.Result.Replace("&", "?"); // Evitar erros de xml
                XDocument xdoc = XDocument.Parse(xml);

                foreach (XElement xmanga in xdoc.Root.Elements("entry"))
                {
                    var item = new Manga()
                    {
                        ID = (long)xmanga.Element("id"),
                        Title = (string)xmanga.Element("title"),
                        Type = GetMangaTypeByName((string)xmanga.Element("type")),
                        Chapters = (int)xmanga.Element("chapters"),
                        Volumes = (int)xmanga.Element("volumes"),
                        Status = GetMangaStatusByName((string)xmanga.Element("status")),
                        Image = (string)xmanga.Element("image"),
                        MyReadChapters = 0,
                        MyReadVolumes = 0,
                        MyScore = 0,
                        MyStatus = 6
                    };

                    this.Mangas.Add(item);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Source);
                MessageBoxResult result = MessageBox.Show("Erro durante a busca.", "Erro!", MessageBoxButton.OK);
            }
            LoadingBar.IsEnabled = false;
            LoadingBar.Visibility = Visibility.Collapsed;
        }

        public int GetAnimeTypeByName(string type)
        {
            switch (type.ToLower())
            {
                case "tv": return 1;
                case "ova": return 2;
                case "movie": return 3;
                case "special": return 4;
                case "ona": return 5;
                case "music": return 6;
                default: return 1;
            }
        }

        public int GetAnimeStatusByName(string status)
        {
            switch (status.ToLower())
            {
                case "currently airing": return 1;
                case "finished airing": return 2;
                case "not yet aired": return 3;
                default: return 1;
            }
        }

        public int GetMangaTypeByName(string type)
        {
            switch (type.ToLower())
            {
                case "manga": return 1;
                case "novel": return 2;
                case "one shot": return 3;
                case "doujin": return 4;
                case "manwha": return 5;
                case "manhua": return 6;
                case "oel": return 6;
                default: return 1;
            }
        }

        public int GetMangaStatusByName(string status)
        {
            switch (status.ToLower())
            {
                case "publishing": return 1;
                case "finished": return 2;
                case "not yet published": return 3;
                default: return 1;
            }
        }

        private void seachEntry_ActionIconTapped(object sender, EventArgs e)
        {
            this.Focus();

            LoadingBar.IsEnabled = true;
            LoadingBar.Visibility = Visibility.Visible;

            searchAnime(seachEntry.Text.ToString());
            searchManga(seachEntry.Text.ToString());
                        
        }

        private void btnItemAnime_Click(object sender, RoutedEventArgs e)
        {
            var anime = (sender as Button).DataContext as Anime;
            App.AnimeSearch = anime;

            NavigationService.Navigate(new Uri("/AnimeDetailPage.xaml", UriKind.RelativeOrAbsolute));
        }

        private void btnItemManga_Click(object sender, RoutedEventArgs e)
        {
            var manga = (sender as Button).DataContext as Manga;
            App.MangaSearch = manga;

            NavigationService.Navigate(new Uri("/MangaDetailPage.xaml", UriKind.RelativeOrAbsolute));
        }
    }
}