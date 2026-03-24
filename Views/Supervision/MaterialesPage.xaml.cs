using BatiaSuite.Controls;
using BatiaSuite.ViewModel.Supervision;

namespace BatiaSuite.Views.Supervision;

public partial class MaterialesPage : MasterPage {

    MaterialesViewModel _viewModel;

    public MaterialesPage() {
        InitializeComponent();
        _viewModel = new MaterialesViewModel();
        BindingContext = _viewModel;
        MasterPageContent.BindingContext = _viewModel;
    }

    protected override void OnAppearing() {
        base.OnAppearing();
        //_viewModel.DisplaceScroll += DisplaceScrollView;
    }

    protected override void OnDisappearing() {
        base.OnDisappearing();
        //_viewModel.DisplaceScroll -= DisplaceScrollView;
    }

    //private void DisplaceScrollView(object? sender, EventArgs e) {
    //    scrollView.ScrollToAsync(labelVideo, ScrollToPosition.End, true);
    //}
}