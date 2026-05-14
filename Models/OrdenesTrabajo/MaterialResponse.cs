using Newtonsoft.Json;

namespace BatiaSuite.Models.OrdenesTrabajo;

public class MaterialResponse {
    public string Clave { get; set; }
    public string Descripcion { get; set; }
    public string Unidad { get; set; }
    public int IdUnidad { get; set; }
    public int IdAlmacen { get; set; }

    [JsonProperty("costo")]
    public float CostoUnitario { get; set; }
    public float Existencia { get; set; }

    int _cantidadUsada;
    [JsonIgnore]
    public int CantidadUsada {
        get => _cantidadUsada;
        set {
            _cantidadUsada = value;

            if(_cantidadUsada == 0) {
                ErrorMessage = "La cantidad de material debe ser mayor a 0.";
            }

            if(!IsCompra && _cantidadUsada > Existencia) {
                ErrorMessage = "La Cantidad capturada supera el disponible de inventario.";
            }
        }
    }

    int _cantidadXCobrar;
    [JsonIgnore]
    public int CantidadXCobrar {
        get => _cantidadXCobrar;
        set {
            _cantidadXCobrar = value;

            if(_cantidadXCobrar > CantidadUsada) {
                ErrorMessage = "La cantidad a cobrar no puede ser mayor a la cantidad de material solicitado.";
            }
        }
    }

    [JsonIgnore]
    public float CostoXCobrar { get => CantidadXCobrar * CostoUnitario; }

    [JsonIgnore]
    public string ErrorMessage { get; set; }

    [JsonIgnore]
    public bool ExistsError { 
        get => (!IsCompra && CantidadUsada > Existencia) 
            || CantidadUsada == 0 
            || (!IsCompra && CantidadXCobrar > CantidadUsada)
            || CantUtilizada > CantidadUsada; 
    }

    int _cantUtilizada;
    [JsonIgnore]
    public int CantUtilizada { //Materiales suministrados por Compra directa
        get => _cantUtilizada;
        set{
            _cantUtilizada = value;

            if(_cantUtilizada > CantidadUsada) {
                ErrorMessage = "La cantidad usada no puede ser mayor que la cantidad comprada.";
            }
        }
    }    

    [JsonIgnore]
    public float Total { get => CostoUnitario * CantidadUsada; } //Materiales suministrados por Compra directa

    [JsonIgnore]
    public bool IsCompra { get; set; }
    public DateTime SyncDate { get; internal set; }

    public static MaterialRequest MaterialConvert(MaterialResponse material) {
        return new MaterialRequest {
            Clave = material.Clave,
            Descripcion = material.Descripcion,
            Cantcob = material.CantidadXCobrar,

            Cantidad = material.CantidadUsada, // Cantidad usada del almacen | Cantidad comprada
            Preciocob = material.CostoUnitario, // Costo del stock | Costo de compra

            CantUtilizada = material.CantUtilizada, //En compra directa

            IsCompra = material.IsCompra,
            IdUnidad=material.IdUnidad,
            IdAlmacen = material.IdAlmacen
        };
    }
}

public class AlmacenModel {
    public int IdAlmacen { get; set; }
    public string Nombre { get; set; }
    public DateTime SyncDate { get; internal set; }
}
