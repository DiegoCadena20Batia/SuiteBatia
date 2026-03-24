using BatiaSuite.Models;
using BatiaSuite.Utils;
using BatiaSuite.Views;
using BatiaSuite.Views.SupplierDeliveries;
using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Windows.Input;


namespace BatiaSuite.ViewModel.SupplierDeliveries {
    public partial class SupplierDeliveriesDetailViewModel : ViewModelBase, IQueryAttributable
    {
        
        public BackButtonBehavior BackButtonBehavior { get; set; }

        private string _userName ;

        [ObservableProperty]
        bool _isLoading;
        public string UserName
        {
            get { return _userName; }
            set { _userName = value; OnPropertyChanged(); }
        }

        private ObservableCollection<ListApp> listApps;

        public ObservableCollection<ListApp> ListApps
        {
            get { return listApps; }
            set { listApps = value; OnPropertyChanged(); }
        }
        public string Inmueble { get; set; }
        public string Cliente { get; set; }
        public ICommand CommandListadoSelec { get; set; }
        public SupplierDeliveriesDetailViewModel()
        {
            BackButtonBehavior = new BackButtonBehavior
            {
                Command = new Command(async () =>
                {
                    // Do something here
                    await Shell.Current.GoToAsync("..");
                })
            };
            UserName += UserSession.NOMBRE;
            CommandListadoSelec = new Command<ListApp>(async (l) => await ListadoSelec(l));
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            string content = query["json"].ToString();
            ListApps = JsonConvert.DeserializeObject<ObservableCollection<ListApp>>(content);
            Inmueble = query["inmueble"].ToString();
            Cliente = query["clienteselected"].ToString();
        }
        //metodo que reciba todos los datos del id que halla tocado el usuario
        private async Task ListadoSelec(ListApp listApp)//pasa como tipo de dato
        {
            try
            {
                IsLoading = true;
                var idlistado = listApp.idlistado;

                Dictionary<string, object> data = new Dictionary<string, object>
                    {
                        { "idlist", idlistado },
                        {"inmueble",Inmueble },
                        {"clienteselected", Cliente }
                    };
                var route = $"{nameof(SupplierListadoMateriales)}";
                await Shell.Current.GoToAsync(route, data);
                IsLoading = false;
                //await Shell.Current.GoToAsync($"/MyDeliveries/MyListaMaterales", true, data);
            }
            catch (Exception ex)
            {
                IsLoading = false;
                await Shell.Current.DisplayAlert("Error", ex.Message, "ok");
            }
            
        }
    }
}