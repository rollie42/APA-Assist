using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApaScoreKeeper
{
    public interface IOrientationHandler
    {
        void ForceLandscape();

        void ForcePortrait();

        void PreventLock();

        void AllowLock();
    }
}
