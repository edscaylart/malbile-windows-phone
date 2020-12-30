using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.ComponentModel;

namespace Malbile.Model
{
    [Table]
    public class Anime : INotifyPropertyChanged, INotifyPropertyChanging
    {
        private long id;
        private string title;
        private string synonyms;
        private int type;
        private int episodes;
        private int status;
        private string image;
        private int myWatchedEpisodes;
        private int myScore;
        private int myStatus;
        private long lastUpdate;
        private bool dirty;

        [Column(IsPrimaryKey = true, CanBeNull = false)]
        public long ID
        {
            get { return id; }
            set
            {
                if (id != value)
                {
                    NotifyPropertyChanging("ID");
                    id = value;
                    NotifyPropertyChanged("ID");
                }
            }
        }

        [Column(CanBeNull = true)]
        public string Title
        {
            get { return title; }
            set
            {
                if (title != value)
                {
                    NotifyPropertyChanging("Title");
                    title = value;
                    NotifyPropertyChanged("Title");
                }
            }
        }

        [Column(CanBeNull = true)]
        public string Synonyms
        {
            get { return synonyms; }
            set
            {
                if (synonyms != value)
                {
                    NotifyPropertyChanging("Synonyms");
                    synonyms = value;
                    NotifyPropertyChanged("Synonyms");
                }
            }
        }

        [Column(CanBeNull = true)]
        public int Type
        {
            get { return type; }
            set
            {
                if (type != value)
                {
                    NotifyPropertyChanging("Type");
                    type = value;
                    NotifyPropertyChanged("Type");
                }
            }
        }

        [Column(CanBeNull = true)]
        public int Episodes
        {
            get { return episodes; }
            set
            {
                if (episodes != value)
                {
                    NotifyPropertyChanging("Episodes");
                    episodes = value;
                    NotifyPropertyChanged("Episodes");
                }
            }
        }

        [Column(CanBeNull = true)]
        public int Status
        {
            get { return status; }
            set
            {
                if (status != value)
                {
                    NotifyPropertyChanging("Status");
                    status = value;
                    NotifyPropertyChanged("Status");
                }
            }
        }

        [Column(CanBeNull = true)]
        public string Image
        {
            get { return image; }
            set
            {
                if (image != value)
                {
                    NotifyPropertyChanging("Image");
                    image = value;
                    NotifyPropertyChanged("Image");
                }
            }
        }
        
        [Column(CanBeNull = true)]
        public int MyWatchedEpisodes
        {
            get { return myWatchedEpisodes; }
            set
            {
                if (myWatchedEpisodes != value)
                {
                    NotifyPropertyChanging("MyWatchedEpisodes");
                    myWatchedEpisodes = value;
                    NotifyPropertyChanged("MyWatchedEpisodes");

                    Dirty = true;
                }
            }
        }

        [Column(CanBeNull = true)]
        public int MyScore
        {
            get { return myScore; }
            set
            {
                if (myScore != value)
                {
                    NotifyPropertyChanging("MyScore");
                    myScore = value;
                    NotifyPropertyChanged("MyScore");

                    Dirty = true;
                }
            }
        }

        [Column(CanBeNull = true)]
        public int MyStatus
        {
            get { return myStatus; }
            set
            {
                if (myStatus != value)
                {
                    NotifyPropertyChanging("MyStatus");
                    myStatus = value;
                    NotifyPropertyChanged("MyStatus");

                    Dirty = true;
                }
            }
        }

        [Column(CanBeNull = true)]
        public long LastUpdate
        {
            get { return lastUpdate; }
            set
            {
                if (lastUpdate != value)
                {
                    NotifyPropertyChanging("LastUpdate");
                    lastUpdate = value;
                    NotifyPropertyChanged("LastUpdate");
                }
            }
        }

        [Column(CanBeNull = true)]
        public bool Dirty
        {
            get { return dirty; }
            private set
            {
                if (dirty != value)
                {
                    NotifyPropertyChanging("Dirty");
                    dirty = value;
                    NotifyPropertyChanged("Dirty");
                }
            }
        }

        /// <summary>
        /// Pega a descrição do Status
        /// </summary>
        /// <returns>
        /// Watching; completed; on-hold; dropped; plan to watch
        /// </returns>
        public string getStatusDescription()
        {
            switch (this.Status)
            {
                case 1: return "Watching";
                case 2: return "Completed";
                case 3: return "On-Hold";
                case 4: return "Dropped";
                case 6: return "Plan To Watch";
                default: return "Watching";
            }
        }

        /// <summary>
        /// Pega a descrição do Tipo de Anime
        /// </summary>
        /// <returns>
        /// TV; OVA; movie; special; ONA; music
        /// </returns>
        public string getTypeDescription()
        {
            switch (this.Type)
            {
                case 1: return "TV";
                case 2: return "OVA";
                case 3: return "Movie";
                case 4: return "Special";
                case 5: return "ONA";
                case 6: return "Music";
                default: return "TV";
            }
        }

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        #region INotifyPropertyChanging Members
        public event PropertyChangingEventHandler PropertyChanging;

        protected void NotifyPropertyChanging(string propertyName)
        {
            if (PropertyChanging != null)
            {
                PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
            }
        }
        #endregion
    }
}
