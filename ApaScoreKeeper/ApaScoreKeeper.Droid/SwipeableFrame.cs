using System;
using Android.Views;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using ApaScoreKeeper;

[assembly: ExportRenderer(typeof(SwipeableFrame), typeof(SwipeableFrameAndroid))]
namespace ApaScoreKeeper
{
    public class SwipeableFrameAndroid : Xamarin.Forms.Platform.Android.FrameRenderer
    {
        public float X1 { get; set; }
        public float X2 { get; set; }
        public float Y1 { get; set; }
        public float Y2 { get; set; }

        public SwipeableFrame SwipeableFrame { get; set; }

        public override bool OnTouchEvent(MotionEvent e)
        {

            if (e.ActionMasked == MotionEventActions.Down)
            {
                X1 = e.GetX();
                Y1 = e.GetY();

                return true;
            }

            X2 = e.GetX();
            Y2 = e.GetY();

            var xChange = X1 - X2;
            var yChange = Y1 - Y2;

            var xChangeSize = Math.Abs(xChange);
            var yChangeSize = Math.Abs(yChange);

            if (xChangeSize > yChangeSize)
            {
                // horizontal
                if (X1 > X2)
                {
                    // left
                    SwipeableFrame.RaiseSwipedLeft();
                }
                else
                {
                    // right
                    SwipeableFrame.RaiseSwipedRight();
                }
            }
            else
            {
                // vertical
                if (Y1 > Y2)
                {
                    // up
                    SwipeableFrame.RaiseSwipedUp();
                }
                else
                {
                    // down
                    SwipeableFrame.RaiseSwipedDown();
                }
            }

            return base.OnTouchEvent(e);
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Frame> ev)
        {
            base.OnElementChanged(ev);

            SwipeableFrame = (SwipeableFrame)ev.NewElement;
        }
    }
}