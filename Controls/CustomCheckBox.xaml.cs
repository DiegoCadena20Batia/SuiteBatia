using System.Windows.Input;

namespace BatiaSuite.Controls;

public partial class CustomCheckBox : ContentView {

    public static readonly BindableProperty TextProperty =
        BindableProperty.Create(nameof(Text), typeof(string), typeof(CustomCheckBox), null, BindingMode.TwoWay);

    public static readonly BindableProperty IsCheckedProperty =
        BindableProperty.Create(nameof(IsChecked), typeof(bool), typeof(CustomCheckBox), false, BindingMode.TwoWay);


    public static readonly BindableProperty CommandProperty =
        BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(OptionsView), null, BindingMode.TwoWay);

    public static readonly BindableProperty CommandParameterProperty =
        BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(OptionsView), null);

    public bool IsChecked {
        get => (bool)GetValue(IsCheckedProperty);
        set => SetValue(IsCheckedProperty, value);
    }

    public string Text {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public ICommand Command {
        get => (ICommand)GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    public object CommandParameter {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }

    public CustomCheckBox() {
        InitializeComponent();
        Content.BindingContext = this;
    }

    private void imageButton_Clicked(object sender, EventArgs e) {
        IsChecked = !IsChecked;
    }
}