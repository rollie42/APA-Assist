using ApaScoreKeeper.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ApaScoreKeeper
{
    public class LeagueMatchPage : ContentPage
    {
        const int buttonWidth = 80;
        const int buttonHeight = 80;

        private Label NotificationLabel = new Label { FontSize = 32 };

        private MatchVM MatchVM { get; set; }

        private Match Match => this.MatchVM.Match;

        public LeagueMatchPage(Player p1, Player p2) : this(new Match(p1, p2))
        {            
        }

        public LeagueMatchPage(Match match)
        {
            match.GameEvents.CollectionChanged += async (sender, e) =>
            {
                await LocalStorage.Write(match);
            };

            var orientationHandler = DependencyService.Get<IOrientationHandler>();
            orientationHandler.ForceLandscape();

            MatchVM = new LeagueMatchVM(match);

            var swipableFrame = new SwipeableFrame
            {
                Content = new StackLayout
                {
                    Orientation = StackOrientation.Vertical,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    Children = {
                        TopStack(),
                        BottomStack(),
                    }
                }
            };

            Content = swipableFrame;

            swipableFrame.SwipedLeft += async (sender, e) =>
            {
                if (MatchVM.ActivePlayer == MatchVM.Player2)
                {
                    MatchVM.Miss();
                    await Notify("Miss");
                }
            };

            swipableFrame.SwipedRight += async (sender, e) =>
            {
                if (MatchVM.ActivePlayer == MatchVM.Player1)
                {
                    MatchVM.Miss();
                    await Notify("Miss");
                }
            };

            this.Appearing += (sender, e) =>
            {
                orientationHandler.PreventLock();
            };

            this.Disappearing += (sender, e) =>
            {
                orientationHandler.AllowLock();
            };
        }

        private Layout TopStack()
        {
            var layout = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Children = {
                        PlayerStack(MatchVM.Player1, "P1"),
                        TopMiddleStack(),
                        PlayerStack(MatchVM.Player2, "P2"),
                    }
            };

            return layout;
        }

        private Layout BottomStack()
        {            
            var manualCorrectButton = new Button { Image = "edit.png", BackgroundColor = Color.Transparent };
            var undoButton = new Button { Image = "undo.png", BackgroundColor = Color.Transparent };
            var earlyNineButton = new Button { Image = "nine.png", BackgroundColor = Color.Transparent };
            var matchSummaryButton = new Button { Image = "grid.png", BackgroundColor = Color.Transparent };
            var settingsButton = new Button { Image = "settings.png", BackgroundColor = Color.Transparent };

            undoButton.Clicked += (s, e) =>
            {
                MatchVM.Undo();
            };

            earlyNineButton.Clicked += (sender, e) =>
            {
                while (MatchVM.BallsOnTable > 1)
                {
                    MatchVM.DeadBall();                    
                }

                MatchVM.AddPoint();
                MatchVM.AddPoint();
            };

            var layout = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.Center,
                Children = {
                        manualCorrectButton,
                        undoButton,
                        earlyNineButton,
                        matchSummaryButton,
                        settingsButton,
                    }
            };

            return layout;
        }
        
        private StackLayout PlayerStack(Player player, string playerSuffix)
        {            
            var safetiesLabel = NewBoundLabel();
            var deadBallsLabel = NewBoundLabel();                        

            safetiesLabel.SetBinding(Label.TextProperty, "Safeties" + playerSuffix, BindingMode.Default, null, "Safeties: {0}");
            deadBallsLabel.SetBinding(Label.TextProperty, "DeadBalls" + playerSuffix, BindingMode.Default, null, "Dead balls: {0}");

            safetiesLabel.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(async () => {
                    if (MatchVM.ActivePlayer == player)
                    {
                        MatchVM.Safety();
                        await Notify("Safety");
                    }
                }),
            });

            deadBallsLabel.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(async () => {
                    if (MatchVM.ActivePlayer == player)
                        MatchVM.DeadBall();
                        await Notify("Dead ball");
                }),
            });

            return new StackLayout
            {
                Padding = new Thickness(20, 0),
                Children = {
                    PlayerNameStack(player, playerSuffix),
                    PointsStack(player, playerSuffix),
                    safetiesLabel,
                    deadBallsLabel,
                }
            };
        }

        private Layout PlayerNameStack(Player player, string playerSuffix)
        {
            var nameLabel = NewBoundLabel(34, player.Name);
            nameLabel.HorizontalTextAlignment = TextAlignment.Center;
            nameLabel.SetBinding(Label.TextColorProperty, "ActivePlayer" + playerSuffix, BindingMode.Default, new LabelTextColorConverter());

            var timeOutLabel1 = new Button
            {
                FontSize = 10,
                BindingContext = MatchVM,
                Text = "T",
                TextColor = Color.Teal,
                BackgroundColor = Color.Transparent,
                WidthRequest = 30,
            };

            timeOutLabel1.Clicked += (sender, e) =>
            {
                if (MatchVM.ActivePlayer == player)
                {
                    MatchVM.TimeOut();
                }
            };

            timeOutLabel1.SetBinding(Button.IsVisibleProperty, "T1Visible" + playerSuffix);

            var timeOutLabel2 = new Button
            {
                FontSize = 10,
                BindingContext = MatchVM,
                Text = "T",
                TextColor = Color.Teal,
                BackgroundColor = Color.Transparent,
                WidthRequest = 30,
            };

            timeOutLabel2.Clicked += (sender, e) =>
            {
                if (MatchVM.ActivePlayer == player)
                {
                    MatchVM.TimeOut();
                }
            };

            timeOutLabel2.SetBinding(Button.IsVisibleProperty, "T2Visible" + playerSuffix);

            var layout = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.Fill,
                Children = {
                        nameLabel,
                        timeOutLabel1,
                        timeOutLabel2,
                    }
            };

            return layout;
        }

        private Layout PointsStack(Player player, string playerSuffix)
        {
            var pointsLabel = NewBoundLabel(80);
            pointsLabel.SetBinding(Label.TextProperty, "Points" + playerSuffix);
            pointsLabel.HorizontalTextAlignment = TextAlignment.End;

            var goalLabel = NewBoundLabel(17);
            goalLabel.VerticalTextAlignment = TextAlignment.End;
            goalLabel.HorizontalTextAlignment = TextAlignment.Start;
            goalLabel.Text = "/" + player.PointsToWinMatch;

            var layout = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Children = {
                        pointsLabel,
                        goalLabel,
                    }
            };

            layout.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(async () => {
                    if (MatchVM.ActivePlayer == player)
                    {
                        MatchVM.AddPoint();
                        if (player.PointsToWinMatch - MatchVM.Points(player) == 5)
                        {
                            await Notify("5 more!");
                        }
                        else if (player.PointsToWinMatch - MatchVM.Points(player) == 3)
                        {
                            await Notify("3 more!");
                        }
                        else if (player.PointsToWinMatch - MatchVM.Points(player) == 1)
                        {
                            await Notify("Game ball!");
                        }
                        else
                        {
                            await Notify("Point");
                        }


                    }
                }),
            });

            return layout;
        }

        private Layout TopMiddleStack()
        {            
            NotificationLabel.TextColor = Color.Red;
            NotificationLabel.HorizontalTextAlignment = TextAlignment.Center;
            NotificationLabel.VerticalTextAlignment = TextAlignment.Center;
            NotificationLabel.VerticalOptions = LayoutOptions.FillAndExpand;
            NotificationLabel.Opacity = 0;

            var layout = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Padding = new Thickness(20, 0),
                Children = {
                    GameStatsStack(),
                    NotificationLabel,
                }
            };

            return layout;
        }

        private Layout GameStatsStack()
        {
            var gameLabel = NewBoundLabel();
            var inningLabel = NewBoundLabel();
            var ballsOnTableLabel = NewBoundLabel();

            gameLabel.HorizontalTextAlignment = TextAlignment.Center;
            inningLabel.HorizontalTextAlignment = TextAlignment.Center;
            ballsOnTableLabel.HorizontalTextAlignment = TextAlignment.Center;

            gameLabel.SetBinding(Label.TextProperty, "GameNumber");
            ballsOnTableLabel.SetBinding(Label.TextProperty, "BallsOnTable");
            inningLabel.SetBinding(Label.TextProperty, "InningNumber");

            var gameTextLabel = NewBoundLabel(19, "Game");
            var ballsOnTableTextLabel = NewBoundLabel(19, "Balls");
            var inningTextLabel = NewBoundLabel(19, "Inning");            

            gameTextLabel.HorizontalTextAlignment = TextAlignment.Center;
            inningTextLabel.HorizontalTextAlignment = TextAlignment.Center;
            ballsOnTableTextLabel.HorizontalTextAlignment = TextAlignment.Center;

            var gameStack = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                Padding = new Thickness(6, 0),
                Children = {
                    gameTextLabel,
                    gameLabel
                }
            };

            var ballsOnTableStack = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                Padding = new Thickness(6, 0),
                Children = {
                    ballsOnTableTextLabel,
                    ballsOnTableLabel
                }
            };

            var inningStack = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                Padding = new Thickness(6, 0),
                Children = {
                    inningTextLabel,
                    inningLabel
                }
            };

            var layout = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Children = {
                    gameStack,
                    ballsOnTableStack,
                    inningStack,
                }
            };
            
            return layout;
        }        

        private Label NewBoundLabel(int fontSize = 19, string text = "")
        {
            return new Label
            {
                FontSize = fontSize,
                BindingContext = MatchVM,
                Text = text,
            };
        }

        private async Task Notify(string text)
        {
            NotificationLabel.Text = text;
            await NotificationLabel.FadeTo(1, 300, Easing.CubicInOut);
            await Task.Delay(400);
            await NotificationLabel.FadeTo(0, 100, Easing.CubicInOut);
            NotificationLabel.Text = string.Empty;
        }
    }
}
