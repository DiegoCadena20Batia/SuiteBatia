using System.Windows.Input;
using BatiaSuite.Utils;

namespace BatiaSuite.Controls;

public partial class MasterPage : ContentPage {

    /* Las Pages que extiendan de esta clase deben de realizar el enlace de datos tanto del Content como del MasterPageContent :
       BindingContext = _viewModel;
       MasterPageContent.BindingContext = _viewModel;

       Además en las propiedades en el XAML se debe especificar el FlyoutBehavior:
       Shell.FlyoutBehavior = Disable|Flyout|Locked
    */


    public static readonly BindableProperty MasterPageTitleProperty =
        BindableProperty.Create(nameof(MasterPageTitle), typeof(string), typeof(MasterPage), string.Empty);

    public static readonly BindableProperty IsLoadingProperty =
        BindableProperty.Create(nameof(IsLoading), typeof(bool), typeof(MasterPage), false);

    public static readonly BindableProperty LoadingTextProperty =
        BindableProperty.Create(nameof(LoadingText), typeof(string), typeof(MasterPage), string.Empty);

    public static readonly BindableProperty IsBackButtonProperty =
        BindableProperty.Create(nameof(IsBackButton), typeof(bool), typeof(MasterPage), true);

    public static readonly BindableProperty IsLastPageProperty =
        BindableProperty.Create(nameof(IsLastPage), typeof(bool), typeof(MasterPage), false);

    public static readonly BindableProperty ShowFloatingButtonProperty =
        BindableProperty.Create(nameof(ShowFloatingButton), typeof(bool), typeof(MasterPage), true);

    public static readonly BindableProperty FloatingButtonCommandProperty =
        BindableProperty.Create(nameof(FloatingButtonCommand), typeof(ICommand), typeof(MasterPage), null);

    public static readonly BindableProperty MasterPageContentProperty =
        BindableProperty.Create(nameof(MasterPageContent), typeof(View), typeof(MasterPage), null);

    public string MasterPageTitle {
        get => (string)GetValue(MasterPageTitleProperty);
        set => SetValue(MasterPageTitleProperty, value);
    }

    public bool IsLoading {
        get => (bool)GetValue(IsLoadingProperty);
        set => SetValue(IsLoadingProperty, value);
    }

    public string LoadingText {
        get => (string)GetValue(LoadingTextProperty);
        set => SetValue(LoadingTextProperty, value);
    }

    public bool IsBackButton {
        get => (bool)GetValue(IsBackButtonProperty);
        set => SetValue(IsBackButtonProperty, value);
    }

    public bool IsLastPage {
        get => (bool)GetValue(IsLastPageProperty);
        set => SetValue(IsLastPageProperty, value);
    }

    public bool ShowFloatingButton {
        get => (bool)GetValue(ShowFloatingButtonProperty);
        set => SetValue(ShowFloatingButtonProperty, value);
    }

    public ICommand FloatingButtonCommand {
        get => (ICommand)GetValue(FloatingButtonCommandProperty);
        set => SetValue(FloatingButtonCommandProperty, value);
    }

    public View MasterPageContent {
        get => (View)GetValue(MasterPageContentProperty);
        set => SetValue(MasterPageContentProperty, value);
    }

    public MasterPage() {
        InitializeComponent();
        Content.BindingContext = this;
    }

    private async void menuIcon_Clicked(object sender, EventArgs e) {
        if(IsBackButton) {
            await Constants.GoToAsync("..");
            return;
        }

        Shell.Current.FlyoutIsPresented = true;
    }

}