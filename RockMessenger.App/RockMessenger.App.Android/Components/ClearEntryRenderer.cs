using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using RockMessenger.App.Components;
using RockMessenger.App.Droid.Components;

[assembly:ExportRenderer(typeof(ClearEntry), typeof(ClearEntryRenderer))]
namespace RockMessenger.App.Droid.Components
{
    public class ClearEntryRenderer : EntryRenderer
    {
        public ClearEntryRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                Control.Background = new ColorDrawable(Android.Graphics.Color.Transparent);
            }
        }
    }
}