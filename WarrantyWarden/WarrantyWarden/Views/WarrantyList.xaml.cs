using System;
using WarrantyWarden.Models;
using WarrantyWarden.Data;
using Xamarin.Forms;
using MarcTron.Plugin;
using System.Linq;
using Xamarin.Essentials;
using Rg.Plugins.Popup.Services;

namespace WarrantyWarden.Views
{
    public partial class WarrantyList : ContentPage
    {
        public WarrantyList()
        {
            InitializeComponent();

            BannerAd.PersonalizedAds = Preferences.Get("PersonalizedAdsConsent", false);

            if (Preferences.Get("FirstRun", true))
            {
                Preferences.Set("FirstRun", false);
                Preferences.Set("Warranties", 5);
                PopupNavigation.Instance.PushAsync(new EUConsentPopup());
            }

            CrossMTAdmob.Current.OnRewardedVideoAdLoaded += (s, args) =>
            {
                CrossMTAdmob.Current.ShowRewardedVideo();
            };
        }

        protected override bool OnBackButtonPressed()
        {
            DisableEvents();
            return base.OnBackButtonPressed();
        }

        // Called when page is loaded to calculate and display warranty summaries
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            SetEvents();

            WarrantyDatabase database = await WarrantyDatabase.Instance;

            var warranties = await database.GetItemsAsync();

            // Calculate time remaining for each warranty and set unit (Days, Moths, Years)
            foreach (Warranty warranty in warranties)
            {                
                if (Convert.ToInt32(Math.Floor(warranty.EndDate.Subtract(DateTime.Today).Days / (365.25))) >= 2)
                {
                    warranty.LargestUnit = "Years";
                    warranty.Priority = 1;
                    warranty.LargestUnitRemaining = Convert.ToInt32(Math.Ceiling(warranty.EndDate.Subtract(DateTime.Today).Days / (365.25)));

                } else if (Convert.ToInt32(Math.Floor(warranty.EndDate.Subtract(DateTime.Today).Days / (365.25 / 12))) > 0)
                {
                    warranty.Priority = 1;
                    if (Convert.ToInt32(Math.Floor(warranty.EndDate.Subtract(DateTime.Today).Days / (365.25 / 12))) <= 4)
                    {
                        warranty.Priority = 2;
                    }
                    warranty.LargestUnit = "Months";
                    warranty.LargestUnitRemaining = Convert.ToInt32(Math.Floor(warranty.EndDate.Subtract(DateTime.Today).Days / (365.25 / 12)));

                } else if (warranty.EndDate.Subtract(DateTime.Today).Days <= 0)
                {
                    warranty.Priority = 0;

                } else
                {
                    warranty.LargestUnit = "Days";
                    warranty.Priority = 3;
                    warranty.LargestUnitRemaining = Convert.ToInt32(warranty.EndDate.Subtract(DateTime.Today).Days);
                }

                await database.SaveItemAsync(warranty);
            }

            ListView.ItemsSource = await database.QueryAsync("SELECT * FROM [Warranty] ORDER BY [Priority] DESC, [EndDate] ASC");
        }

        void SetEvents()
        {
            CrossMTAdmob.Current.OnRewarded += AdRewardSuccess;
            CrossMTAdmob.Current.OnRewardedVideoAdClosed += AdRewardSomethingWentWrong;
            CrossMTAdmob.Current.OnRewardedVideoAdFailedToLoad += AdRewardSomethingWentWrong;
            CrossMTAdmob.Current.OnRewardedVideoAdLeftApplication += AdRewardSomethingWentWrong;
        }

        private void DisableEvents()
        {
            CrossMTAdmob.Current.OnRewarded -= AdRewardSuccess;
            CrossMTAdmob.Current.OnRewardedVideoAdClosed -= AdRewardSomethingWentWrong;
            CrossMTAdmob.Current.OnRewardedVideoAdFailedToLoad -= AdRewardSomethingWentWrong;
            CrossMTAdmob.Current.OnRewardedVideoAdLeftApplication -= AdRewardSomethingWentWrong;
        }

        // Sends an alert if the ad gets closed for any reason
        private void AdRewardSomethingWentWrong(object sender, EventArgs e)
        {
            DisplayAlert("Oops", "Something went wrong serving you this ad, please try again", "Continue");
        }

        // Plus Button clicked to add warranty
        async void OnItemAdded(object sender, EventArgs e)
        {
            WarrantyDatabase database = await WarrantyDatabase.Instance;

            var warranties = await database.GetItemsAsync();

            // Check if the use is a paid user
            if (Preferences.Get("PaidUser", false))
            {
                await Navigation.PushAsync(new AddWarranty
                {
                    BindingContext = new Warranty()
                });
            }
            // Check if the user has at least 5 warranty entries already
            else if ((warranties.Count() >= Preferences.Get("Warranties", 5)) && !(Preferences.Get("PaidUser", false)))
            {
                // Ask whether they want to become a paying user
                var answer = await DisplayAlert("Oops", "Please watch an ad to enter 5 more warranties.", "Become paid user", "Continue");
                if (answer)
                {
                    // Insert in app purchase page here, remove temp next line
                    Preferences.Set("PaidUser", true);
                }
                else
                {
                    ShowRewardAd();
                }
            }
            else
            {
                DisableEvents();

                await Navigation.PushAsync(new AddWarranty
                {
                    BindingContext = new Warranty()
                });
            }
        }

        void ShowRewardAd()
        {
            CrossMTAdmob.Current.UserPersonalizedAds = Preferences.Get("PersonalizedAdsConsent", false);
            CrossMTAdmob.Current.LoadRewardedVideo("ca-app-pub-2438123329717714/8121719713");
            CrossMTAdmob.Current.UserPersonalizedAds = Preferences.Get("PersonalizedAdsConsent", false);
        }

        async void AdRewardSuccess(object sender, EventArgs e)
        {
            DisableEvents();

            int temp = Preferences.Get("Warranties", 5);
            temp = temp + 5;
            Preferences.Set("Warranties", temp);

            await Navigation.PushAsync(new AddWarranty
            {
                BindingContext = new Warranty()
            });
        }

        // Tapping on list item to view details
        async void OnListItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            DisableEvents();

            if (e.SelectedItem != null)
            {
                await Navigation.PushAsync(new ViewWarranty
                {
                    BindingContext = e.SelectedItem as Warranty
                });
            }
        }

        // Deletes the selected item after confirmation message
        async void OnDeleteButtonClicked(object sender, EventArgs e)
        {
            var answer = await DisplayAlert("Delete", "Do you want to delete the warranty?", "Yes", "No");
            if (answer)
            {
                // The sender is the menuItem
                MenuItem menuItem = sender as MenuItem;

                // Access the list item through the BindingContext
                var contextItem = menuItem.BindingContext;

                var Warranty = contextItem as Warranty;
                WarrantyDatabase database = await WarrantyDatabase.Instance;
                await database.DeleteItemAsync(Warranty);
                await Navigation.PopAsync();
                RefreshData();
            }
        }

        // Code to refresh data
        protected void ListItems_Refreshing(object sender, EventArgs e)
        {
            RefreshData();
            ListView.EndRefresh();
        }

        async void RefreshData()
        {
            WarrantyDatabase database = await WarrantyDatabase.Instance;

            var warranties = await database.GetItemsAsync();

            foreach (Warranty warranty in warranties)
            {
                if (Convert.ToInt32(Math.Floor(warranty.EndDate.Subtract(DateTime.Today).Days / (365.25))) >= 2)
                {
                    warranty.LargestUnit = "Years";
                    warranty.Priority = 1;
                    warranty.LargestUnitRemaining = Convert.ToInt32(Math.Ceiling(warranty.EndDate.Subtract(DateTime.Today).Days / (365.25)));

                }
                else if (Convert.ToInt32(Math.Floor(warranty.EndDate.Subtract(DateTime.Today).Days / (365.25 / 12))) > 0)
                {
                    if (Convert.ToInt32(Math.Floor(warranty.EndDate.Subtract(DateTime.Today).Days / (365.25 / 12))) <= 4)
                    {
                        warranty.Priority = 2;
                    }
                    warranty.LargestUnit = "Months";
                    warranty.LargestUnitRemaining = Convert.ToInt32(Math.Floor(warranty.EndDate.Subtract(DateTime.Today).Days / (365.25 / 12)));

                }
                else if (warranty.EndDate.Subtract(DateTime.Today).Days <= 0)
                {
                    warranty.Priority = 0;
                }
                else
                {
                    warranty.LargestUnit = "Days";
                    warranty.Priority = 3;
                    warranty.LargestUnitRemaining = Convert.ToInt32(warranty.EndDate.Subtract(DateTime.Today).Days);
                }

                await database.SaveItemAsync(warranty);
            }

            ListView.ItemsSource = await database.QueryAsync("SELECT * FROM [Warranty] ORDER BY [Priority] DESC, [EndDate] ASC");
        }
    }
}