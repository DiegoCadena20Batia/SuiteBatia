using BatiaSuite.Models;

namespace BatiaSuite.Utils;

public class UserSession {

    static readonly string PER_NOMBRE_KEY = "PerNombre key";
    static readonly string ID_EMPLEADO_KEY = "IdEmpleado key";
    static readonly string ID_PERSONAL_KEY = "IdPersonal key";
    static readonly string ID_CLIENTEL_KEY = "IdCliente key";
    static readonly string CLIENTEL_KEY = "Cliente key";
    static readonly string MODULO_KEY = "Modulo key";
    static readonly string SEGUIMIENTO_KEY = "Seguimiento key";
    static readonly string LISTADOS_DISPONIBLES = "ListadosDisponibles key";

    // --- NUEVAS CLAVES PARA EL CONTROL DE RUTAS ---
    static readonly string ID_RUTA_TRACKING = "IdRutaTracking key";
    static readonly string RUTA_NAME_TRACKING = "RutaNameTracking key";
    static readonly string TIPO_LISTADO_TRACKING = "TipoListado key";

    static readonly string ID_INMUEBLE_TRACKING = "IdInmuebleTracking key";
    static readonly string INMUEBLE_TRACKING = "InmuebleTracking key";
    static readonly string ID_MES_TRACKING = "IdMesTracking key";
    static readonly string ID_ANIO_TRACKING = "IdAdnioTraking key";
    static readonly string INMUEBLE_NAME_TRACKING = "InmuebleNameTraking key";
    static readonly string CLIENTE_NAME_TRACKING = "ClienteNameTraking key";
    static readonly string INMUEBLE_LATITUD_TRACKING = "InmuebleLatitudTraking key";
    static readonly string INMUEBLE_LONGITUD_TRACKING = "InmuebleLongitudTraking key";
    static readonly string IS_DELIVERING = "IsDelivering key";
    static readonly string ID_PROVEEDOR = "IdProveedor key";
    static readonly string SHOW_ACCEPT_TRACKING = "ShowAcceptTraking key";
    static readonly string ID_CLIENTE_CHECKLIST = "IdClienteChecklist key";
    static readonly string ID_INMUEBLE_CHECKLIST = "IdInmuebleChecklist key";
    static readonly string ID_PUESTO = "IdPuesto key";

    public static string NOMBRE {
        get => Preferences.Default.ContainsKey(PER_NOMBRE_KEY)
            ? Preferences.Default.Get(PER_NOMBRE_KEY, "") : "";
        set => Preferences.Default.Set(PER_NOMBRE_KEY, value);
    }

    public static int IdEmpleado {
        get => Preferences.Default.ContainsKey(ID_EMPLEADO_KEY)
            ? Preferences.Default.Get(ID_EMPLEADO_KEY, 0) : 0;
        set => Preferences.Default.Set(ID_EMPLEADO_KEY, value);
    }
    public static int IdPuesto {
        get => Preferences.Default.ContainsKey(ID_PUESTO)
            ? Preferences.Default.Get(ID_PUESTO, 0) : 0;
        set => Preferences.Default.Set(ID_PUESTO, value);
    }

    public static int IdPersonal {
        get => Preferences.Default.ContainsKey(ID_PERSONAL_KEY)
            ? (int)Preferences.Default.Get(ID_PERSONAL_KEY, 0) : 0;
        set => Preferences.Default.Set(ID_PERSONAL_KEY, value);
    }

    public static int IdCliente {
        get => Preferences.Default.ContainsKey(ID_CLIENTEL_KEY)
            ? (int)Preferences.Default.Get(ID_CLIENTEL_KEY, 0) : 0;
        set => Preferences.Default.Set(ID_CLIENTEL_KEY, value);
    }
    public static int Cliente {
        get => Preferences.Default.ContainsKey(CLIENTEL_KEY)
            ? (int)Preferences.Default.Get(CLIENTEL_KEY, 0) : 0;
        set => Preferences.Default.Set(CLIENTEL_KEY, value);
    }

    public static string Modulos {
        get => Preferences.Default.ContainsKey(MODULO_KEY)
            ? (string)Preferences.Default.Get(MODULO_KEY, "") : "";
        set => Preferences.Default.Set(MODULO_KEY, value);
    }

    public static bool SeguimientoGps {
        get => Preferences.Default.ContainsKey(SEGUIMIENTO_KEY)
            ? (bool)Preferences.Default.Get(SEGUIMIENTO_KEY, false) : false;
        set => Preferences.Default.Set(SEGUIMIENTO_KEY, value);
    }

    // --- NUEVAS PROPIEDADES PARA RUTA ---
    public static int IdRutaTracking {
        get => Preferences.Default.ContainsKey(ID_RUTA_TRACKING)
            ? Preferences.Default.Get<int>(ID_RUTA_TRACKING, 0) : 0;
        set => Preferences.Default.Set(ID_RUTA_TRACKING, value);
    }

    public static string RutaNameTracking {
        get => Preferences.Default.ContainsKey(RUTA_NAME_TRACKING)
            ? Preferences.Default.Get<string>(RUTA_NAME_TRACKING, "") : "";
        set => Preferences.Default.Set(RUTA_NAME_TRACKING, value);
    }

    // --- PROPIEDADES DE SUCURSAL / INMUEBLE ---
    public static int IdInmuebleTracking {
        get => Preferences.Default.ContainsKey(ID_INMUEBLE_TRACKING)
            ? (int)Preferences.Default.Get(ID_INMUEBLE_TRACKING, 0) : 0;
        set => Preferences.Default.Set(ID_INMUEBLE_TRACKING, value);
    }
    public static string InmuebleTracking {
        get => Preferences.Default.ContainsKey(INMUEBLE_TRACKING)
            ? (string)Preferences.Default.Get(INMUEBLE_TRACKING, "") : "";
        set => Preferences.Default.Set(INMUEBLE_TRACKING, value);
    }
    public static int IdMesTracking {
        get => Preferences.Default.ContainsKey(ID_MES_TRACKING)
            ? (int)Preferences.Default.Get(ID_MES_TRACKING, 0) : 0;
        set => Preferences.Default.Set(ID_MES_TRACKING, value);
    }
    public static int IdAnioTracking {
        get => Preferences.Default.ContainsKey(ID_ANIO_TRACKING)
            ? (int)Preferences.Default.Get(ID_ANIO_TRACKING, 0) : 0;
        set => Preferences.Default.Set(ID_ANIO_TRACKING, value);
    }
    public static int ListadosDisponibles {
        get => Preferences.Default.ContainsKey(LISTADOS_DISPONIBLES)
            ? (int)Preferences.Default.Get(LISTADOS_DISPONIBLES, 0) : 0;
        set => Preferences.Default.Set(LISTADOS_DISPONIBLES, value);
    }
    public static string TipoListadoTracking {
        get => Preferences.Default.ContainsKey(TIPO_LISTADO_TRACKING)
            ? (string)Preferences.Default.Get(TIPO_LISTADO_TRACKING, "") : "";
        set => Preferences.Default.Set(TIPO_LISTADO_TRACKING, value);
    }
    public static string InmuebleNameTracking {
        get => Preferences.Default.ContainsKey(INMUEBLE_NAME_TRACKING)
            ? (string)Preferences.Default.Get(INMUEBLE_NAME_TRACKING, "") : "";
        set => Preferences.Default.Set(INMUEBLE_NAME_TRACKING, value);
    }
    public static string ClienteNameTracking {
        get => Preferences.Default.ContainsKey(CLIENTE_NAME_TRACKING)
            ? (string)Preferences.Default.Get(CLIENTE_NAME_TRACKING, "") : "";
        set => Preferences.Default.Set(CLIENTE_NAME_TRACKING, value);
    }
    public static string InmuebleLatitudTracking {
        get => Preferences.Default.ContainsKey(INMUEBLE_LATITUD_TRACKING)
            ? (string)Preferences.Default.Get(INMUEBLE_LATITUD_TRACKING, "") : "";
        set => Preferences.Default.Set(INMUEBLE_LATITUD_TRACKING, value);
    }
    public static string InmuebleLongitudTracking {
        get => Preferences.Default.ContainsKey(INMUEBLE_LONGITUD_TRACKING)
            ? (string)Preferences.Default.Get(INMUEBLE_LONGITUD_TRACKING, "") : "";
        set => Preferences.Default.Set(INMUEBLE_LONGITUD_TRACKING, value);
    }
    public static bool IsDelivering {
        get => Preferences.Default.ContainsKey(IS_DELIVERING)
            ? (bool)Preferences.Default.Get(IS_DELIVERING, false) : false;
        set => Preferences.Default.Set(IS_DELIVERING, value);
    }
    public static int IdProveedor {
        get => Preferences.Default.ContainsKey(ID_PROVEEDOR)
            ? (int)Preferences.Default.Get(ID_PROVEEDOR, 0) : 0;
        set => Preferences.Default.Set(ID_PROVEEDOR, value);
    }

    public static bool ShowAcceptTracking {
        get => Preferences.Default.ContainsKey(SHOW_ACCEPT_TRACKING)
            ? (bool)Preferences.Default.Get(SHOW_ACCEPT_TRACKING, false) : false;
        set => Preferences.Default.Set(SHOW_ACCEPT_TRACKING, value);
    }

    public static int IdClienteCheckList {
        get => Preferences.Default.ContainsKey(ID_CLIENTE_CHECKLIST)
            ? (int)Preferences.Default.Get(ID_CLIENTE_CHECKLIST, 0) : 0;
        set => Preferences.Default.Set(ID_CLIENTE_CHECKLIST, value);
    }
    public static int IdInmuebleCheckList {
        get => Preferences.Default.ContainsKey(ID_INMUEBLE_CHECKLIST)
            ? (int)Preferences.Default.Get(ID_INMUEBLE_CHECKLIST, 0) : 0;
        set => Preferences.Default.Set(ID_INMUEBLE_CHECKLIST, value);
    }


    public static void SetData(LogueoModel data) {
        NOMBRE = data.per_Nombre;
        IdEmpleado = data.idEmpleado;
        IdPersonal = data.idPersonal;
        IdCliente = data.idCliente;
        Cliente = data.cliente;
        IdProveedor = data.idProveedor;
        if(data.Modulos != null && data.Modulos.Count > 0) {
            Modulos = string.Join(",", data.Modulos);
        }
        SeguimientoGps = false;
        ShowAcceptTracking = false;
        IdClienteCheckList = 0;
        IdInmuebleCheckList = 0;
        IdRutaTracking = 0;
        RutaNameTracking = "";
        IdPuesto = data.idPuesto;
    }

    public static void ClearSession() {
        Preferences.Default.Clear();
        ShowAcceptTracking = false;
    }
}