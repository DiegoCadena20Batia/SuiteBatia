using Android.App;
using Android.Content.PM;
using Android.OS;
// ✅ NECESARIO para MauiMaps.Init
using Mopups.Interfaces;
using Mopups.Services;

namespace BatiaSuite;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density | ConfigChanges.Orientation)]
public class MainActivity : MauiAppCompatActivity {
    protected override void OnCreate(Bundle savedInstanceState) {
        base.OnCreate(savedInstanceState);
        //CrossFingerprint.SetCurrentActivityResolver(() => this);
    }
}
