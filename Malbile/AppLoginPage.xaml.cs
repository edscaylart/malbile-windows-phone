using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Xml.Linq;
using Malbile.Context;

namespace Malbile
{
    public partial class AppLogin : PhoneApplicationPage
    {
        public static String URI = "http://myanimelist.net/api/account/verify_credentials.xml";

        public AppLogin()
        {
            InitializeComponent();
        }

        private bool isValid()
        {
            if (txtUsername.Text.Equals(""))
            {
                MessageBoxResult result = MessageBox.Show("Username Blank!", "Attention!", MessageBoxButton.OK);
                return false;
            }
            if (txtPassword.Password.Equals(""))
            {
                MessageBoxResult result = MessageBox.Show("Password Blank!", "Attention!", MessageBoxButton.OK);
                return false;
            }
            return true;
        }

        void cliente_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                XDocument xdoc = XDocument.Parse(e.Result);

                string id = (string)xdoc.Root.Element("id");
                string username = (string)xdoc.Root.Element("username");
                AppContext.StoreSetting("cUser", id);
                AppContext.StoreSetting("xUser", username);
                AppContext.StoreSetting("pUser", txtPassword.Password.ToString());

                App.ViewModel.downloadData(username, "ANIME");
                App.ViewModel.downloadData(username, "MANGA");

                NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.RelativeOrAbsolute));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Source);
                MessageBoxResult result = MessageBox.Show("Login não efetuado! Verifique seu usuário e senha.", "Erro!", MessageBoxButton.OK);
            }
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (isValid())
            {
                //var webRequest = WebRequest.Create(URI);
                //webRequest.Headers["Authorization"] = "Basic " + Convert.ToBase64String(System.Text.Encoding.Unicode.GetBytes(txtUsername.Text.ToString() + ":" + txtPassword.Text.ToString()));
                //webRequest.Credentials = new NetworkCredential(txtUsername.Text.ToString(), txtPassword.Text.ToString());
                // using (var webResponse = webRequest.GetRequestStreamAsync())
                // {
                //     return new 
                // }
                if (App.ViewModel.IsConnected())
                {
                    WebClient client = new WebClient();
                    String credentials = Convert.ToBase64String(System.Text.Encoding.Unicode.GetBytes(txtUsername.Text.ToString() + ":" + txtPassword.Password.ToString()));
                    client.Credentials = new NetworkCredential(txtUsername.Text.ToString(), txtPassword.Password.ToString());
                    client.Headers[HttpRequestHeader.UserAgent] = "api-team-692e8861471e4de2fd84f6d91d1175c0";

                    client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(cliente_DownloadStringCompleted);
                    client.DownloadStringAsync(new Uri(URI));
                }
                else
                {
                    MessageBoxResult result = MessageBox.Show("Não há conexão com internet.", "Erro!", MessageBoxButton.OK);
                }
            }
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Check if ExtendedSplashscreen.xaml is on the backstack  and remove 
            if (NavigationService.BackStack.Count() == 1)
            {
                NavigationService.RemoveBackEntry();
            }

        }
    }
}