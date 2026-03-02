using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Dogfight_Arena.Services;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Net.Sockets;
using System.Security;
using System.Text.RegularExpressions;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Dogfight_Arena
{
    public partial class MenuPage : Page
    {
        private bool isLoggedIn = true;

        public MenuPage()
        {
            InitializeComponent();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            
            
                ConnectionTypeGrid.Visibility = Visibility.Visible;
                
            
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void AboutButton_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            
        }

        

        private void OfflineButton_Click(object sender, RoutedEventArgs e)
        {
            GameManager.IsOnline = false;
            Frame.Navigate(typeof(Pages.GamePage));
        }

        private void OnlineButton_Click(object sender, RoutedEventArgs e)
        {
            EnterIpGrid.Visibility = Visibility.Visible;

           
        }

        private void SubmitUsernameButton_Click(object sender, RoutedEventArgs e)
        {

            string pattern = @"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b";

           if (!string.IsNullOrEmpty(UsernameTextBox.Text) && Regex.IsMatch(UsernameTextBox.Text,pattern))
            {
                GameManager.targetIp = UsernameTextBox.Text;
                UsernameTextBox.IsEnabled = false;
                EnterIpGrid.Visibility=Visibility.Collapsed;
                GameManager.IsOnline = true;
                Frame.Navigate(typeof(Pages.GamePage));
            }

        }
    }
}
