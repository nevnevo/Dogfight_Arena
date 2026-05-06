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
using Windows.UI.ViewManagement;
using Windows.Foundation;
using Dogfight_Arena.Communication;
using System.Threading.Tasks;


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
            GameManager.GameEvents.TakeHit -= TakeHit;
            GameManager.GameEvents.AddHealthPoint -= AddHealthPoint;

            _GameManager = new GameManager(GameCanvas);

            

            GameManager.GameEvents.TakeHit += TakeHit;
            GameManager.GameEvents.AddHealthPoint += AddHealthPoint;
            if (GameManager.IsOnline)
            {
                if (GameManager.LocalPlayerType == Plane.PlaneTypes.LeftPlane)
                {
                    sideOfLocalPlayer.Text = "Blue";
                }
                else
                {
                    sideOfLocalPlayer.Text = "Red";
                }
            }
            


            


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

                GameManager.GameEvents.TakeHit -= TakeHit;

                
                Frame.Navigate(typeof(MenuPage));
            }
            else // result == ContentDialogResult.Secondary
            {
                ResetGame();//need to rewrite that for the online funcitonality
            }
        }

        private async void ResetGame()
        {
            if (!GameManager.IsOnline)
            {
                _GameManager.UnsubscribeAllEvents();
                Frame.Navigate(typeof(RefreshGame));
                return;
            }

            GameManager.client.ResetPlayAgainState();

            var pkt = new Packet(Packet.PacketType.PlayAgain);
            GameManager.client.SendData(pkt);
            Client.playAgainRequested = true;

            var startTimeTask = GameManager.client.PlayAgainStartTimeSource.Task;
            var completed = await Task.WhenAny(startTimeTask, Task.Delay(15000));

            if (completed != startTimeTask)
            {
                Frame.Navigate(typeof(MenuPage));
                return;
            }

            long startingTime = startTimeTask.Result;
            long delay = startingTime - DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            if (delay > 0)
                await Task.Delay((int)delay);

            LeftPlayerHealth = 5;
            RightPlayerHealth = 5;
            healthBarLeftPlayer.Text = "5❤️";
            healthBarRightPlayer.Text = "❤️5";

            // ↓ The new lines go here, replacing the old two-liner
            GameManager.GameEvents.TakeHit -= TakeHit;
            GameManager.GameEvents.AddHealthPoint -= AddHealthPoint;
            _GameManager.UnsubscribeAllEvents();
            _GameManager.OnlineReset(GameCanvas, GameManager.client);
            GameManager.GameEvents.TakeHit += TakeHit;
            GameManager.GameEvents.AddHealthPoint += AddHealthPoint;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            
            

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
