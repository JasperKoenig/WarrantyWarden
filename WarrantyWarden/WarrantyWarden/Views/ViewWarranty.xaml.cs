using System;
using WarrantyWarden.Models;
using WarrantyWarden.Data;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.IO;
using Xamarin.Essentials;
using System.Collections.Generic;

namespace WarrantyWarden.Views
{
    public partial class ViewWarranty : ContentPage
    {
        public ViewWarranty()
        {
            InitializeComponent();
        }

        async void OnBackButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        async void OnShareButtonClicked(object sender, EventArgs e)
        {
            Warranty warranty = (Warranty)BindingContext;

            if (warranty.ProofOfPurchaseLocation == "N/A" && warranty.WarrantyCardLocation == "N/A") 
            {
                await DisplayAlert("Error", "There are no images to share", "OK");
            }
            else if (warranty.WarrantyCardLocation == "N/A")
            {
                var file = new ShareFile(warranty.ProofOfPurchaseLocation); 
                
                await Share.RequestAsync(new ShareFileRequest
                {
                    Title = "Share Image",
                    File = file
                });
            }
            else if (warranty.ProofOfPurchaseLocation == "N/A")
            {
                var file = new ShareFile(warranty.WarrantyCardLocation);

                await Share.RequestAsync(new ShareFileRequest
                {
                    Title = "Share Image",
                    File = file
                });
            }
            else
            {
                var file1 = warranty.ProofOfPurchaseLocation;
                var file2 = warranty.WarrantyCardLocation;

                var filesToShare = new List<ShareFile>();
                filesToShare.Add(new ShareFile(file1));
                filesToShare.Add(new ShareFile(file2));

                await Share.RequestAsync(new ShareMultipleFilesRequest
                {
                    Title = "ShareFilesTitle",
                    Files = filesToShare
                });
            }
        }
    }
}