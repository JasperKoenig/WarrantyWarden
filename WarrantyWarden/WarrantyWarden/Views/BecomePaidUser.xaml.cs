using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WarrantyWarden.Views
{
    public partial class BecomePaidUser : ContentPage
    {
        public BecomePaidUser()
        {
            InitializeComponent();
        }

        public void BecomePaid(object sender, EventArgs e)
        {
            Preferences.Set("PaidUser", true);
        }

        public void BecomeFree(object sender, EventArgs e)
        {
            Preferences.Set("PaidUser", false);
        }
    }
}