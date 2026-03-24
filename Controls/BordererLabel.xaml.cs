using System.Windows.Input;

namespace BatiaSuite.Controls;

public partial class BordererLabel : ContentView {

    public static readonly BindableProperty TextProperty =
        BindableProperty.Create(nameof(Text), typeof(string), typeof(BordererLabel), null, BindingMode.TwoWay);

    public static readonly BindableProperty PlaceholderProperty =
        BindableProperty.Create(nameof(Placeholder), typeof(string), typeof(BordererLabel), null);

    public static readonly BindableProperty IsEnabledCommandProperty =
        BindableProperty.Create(nameof(IsEnabledCommand), typeof(bool), typeof(BordererLabel), true, BindingMode.TwoWay);

    public static readonly BindableProperty CommandProperty =
         BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(BordererLabel), null);

    public static readonly BindableProperty CommandParameterProperty =
        BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(BordererLabel), null);

    public string Text {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public string Placeholder {
        get => (string)GetValue(PlaceholderProperty);
        set => SetValue(PlaceholderProperty, value);
    }

    public bool IsEnabledCommand {
        get => (bool)GetValue(IsEnabledCommandProperty);
        set => SetValue(IsEnabledCommandProperty, value);
    }

    public ICommand Command {
        get => (ICommand)GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    public object CommandParameter {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }

    public BordererLabel() {
        InitializeComponent();
        Content.BindingContext = this;
    }

    private async void Frame_Tapped(object sender, TappedEventArgs e) {
        if(Command is null) {
            return;
        }
        Frame selectedFrame = (Frame)sender;
        selectedFrame.BackgroundColor = Color.FromArgb("#FFC8C8C8");
        await Task.Delay(100);
        selectedFrame.BackgroundColor = Color.FromArgb("#FFFFFFFF");
    }
}