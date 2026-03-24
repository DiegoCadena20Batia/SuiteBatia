using Microsoft.Maui.Handlers;

namespace BatiaSuite.Controls;

public partial class BordererDatePicker : ContentView {

    public static readonly BindableProperty DateProperty =
        BindableProperty.Create(nameof(Date), typeof(DateTime), typeof(BordererDatePicker), new DateTime(1900, 1, 1), BindingMode.TwoWay);

    public static readonly BindableProperty PlaceholderProperty =
        BindableProperty.Create(nameof(Placeholder), typeof(string), typeof(BordererDatePicker), string.Empty);

    public DateTime Date {
        get => (DateTime)GetValue(DateProperty);
        set => SetValue(DateProperty, value);
    }

    public string Placeholder {
        get => (string)GetValue(PlaceholderProperty);
        set => SetValue(PlaceholderProperty, value);
    }

    public BordererDatePicker() {
        InitializeComponent();
        Content.BindingContext = this;
    }

    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e) {
#if ANDROID
        var handler = datePicker.Handler as IDatePickerHandler;
        handler.PlatformView.PerformClick();
#endif
    }
}