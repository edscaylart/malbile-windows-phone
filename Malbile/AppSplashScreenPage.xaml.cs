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

namespace Malbile
{
    public partial class AppSplashScreen : PhoneApplicationPage
    {
        public AppSplashScreen()
        {
            InitializeComponent();

            Splash_Screen();
        }

        async void Splash_Screen()
        {
            await Task.Delay(TimeSpan.FromSeconds(3)); // set your desired delay

            var context = new DBContext();

            // Cria banco de dados se ele nao existir
            if (!context.DatabaseExists())
            {
                context.CreateDatabase();
            }

            string username;

            if (AppContext.TryGetSetting<string>("xUser", out username) == true)
            {
                NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
            }
            else
            {
                NavigationService.Navigate(new Uri("/AppLoginPage.xaml", UriKind.Relative));
            }

        }
    }
}