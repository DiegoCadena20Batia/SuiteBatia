using System.Collections;

namespace BatiaSuite.Controls;

public partial class BordererPicker : ContentView {

    public static readonly BindableProperty ItemsSourceProperty =
        BindableProperty.Create(nameof(ItemsSource), typeof(IEnumerable), typeof(BordererPicker), null);

    public static readonly BindableProperty ItemDisplayBindingProperty =
        BindableProperty.Create(nameof(ItemDisplayBinding), typeof(string), typeof(BordererPicker), null,
            propertyChanged: (bindable, oldValue, newValue) => {
                BordererPicker bordererPicker = (BordererPicker)bindable;
                bordererPicker.transparentPicker.ItemDisplayBinding = new Binding(newValue.ToString());
            });

    public static readonly BindableProperty SelectedItemProperty =
        BindableProperty.Create(nameof(SelectedItem), typeof(object), typeof(BordererPicker), null, BindingMode.TwoWay);

    public static readonly BindableProperty TitleProperty =
        BindableProperty.Create(nameof(Title), typeof(string), typeof(BordererPicker), null);

    public IEnumerable ItemsSource {
        get => (IEnumerable)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    public string ItemDisplayBinding {
        get => (string)GetValue(ItemDisplayBindingProperty);
        set => SetValue(ItemDisplayBindingProperty, value);
    }

    public object SelectedItem {
        get => (object)GetValue(SelectedItemProperty);
        set => SetValue(SelectedItemProperty, value);
    }

    public string Title {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public BordererPicker() {
        InitializeComponent();
        Content.BindingContext = this;
    }
}