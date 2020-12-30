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
    public class Manga : INotifyPropertyChanged, INotifyPropertyChanging
    {
        private long id;
        private string title;
        private string synonyms;
        private int type;
        private int volumes;
        private int chapters;
        private int status;
        private string image;
        private int myReadChapters;
        private int myReadVolumes;
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
        public int Chapters
        {
            get { return chapters; }
            set
            {
                if (chapters != value)
                {
                    NotifyPropertyChanging("Chapters");
                    chapters = value;
                    NotifyPropertyChanged("Chapters");
                }
            }
        }

        [Column(CanBeNull = true)]
        public int Volumes
        {
            get { return volumes; }
            set
            {
                if (volumes != value)
                {
                    NotifyPropertyChanging("Volumes");
                    volumes = value;
                    NotifyPropertyChanged("Volumes");
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
        public int MyReadChapters
        {
            get { return myReadChapters; }
            set
            {
                if (myReadChapters != value)
                {
                    NotifyPropertyChanging("MyReadChapters");
                    myReadChapters = value;
                    NotifyPropertyChanged("MyReadChapters");
                }
            }
        }

        [Column(CanBeNull = true)]
        public int MyReadVolumes
        {
            get { return myReadVolumes; }
            set
            {
                if (myReadVolumes != value)
                {
                    NotifyPropertyChanging("MyReadVolumes");
                    myReadVolumes = value;
                    NotifyPropertyChanged("MyReadVolumes");
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
        /// Reading; completed; on-hold; dropped; plan to read
        /// </returns>
        public String getStatusDescription()
        {
            switch (this.Status)
            {
                case 1: return "Reading";
                case 2: return "Completed";
                case 3: return "On-Hold";
                case 4: return "Dropped";
                case 6: return "Plan To Read";
                default: return "Reading";
            }
        }

        /// <summary>
        /// Pega a descrição do Tipo do Manga
        /// </summary>
        /// <returns>
        /// Manga; novel; one shot; doujin; manwha; manhua; oel;
        /// </returns>
        public String getTypeDescription()
        {
            switch (this.Type)
            {
                case 1: return "Manga";
                case 2: return "Novel";
                case 3: return "One Shot";
                case 4: return "Doujin";
                case 5: return "Manwha";
                case 6: return "Manhua";
                case 7: return "OEL";
                default: return "Manga";
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
