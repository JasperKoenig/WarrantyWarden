using System;
using System.IO;
using WarrantyWarden.Models;
using WarrantyWarden.Data;
using WarrantyWarden.Views;
using Xamarin.Forms;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace WarrantyWarden.Views
{
    public partial class AddWarranty : ContentPage
    {
        public Warranty Warranty { get; set; }

        string WarrantyCardFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "WarrantyCards");
        string ProofOfPurchaseFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ProofOfPurchase");
        string WarrantyCardLocation = "N/A";
        string ProofOfPurchaseLocation = "N/A";

        public AddWarranty()
        {
            InitializeComponent();

            if (!Directory.Exists(WarrantyCardFolder))
            {
                Directory.CreateDirectory(WarrantyCardFolder);
            }
            if (!Directory.Exists(ProofOfPurchaseFolder))
            {
                Directory.CreateDirectory(ProofOfPurchaseFolder);
            }
        }

        void OnDateSelected(object sender, DateChangedEventArgs args)
        {
            Calculate();
        }

        void OnYearChanged(object sender, ToggledEventArgs args)
        {
            double value = ((Stepper)sender).Value;
            durationYearsLabel.Text = string.Format("{0} Years", value);

            Calculate();
        }

        void OnMonthChanged(object sender, ToggledEventArgs args)
        {
            double value = ((Stepper)sender).Value;
            durationMonthsLabel.Text = string.Format("{0} Months", value);

            Calculate();
        }

        void Calculate()
        {
            DateTime startDate = startDatePicker.Date;
            int years = Convert.ToInt32(durationYears.Value);
            int months = Convert.ToInt32(durationMonths.Value);

            DateTime endDate = startDate.AddYears(years);
            endDate = endDate.AddMonths(months);

            resultLabel.Text = string.Format("{0}", endDate.ToShortDateString());
        }

        async void OnProofButtonClicked(object sender, EventArgs e)
        {
            string action = await DisplayActionSheet("Select a Medium", "Cancel", null, "Take a Photo", "Select a Photo");
            switch (action)
            {
                case "Take a Photo":
                    ProofOfPurchaseLocation = await TakePhotoAsync(ProofOfPurchaseFolder);
                    //await DisplayAlert("Location", ProofOfPurchaseLocation, "Continue");
                    break;
                case "Select a Photo":
                    ProofOfPurchaseLocation = await PickPhotoAsync(ProofOfPurchaseFolder);
                    //await DisplayAlert("Location", ProofOfPurchaseLocation, "Continue");
                    break;
            }
        }

        async void OnWarrantyButtonClicked(object sender, EventArgs e)
        {
            string action = await DisplayActionSheet("Select a Medium", "Cancel", null, "Take a Photo", "Select a Photo");
            switch (action)
            {
                case "Take a Photo":
                    WarrantyCardLocation = await TakePhotoAsync(WarrantyCardFolder);
                    //await DisplayAlert("Location", WarrantyCardLocation, "Continue");
                    break;
                case "Select a Photo":
                    WarrantyCardLocation = await PickPhotoAsync(WarrantyCardFolder);
                    //await DisplayAlert("Location", WarrantyCardLocation, "Continue");
                    break;
            }
        }

        async void OnSaveButtonClicked(object sender, EventArgs e)
        {
            Warranty newWarranty = new Warranty()
            {
                Product = productName.Text,
                StartDate = startDatePicker.Date,
                EndDate = DateTime.Parse(resultLabel.Text),
                Months = Convert.ToInt32(durationMonths.Value),
                Years = Convert.ToInt32(durationYears.Value),
                WarrantyCardLocation = WarrantyCardLocation,
                ProofOfPurchaseLocation = ProofOfPurchaseLocation
            };

            WarrantyDatabase database = await WarrantyDatabase.Instance;
            await database.SaveItemAsync(newWarranty);
            await Navigation.PopAsync();
        }

        async void OnCancelButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        async Task<string> TakePhotoAsync(string location)
        {
            try
            {
                var photo = await MediaPicker.CapturePhotoAsync();
                return await LoadPhotoAsync(photo, location);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Something went wrong", "Please ensure the app has camera permissions and try again.", "Cancel");
                return "N/A";
            }
        }

        async Task<string> LoadPhotoAsync(FileResult photo, string location)
        {
            try
            {
                // canceled
                if (photo == null)
                {
                    return "N/A";
                }
                // save the file into local storage
                var newFile = Path.Combine(location, photo.FileName);
                using (var stream = await photo.OpenReadAsync())
                using (var newStream = File.OpenWrite(newFile))
                    await stream.CopyToAsync(newStream);

                return newFile;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Something went wrong", "Please ensure the app has storage permissions and try again.", "Cancel");
                return "N/A";
            }
        }

        async Task<String> PickPhotoAsync(string location)
        {
            try
            {
                var photo = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
                {
                    Title = "Please choose a photo:"
                });

                return await LoadPhotoAsync(photo, location);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Something went wrong", "Please ensure the app has camera permissions and try again.", "Cancel");
                return "N/A";
            }
        }
    }
}