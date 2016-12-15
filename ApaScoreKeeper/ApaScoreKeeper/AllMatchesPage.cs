using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ApaScoreKeeper
{
    public class AllMatchesPage : ContentPage
    {
        public Button NewMatchBtn { get; set; } = new Button() { Text = "New Match" };
        public Button QuickGameBtn { get; set; } = new Button() { Text = "Quick Game" };
        public ListView MatchesList { get; set; } = new ListView() { RowHeight = 40 };

        public AllMatchesPage(IEnumerable<string> matches)
        {
            MatchesList.ItemsSource = matches;
            
            QuickGameBtn.Clicked += async (sender, e) => {
                await Navigation.PushModalAsync(new NewMatchPage());
                //var player1 = new Player() { Name = "Ryan", Skill = 5 };
                //var player2 = new Player() { Name = "Amjad", Skill = 3 };
                //player1 = await NewPlayerPage.GetPlayerModal(this.Navigation);
                //player2 = await NewPlayerPage.GetPlayerModal(this.Navigation);
                //await Navigation.PushModalAsync(new MatchPage(player1, player2));
            };

            Content = new StackLayout
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                Children = {                        
                        new Label {
                            HorizontalTextAlignment = TextAlignment.Center,
                            Text = "Welcome to Xamarin Forms!"
                        },
                        new ListView { RowHeight = 40,
                            ItemsSource = matches },
                        NewMatchBtn,
                        QuickGameBtn,
                    }
            };
        }
    }
}
