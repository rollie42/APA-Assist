using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ApaScoreKeeper
{
    public class SwipeableFrame : Frame
    {
        public event EventHandler SwipedUp;
        public event EventHandler SwipedDown;
        public event EventHandler SwipedLeft;
        public event EventHandler SwipedRight;

        public void RaiseSwipedUp()
        {
            SwipedUp?.Invoke(this, new EventArgs());
        }

        public void RaiseSwipedDown()
        {
            SwipedDown?.Invoke(this, new EventArgs());
        }

        public void RaiseSwipedLeft()
        {
            SwipedLeft?.Invoke(this, new EventArgs());
        }

        public void RaiseSwipedRight()
        {
            SwipedRight?.Invoke(this, new EventArgs());
        }
    }
}