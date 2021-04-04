using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WarrantyWarden.Views
{
    public partial class CalendarSync : ContentPage
    {
        var calendarsUri = CalendarContract.Calendars.ContentUri; 
        
        public CalendarSync()
        {
            InitializeComponent();
        }
    }
}