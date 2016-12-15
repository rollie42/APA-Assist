using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ApaScoreKeeper
{
    public class NewMatchPage : ContentPage
    {
        public Player Player1 { get; set; }
        public Player Player2 { get; set; }

        public NewMatchPage()
        {
            var player1Button = new Button
            {
                Text = "Player 1",
            };

            player1Button.Clicked += async (s, e) =>
            {                
                var player = await ChoosePlayerPage.GetPlayerModal(this.Navigation, null, ApplicationProperties.KnownPlayers);                
                ((Button)s).Text = player.Name;
                Player1 = player;
            };

            var player2Button = new Button
            {
                Text = "Player 2",
            };

            player2Button.Clicked += async (s, e) =>
            {
                var player = await ChoosePlayerPage.GetPlayerModal(this.Navigation, null, ApplicationProperties.KnownPlayers);
                ((Button)s).Text = player.Name;
                Player2 = player;
            };

            // Load from previous match, if available
            var lastMatchPlayer1 = ApplicationProperties.LastQuickMatchPlayer1();
            var lastMatchPlayer2 = ApplicationProperties.LastQuickMatchPlayer2();
            if (lastMatchPlayer1 != null)
            {
                player1Button.Text = lastMatchPlayer1.Name;
                Player1 = lastMatchPlayer1;
                player2Button.Text = lastMatchPlayer2.Name;
                Player2 = lastMatchPlayer2;
            }

            var vsLabel = new Label
            {
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                Text = "vs",
            };

            var playerVsPlayerRow = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Children =
                {
                    player1Button,
                    vsLabel,
                    player2Button,
                },
            };

            var matchTypeLabel = new Label
            {
                Text = "Match type",
            };

            var matchTypeDropdown = new Picker { };
            matchTypeDropdown.Items.Add("Quick Game");
            matchTypeDropdown.Items.Add("Match");
            matchTypeDropdown.SelectedIndex = 0;

            var matchTypeRow = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Children =
                {
                    matchTypeLabel,
                    matchTypeDropdown,
                }
            };

            var playButton = new Button { Text = "Play" };
            var resumeButton = new Button { Text = "Reusme previous" };

            playButton.Clicked += async (s, e) =>
            {
                if (matchTypeDropdown.Items[matchTypeDropdown.SelectedIndex] == "Quick Game")
                {
                    // Save these settings for next quickmatch
                    await ApplicationProperties.LastQuickMatchPlayer1(Player1);
                    await ApplicationProperties.LastQuickMatchPlayer2(Player2);
                }

                var match = new Match(Player1, Player2);
                await LocalStorage.AddRecentMatch(match);
                await Navigation.PushModalAsync(new MatchPage(match));
            };

            // TODO: disable if empty
            resumeButton.Clicked += async (s, e) =>
            {
                await Navigation.PushModalAsync(new MatchPage(await LocalStorage.GetMostRecentMatch()));
            };

            Content = new StackLayout
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                Children = {
                    playerVsPlayerRow,
                    matchTypeRow,
                    playButton,
                    resumeButton,
                }
            };
        }       
    }
}
