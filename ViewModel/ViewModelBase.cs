using BatiaSuite.Models.Supervision;
using BatiaSuite.Utils;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BatiaSuite.ViewModel;

public abstract partial class ViewModelBase : ObservableObject {

    public readonly HttpHelper _httpHelper;

    protected ViewModelBase() {
        _httpHelper = new HttpHelper();
    }

    public List<ArchivoModel> ConvertertFotoList(IEnumerable<string> listPath, int seccion) {
        List<ArchivoModel> archivoList = new List<ArchivoModel>();
        foreach(string path in listPath) {

            archivoList.Add(new ArchivoModel {
                Path = path,
                Seccion = seccion
            });
        }

        return archivoList;
    }
}