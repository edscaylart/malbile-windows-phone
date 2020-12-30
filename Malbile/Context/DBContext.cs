using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using Malbile.Context;
using Malbile.Model;

namespace Malbile.Context
{
    public class DBContext : DataContext
    {
        public DBContext() : base("DataSource=isostore:/mangalist.sdf;") { }

        public Table<Anime> Animes;
        public Table<Manga> Mangas;
    }
}
