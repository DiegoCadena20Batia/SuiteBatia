using Mopups.Services;

namespace BatiaSuite.Popups.RutasEntregas;

public partial class ConfirmarInicioRutaPopup : Mopups.Pages.PopupPage {
    private TaskCompletionSource<bool> _taskCompletionSource;

    public Task<bool> PopupResult => _taskCompletionSource.Task;

    public ConfirmarInicioRutaPopup() {
        InitializeComponent();
        _taskCompletionSource = new TaskCompletionSource<bool>();
    }

    private async void OnConfirmarClicked(object sender, EventArgs e) {
        await MopupService.Instance.PopAsync();

        _taskCompletionSource.TrySetResult(true);
    }

    private async void OnCancelarClicked(object sender, EventArgs e) {
        await MopupService.Instance.PopAsync();

        _taskCompletionSource.TrySetResult(false);
    }

    protected override bool OnBackgroundClicked() {
        _taskCompletionSource.TrySetResult(false);
        return base.OnBackgroundClicked();
    }

    protected override bool OnBackButtonPressed() {
        _taskCompletionSource.TrySetResult(false);
        return base.OnBackButtonPressed();
    }
}
