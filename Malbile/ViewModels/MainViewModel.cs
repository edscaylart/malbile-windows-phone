using System;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows;
using System.Threading.Tasks;
using Microsoft.Phone.Net.NetworkInformation;
using System.Collections.ObjectModel;
using Malbile.Context;
using Malbile.Model;
using System.Xml.Linq;

namespace Malbile.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public MainViewModel()
        {
            this.Animes = new ObservableCollection<Anime>();
            this.RecentAnimes = new ObservableCollection<Anime>();
            this.AnimesWatching = new ObservableCollection<Anime>();
            this.AnimesCompleted = new ObservableCollection<Anime>();
            this.AnimesOnHold = new ObservableCollection<Anime>();
            this.AnimesDropped = new ObservableCollection<Anime>();
            this.AnimesPlanned = new ObservableCollection<Anime>();

            this.Mangas = new ObservableCollection<Manga>();
            this.RecentMangas = new ObservableCollection<Manga>();
            this.MangasReading = new ObservableCollection<Manga>();
            this.MangasCompleted = new ObservableCollection<Manga>();
            this.MangasOnHold = new ObservableCollection<Manga>();
            this.MangasDropped = new ObservableCollection<Manga>();
            this.MangasPlanned = new ObservableCollection<Manga>();
        }

        public static String URL_ANIME = "http://myanimelist.net/malappinfo.php?u={0}&status=all&type=anime";
        public static String URL_MANGA = "http://myanimelist.net/malappinfo.php?u={0}&status=all&type=manga";

        // Animes ObservableCollection
        public ObservableCollection<Anime> Animes { get; private set; }
        public ObservableCollection<Anime> RecentAnimes { get; private set; }
        public ObservableCollection<Anime> AnimesWatching { get; private set; }
        public ObservableCollection<Anime> AnimesCompleted { get; private set; }
        public ObservableCollection<Anime> AnimesOnHold { get; private set; }
        public ObservableCollection<Anime> AnimesDropped { get; private set; }
        public ObservableCollection<Anime> AnimesPlanned { get; private set; }

        // Mangas ObservableCollection
        public ObservableCollection<Manga> Mangas { get; private set; }
        public ObservableCollection<Manga> RecentMangas { get; private set; }
        public ObservableCollection<Manga> MangasReading { get; private set; }
        public ObservableCollection<Manga> MangasCompleted { get; private set; }
        public ObservableCollection<Manga> MangasOnHold { get; private set; }
        public ObservableCollection<Manga> MangasDropped { get; private set; }
        public ObservableCollection<Manga> MangasPlanned { get; private set; }

        public bool IsConnected()
        {
            if (DeviceNetworkInformation.IsNetworkAvailable)
                return true;
            else
                return false;
        }

        public bool IsDataLoaded
        {
            get;
            private set;
        }

        public void LoadData(bool forceSync = false)
        {
            if (!forceSync)
            {
                LoadAnime();
                LoadManga();
            }
            else
            {
                string username;
                if (AppContext.TryGetSetting<string>("xUser", out username))
                {
                    if (IsConnected())
                    {
                        downloadData(username, "ANIME");
                        downloadData(username, "MANGA");
                    }
                    else
                    {
                        MessageBoxResult result = MessageBox.Show("Não há conexão com internet.", "Erro!", MessageBoxButton.OK);
                    }
                }
            }
        }

        public void LoadAnime()
        {
            using (var db = new DBContext())
            {
                var animes = from a in db.Animes select a;
                foreach (var item in animes)
                {
                    this.Animes.Add(item);
                    IsDataLoaded = true;
                }

                var recent = (from r in db.Animes orderby r.LastUpdate descending select r).Take(10);
                foreach (var item in recent)
                {
                    this.RecentAnimes.Add(item);
                    IsDataLoaded = true;
                }

                var watching = from w in db.Animes where w.MyStatus == 1 orderby w.LastUpdate descending select w;
                foreach (var item in watching)
                {
                    this.AnimesWatching.Add(item);
                    IsDataLoaded = true;
                }

                var completed = from c in db.Animes where c.MyStatus == 2 orderby c.LastUpdate descending select c;
                foreach (var item in completed)
                {
                    this.AnimesCompleted.Add(item);
                    IsDataLoaded = true;
                }

                var onhold = from o in db.Animes where o.MyStatus == 3 orderby o.LastUpdate descending select o;
                foreach (var item in onhold)
                {
                    this.AnimesOnHold.Add(item);
                    IsDataLoaded = true;
                }

                var dropped = from d in db.Animes where d.MyStatus == 4 orderby d.LastUpdate descending select d;
                foreach (var item in dropped)
                {
                    this.AnimesDropped.Add(item);
                    IsDataLoaded = true;
                }

                var planned = from p in db.Animes where p.MyStatus == 6 orderby p.LastUpdate descending select p;
                foreach (var item in planned)
                {
                    this.AnimesPlanned.Add(item);
                    IsDataLoaded = true;
                }
            }
        }

        public void downloadData(string username, string listType)
        {
            WebClient client = new WebClient();
            client.Headers[HttpRequestHeader.UserAgent] = "api-team-692e8861471e4de2fd84f6d91d1175c0";
            if (listType.Equals("ANIME"))
            {
                client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(downloadAnimeStringCompleted);
                client.DownloadStringAsync(new Uri(string.Format(URL_ANIME, new String[] { username })));
            }
            else
            {
                client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(downloadMangaStringCompleted);
                client.DownloadStringAsync(new Uri(string.Format(URL_MANGA, new String[] { username })));
            }
        }

        void downloadAnimeStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                using (var db = new DBContext())
                {
                    XDocument xdoc = XDocument.Parse(e.Result);

                    foreach (XElement xanime in xdoc.Root.Elements("anime"))
                    {
                        var item = new Anime()
                        {
                            ID = (long)xanime.Element("series_animedb_id"),
                            Title = (string)xanime.Element("series_title"),
                            Synonyms = (string)xanime.Element("series_synonyms"),
                            Type = (int)xanime.Element("series_type"),
                            Episodes = (int)xanime.Element("series_episodes"),
                            Status = (int)xanime.Element("series_status"),
                            Image = (string)xanime.Element("series_image"),
                            MyWatchedEpisodes = (int)xanime.Element("my_watched_episodes"),
                            MyScore = (int)xanime.Element("my_score"),
                            MyStatus = (int)xanime.Element("my_status"),
                            LastUpdate = (long)xanime.Element("my_last_updated")
                        };

                        // verifica se o anime já se encontra na lista
                        var animes = from g in db.Animes where g.ID == item.ID select g;

                        if (animes.ToList().Count > 0)
                        {
                            foreach (Anime anime in animes.ToList())
                            {
                                anime.Title = item.Title;
                                anime.Synonyms = item.Synonyms;
                                anime.Type = item.Type;
                                anime.Episodes = item.Episodes;
                                anime.Status = item.Status;
                                anime.Image = item.Image;
                                anime.MyWatchedEpisodes = item.MyWatchedEpisodes;
                                anime.MyScore = item.MyScore;
                                anime.MyStatus = item.MyStatus;
                                anime.LastUpdate = item.LastUpdate;
                            }
                        }
                        else
                        {
                            db.Animes.InsertOnSubmit(item);
                        }
                    }
                    db.SubmitChanges();
                }
                // carrega os dados;
                LoadAnime();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Source);
                MessageBoxResult result = MessageBox.Show(ex.Message, "Erro!", MessageBoxButton.OK);
            }
        }

        public void LoadManga()
        {
            using (var db = new DBContext())
            {
                var mangas = from m in db.Mangas select m;
                foreach (var item in mangas)
                {
                    this.Mangas.Add(item);
                    IsDataLoaded = true;
                }

                var recent = (from r in db.Mangas orderby r.LastUpdate descending select r).Take(10);
                foreach (var item in recent)
                {
                    this.RecentMangas.Add(item);
                    IsDataLoaded = true;
                }

                var reading = from w in db.Mangas where w.MyStatus == 1 orderby w.LastUpdate descending select w;
                foreach (var item in reading)
                {
                    this.MangasReading.Add(item);
                    IsDataLoaded = true;
                }

                var completed = from c in db.Mangas where c.MyStatus == 2 orderby c.LastUpdate descending select c;
                foreach (var item in completed)
                {
                    this.MangasCompleted.Add(item);
                    IsDataLoaded = true;
                }

                var onhold = from o in db.Mangas where o.MyStatus == 3 orderby o.LastUpdate descending select o;
                foreach (var item in onhold)
                {
                    this.MangasOnHold.Add(item);
                    IsDataLoaded = true;
                }

                var dropped = from d in db.Mangas where d.MyStatus == 4 orderby d.LastUpdate descending select d;
                foreach (var item in dropped)
                {
                    this.MangasDropped.Add(item);
                    IsDataLoaded = true;
                }

                var planned = from p in db.Mangas where p.MyStatus == 6 orderby p.LastUpdate descending select p;
                foreach (var item in planned)
                {
                    this.MangasPlanned.Add(item);
                    IsDataLoaded = true;
                }
            }
        }

        void downloadMangaStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                using (var db = new DBContext())
                {
                    XDocument xdoc = XDocument.Parse(e.Result);

                    foreach (XElement xmanga in xdoc.Root.Elements("manga"))
                    {
                        var item = new Manga()
                        {
                            ID = (long)xmanga.Element("series_mangadb_id"),
                            Title = (string)xmanga.Element("series_title"),
                            Synonyms = (string)xmanga.Element("series_synonyms"),
                            Type = (int)xmanga.Element("series_type"),
                            Chapters = (int)xmanga.Element("series_chapters"),
                            Volumes = (int)xmanga.Element("series_volumes"),
                            Status = (int)xmanga.Element("series_status"),
                            Image = (string)xmanga.Element("series_image"),
                            MyReadChapters = (int)xmanga.Element("my_read_chapters"),
                            MyReadVolumes = (int)xmanga.Element("my_read_volumes"),
                            MyScore = (int)xmanga.Element("my_score"),
                            MyStatus = (int)xmanga.Element("my_status"),
                            LastUpdate = (long)xmanga.Element("my_last_updated")
                        };

                        // verifica se o manga já se encontra na lista
                        var mangas = from g in db.Mangas where g.ID == item.ID select g;

                        if (mangas.ToList().Count > 0)
                        {
                            foreach (Manga manga in mangas.ToList())
                            {
                                manga.Title = item.Title;
                                manga.Synonyms = item.Synonyms;
                                manga.Type = item.Type;
                                manga.Chapters = item.Chapters;
                                manga.Volumes = item.Volumes;
                                manga.Status = item.Status;
                                manga.Image = item.Image;
                                manga.MyReadChapters = item.MyReadChapters;
                                manga.MyReadVolumes = item.MyReadVolumes;
                                manga.MyScore = item.MyScore;
                                manga.MyStatus = item.MyStatus;
                                manga.LastUpdate = item.LastUpdate;
                            }
                        }
                        else
                        {
                            db.Mangas.InsertOnSubmit(item);
                        }
                    }
                    db.SubmitChanges();
                }
                // carrega os dados;
                LoadManga();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Source);
                MessageBoxResult result = MessageBox.Show(ex.Message, "Erro!", MessageBoxButton.OK);
            }
        }

        public Anime GetAnimeById(long id)
        {
            return this.Animes.FirstOrDefault(x => x.ID == id);
        }

        public Manga GetMangaById(long id)
        {
            return this.Mangas.FirstOrDefault(x => x.ID == id);
        }

        public void InsertAnimeCollection(Anime anime)
        {
            this.Animes.Add(anime);

            this.RecentAnimes.Remove(this.RecentAnimes.LastOrDefault());
            this.RecentAnimes.Insert(0, anime);

            switch (anime.MyStatus)
            {
                case 1: this.AnimesWatching.Insert(0, anime); break;
                case 2: this.AnimesCompleted.Insert(0, anime); break;
                case 3: this.AnimesOnHold.Insert(0, anime); break;
                case 4: this.AnimesDropped.Insert(0, anime); break;
                case 6: this.AnimesPlanned.Insert(0, anime); break;
            }
        }

        public void UpdateAnimeCollection(Anime anime)
        {
            var item = this.Animes.FirstOrDefault(a => a.ID == anime.ID);
            if (item != null)
            {
                this.Animes[this.Animes.IndexOf(item)] = anime;

                var i = this.RecentAnimes.IndexOf(item);
                if (i >= 0)
                {
                    this.RecentAnimes[i] = anime;
                    this.RecentAnimes.Move(i, 0);
                }
                else
                {
                    this.RecentAnimes.Remove(this.RecentAnimes.LastOrDefault());
                    this.RecentAnimes.Insert(0, anime);
                }

                var i1 = this.AnimesWatching.IndexOf(item);
                var i2 = this.AnimesCompleted.IndexOf(item);
                var i3 = this.AnimesOnHold.IndexOf(item);
                var i4 = this.AnimesDropped.IndexOf(item);
                var i6 = this.AnimesPlanned.IndexOf(item);

                if (item.MyStatus == anime.MyStatus)
                {
                    // se o status é o mesmo, apenas atualiza a lista
                    switch (item.MyStatus)
                    {
                        case 1: if (i1 >= 0) this.AnimesWatching[i1] = anime; break;
                        case 2: if (i2 >= 0) this.AnimesCompleted[i2] = anime; break;
                        case 3: if (i3 >= 0) this.AnimesOnHold[i3] = anime; break;
                        case 4: if (i4 >= 0) this.AnimesDropped[i4] = anime; break;
                        case 6: if (i6 >= 0) this.AnimesPlanned[i6] = anime; break;
                    }
                }
                else
                {
                    // se o status for diferente, remova da lista antiga e adicione na lista pertencente
                    switch (item.MyStatus)
                    {
                        case 1: if (i1 >= 0) this.AnimesWatching.RemoveAt(i1); break;
                        case 2: if (i2 >= 0) this.AnimesCompleted.RemoveAt(i2); break;
                        case 3: if (i3 >= 0) this.AnimesOnHold.RemoveAt(i3); break;
                        case 4: if (i4 >= 0) this.AnimesDropped.RemoveAt(i4); break;
                        case 6: if (i6 >= 0) this.AnimesPlanned.RemoveAt(i6); break;
                    }
                    switch (anime.MyStatus)
                    {
                        case 1: this.AnimesWatching.Insert(0, anime); break;
                        case 2: this.AnimesCompleted.Insert(0, anime); break;
                        case 3: this.AnimesOnHold.Insert(0, anime); break;
                        case 4: this.AnimesDropped.Insert(0, anime); break;
                        case 6: this.AnimesPlanned.Insert(0, anime); break;
                    }
                }
            }
        }

        public void RemoveAnimeCollection(Anime anime)
        {
            var item = this.Animes.FirstOrDefault(a => a.ID == anime.ID);

            var i = this.RecentAnimes.IndexOf(item);
            var i1 = this.AnimesWatching.IndexOf(item);
            var i2 = this.AnimesCompleted.IndexOf(item);
            var i3 = this.AnimesOnHold.IndexOf(item);
            var i4 = this.AnimesDropped.IndexOf(item);
            var i6 = this.AnimesPlanned.IndexOf(item);

            if (i >= 0) this.RecentAnimes.RemoveAt(i);
            if (i1 >= 0) this.AnimesWatching.RemoveAt(i1);
            if (i2 >= 0) this.AnimesCompleted.RemoveAt(i2);
            if (i3 >= 0) this.AnimesOnHold.RemoveAt(i3);
            if (i4 >= 0) this.AnimesDropped.RemoveAt(i4);
            if (i6 >= 0) this.AnimesPlanned.RemoveAt(i6);

            this.Animes.Remove(anime);
        }

        public void InsertMangaCollection(Manga manga)
        {
            this.Mangas.Add(manga);

            this.RecentMangas.Remove(this.RecentMangas.LastOrDefault());
            this.RecentMangas.Insert(0, manga);

            switch (manga.MyStatus)
            {
                case 1: this.MangasReading.Insert(0, manga); break;
                case 2: this.MangasCompleted.Insert(0, manga); break;
                case 3: this.MangasOnHold.Insert(0, manga); break;
                case 4: this.MangasDropped.Insert(0, manga); break;
                case 6: this.MangasPlanned.Insert(0, manga); break;
            }
        }

        public void UpdateMangaCollection(Manga manga)
        {
            var item = this.Mangas.FirstOrDefault(a => a.ID == manga.ID);
            if (item != null)
            {
                this.Mangas[this.Mangas.IndexOf(item)] = manga;

                var i = this.RecentMangas.IndexOf(item);
                if (i >= 0)
                {
                    this.RecentMangas[i] = manga;
                    this.RecentMangas.Move(i, 0);
                }
                else
                {
                    this.RecentMangas.Remove(this.RecentMangas.LastOrDefault());
                    this.RecentMangas.Insert(0, manga);
                }

                var i1 = this.MangasReading.IndexOf(item);
                var i2 = this.MangasCompleted.IndexOf(item);
                var i3 = this.MangasOnHold.IndexOf(item);
                var i4 = this.MangasDropped.IndexOf(item);
                var i6 = this.MangasPlanned.IndexOf(item);

                if (item.MyStatus == manga.MyStatus)
                {
                    // se o status é o mesmo, apenas atualiza a lista
                    switch (item.MyStatus)
                    {
                        case 1: if (i1 >= 0) this.MangasReading[i1] = manga; break;
                        case 2: if (i2 >= 0) this.MangasCompleted[i2] = manga; break;
                        case 3: if (i3 >= 0) this.MangasOnHold[i3] = manga; break;
                        case 4: if (i4 >= 0) this.MangasDropped[i4] = manga; break;
                        case 6: if (i6 >= 0) this.MangasPlanned[i6] = manga; break;
                    }
                }
                else
                {
                    // se o status for diferente, remova da lista antiga e adicione na lista pertencente
                    switch (item.MyStatus)
                    {
                        case 1: if (i1 >= 0) this.MangasReading.RemoveAt(i1); break;
                        case 2: if (i2 >= 0) this.MangasCompleted.RemoveAt(i2); break;
                        case 3: if (i3 >= 0) this.MangasOnHold.RemoveAt(i3); break;
                        case 4: if (i4 >= 0) this.MangasDropped.RemoveAt(i4); break;
                        case 6: if (i6 >= 0) this.MangasPlanned.RemoveAt(i6); break;
                    }
                    switch (manga.MyStatus)
                    {
                        case 1: this.MangasReading.Insert(0, manga); break;
                        case 2: this.MangasCompleted.Insert(0, manga); break;
                        case 3: this.MangasOnHold.Insert(0, manga); break;
                        case 4: this.MangasDropped.Insert(0, manga); break;
                        case 6: this.MangasPlanned.Insert(0, manga); break;
                    }
                }
            }
        }

        public void RemoveMangaCollection(Manga manga)
        {
            var item = this.Mangas.FirstOrDefault(a => a.ID == manga.ID);

            var i = this.RecentMangas.IndexOf(item);
            var i1 = this.MangasReading.IndexOf(item);
            var i2 = this.MangasCompleted.IndexOf(item);
            var i3 = this.MangasOnHold.IndexOf(item);
            var i4 = this.MangasDropped.IndexOf(item);
            var i6 = this.MangasPlanned.IndexOf(item);

            if (i >= 0) this.RecentMangas.RemoveAt(i);
            if (i1 >= 0) this.MangasReading.RemoveAt(i1);
            if (i2 >= 0) this.MangasCompleted.RemoveAt(i2);
            if (i3 >= 0) this.MangasOnHold.RemoveAt(i3);
            if (i4 >= 0) this.MangasDropped.RemoveAt(i4);
            if (i6 >= 0) this.MangasPlanned.RemoveAt(i6);

            this.Mangas.Remove(manga);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}