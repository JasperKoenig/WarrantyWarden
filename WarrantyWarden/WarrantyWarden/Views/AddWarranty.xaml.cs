using System;
using System.IO;
using WarrantyWarden.Models;
using WarrantyWarden.Data;
using WarrantyWarden.Views;
using Xamarin.Forms;

namespace WarrantyWarden.Views
{
    public partial class AddWarranty : ContentPage
    {
        public Warranty Warranty { get; set; }

        public AddWarranty()
        {
            InitializeComponent();
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

        async void OnSaveButtonClicked(object sender, EventArgs e)
        {
            Warranty newWarranty = new Warranty()
            {
                Product = productName.Text,
                StartDate = startDatePicker.Date,
                EndDate = DateTime.Parse(resultLabel.Text),
            };

            WarrantyDatabase database = await WarrantyDatabase.Instance;
            await database.SaveItemAsync(newWarranty);
            await Navigation.PopAsync();
        }

        async void OnCancelButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}