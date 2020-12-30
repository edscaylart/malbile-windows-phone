using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Malbile.Context;
using Malbile.Model;

namespace Malbile
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Set the data context of the listbox control to the sample data
            DataContext = App.ViewModel;
        }

        // Load data for the ViewModel Items
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!App.ViewModel.IsDataLoaded)
            {
                App.ViewModel.LoadData(true);
            }

            // Check if ExtendedSplashscreen.xaml is on the backstack  and remove 
            if (NavigationService.BackStack.Count() == 1)
            {
                NavigationService.RemoveBackEntry();
            }
        }       

        private void btnAnimes_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/AnimeListPage.xaml", UriKind.RelativeOrAbsolute));
        }

        private void btnMangas_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/MangaListPage.xaml", UriKind.RelativeOrAbsolute));
        }

        private void btnListRecentAnime_Click(object sender, RoutedEventArgs e)
        {
            var anime = (sender as Button).DataContext as Anime;
            NavigationService.Navigate(new Uri("/AnimeDetailPage.xaml?id=" + anime.ID, UriKind.RelativeOrAbsolute));
        }

        private void btnListRecentManga_Click(object sender, RoutedEventArgs e)
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