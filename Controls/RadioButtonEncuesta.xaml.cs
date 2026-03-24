using System.Windows.Input;

namespace BatiaSuite.Controls;

public partial class RadioButtonEncuesta : RadioButton {

    public static readonly BindableProperty CheckedChangedCommandProperty =
        BindableProperty.Create(nameof(CheckedChangedCommand), typeof(ICommand), typeof(RadioButtonEncuesta), null);

    public ICommand CheckedChangedCommand {
        get => (ICommand)GetValue(CheckedChangedCommandProperty);
        set => SetValue(CheckedChangedCommandProperty, value);
    }

    public RadioButtonEncuesta() {
        InitializeComponent();
    }
}