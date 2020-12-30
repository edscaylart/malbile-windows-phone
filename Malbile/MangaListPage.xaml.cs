using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Malbile.Model;

namespace Malbile
{
    public partial class MangaListPage : PhoneApplicationPage
    {
        public MangaListPage()
        {
            InitializeComponent();

            DataContext = App.ViewModel;
        }

        private void btnListItem_Click(object sender, RoutedEventArgs e)
        {
            var manga = (sender as Button).DataContext as Manga;
            NavigationService.Navigate(new Uri("/MangaDetailPage.xaml?id=" + manga.ID, UriKind.RelativeOrAbsolute));
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/SearchPage.xaml", UriKind.RelativeOrAbsolute));
        }
    }
}