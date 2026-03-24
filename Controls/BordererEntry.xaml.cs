using System.Windows.Input;

namespace BatiaSuite.Controls;

public partial class BordererEntry : ContentView {

    public static readonly BindableProperty TextProperty =
        BindableProperty.Create(nameof(Text), typeof(string), typeof(BordererEntry), null, BindingMode.TwoWay);

    public static readonly BindableProperty PlaceholderProperty =
        BindableProperty.Create(nameof(Placeholder), typeof(string), typeof(BordererEntry), null);

    public static readonly BindableProperty ReturnTypeProperty =
        BindableProperty.Create(nameof(ReturnType), typeof(ReturnType), typeof(BordererEntry), null);

    public static readonly BindableProperty ReturnCommandProperty =
        BindableProperty.Create(nameof(ReturnCommand), typeof(ICommand), typeof(BordererEntry), null);

    public static readonly BindableProperty ReturnCommandParameterProperty =
        BindableProperty.Create(nameof(ReturnCommandParameter), typeof(object), typeof(BordererEntry), null);

    public static readonly BindableProperty KeyboardProperty =
        BindableProperty.Create(nameof(Keyboard), typeof(Microsoft.Maui.Keyboard), typeof(BordererEntry), Microsoft.Maui.Keyboard.Default);

    public static readonly BindableProperty BordererEntryFontAttributesProperty =
        BindableProperty.Create(nameof(BordererEntryFontAttributes), typeof(FontAttributes), typeof(BordererEntry), FontAttributes.None);

    public static readonly BindableProperty CommandProperty =
        BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(BordererEntry), null);

    public static readonly BindableProperty CommandParameterProperty =
        BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(BordererEntry), null);

    public static readonly BindableProperty MaxLengthProperty =
        BindableProperty.Create(nameof(MaxLength), typeof(int), typeof(BordererEntry), int.MaxValue);

    public static readonly BindableProperty TextTransformProperty =
        BindableProperty.Create(nameof(TextTransform), typeof(TextTransform), typeof(BordererEntry), TextTransform.Default);

    public string Text {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public string Placeholder {
        get => (string)GetValue(PlaceholderProperty);
        set => SetValue(PlaceholderProperty, value);
    }

    public ReturnType ReturnType {
        get => (ReturnType)GetValue(ReturnTypeProperty);
        set => SetValue(ReturnTypeProperty, value);
    }

    public ICommand ReturnCommand {
        get => (ICommand)GetValue(ReturnCommandProperty);
        set => SetValue(ReturnCommandProperty, value);
    }

    public object ReturnCommandParameter {
        get => GetValue(ReturnCommandParameterProperty);
        set => SetValue(ReturnCommandParameterProperty, value);
    }

    public Microsoft.Maui.Keyboard Keyboard {
        get => (Microsoft.Maui.Keyboard)GetValue(KeyboardProperty);
        set => SetValue(KeyboardProperty, value);
    }

    public FontAttributes BordererEntryFontAttributes {
        get => (FontAttributes)GetValue(BordererEntryFontAttributesProperty);
        set => SetValue(BordererEntryFontAttributesProperty, value);
    }

    public ICommand Command {
        get => (ICommand)GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    public object CommandParameter {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }

    public int MaxLength {
        get => (int)GetValue(MaxLengthProperty);
        set => SetValue(MaxLengthProperty, value);
    }

    public TextTransform TextTransform {
        get => (TextTransform)GetValue(TextTransformProperty);
        set => SetValue(TextTransformProperty, value);
    }

    public BordererEntry() {
        InitializeComponent();
        Content.BindingContext = this;
    }
}