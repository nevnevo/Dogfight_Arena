using Dogfight_Arena.Objects;
using Dogfight_Arena.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using static Dogfight_Arena.Objects.Plane;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Dogfight_Arena.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GamePage : Page
    {
        public GameManager _GameManager;
        private int RightPlayerHealth = 5;
        private int LeftPlayerHealth = 5;
        
        public GamePage()
        {
            this.InitializeComponent();
            


        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            
            _GameManager = new GameManager(GameCanvas);
            
            
            GameManager.GameEvents.TakeHit += TakeHit;
            GameManager.GameEvents.AddHealthPoint += AddHealthPoint;
            
            
                


        }

        private void AddHealthPoint(Plane.PlaneTypes PlaneType)
        {
            if (PlaneType == Plane.PlaneTypes.LeftPlane)
            {

                LeftPlayerHealth++;
                healthBarLeftPlayer.Text = LeftPlayerHealth + "❤️";
            }
            else
            {

                RightPlayerHealth++;
                healthBarRightPlayer.Text = "❤️" + RightPlayerHealth;
            }
        }

        private async void ShowDialog()
        {
            
            ContentDialogResult result = await InescapableDialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                Frame.Navigate(typeof(MenuPage));
            }
            else // result == ContentDialogResult.Secondary or ContentDialogResult.None
            {
                ResetGame();
            }
        }

        private void ResetGame()
        {

            Frame.Navigate(typeof(RefreshGame));
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            GameManager.GameEvents.TakeHit -= TakeHit;
            _GameManager.UnsubscribeAllEvents();

            base.OnNavigatedFrom(e);
        }
        private void TakeHit(Plane.PlaneTypes PlaneType)
        {
            
            if (PlaneType == Plane.PlaneTypes.LeftPlane)
            {
                
                LeftPlayerHealth--;
                if(LeftPlayerHealth == 0)
                {
                    InescapableDialog.Content = $"Red plane has won! \nplease choose one of the two options below:";
                    ShowDialog();
                }
                healthBarLeftPlayer.Text = LeftPlayerHealth + "❤️";
            }
            else
            {

                RightPlayerHealth--;
                if (RightPlayerHealth==0)
                {
                    InescapableDialog.Content = $"Blue plane has won! \nplease choose one of the two options below:";
                    ShowDialog();
                }
                healthBarRightPlayer.Text = "❤️"+RightPlayerHealth;
            }

        }
    }
}
