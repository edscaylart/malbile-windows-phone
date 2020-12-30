using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Malbile.Util
{
    public class AnimeTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                string parameterConvert = parameter as string;
                int convertValue = (int)value;

                switch (convertValue)
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

            return String.Empty;
        }

        // No need to implement converting back on a one-way binding
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }

    public class AnimeMyStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                string parameterConvert = parameter as string;
                int convertValue = (int)value;

                switch (convertValue)
                {
                    case 1: return "Watching";
                    case 2: return "Completed";
                    case 3: return "On-Hold";
                    case 4: return "Dropped";
                    case 6: return "Plan To Watch";
                    default: return "Watching";
                }
            }

            return String.Empty;
        }

        // No need to implement converting back on a one-way binding
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }

    public class AnimeStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                string parameterConvert = parameter as string;
                int convertValue = (int)value;

                switch (convertValue)
                {
                    case 1: return "Currently Airing";
                    case 2: return "Finished Airing";
                    case 3: return "Not Yet Aired";
                    default: return "Finished Airing";
                }
            }

            return String.Empty;
        }

        // No need to implement converting back on a one-way binding
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }

    public class MangaTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                string parameterConvert = parameter as string;
                int convertValue = (int)value;

                switch (convertValue)
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

            return String.Empty;
        }

        // No need to implement converting back on a one-way binding
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }

    public class MangaMyStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                string parameterConvert = parameter as string;
                int convertValue = (int)value;

                switch (convertValue)
                {
                    case 1: return "Reading";
                    case 2: return "Completed";
                    case 3: return "On-Hold";
                    case 4: return "Dropped";
                    case 6: return "Plan To Read";
                    default: return "Reading";
                }
            }

            return String.Empty;
        }

        // No need to implement converting back on a one-way binding
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }

    public class MangaStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                string parameterConvert = parameter as string;
                int convertValue = (int)value;

                switch (convertValue)
                {
                    case 1: return "Publishing";
                    case 2: return "Finished";
                    case 3: return "Not Yet Published";
                    default: return "Finished";
                }
            }

            return String.Empty;
        }

        // No need to implement converting back on a one-way binding
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}
