using System;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WarrantyWarden.Views
{
    public partial class EUConsentPage : ContentPage
    {
        // Launcher.OpenAsync is provided by Xamarin.Essentials.
        public ICommand TapCommand => new Command<string>(async (url) => await Launcher.OpenAsync(url));

        public EUConsentPage()
        {
            InitializeComponent();
            BindingContext = this;
        }

        public void OnConsentButtonClicked(object sender, System.EventArgs e)
        {
            Preferences.Set("PersonalizedAdsConsent", true);
            DisplayAlert("Confirmed", "Relevant ads have been enabled", "OK");
        }

        public void OnRejectButtonClicked(object sender, System.EventArgs e)
        {
            Preferences.Set("PersonalizedAdsConsent", false);
            DisplayAlert("Confirmed", "Relevant ads have been disabled", "OK");
        }

        public void OnIAPButtonClicked(object sender, System.EventArgs e)
        {
            // Add In App Purchase trigger
            Preferences.Set("PaidUser", true);
        }
    }
}