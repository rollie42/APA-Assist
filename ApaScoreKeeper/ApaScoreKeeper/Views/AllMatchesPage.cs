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
        public Button ClearLocalStorageBtn { get; set; } = new Button() { Text = "Clear Local Storage" };
        public ListView MatchesList { get; set; } = new ListView() { RowHeight = 40 };

        public AllMatchesPage()
        {
            MatchesList.ItemsSource = Task.Run(async () => await LocalStorage.GetRecentMatches()).Result;
            MatchesList.ItemTemplate = new MatchDataTemplate();

            NewMatchBtn.Clicked += async (sender, e) => {
                await Navigation.PushModalAsync(new NewMatchPage());
            };

            ClearLocalStorageBtn.Clicked += async (sender, e) => {
                await LocalStorage.ClearLocalStorage();
            };

            Content = new StackLayout
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                Children = {                        
                        new Label {
                            HorizontalTextAlignment = TextAlignment.Center,
                            Text = "Welcome to Xamarin Forms!"
                        },
                        MatchesList,
                        NewMatchBtn,
                        ClearLocalStorageBtn,
                    }
            };
        }
    }
}
