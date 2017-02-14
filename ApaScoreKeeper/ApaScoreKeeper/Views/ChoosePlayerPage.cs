using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ApaScoreKeeper
{
    public class ChoosePlayerPage : ContentPage
    {
        private TaskCompletionSource<Player> _tcs = new TaskCompletionSource<Player>();
        public ChoosePlayerPage(List<Team> knownTeams, IList<Player> knownPlayers)
        {
            knownTeams = knownTeams ?? new List<Team>();
            knownPlayers = knownPlayers ?? new List<Player>();                        

            var content = new StackLayout
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                Children = { }
            };

            var newPlayerButton = new Button
            {
                Text = "New Player",
            };

            newPlayerButton.Clicked += async (s, e) =>
            {
                var player = await NewPlayerPage.GetPlayerModal(this.Navigation);

                // Add to global list of known players
                ApplicationProperties.KnownPlayers.Add(player);
                _tcs.SetResult(player);
            };

            // TODO: support teams
            // TODO: add image to row, color, size, etc
            var playerListView = new ListView { ItemsSource = knownPlayers };
            playerListView.ItemTemplate = new DataTemplate(typeof(ImageCell));
            playerListView.ItemTemplate.SetBinding(ImageCell.TextProperty, "Name");
            playerListView.ItemTemplate.SetBinding(ImageCell.DetailProperty, "Skill");

            playerListView.ItemSelected += (s, e) =>
            {
                if (e.SelectedItem == null)
                {
                    return;
                }

                var player = (Player)e.SelectedItem;
                _tcs.SetResult(player);
            };

            content.Children.Add(playerListView);
            content.Children.Add(newPlayerButton);

            Content = content;
        }

        static public async Task<Player> GetPlayerModal(INavigation navigation, List<Team> knownTeams, IList<Player> knownPlayers)
        {
            var page = new ChoosePlayerPage(knownTeams, knownPlayers);
            await navigation.PushModalAsync(page);
            var player = await page._tcs.Task;
            await navigation.PopModalAsync();
            return player;
        }
    }
}
