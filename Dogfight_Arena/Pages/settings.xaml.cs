using System;
using System.Collections.Generic;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Dogfight_Arena.Services;
using System.Net.Sockets;
using System.Net;

namespace Dogfight_Arena.Pages
{
    public sealed partial class settings : Page
    {
        private string _waitingForAction = null;
        private bool _waitingForP2 = false;

        // Load directly from Keys class
        private Dictionary<string, VirtualKey> _p1KeyBindings = new Dictionary<string, VirtualKey>()
        {
            { "SpinCCW",    Keys.RotateCounterClockWiseLeftPlayer },
            { "SpinCW",     Keys.RotateClockWiseLeftPlayer        },
            { "Accelerate", Keys.AccelerateLeftPlayer             },
            { "Decelerate", Keys.DeccelerateleftPlayer            }
        };

        private Dictionary<string, VirtualKey> _p2KeyBindings = new Dictionary<string, VirtualKey>()
        {
            { "SpinCCW",    Keys.RotateCounterClockWiseRightPlayer },
            { "SpinCW",     Keys.RotateClockWiseLRightPlayer       },
            { "Accelerate", Keys.AccelerateRightPlayer             },
            { "Decelerate", Keys.DeccelerateRightPlayer            }
        };

        public settings()
        {
            this.InitializeComponent();
            Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateAllTexts();
            List<string> possibleIps = GetLocalIPAddress();
            if (possibleIps.Count > 0)
            {
                LocalIpText.Text = $"Your local IP(s): \n{string.Join(",\n", possibleIps)}";
            }
            else
            {
                LocalIpText.Text = "Could not retrieve local IP address.";
            }
        }

        private void CoreWindow_KeyDown(CoreWindow sender, KeyEventArgs args)
        {
            if (_waitingForAction != null)
            {
                if (args.VirtualKey == VirtualKey.Escape)
                {
                    _waitingForAction = null;
                    return;
                }

                if (_waitingForP2)
                {
                    _p2KeyBindings[_waitingForAction] = args.VirtualKey;
                    SaveToKeysClass(_waitingForAction, args.VirtualKey, isP2: true);
                    UpdateText(_waitingForAction, args.VirtualKey, isP2: true);
                }
                else
                {
                    _p1KeyBindings[_waitingForAction] = args.VirtualKey;
                    SaveToKeysClass(_waitingForAction, args.VirtualKey, isP2: false);
                    UpdateText(_waitingForAction, args.VirtualKey, isP2: false);
                }

                _waitingForAction = null;
                return;
            }

            // P1 input
            if (args.VirtualKey == _p1KeyBindings["SpinCCW"]) { /* P1 spin CCW   */ }
            else if (args.VirtualKey == _p1KeyBindings["SpinCW"]) { /* P1 spin CW    */ }
            else if (args.VirtualKey == _p1KeyBindings["Accelerate"]) { /* P1 accelerate */ }
            else if (args.VirtualKey == _p1KeyBindings["Decelerate"]) { /* P1 decelerate */ }

            // P2 input
            if (args.VirtualKey == _p2KeyBindings["SpinCCW"]) { /* P2 spin CCW   */ }
            else if (args.VirtualKey == _p2KeyBindings["SpinCW"]) { /* P2 spin CW    */ }
            else if (args.VirtualKey == _p2KeyBindings["Accelerate"]) { /* P2 accelerate */ }
            else if (args.VirtualKey == _p2KeyBindings["Decelerate"]) { /* P2 decelerate */ }
        }

        // Writes the new key back to the static Keys class
        private void SaveToKeysClass(string action, VirtualKey key, bool isP2)
        {
            if (!isP2)
            {
                switch (action)
                {
                    case "SpinCCW": Keys.RotateCounterClockWiseLeftPlayer = key; break;
                    case "SpinCW": Keys.RotateClockWiseLeftPlayer = key; break;
                    case "Accelerate": Keys.AccelerateLeftPlayer = key; break;
                    case "Decelerate": Keys.DeccelerateleftPlayer = key; break;
                }
            }
            else
            {
                switch (action)
                {
                    case "SpinCCW": Keys.RotateCounterClockWiseRightPlayer = key; break;
                    case "SpinCW": Keys.RotateClockWiseLRightPlayer = key; break;
                    case "Accelerate": Keys.AccelerateRightPlayer = key; break;
                    case "Decelerate": Keys.DeccelerateRightPlayer = key; break;
                }
            }
        }

        // P1 button handlers
        private void ChangeSpinCCW_Click(object sender, RoutedEventArgs e) { _waitingForP2 = false; _waitingForAction = "SpinCCW"; }
        private void ChangeSpinCW_Click(object sender, RoutedEventArgs e) { _waitingForP2 = false; _waitingForAction = "SpinCW"; }
        private void ChangeAccelerate_Click(object sender, RoutedEventArgs e) { _waitingForP2 = false; _waitingForAction = "Accelerate"; }
        private void ChangeDecelerate_Click(object sender, RoutedEventArgs e) { _waitingForP2 = false; _waitingForAction = "Decelerate"; }

        // P2 button handlers
        private void ChangeP2SpinCCW_Click(object sender, RoutedEventArgs e) { _waitingForP2 = true; _waitingForAction = "SpinCCW"; }
        private void ChangeP2SpinCW_Click(object sender, RoutedEventArgs e) { _waitingForP2 = true; _waitingForAction = "SpinCW"; }
        private void ChangeP2Accelerate_Click(object sender, RoutedEventArgs e) { _waitingForP2 = true; _waitingForAction = "Accelerate"; }
        private void ChangeP2Decelerate_Click(object sender, RoutedEventArgs e) { _waitingForP2 = true; _waitingForAction = "Decelerate"; }

        private void UpdateText(string action, VirtualKey key, bool isP2)
        {
            switch (action)
            {
                case "SpinCCW":
                    if (isP2) P2SpinCCWText.Text = $"current: {key}";
                    else SpinCCWText.Text = $"current: {key}";
                    break;
                case "SpinCW":
                    if (isP2) P2SpinCWText.Text = $"current: {key}";
                    else SpinCWText.Text = $"current: {key}";
                    break;
                case "Accelerate":
                    if (isP2) P2AccelerateText.Text = $"current: {key}";
                    else AccelerateText.Text = $"current: {key}";
                    break;
                case "Decelerate":
                    if (isP2) P2DecelerateText.Text = $"current: {key}";
                    else DecelerateText.Text = $"current: {key}";
                    break;
            }
        }

        private void UpdateAllTexts()
        {
            UpdateText("SpinCCW", _p1KeyBindings["SpinCCW"], isP2: false);
            UpdateText("SpinCW", _p1KeyBindings["SpinCW"], isP2: false);
            UpdateText("Accelerate", _p1KeyBindings["Accelerate"], isP2: false);
            UpdateText("Decelerate", _p1KeyBindings["Decelerate"], isP2: false);

            UpdateText("SpinCCW", _p2KeyBindings["SpinCCW"], isP2: true);
            UpdateText("SpinCW", _p2KeyBindings["SpinCW"], isP2: true);
            UpdateText("Accelerate", _p2KeyBindings["Accelerate"], isP2: true);
            UpdateText("Decelerate", _p2KeyBindings["Decelerate"], isP2: true);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MenuPage));
        }
        public static List<string> GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            List<string> ipAddresses = new List<string>();
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    ipAddresses.Add(ip.ToString());


                }
            }
            return ipAddresses;
        }
    }
}