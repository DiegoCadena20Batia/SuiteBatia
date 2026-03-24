namespace BatiaSuite.Controls;

public partial class OptionsViewMantenimiento : ContentView {

    public static readonly BindableProperty TextProperty =
        BindableProperty.Create(nameof(Text), typeof(string), typeof(OptionsViewMantenimiento), null);

    public static readonly BindableProperty ValorProperty =
        BindableProperty.Create(nameof(Valor), typeof(object), typeof(OptionsViewMantenimiento), null, BindingMode.TwoWay,
            propertyChanged: (bindable, oldValue, newValue) => {
                OptionsViewMantenimiento currentControl = (OptionsViewMantenimiento)bindable;

                if(newValue is null) {
                    currentControl.CambiarBackgroundColor(0);
                } else {
                    if(currentControl.IsFloatValue) {
                        switch(newValue) {
                            case 1.0f:
                                currentControl.CambiarBackgroundColor(1);
                                break;
                            case 0.5f:
                                currentControl.CambiarBackgroundColor(2);
                                break;
                            case -0.5f:
                                currentControl.CambiarBackgroundColor(3);
                                break;
                            case 0.0f:
                                currentControl.CambiarBackgroundColor(0);
                                break;
                        }
                    } else {
                        switch(newValue) {
                            case 3:
                                currentControl.CambiarBackgroundColor(1);
                                break;
                            case 2:
                                currentControl.CambiarBackgroundColor(2);
                                break;
                            case 1:
                                currentControl.CambiarBackgroundColor(3);
                                break;
                            case 0:
                                currentControl.CambiarBackgroundColor(0);
                                break;
                        }
                    }
                }
            });

    public static readonly BindableProperty IsFloatValueProperty =
        BindableProperty.Create(nameof(IsFloatValue), typeof(bool), typeof(OptionsViewMantenimiento), false, BindingMode.TwoWay);

    public string Text {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public object Valor {
        get => GetValue(ValorProperty);
        set => SetValue(ValorProperty, value);
    }

    public bool IsFloatValue {
        get => (bool)GetValue(IsFloatValueProperty);
        set => SetValue(IsFloatValueProperty, value);
    }

    public OptionsViewMantenimiento() {
        InitializeComponent();
        Content.BindingContext = this;
    }

    private void bien_Clicked(object sender, EventArgs e) {
        Valor = IsFloatValue ? 1.0f : 3;
    }

    private void regular_Clicked(object sender, EventArgs e) {
        Valor = IsFloatValue ? 0.5f : 2;
    }

    private void mal_Clicked(object sender, EventArgs e) {
        Valor = IsFloatValue ? -0.5f : 1;
    }

    private void no_Clicked(object sender, EventArgs e) {
        Valor = IsFloatValue ? 0.0f : 0;
        CambiarBackgroundColor(0, false);
    }

    void CambiarBackgroundColor(int value, bool deseleccionarTodos = true) {
        Color blanco = Color.FromArgb("ffff");
        Color azulAux = Color.FromArgb("aa53c7e0");

        bien.BackgroundColor = value == 1 ? azulAux : blanco;
        //regular.BackgroundColor = value == 2 ? azulAux : blanco;
        mal.BackgroundColor = value == 3 ? azulAux : blanco;
        no.BackgroundColor = value == 0 && !deseleccionarTodos ? azulAux : blanco;
    }

}