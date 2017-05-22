using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ApaScoreKeeper
{
    public class MatchDataTemplate : DataTemplate
    {
        public MatchDataTemplate() : base(() => new ViewCell { View = Layout() })
        {
        }

        private static Layout Layout()
        {
            var player1Name = new Label();
            var player1Win = new Image();
            var vsLabel = new Label { Text = "vs" };
            var player2Name = new Label();
            var player2Win = new Image();

            player1Name.SetBinding(Label.TextProperty, "Player1.Name");
            player2Name.SetBinding(Label.TextProperty, "Player2.Name");

            var layout = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Children =
                {
                    player1Name,
                    vsLabel,
                    player2Name,
                }
            };

            return layout;
        }
    }
}
