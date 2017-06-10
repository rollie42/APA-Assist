using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace ApaScoreKeeper.Views
{
    public partial class HomePage : ContentPage
    {
        public HomePage()
        {
            InitializeComponent();
            MyTeams.ItemsSource = new string[] { "team1", "team2", "team3" };
            AddTeamButton.Clicked += async (s, e) => {
                await Navigation.PushModalAsync(new NewTeamPage());
            };

            MyRecentMatches.ItemsSource = new string[] { "match1", "match2", "match3" };
            AddMatchButton.Clicked += async (s, e) => {
                await Navigation.PushModalAsync(new NewMatchPage());
            };
        }
    }
}
