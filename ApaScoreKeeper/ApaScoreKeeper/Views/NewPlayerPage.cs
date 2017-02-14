using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ApaScoreKeeper
{
    public class NewPlayerPage : ContentPage
    {
        private TaskCompletionSource<Player> _tcs = new TaskCompletionSource<Player>();
        private Button DoneButton { get; set; } = new Button { Text = "Done" };
        private Entry NameEntry { get; } = new Entry();
        private Picker SkillEntry { get; } = new Picker() { };
        public NewPlayerPage()
        {
            DoneButton.Clicked += DoneButton_Clicked;
            Constants.TimeOutsForSkill.Keys.ForEach(s => SkillEntry.Items.Add(s.ToString()));
            SkillEntry.SelectedIndex = 3;
            Content = new StackLayout
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                Children = {
                        new Label {
                            Text = "Name"
                        },
                        NameEntry,
                        new Label {
                            Text = "Skill"
                        },
                        SkillEntry,
                        DoneButton,
                    }
            };
        }

        private void DoneButton_Clicked(object sender, EventArgs e)
        {
            var player = new Player
            {
                Name = NameEntry.Text,
                Skill = int.Parse(SkillEntry.Items[SkillEntry.SelectedIndex]),
            };

            _tcs.SetResult(player);
        }        

        static public async Task<Player> GetPlayerModal(INavigation navigation)
        {
            var page = new NewPlayerPage();
            await navigation.PushModalAsync(page);
            var player = await page._tcs.Task;
            await navigation.PopModalAsync();
            return player;
        }
    }
}
