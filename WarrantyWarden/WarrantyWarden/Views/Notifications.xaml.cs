using System;
using WarrantyWarden.Models;
using WarrantyWarden.Data;
using Xamarin.Forms;


namespace WarrantyWarden.Views
{
    public partial class Notifications : ContentPage
    {
        INotificationManager notificationManager;

        public Notifications()
        {
            InitializeComponent();

            notificationManager = DependencyService.Get<INotificationManager>();
            notificationManager.NotificationReceived += (sender, eventArgs) =>
            {
                var evtData = (NotificationEventArgs)eventArgs;
            };
        }

        void OnMonthChanged(object sender, ToggledEventArgs args)
        {
            double value = ((Stepper)sender).Value;
            Months.Text = string.Format("{0} Months", value);
        }

        async void OnSaveButtonClicked(object sender, EventArgs e)
        {
            WarrantyDatabase database = await WarrantyDatabase.Instance;

            var warranties = await database.GetItemsAsync();


            foreach (Warranty warranty in warranties)
            {
                if (warranty.Priority != 0)
                {
                    string title = "Warranty Warden";
                    string message = $"Your {warranty.Product} warranty will expire in less than {Months.Text}";
                    notificationManager.DeleteAllNotifications();
                    notificationManager.SendNotification(title, message, (warranty.EndDate.AddMonths(Convert.ToInt32(MonthsStepper.Value) * -1)));
                }
            }
        }

        public void OnCancelButtonClicked(object sender, EventArgs e)
        {
            notificationManager.DeleteAllNotifications();
        }
    }
}