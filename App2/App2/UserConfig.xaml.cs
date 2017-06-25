using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Plugin.DeviceInfo;

namespace NowMine
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UserConfig : ContentPage
    {
        public UserConfig()
        {
            InitializeComponent();
            this.Title = "Ustawienia";
            this.BackgroundColor = Color.Purple;
        }

        private void entuserName_Completed(object sender, EventArgs e)
        {
            //if entuserName.text != config.user name
            //sendn new user name
            var deviceInfo = new Plugin.DeviceInfo.CrossDeviceInfo();
            string asdf = CrossDeviceInfo.Current.Model;
            lblName.Text = asdf;
        }
    }
}
