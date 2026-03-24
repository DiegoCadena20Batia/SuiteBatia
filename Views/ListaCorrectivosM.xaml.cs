using BatiaSuite.ViewModel;

namespace BatiaSuite.Views;

public partial class ListaCorrectivosM : ContentPage
{
    ListaCorrectivosMViewModel listaCorrectivosMViewModel = new ListaCorrectivosMViewModel();
    public ListaCorrectivosM(IMediaPicker mediaPicker)
	{
		InitializeComponent(); 
		BindingContext = listaCorrectivosMViewModel;
        listaCorrectivosMViewModel.mediaPicker = mediaPicker;
    }
}