using System.Windows.Input;

namespace BatiaSuite.Controls;

public partial class ItemView : ContentView {

    public static readonly BindableProperty IsBusyProperty =
        BindableProperty.Create(nameof(IsBusy), typeof(bool), typeof(ItemView), false, BindingMode.TwoWay);

    public static readonly BindableProperty HasCommandProperty =
        BindableProperty.Create(nameof(HasCommand), typeof(bool), typeof(ItemView), false);

    public static readonly BindableProperty CommandProperty =
        BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(ItemView), null);

    public static readonly BindableProperty CommandParameterProperty =
        BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(ItemView), null);

    public static readonly BindableProperty HasSwipeProperty =
        BindableProperty.Create(nameof(HasSwipe), typeof(bool), typeof(ItemView), true);

    public static readonly BindableProperty SwipeCommandProperty =
        BindableProperty.Create(nameof(SwipeCommand), typeof(ICommand), typeof(ItemView), null);

    public static readonly BindableProperty SwipeCommandParameterProperty =
        BindableProperty.Create(nameof(SwipeCommandParameter), typeof(object), typeof(ItemView), null);

    public static readonly BindableProperty ItemContentProperty =
        BindableProperty.Create(nameof(ItemContent), typeof(View), typeof(ItemView), null);

    public bool IsBusy {
        get => (bool)GetValue(IsBusyProperty);
        set => SetValue(IsBusyProperty, value);
    }

    public bool HasCommand {
        get => (bool)GetValue(HasCommandProperty);
        set => SetValue(HasCommandProperty, value);
    }

    public ICommand Command {
        get => (ICommand)GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    public object CommandParameter {
        get => (object)GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }

    public bool HasSwipe {
        get => (bool)GetValue(HasSwipeProperty);
        set => SetValue(HasSwipeProperty, value);
    }

    public ICommand SwipeCommand {
        get => (ICommand)GetValue(SwipeCommandProperty);
        set => SetValue(SwipeCommandProperty, value);
    }

    public object SwipeCommandParameter {
        get => GetValue(SwipeCommandParameterProperty);
        set => SetValue(SwipeCommandParameterProperty, value);
    }

    public View ItemContent {
        get => (View)GetValue(ItemContentProperty);
        set => SetValue(ItemContentProperty, value);
    }

    public ItemView() {
        InitializeComponent();
        Content.BindingContext = this;
    }

    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e) {

    }
}