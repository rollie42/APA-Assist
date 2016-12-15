using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(ApaScoreKeeper.OrientationHandler))]

namespace ApaScoreKeeper.Droid
{
    [Activity(Label = "ApaScoreKeeper", Icon = "@drawable/icon", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsApplicationActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);
            LoadApplication(new App());
        }
    }
}

namespace ApaScoreKeeper
{
    public class OrientationHandler : IOrientationHandler
    {
        public void ForceLandscape()
        {
            var activity = (Activity)Forms.Context;
            activity.RequestedOrientation = ScreenOrientation.Landscape;
        }

        public void ForcePortrait()
        {
            var activity = (Activity)Forms.Context;
            activity.RequestedOrientation = ScreenOrientation.Portrait;
        }

        public void PreventLock()
        {
            this.Activity.Window.AddFlags(WindowManagerFlags.KeepScreenOn); 
        }

        public void AllowLock()
        {
            this.Activity.Window.ClearFlags(WindowManagerFlags.KeepScreenOn);
        }

        private Activity Activity => (Activity)Forms.Context;
    }
}

