using System;
using WarrantyWarden.Models;
using WarrantyWarden.Data;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

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
    }
}