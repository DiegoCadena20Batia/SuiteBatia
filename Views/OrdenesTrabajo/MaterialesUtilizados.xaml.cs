using BatiaSuite.Models.OrdenesTrabajo;
using BatiaSuite.ViewModel.OrdenesTrabajo;
using BatiaSuite.ViewModel.Popups;
using System.Windows.Input;

namespace BatiaSuite.Views.OrdenesTrabajo;

public partial class MaterialesUtilizados : ContentPage {
    public MaterialesUtilizados(MaterialesUtilizadosViewModel vm) {
        InitializeComponent();
        BindingContext = vm;
    }

    void OnQuitarInvoked(object sender, EventArgs e) {
        // BindingContext = el elemento; CommandParameter = el comando Remove de esa lista
        if(sender is not SwipeItem { BindingContext: { } item, CommandParameter: ICommand command } swipeItem)
            return;

        Element parent = swipeItem.Parent;
        while(parent is not null and not SwipeView)
            parent = parent.Parent;

        (parent as SwipeView)?.Close(animated: false);

        Dispatcher.Dispatch(() =>
        {
            if(command.CanExecute(item))
                command.Execute(item);
        });
    }
}