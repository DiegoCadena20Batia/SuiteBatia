using CommunityToolkit.Mvvm.Input;
using Mopups.Services;

namespace BatiaSuite.ViewModel.VersionApp {

    public partial class VersionAppViewModel {

        public VersionAppViewModel() {
        }

        [RelayCommand]
        private async Task VerTienda() {
            try {
#if ANDROID
                await Launcher.OpenAsync("market://details?id=com.GrupoBatia.SuiteBatia");
                System.Diagnostics.Process.GetCurrentProcess().Kill();
#elif IOS
        await Launcher.OpenAsync("itms-apps://itunes.apple.com/app/id6737812258");
#endif
            } catch(Exception ex) {
                Console.WriteLine("ocurio un error al abrir la tienda de aplicaciones :" + ex.Message);
            }
        }

        [RelayCommand]
        private async Task Cancelar() {
            MopupService.Instance.PopAsync();
        }
    }
}