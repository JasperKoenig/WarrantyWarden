using System;
using WarrantyWarden.Models;
using WarrantyWarden.Data;
using Xamarin.Forms;
using MarcTron.Plugin;
using System.Linq;

namespace WarrantyWarden.Views
{
    public partial class WarrantyList : ContentPage
    {
        public WarrantyList()
        {
            InitializeComponent();

            CrossMTAdmob.Current.OnRewardedVideoAdLoaded += (s, args) =>
            {
                CrossMTAdmob.Current.ShowRewardedVideo();
            };
        }

        // Called when page is loaded to calculate and display warranty summaries
        protected override async void OnAppearing()
        {
            base.OnAppearing();

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

        // Plus Button clicked to add warranty
        async void OnItemAdded(object sender, EventArgs e)
        {
            WarrantyDatabase database = await WarrantyDatabase.Instance;

            var warranties = await database.GetItemsAsync();

            if (warranties.Count() > 5)
            {

            } 
            
            await Navigation.PushAsync(new AddWarranty
            {
                BindingContext = new Warranty()
            });
        }

        // Tapping on list item to view details
        async void OnListItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
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