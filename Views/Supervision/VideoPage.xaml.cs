using BatiaSuite.Controls;
using BatiaSuite.ViewModel.Supervision;

namespace BatiaSuite.Views.Supervision;

public partial class VideoPage : MasterPage {

    VideoViewModel _viewModel;

    public VideoPage() {
        InitializeComponent();
        _viewModel = new VideoViewModel(mediaElement);
        BindingContext = _viewModel;
        MasterPageContent.BindingContext = _viewModel;
    }

    protected override void OnAppearing() {
        base.OnAppearing();
        _viewModel.DisplaceScroll += DisplaceScrollView;
        
    }

    protected override void OnDisappearing() {
        base.OnDisappearing();
        _viewModel.DisplaceScroll -= DisplaceScrollView;
    }

    private void DisplaceScrollView(object? sender, EventArgs e) {
        scrollView.ScrollToAsync(labelVideo, ScrollToPosition.End, true);
    }
    private async void OnCapturarVideoClicked(object sender, EventArgs e)
    {
        if (BindingContext is VideoViewModel vm)
        {
            await vm.CapturarVideoCommand.ExecuteAsync(CameraView);
        }
    }
}