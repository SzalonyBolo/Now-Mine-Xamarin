using NControl.Abstractions;
using NGraphics;
using NowMine.Network;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NowMine
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UserConfigPage : ContentPage
    {
        ServerConnection serverConnection;
        public UserConfigPage(ServerConnection serverConnection)
        {
            InitializeComponent();
            this.serverConnection = serverConnection;
            var colorPicker = new ColorPicker();
            stlConfigLayout.Children.Add(colorPicker);
            ChangeNameEntry.Text = User.DeviceUser.Name;
        }

        private async void Entry_Completed(object sender, EventArgs e)
        {
            Entry entry = (Entry)sender;
            string newUserName = entry.Text;
            bool isLegal = await serverConnection.ChangeName(newUserName);
            Debug.WriteLine("New User Name Accepted: {0}", isLegal);
            if (isLegal)
            {
                entry.TextColor = Xamarin.Forms.Color.Green;
                User.DeviceUser.Name = newUserName;
            }
            else
            {
                entry.Text = User.DeviceUser.Name;
                entry.TextColor = Xamarin.Forms.Color.Red;
            }
        }
    }
}
