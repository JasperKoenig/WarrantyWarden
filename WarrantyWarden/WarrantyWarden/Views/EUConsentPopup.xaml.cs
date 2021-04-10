using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WarrantyWarden.Views
{
    public partial class EUConsentPopup : PopupPage
    {
        // Launcher.OpenAsync is provided by Xamarin.Essentials.
        public ICommand TapCommand => new Command<string>(async (url) => await Launcher.OpenAsync(url));

        public EUConsentPopup()
        {
            InitializeComponent();
            BindingContext = this;
        }

        public void OnConsentButtonClicked(object sender, System.EventArgs e)
        {
            Preferences.Set("PersonalizedAdsConsent", true);
            PopupNavigation.Instance.PopAsync();
        }

        public void OnRejectButtonClicked(object sender, System.EventArgs e)
        {
            Preferences.Set("PersonalizedAdsConsent", false);
            PopupNavigation.Instance.PopAsync();
        }

        public void OnIAPButtonClicked(object sender, System.EventArgs e)
        {
            // Add In App Purchase trigger
            PopupNavigation.Instance.PopAsync();
        }
    }
}