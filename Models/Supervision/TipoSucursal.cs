using BatiaSuite.Data;
using BatiaSuite.Utils;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace BatiaSuite.Models.Supervision;

public enum TipoSucursal
{
    OFICINA_CORPORATIVA = 1,
    PUNTO_VENTA = 2,
    RETAIL_BOUTIQUE = 3,
    PLANTA_INDUSTRIAL = 4
}

public class SeccionTipoSucursal
{
    public TipoSucursal TipoSucursal { get; set; }
    public int Id { get; set; }
    public string Descripcion { get; set; }

    static List<SeccionTipoSucursal> _secciones = new List<SeccionTipoSucursal> {
        new SeccionTipoSucursal {
            TipoSucursal = TipoSucursal.OFICINA_CORPORATIVA,
            Id = 1,
            Descripcion = "Valoración de la operativa del servicio"
        },
        new SeccionTipoSucursal {
            TipoSucursal = TipoSucursal.OFICINA_CORPORATIVA,
            Id = 2,
            Descripcion = "Valoración del nivel general de limpieza de los espacios"
        },
        new SeccionTipoSucursal {
            TipoSucursal = TipoSucursal.OFICINA_CORPORATIVA,
            Id = 3,
            Descripcion = "Valoración del nivel específico de limpieza"
        },
        new SeccionTipoSucursal {
            TipoSucursal = TipoSucursal.OFICINA_CORPORATIVA,
            Id = 4,
            Descripcion = "Bancos"
        },


        new SeccionTipoSucursal {
            TipoSucursal = TipoSucursal.PUNTO_VENTA,
            Id = 5,
            Descripcion = "Valoración de la operativa del servicio"
        },
        new SeccionTipoSucursal {
            TipoSucursal = TipoSucursal.PUNTO_VENTA,
            Id = 6,
            Descripcion = "Valoración del nivel general de limpieza de los espacios"
        },
        new SeccionTipoSucursal {
            TipoSucursal = TipoSucursal.PUNTO_VENTA,
            Id = 7,
            Descripcion = "Valoración del nivel específico de limpieza"
        },
        new SeccionTipoSucursal {
            TipoSucursal = TipoSucursal.PUNTO_VENTA,
            Id = 8,
            Descripcion = "Bancos"
        },



        new SeccionTipoSucursal {
            TipoSucursal = TipoSucursal.RETAIL_BOUTIQUE,
            Id = 9,
            Descripcion = "Valoración de la operativa del servicio"
        },
        new SeccionTipoSucursal {
            TipoSucursal = TipoSucursal.RETAIL_BOUTIQUE,
            Id = 10,
            Descripcion = "Valoración del nivel general de limpieza de los espacios"
        },
        new SeccionTipoSucursal {
            TipoSucursal = TipoSucursal.RETAIL_BOUTIQUE,
            Id =11,
            Descripcion = "Valoración del nivel específico de limpieza"
        },
        new SeccionTipoSucursal {
            TipoSucursal = TipoSucursal.RETAIL_BOUTIQUE,
            Id = 12,
            Descripcion = "Bancos"
        },

        new SeccionTipoSucursal {
            TipoSucursal = TipoSucursal.PLANTA_INDUSTRIAL,
            Id = 13,
            Descripcion = "Valoración de la operativa del servicio"
        },
        new SeccionTipoSucursal {
            TipoSucursal = TipoSucursal.PLANTA_INDUSTRIAL,
            Id = 14,
            Descripcion = "Valoración del nivel general de limpieza"
        },
        new SeccionTipoSucursal {
            TipoSucursal = TipoSucursal.PLANTA_INDUSTRIAL,
            Id = 15,
            Descripcion = "Valoración del nivel específico de limpieza"
        },
        new SeccionTipoSucursal {
            TipoSucursal = TipoSucursal.PLANTA_INDUSTRIAL,
            Id = 16,
            Descripcion = "Bancos"
        },
    };

    public static async Task<List<SeccionTipoSucursal>> ObtenerSeccionesPorTipoSucursal(TipoSucursal tipoSucursal, int idInmueble)
    {
        List<SeccionTipoSucursal> preguntasRemoto = new List<SeccionTipoSucursal>();
        if (!Utils.InternetUtil.IsConnectedInternet())
        {
            var secciones = _secciones.Where(s => s.TipoSucursal == tipoSucursal).ToList();
            DbContext _dbContext = new DbContext();
            var result = await _dbContext.VerificarBancoInmueble(idInmueble);
            if (!result)
            {
                var seccionesFiltradas = secciones.Where(s => s.Id != 4).ToList();
                return seccionesFiltradas;
            }
            return secciones;
        }
        else
        {
            try
            {
                string url = $"{Constants.GET_SUPERVISION_SECCIONTIPO}/{(int)tipoSucursal}/{idInmueble}";
                HttpHelper httpHelper = new HttpHelper();
                preguntasRemoto = await httpHelper.GetAsync<List<SeccionTipoSucursal>>(url); //no me esta devolviendo el Id, seguramente el IdSeccion
                if (preguntasRemoto != null)
                {
                    return preguntasRemoto;
                }
                else
                {
                    return _secciones.Where(s => s.TipoSucursal == tipoSucursal).ToList();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error ObtenerSeccionesPorTipoSucursal: " + ex.Message.ToString());
                return _secciones.Where(s => s.TipoSucursal == tipoSucursal).ToList();
            }
        }
    }
}

public class SeccionTipoSucursalMantenimiento {
    public TipoSucursal TipoSucursal { get; set; }
    public int Id { get; set; }
    public string Descripcion { get; set; }

    static List<SeccionTipoSucursal> _secciones = new List<SeccionTipoSucursal> {
        new SeccionTipoSucursal {
            TipoSucursal = TipoSucursal.OFICINA_CORPORATIVA,
            Id = 1,
            Descripcion = "Sanitarios"
        },
        new SeccionTipoSucursal {
            TipoSucursal = TipoSucursal.OFICINA_CORPORATIVA,
            Id = 2,
            Descripcion = "Luminarias"
        },
        new SeccionTipoSucursal {
            TipoSucursal = TipoSucursal.OFICINA_CORPORATIVA,
            Id = 3,
            Descripcion = "Tinacos y sisternas"
        },
        new SeccionTipoSucursal {
            TipoSucursal = TipoSucursal.OFICINA_CORPORATIVA,
            Id = 4,
            Descripcion = "Bancos"
        },


        new SeccionTipoSucursal {
            TipoSucursal = TipoSucursal.PUNTO_VENTA,
            Id = 5,
            Descripcion = "Sanitarios"
        },
        new SeccionTipoSucursal {
            TipoSucursal = TipoSucursal.PUNTO_VENTA,
            Id = 6,
            Descripcion = "Luminarias"
        },
        new SeccionTipoSucursal {
            TipoSucursal = TipoSucursal.PUNTO_VENTA,
            Id = 7,
            Descripcion = "Tinacos y sisternas"
        },
        new SeccionTipoSucursal {
            TipoSucursal = TipoSucursal.PUNTO_VENTA,
            Id = 8,
            Descripcion = "Bancos"
        },



        new SeccionTipoSucursal {
            TipoSucursal = TipoSucursal.RETAIL_BOUTIQUE,
            Id = 9,
            Descripcion = "Sanitarios"
        },
        new SeccionTipoSucursal {
            TipoSucursal = TipoSucursal.RETAIL_BOUTIQUE,
            Id = 10,
            Descripcion = "Luminarias"
        },
        new SeccionTipoSucursal {
            TipoSucursal = TipoSucursal.RETAIL_BOUTIQUE,
            Id =11,
            Descripcion = "Tinacos y sisternas"
        },
        new SeccionTipoSucursal {
            TipoSucursal = TipoSucursal.RETAIL_BOUTIQUE,
            Id = 12,
            Descripcion = "Sistema de bombeo"
        },

        new SeccionTipoSucursal {
            TipoSucursal = TipoSucursal.PLANTA_INDUSTRIAL,
            Id = 13,
            Descripcion = "Sanitarios"
        },
        new SeccionTipoSucursal {
            TipoSucursal = TipoSucursal.PLANTA_INDUSTRIAL,
            Id = 14,
            Descripcion = "Luminaria"
        },
        new SeccionTipoSucursal {
            TipoSucursal = TipoSucursal.PLANTA_INDUSTRIAL,
            Id = 15,
            Descripcion = "Tinacos y sisternas"
        },
        new SeccionTipoSucursal {
            TipoSucursal = TipoSucursal.PLANTA_INDUSTRIAL,
            Id = 16,
            Descripcion = "Sistema de bombeo"
        },
    };

    public static async Task<List<SeccionTipoSucursal>> ObtenerSeccionesPorTipoSucursalMantenimiento(TipoSucursal tipoSucursal, int idInmueble) {
        List<SeccionTipoSucursal> preguntasRemoto = new List<SeccionTipoSucursal>();
        //if(!Utils.InternetUtil.IsConnectedInternet()) {
            var secciones = _secciones.Where(s => s.TipoSucursal == tipoSucursal).ToList();
            //DbContext _dbContext = new DbContext();
            //var result = await _dbContext.VerificarBancoInmueble(idInmueble);
            //if(!result) {
            //    var seccionesFiltradas = secciones.Where(s => s.Id != 4).ToList();
            //    return seccionesFiltradas;
            //}
            return secciones;
        //} else {
        //    try {
        //        string url = $"{Constants.GET_SUPERVISION_SECCIONTIPO}/{(int)tipoSucursal}/{idInmueble}";
        //        HttpHelper httpHelper = new HttpHelper();
        //        preguntasRemoto = await httpHelper.GetAsync<List<SeccionTipoSucursal>>(url); //no me esta devolviendo el Id, seguramente el IdSeccion
        //        if(preguntasRemoto != null) {
        //            return preguntasRemoto;
        //        } else {
        //            return _secciones.Where(s => s.TipoSucursal == tipoSucursal).ToList();
        //        }
        //    } catch(Exception ex) {
        //        Console.WriteLine("Error ObtenerSeccionesPorTipoSucursal: " + ex.Message.ToString());
        //        return _secciones.Where(s => s.TipoSucursal == tipoSucursal).ToList();
        //    }
        //}
    }
}



public partial class SupervisionPregunta : BaseNotify
{
    [JsonIgnore]
    public int IdSupervision { get; set; }
    public int IdSeccion { get; set; }
    public int IdPregunta { get; set; }
    public string Descripcion { get; set; }

    float? _valor;
    public float? Valor
    {
        get => _valor;
        set
        {
            if (_valor == value)
            {
                return;
            }

            _valor = value;
            Observaciones = string.Empty;
            OnPropertyChanged();
        }
    }

    string _observaciones;
    public string Observaciones
    {
        get => _observaciones;
        set
        {
            if (_observaciones == value)
            {
                return;
            }

            _observaciones = value;
            OnPropertyChanged();
        }
    }

    public static async Task<List<SupervisionPregunta>> ObtenerPreguntasPorIdSeccion(int idSeccion)
    {
        if (!Utils.InternetUtil.IsConnectedInternet())
        {
            List<SupervisionPregunta> preguntas = new List<SupervisionPregunta> {
                new SupervisionPregunta { IdPregunta=1,  IdSeccion =1,Descripcion = "Cumplimiento de la programación de los trabajos"},
                new SupervisionPregunta { IdPregunta=2,  IdSeccion =1,Descripcion = "Cumplimiento de los horarios del personal "},
                new SupervisionPregunta { IdPregunta=3,  IdSeccion =1,Descripcion = "Disponibilidad de supervisores"},
                new SupervisionPregunta { IdPregunta=4,  IdSeccion =1,Descripcion = "Trato del personal de la empresa de limpieza a los clientes"},
                new SupervisionPregunta { IdPregunta=5,  IdSeccion =1,Descripcion = "Aspecto general de la persona (UNIFORMES)"},
                new SupervisionPregunta { IdPregunta=6,  IdSeccion =2,Descripcion = "Zona atención al cliente y recepcion"},
                new SupervisionPregunta { IdPregunta=7,  IdSeccion =2,Descripcion = "Baños en general"},
                new SupervisionPregunta { IdPregunta=8,  IdSeccion =2,Descripcion = "Equipo de telefonia y computo"},
                new SupervisionPregunta { IdPregunta=9,  IdSeccion =2,Descripcion = "Limpieza de vidrios "},
                new SupervisionPregunta { IdPregunta=10,  IdSeccion =2,Descripcion = "pasillos y andadores "},
                new SupervisionPregunta { IdPregunta=11,  IdSeccion =2,Descripcion = "Espacios Comunes"},
                new SupervisionPregunta { IdPregunta=12,  IdSeccion =2,Descripcion = "Area de comedor o cocinetas"},
                new SupervisionPregunta { IdPregunta=13,  IdSeccion =3,Descripcion = "Mobiliario (Mesas, sillones y sillas, partes bajas )"},
                new SupervisionPregunta { IdPregunta=14,  IdSeccion =3,Descripcion = "Limpieza de bodega"},
                new SupervisionPregunta { IdPregunta=15,  IdSeccion =3,Descripcion = "Cristales Externos"},
                new SupervisionPregunta { IdPregunta=16,  IdSeccion =3,Descripcion = "Espejos, carteles y acabados"},
                new SupervisionPregunta { IdPregunta=17,  IdSeccion =3,Descripcion = "Limipeza de piso (loceta vinilica e interceramica, marmol, duela laminada, azulejo, cemento pulido)"},
                new SupervisionPregunta { IdPregunta=18,  IdSeccion =3,Descripcion = "Inventarios de material y maquinaria "},
                new SupervisionPregunta { IdPregunta=19,  IdSeccion =4,Descripcion = "¿El piso de la zona de cajeros automáticos exterior se encuentra limpio?"},
                new SupervisionPregunta { IdPregunta=20,  IdSeccion =4,Descripcion = "¿Las paredes de la zona de cajeros automáticos exterior se encuentran limpias?"},
                new SupervisionPregunta { IdPregunta=21,  IdSeccion =4,Descripcion = "¿Los cristales de la zona de cajeros automáticos exterior se encuentran limpios?"},
                new SupervisionPregunta { IdPregunta=22,  IdSeccion =4,Descripcion = "¿El cajero automático exterior se encuentra limpio?"},
                new SupervisionPregunta { IdPregunta=23,  IdSeccion =4,Descripcion = "¿El bote de basura de la zona de cajeros automáticos exterior se encuentra limpio?"},
                new SupervisionPregunta { IdPregunta=24,  IdSeccion =4,Descripcion = "¿El piso de la zona de cajeros automáticos interior se encuentra limpio?"},
                new SupervisionPregunta { IdPregunta=25,  IdSeccion =4,Descripcion = "¿Las paredes de la zona de cajeros automáticos interior se encuentran limpias?"},
                new SupervisionPregunta { IdPregunta=26,  IdSeccion =4,Descripcion = "¿El cajero automático interior se encuentra limpio?"},
                new SupervisionPregunta { IdPregunta=27,  IdSeccion =4,Descripcion = "¿El bote de basura de la zona de cajeros automáticos interior se encuentra limpio?"},
                new SupervisionPregunta { IdPregunta=28,  IdSeccion =4,Descripcion = "¿Los pisos de la zona de Asesores financieros se encuentran limpios? "},
                new SupervisionPregunta { IdPregunta=29,  IdSeccion =4,Descripcion = "¿Los escritorios de los Asesores financieros se encuentran limpios?"},
                new SupervisionPregunta { IdPregunta=30,  IdSeccion =4,Descripcion = " ¿Los botes de basura de la zona de Asesores financieros se encuentran limpios?"},
                new SupervisionPregunta { IdPregunta=31,  IdSeccion =4,Descripcion = "¿Las sillas de los Asesores financieros se encuentran limpias? "},
                new SupervisionPregunta { IdPregunta=32,  IdSeccion =4,Descripcion = "¿Las sillas de los clientes en patio bancario se encuentran limpias?"},
                new SupervisionPregunta { IdPregunta=33,  IdSeccion =4,Descripcion = "¿Los postes de la unifila se encuentran sin polvo o basura?"},
                new SupervisionPregunta { IdPregunta=34,  IdSeccion =4,Descripcion = "¿Las ventanillas se encuentran limpias? "},
                new SupervisionPregunta { IdPregunta=35,  IdSeccion =4,Descripcion = "¿Los pisos de la zona de cajas se encuentran limpios?"},
                new SupervisionPregunta { IdPregunta=36,  IdSeccion =5,Descripcion = "Cumplimiento de la programación de los trabajos"},
                new SupervisionPregunta { IdPregunta=37,  IdSeccion =5,Descripcion = "Cumplimiento de los horarios del personal "},
                new SupervisionPregunta { IdPregunta=38,  IdSeccion =5,Descripcion = "Disponibilidad de supervisores"},
                new SupervisionPregunta { IdPregunta=39,  IdSeccion =5,Descripcion = "Trato del personal de la empresa de limpieza a los clientes"},
                new SupervisionPregunta { IdPregunta=40,  IdSeccion =5,Descripcion = "Aspecto general de la persona (UNIFORMES)"},
                new SupervisionPregunta { IdPregunta=41,  IdSeccion =6,Descripcion = "Zona atención al cliente y recepcion"},
                new SupervisionPregunta { IdPregunta=42,  IdSeccion =6,Descripcion = "Baños en general"},
                new SupervisionPregunta { IdPregunta=43,  IdSeccion =6,Descripcion = "Equipo de telefonia y computo"},
                new SupervisionPregunta { IdPregunta=44,  IdSeccion =6,Descripcion = "Limpieza de Cajas"},
                new SupervisionPregunta { IdPregunta=45,  IdSeccion =6,Descripcion = "Piso de Venta"},
                new SupervisionPregunta { IdPregunta=46,  IdSeccion =6,Descripcion = "Espacios Comunes"},
                new SupervisionPregunta { IdPregunta=47,  IdSeccion =6,Descripcion = "Area de comedor o cocinetas"},
                new SupervisionPregunta { IdPregunta=48,  IdSeccion =7,Descripcion = "Mobiliario (Mesas, sillones y sillas)"},
                new SupervisionPregunta { IdPregunta=49,  IdSeccion =7,Descripcion = "Limpieza de bodega"},
                new SupervisionPregunta { IdPregunta=50,  IdSeccion =7,Descripcion = "Cristales Externos"},
                new SupervisionPregunta { IdPregunta=51,  IdSeccion =7,Descripcion = "Espejos, carteles y acabados"},
                new SupervisionPregunta { IdPregunta=52,  IdSeccion =7,Descripcion = "Limipeza de piso (loceta vinilica e interceramica, marmol, duela laminada, azulejo, cemento pulido)"},
                new SupervisionPregunta { IdPregunta=53,  IdSeccion =7,Descripcion = "Exhibidores"},
                new SupervisionPregunta { IdPregunta=54,  IdSeccion =7,Descripcion = "Inventarios de material y maquinaria "},
                new SupervisionPregunta { IdPregunta=55,  IdSeccion =8,Descripcion = "¿El piso de la zona de cajeros automáticos exterior se encuentra limpio?"},
                new SupervisionPregunta { IdPregunta=56,  IdSeccion =8,Descripcion = "¿Las paredes de la zona de cajeros automáticos exterior se encuentran limpias?"},
                new SupervisionPregunta { IdPregunta=57,  IdSeccion =8,Descripcion = "¿Los cristales de la zona de cajeros automáticos exterior se encuentran limpios?"},
                new SupervisionPregunta { IdPregunta=58,  IdSeccion =8,Descripcion = "¿El cajero automático exterior se encuentra limpio?"},
                new SupervisionPregunta { IdPregunta=59,  IdSeccion =8,Descripcion = "¿El bote de basura de la zona de cajeros automáticos exterior se encuentra limpio?"},
                new SupervisionPregunta { IdPregunta=60,  IdSeccion =8,Descripcion = "¿El piso de la zona de cajeros automáticos interior se encuentra limpio?"},
                new SupervisionPregunta { IdPregunta=61,  IdSeccion =8,Descripcion = "¿Las paredes de la zona de cajeros automáticos interior se encuentran limpias?"},
                new SupervisionPregunta { IdPregunta=62,  IdSeccion =8,Descripcion = "¿El cajero automático interior se encuentra limpio?"},
                new SupervisionPregunta { IdPregunta=63,  IdSeccion =8,Descripcion = "¿El bote de basura de la zona de cajeros automáticos interior se encuentra limpio?"},
                new SupervisionPregunta { IdPregunta=64,  IdSeccion =8,Descripcion = "¿Los pisos de la zona de Asesores financieros se encuentran limpios? "},
                new SupervisionPregunta { IdPregunta=65,  IdSeccion =8,Descripcion = "¿Los escritorios de los Asesores financieros se encuentran limpios?"},
                new SupervisionPregunta { IdPregunta=66,  IdSeccion =8,Descripcion = " ¿Los botes de basura de la zona de Asesores financieros se encuentran limpios?"},
                new SupervisionPregunta { IdPregunta=67,  IdSeccion =8,Descripcion = "¿Las sillas de los Asesores financieros se encuentran limpias? "},
                new SupervisionPregunta { IdPregunta=68,  IdSeccion =8,Descripcion = "¿Las sillas de los clientes en patio bancario se encuentran limpias?"},
                new SupervisionPregunta { IdPregunta=69,  IdSeccion =8,Descripcion = "¿Los postes de la unifila se encuentran sin polvo o basura?"},
                new SupervisionPregunta { IdPregunta=70,  IdSeccion =8,Descripcion = "¿Las ventanillas de se encuentran limpias? "},
                new SupervisionPregunta { IdPregunta=71,  IdSeccion =8,Descripcion = "¿Los pisos de la zona de cajas se encuentran limpios?"},
                new SupervisionPregunta { IdPregunta=72,  IdSeccion =9,Descripcion = "Cumplimiento de la programación de los trabajos"},
                new SupervisionPregunta { IdPregunta=73,  IdSeccion =9,Descripcion = "Cumplimiento de los horarios del personal "},
                new SupervisionPregunta { IdPregunta=74,  IdSeccion =9,Descripcion = "Disponibilidad de supervisores"},
                new SupervisionPregunta { IdPregunta=75,  IdSeccion =9,Descripcion = "Trato del personal de la empresa de limpieza a los clientes"},
                new SupervisionPregunta { IdPregunta=76,  IdSeccion =9,Descripcion = "Aspecto general de la persona (UNIFORMES)"},
                new SupervisionPregunta { IdPregunta=77,  IdSeccion =10,Descripcion = "Pasillos "},
                new SupervisionPregunta { IdPregunta=78,  IdSeccion =10,Descripcion = "Baños en general"},
                new SupervisionPregunta { IdPregunta=79,  IdSeccion =10,Descripcion = "Equipo de telefonia y computo"},
                new SupervisionPregunta { IdPregunta=80,  IdSeccion =10,Descripcion = "Limpieza de Cajas"},
                new SupervisionPregunta { IdPregunta=81,  IdSeccion =10,Descripcion = "Piso de Venta"},
                new SupervisionPregunta { IdPregunta=82,  IdSeccion =10,Descripcion = "Espacios Comunes"},
                new SupervisionPregunta { IdPregunta=83,  IdSeccion =10,Descripcion = "Area de comedor o cocinetas"},
                new SupervisionPregunta { IdPregunta=84,  IdSeccion =11,Descripcion = "Mobiliario (Mesas, sillones y sillas)"},
                new SupervisionPregunta { IdPregunta=85,  IdSeccion =11,Descripcion = "Limpieza de bodega"},
                new SupervisionPregunta { IdPregunta=86,  IdSeccion =11,Descripcion = "Cristales Externos"},
                new SupervisionPregunta { IdPregunta=87,  IdSeccion =11,Descripcion = "Espejos, carteles y acabados"},
                new SupervisionPregunta { IdPregunta=88,  IdSeccion =11,Descripcion = "Limipeza de piso (loceta vinilica e interceramica, marmol, duela laminada, azulejo, cemento pulido)"},
                new SupervisionPregunta { IdPregunta=89,  IdSeccion =11,Descripcion = "Exhibidores"},
                new SupervisionPregunta { IdPregunta=90,  IdSeccion =11,Descripcion = "limpieza de covacha y inventarios de materiales y maquinaria "},
                new SupervisionPregunta { IdPregunta=91,  IdSeccion =12,Descripcion = "¿El piso de la zona de cajeros automáticos exterior se encuentra limpio?"},
                new SupervisionPregunta { IdPregunta=92,  IdSeccion =12,Descripcion = "¿Las paredes de la zona de cajeros automáticos exterior se encuentran limpias?"},
                new SupervisionPregunta { IdPregunta=93,  IdSeccion =12,Descripcion = "¿Los cristales de la zona de cajeros automáticos exterior se encuentran limpios?"},
                new SupervisionPregunta { IdPregunta=94,  IdSeccion =12,Descripcion = "¿El cajero automático exterior se encuentra limpio?"},
                new SupervisionPregunta { IdPregunta=95,  IdSeccion =12,Descripcion = "¿El bote de basura de la zona de cajeros automáticos exterior se encuentra limpio?"},
                new SupervisionPregunta { IdPregunta=96,  IdSeccion =12,Descripcion = "¿El piso de la zona de cajeros automáticos interior se encuentra limpio?"},
                new SupervisionPregunta { IdPregunta=97,  IdSeccion =12,Descripcion = "¿Las paredes de la zona de cajeros automáticos interior se encuentran limpias?"},
                new SupervisionPregunta { IdPregunta=98,  IdSeccion =12,Descripcion = "¿El cajero automático interior se encuentra limpio?"},
                new SupervisionPregunta { IdPregunta=99,  IdSeccion =12,Descripcion = "¿El bote de basura de la zona de cajeros automáticos interior se encuentra limpio?"},
                new SupervisionPregunta { IdPregunta=100,  IdSeccion =12,Descripcion = "¿Los pisos de la zona de Asesores financieros se encuentran limpios? "},
                new SupervisionPregunta { IdPregunta=101,  IdSeccion =12,Descripcion = "¿Los escritorios de los Asesores financieros se encuentran limpios?"},
                new SupervisionPregunta { IdPregunta=102,  IdSeccion =12,Descripcion = " ¿Los botes de basura de la zona de Asesores financieros se encuentran limpios?"},
                new SupervisionPregunta { IdPregunta=103,  IdSeccion =12,Descripcion = "¿Las sillas de los Asesores financieros se encuentran limpias? "},
                new SupervisionPregunta { IdPregunta=104,  IdSeccion =12,Descripcion = "¿Las sillas de los clientes en patio bancario se encuentran limpias?"},
                new SupervisionPregunta { IdPregunta=105,  IdSeccion =12,Descripcion = "¿Los postes de la unifila se encuentran sin polvo o basura?"},
                new SupervisionPregunta { IdPregunta=106,  IdSeccion =12,Descripcion = "¿Las ventanillas de se encuentran limpias? "},
                new SupervisionPregunta { IdPregunta=107,  IdSeccion =12,Descripcion = "¿Los pisos de la zona de cajas se encuentran limpios?"},
                new SupervisionPregunta { IdPregunta=108,  IdSeccion =13,Descripcion = "Cumplimiento de la programación de los trabajos"},
                new SupervisionPregunta { IdPregunta=109,  IdSeccion =13,Descripcion = "Cumplimiento de los horarios del personal "},
                new SupervisionPregunta { IdPregunta=110,  IdSeccion =13,Descripcion = "Disponibilidad de supervisores"},
                new SupervisionPregunta { IdPregunta=111,  IdSeccion =13,Descripcion = "Trato del personal de la empresa de limpieza a los clientes"},
                new SupervisionPregunta { IdPregunta=112,  IdSeccion =13,Descripcion = "Aspecto general de la persona (UNIFORMES)"},
                new SupervisionPregunta { IdPregunta=113,  IdSeccion =13,Descripcion = "Botas de seguridad, chaleco reflejante y casco"},
                new SupervisionPregunta { IdPregunta=114,  IdSeccion =14,Descripcion = "Zona atención al cliente y recepcion"},
                new SupervisionPregunta { IdPregunta=115,  IdSeccion =14,Descripcion = "Baños en general"},
                new SupervisionPregunta { IdPregunta=116,  IdSeccion =14,Descripcion = "Equipo de telefonia y computo"},
                new SupervisionPregunta { IdPregunta=117,  IdSeccion =14,Descripcion = "Limpieza de Cajas"},
                new SupervisionPregunta { IdPregunta=118,  IdSeccion =14,Descripcion = "pasillos y andadores "},
                new SupervisionPregunta { IdPregunta=119,  IdSeccion =14,Descripcion = "Espacios Comunes"},
                new SupervisionPregunta { IdPregunta=120,  IdSeccion =14,Descripcion = "Area de comedor o cocinetas"},
                new SupervisionPregunta { IdPregunta=121,  IdSeccion =14,Descripcion = "Estacionamientos y area de carga  ( area de proovedores )"},
                new SupervisionPregunta { IdPregunta=122,  IdSeccion =14,Descripcion = "Area de maquinas "},
                new SupervisionPregunta { IdPregunta=123,  IdSeccion =15,Descripcion = "Mobiliario (Mesas, sillones y sillas)"},
                new SupervisionPregunta { IdPregunta=124,  IdSeccion =15,Descripcion = "Limpieza de bodega"},
                new SupervisionPregunta { IdPregunta=125,  IdSeccion =15,Descripcion = "Cristales Externos"},
                new SupervisionPregunta { IdPregunta=126,  IdSeccion =15,Descripcion = "Espejos, carteles y acabados"},
                new SupervisionPregunta { IdPregunta=127,  IdSeccion =15,Descripcion = "Limipeza de piso (loceta vinilica e interceramica, marmol, duela laminada, azulejo, cemento pulido)"},
                new SupervisionPregunta { IdPregunta=128,  IdSeccion =15,Descripcion = "Exhibidores"},
                new SupervisionPregunta { IdPregunta=129,  IdSeccion =16,Descripcion = "¿El piso de la zona de cajeros automáticos exterior se encuentra limpio?"},
                new SupervisionPregunta { IdPregunta=130,  IdSeccion =16,Descripcion = "¿Las paredes de la zona de cajeros automáticos exterior se encuentran limpias?"},
                new SupervisionPregunta { IdPregunta=131,  IdSeccion =16,Descripcion = "¿Los cristales de la zona de cajeros automáticos exterior se encuentran limpios?"},
                new SupervisionPregunta { IdPregunta=132,  IdSeccion =16,Descripcion = "¿El cajero automático exterior se encuentra limpio?"},
                new SupervisionPregunta { IdPregunta=133,  IdSeccion =16,Descripcion = "¿El bote de basura de la zona de cajeros automáticos exterior se encuentra limpio?"},
                new SupervisionPregunta { IdPregunta=134,  IdSeccion =16,Descripcion = "¿El piso de la zona de cajeros automáticos interior se encuentra limpio?"},
                new SupervisionPregunta { IdPregunta=135,  IdSeccion =16,Descripcion = "¿Las paredes de la zona de cajeros automáticos interior se encuentran limpias?"},
                new SupervisionPregunta { IdPregunta=136,  IdSeccion =16,Descripcion = "¿El cajero automático interior se encuentra limpio?"},
                new SupervisionPregunta { IdPregunta=137,  IdSeccion =16,Descripcion = "¿El bote de basura de la zona de cajeros automáticos interior se encuentra limpio?"},
                new SupervisionPregunta { IdPregunta=138,  IdSeccion =16,Descripcion = "¿Los pisos de la zona de Asesores financieros se encuentran limpios? "},
                new SupervisionPregunta { IdPregunta=139,  IdSeccion =16,Descripcion = "¿Los escritorios de los Asesores financieros se encuentran limpios?"},
                new SupervisionPregunta { IdPregunta=140,  IdSeccion =16,Descripcion = "¿Los botes de basura de la zona de Asesores financieros se encuentran limpios?"},
                new SupervisionPregunta { IdPregunta=141,  IdSeccion =16,Descripcion = "¿Las sillas de los Asesores financieros se encuentran limpias? "},
                new SupervisionPregunta { IdPregunta=142,  IdSeccion =16,Descripcion = "¿Las sillas de los clientes en patio bancario se encuentran limpias?"},
                new SupervisionPregunta { IdPregunta=143,  IdSeccion =16,Descripcion = "¿Los postes de la unifila se encuentran sin polvo o basura?"},
                new SupervisionPregunta { IdPregunta=144,  IdSeccion =16,Descripcion = "¿Las ventanillas se encuentran limpias? "},
                new SupervisionPregunta { IdPregunta=145,  IdSeccion =16,Descripcion = "¿Los pisos de la zona de cajas se encuentran limpios?"},

            };
            return preguntas.Where(p => p.IdSeccion == idSeccion).ToList();
        }
        else
        {
            List<SupervisionPregunta> preguntasRemoto = new List<SupervisionPregunta>();
            string url = $"{Constants.GET_SUPERVISION_PREGUNTAS}/{idSeccion}";
            HttpHelper httpHelper = new HttpHelper();
            preguntasRemoto = await httpHelper.GetAsync<List<SupervisionPregunta>>(url);
            if (preguntasRemoto != null)
            {
                return preguntasRemoto;
            }
            else
            {
                List<SupervisionPregunta> preguntas = new List<SupervisionPregunta> {
                new SupervisionPregunta { IdPregunta=1,  IdSeccion =1,Descripcion = "Cumplimiento de la programación de los trabajos"},
                new SupervisionPregunta { IdPregunta=2,  IdSeccion =1,Descripcion = "Cumplimiento de los horarios del personal "},
                new SupervisionPregunta { IdPregunta=3,  IdSeccion =1,Descripcion = "Disponibilidad de supervisores"},
                new SupervisionPregunta { IdPregunta=4,  IdSeccion =1,Descripcion = "Trato del personal de la empresa de limpieza a los clientes"},
                new SupervisionPregunta { IdPregunta=5,  IdSeccion =1,Descripcion = "Aspecto general de la persona (UNIFORMES)"},
                new SupervisionPregunta { IdPregunta=6,  IdSeccion =2,Descripcion = "Zona atención al cliente y recepcion"},
                new SupervisionPregunta { IdPregunta=7,  IdSeccion =2,Descripcion = "Baños en general"},
                new SupervisionPregunta { IdPregunta=8,  IdSeccion =2,Descripcion = "Equipo de telefonia y computo"},
                new SupervisionPregunta { IdPregunta=9,  IdSeccion =2,Descripcion = "Limpieza de vidrios "},
                new SupervisionPregunta { IdPregunta=10,  IdSeccion =2,Descripcion = "pasillos y andadores "},
                new SupervisionPregunta { IdPregunta=11,  IdSeccion =2,Descripcion = "Espacios Comunes"},
                new SupervisionPregunta { IdPregunta=12,  IdSeccion =2,Descripcion = "Area de comedor o cocinetas"},
                new SupervisionPregunta { IdPregunta=13,  IdSeccion =3,Descripcion = "Mobiliario (Mesas, sillones y sillas, partes bajas )"},
                new SupervisionPregunta { IdPregunta=14,  IdSeccion =3,Descripcion = "Limpieza de bodega"},
                new SupervisionPregunta { IdPregunta=15,  IdSeccion =3,Descripcion = "Cristales Externos"},
                new SupervisionPregunta { IdPregunta=16,  IdSeccion =3,Descripcion = "Espejos, carteles y acabados"},
                new SupervisionPregunta { IdPregunta=17,  IdSeccion =3,Descripcion = "Limipeza de piso (loceta vinilica e interceramica, marmol, duela laminada, azulejo, cemento pulido)"},
                new SupervisionPregunta { IdPregunta=18,  IdSeccion =3,Descripcion = "Inventarios de material y maquinaria "},
                new SupervisionPregunta { IdPregunta=19,  IdSeccion =4,Descripcion = "¿El piso de la zona de cajeros automáticos exterior se encuentra limpio?"},
                new SupervisionPregunta { IdPregunta=20,  IdSeccion =4,Descripcion = "¿Las paredes de la zona de cajeros automáticos exterior se encuentran limpias?"},
                new SupervisionPregunta { IdPregunta=21,  IdSeccion =4,Descripcion = "¿Los cristales de la zona de cajeros automáticos exterior se encuentran limpios?"},
                new SupervisionPregunta { IdPregunta=22,  IdSeccion =4,Descripcion = "¿El cajero automático exterior se encuentra limpio?"},
                new SupervisionPregunta { IdPregunta=23,  IdSeccion =4,Descripcion = "¿El bote de basura de la zona de cajeros automáticos exterior se encuentra limpio?"},
                new SupervisionPregunta { IdPregunta=24,  IdSeccion =4,Descripcion = "¿El piso de la zona de cajeros automáticos interior se encuentra limpio?"},
                new SupervisionPregunta { IdPregunta=25,  IdSeccion =4,Descripcion = "¿Las paredes de la zona de cajeros automáticos interior se encuentran limpias?"},
                new SupervisionPregunta { IdPregunta=26,  IdSeccion =4,Descripcion = "¿El cajero automático interior se encuentra limpio?"},
                new SupervisionPregunta { IdPregunta=27,  IdSeccion =4,Descripcion = "¿El bote de basura de la zona de cajeros automáticos interior se encuentra limpio?"},
                new SupervisionPregunta { IdPregunta=28,  IdSeccion =4,Descripcion = "¿Los pisos de la zona de Asesores financieros se encuentran limpios? "},
                new SupervisionPregunta { IdPregunta=29,  IdSeccion =4,Descripcion = "¿Los escritorios de los Asesores financieros se encuentran limpios?"},
                new SupervisionPregunta { IdPregunta=30,  IdSeccion =4,Descripcion = " ¿Los botes de basura de la zona de Asesores financieros se encuentran limpios?"},
                new SupervisionPregunta { IdPregunta=31,  IdSeccion =4,Descripcion = "¿Las sillas de los Asesores financieros se encuentran limpias? "},
                new SupervisionPregunta { IdPregunta=32,  IdSeccion =4,Descripcion = "¿Las sillas de los clientes en patio bancario se encuentran limpias?"},
                new SupervisionPregunta { IdPregunta=33,  IdSeccion =4,Descripcion = "¿Los postes de la unifila se encuentran sin polvo o basura?"},
                new SupervisionPregunta { IdPregunta=34,  IdSeccion =4,Descripcion = "¿Las ventanillas se encuentran limpias? "},
                new SupervisionPregunta { IdPregunta=35,  IdSeccion =4,Descripcion = "¿Los pisos de la zona de cajas se encuentran limpios?"},
                new SupervisionPregunta { IdPregunta=36,  IdSeccion =5,Descripcion = "Cumplimiento de la programación de los trabajos"},
                new SupervisionPregunta { IdPregunta=37,  IdSeccion =5,Descripcion = "Cumplimiento de los horarios del personal "},
                new SupervisionPregunta { IdPregunta=38,  IdSeccion =5,Descripcion = "Disponibilidad de supervisores"},
                new SupervisionPregunta { IdPregunta=39,  IdSeccion =5,Descripcion = "Trato del personal de la empresa de limpieza a los clientes"},
                new SupervisionPregunta { IdPregunta=40,  IdSeccion =5,Descripcion = "Aspecto general de la persona (UNIFORMES)"},
                new SupervisionPregunta { IdPregunta=41,  IdSeccion =6,Descripcion = "Zona atención al cliente y recepcion"},
                new SupervisionPregunta { IdPregunta=42,  IdSeccion =6,Descripcion = "Baños en general"},
                new SupervisionPregunta { IdPregunta=43,  IdSeccion =6,Descripcion = "Equipo de telefonia y computo"},
                new SupervisionPregunta { IdPregunta=44,  IdSeccion =6,Descripcion = "Limpieza de Cajas"},
                new SupervisionPregunta { IdPregunta=45,  IdSeccion =6,Descripcion = "Piso de Venta"},
                new SupervisionPregunta { IdPregunta=46,  IdSeccion =6,Descripcion = "Espacios Comunes"},
                new SupervisionPregunta { IdPregunta=47,  IdSeccion =6,Descripcion = "Area de comedor o cocinetas"},
                new SupervisionPregunta { IdPregunta=48,  IdSeccion =7,Descripcion = "Mobiliario (Mesas, sillones y sillas)"},
                new SupervisionPregunta { IdPregunta=49,  IdSeccion =7,Descripcion = "Limpieza de bodega"},
                new SupervisionPregunta { IdPregunta=50,  IdSeccion =7,Descripcion = "Cristales Externos"},
                new SupervisionPregunta { IdPregunta=51,  IdSeccion =7,Descripcion = "Espejos, carteles y acabados"},
                new SupervisionPregunta { IdPregunta=52,  IdSeccion =7,Descripcion = "Limipeza de piso (loceta vinilica e interceramica, marmol, duela laminada, azulejo, cemento pulido)"},
                new SupervisionPregunta { IdPregunta=53,  IdSeccion =7,Descripcion = "Exhibidores"},
                new SupervisionPregunta { IdPregunta=54,  IdSeccion =7,Descripcion = "Inventarios de material y maquinaria "},
                new SupervisionPregunta { IdPregunta=55,  IdSeccion =8,Descripcion = "¿El piso de la zona de cajeros automáticos exterior se encuentra limpio?"},
                new SupervisionPregunta { IdPregunta=56,  IdSeccion =8,Descripcion = "¿Las paredes de la zona de cajeros automáticos exterior se encuentran limpias?"},
                new SupervisionPregunta { IdPregunta=57,  IdSeccion =8,Descripcion = "¿Los cristales de la zona de cajeros automáticos exterior se encuentran limpios?"},
                new SupervisionPregunta { IdPregunta=58,  IdSeccion =8,Descripcion = "¿El cajero automático exterior se encuentra limpio?"},
                new SupervisionPregunta { IdPregunta=59,  IdSeccion =8,Descripcion = "¿El bote de basura de la zona de cajeros automáticos exterior se encuentra limpio?"},
                new SupervisionPregunta { IdPregunta=60,  IdSeccion =8,Descripcion = "¿El piso de la zona de cajeros automáticos interior se encuentra limpio?"},
                new SupervisionPregunta { IdPregunta=61,  IdSeccion =8,Descripcion = "¿Las paredes de la zona de cajeros automáticos interior se encuentran limpias?"},
                new SupervisionPregunta { IdPregunta=62,  IdSeccion =8,Descripcion = "¿El cajero automático interior se encuentra limpio?"},
                new SupervisionPregunta { IdPregunta=63,  IdSeccion =8,Descripcion = "¿El bote de basura de la zona de cajeros automáticos interior se encuentra limpio?"},
                new SupervisionPregunta { IdPregunta=64,  IdSeccion =8,Descripcion = "¿Los pisos de la zona de Asesores financieros se encuentran limpios? "},
                new SupervisionPregunta { IdPregunta=65,  IdSeccion =8,Descripcion = "¿Los escritorios de los Asesores financieros se encuentran limpios?"},
                new SupervisionPregunta { IdPregunta=66,  IdSeccion =8,Descripcion = " ¿Los botes de basura de la zona de Asesores financieros se encuentran limpios?"},
                new SupervisionPregunta { IdPregunta=67,  IdSeccion =8,Descripcion = "¿Las sillas de los Asesores financieros se encuentran limpias? "},
                new SupervisionPregunta { IdPregunta=68,  IdSeccion =8,Descripcion = "¿Las sillas de los clientes en patio bancario se encuentran limpias?"},
                new SupervisionPregunta { IdPregunta=69,  IdSeccion =8,Descripcion = "¿Los postes de la unifila se encuentran sin polvo o basura?"},
                new SupervisionPregunta { IdPregunta=70,  IdSeccion =8,Descripcion = "¿Las ventanillas de se encuentran limpias? "},
                new SupervisionPregunta { IdPregunta=71,  IdSeccion =8,Descripcion = "¿Los pisos de la zona de cajas se encuentran limpios?"},
                new SupervisionPregunta { IdPregunta=72,  IdSeccion =9,Descripcion = "Cumplimiento de la programación de los trabajos"},
                new SupervisionPregunta { IdPregunta=73,  IdSeccion =9,Descripcion = "Cumplimiento de los horarios del personal "},
                new SupervisionPregunta { IdPregunta=74,  IdSeccion =9,Descripcion = "Disponibilidad de supervisores"},
                new SupervisionPregunta { IdPregunta=75,  IdSeccion =9,Descripcion = "Trato del personal de la empresa de limpieza a los clientes"},
                new SupervisionPregunta { IdPregunta=76,  IdSeccion =9,Descripcion = "Aspecto general de la persona (UNIFORMES)"},
                new SupervisionPregunta { IdPregunta=77,  IdSeccion =10,Descripcion = "Pasillos "},
                new SupervisionPregunta { IdPregunta=78,  IdSeccion =10,Descripcion = "Baños en general"},
                new SupervisionPregunta { IdPregunta=79,  IdSeccion =10,Descripcion = "Equipo de telefonia y computo"},
                new SupervisionPregunta { IdPregunta=80,  IdSeccion =10,Descripcion = "Limpieza de Cajas"},
                new SupervisionPregunta { IdPregunta=81,  IdSeccion =10,Descripcion = "Piso de Venta"},
                new SupervisionPregunta { IdPregunta=82,  IdSeccion =10,Descripcion = "Espacios Comunes"},
                new SupervisionPregunta { IdPregunta=83,  IdSeccion =10,Descripcion = "Area de comedor o cocinetas"},
                new SupervisionPregunta { IdPregunta=84,  IdSeccion =11,Descripcion = "Mobiliario (Mesas, sillones y sillas)"},
                new SupervisionPregunta { IdPregunta=85,  IdSeccion =11,Descripcion = "Limpieza de bodega"},
                new SupervisionPregunta { IdPregunta=86,  IdSeccion =11,Descripcion = "Cristales Externos"},
                new SupervisionPregunta { IdPregunta=87,  IdSeccion =11,Descripcion = "Espejos, carteles y acabados"},
                new SupervisionPregunta { IdPregunta=88,  IdSeccion =11,Descripcion = "Limipeza de piso (loceta vinilica e interceramica, marmol, duela laminada, azulejo, cemento pulido)"},
                new SupervisionPregunta { IdPregunta=89,  IdSeccion =11,Descripcion = "Exhibidores"},
                new SupervisionPregunta { IdPregunta=90,  IdSeccion =11,Descripcion = "limpieza de covacha y inventarios de materiales y maquinaria "},
                new SupervisionPregunta { IdPregunta=91,  IdSeccion =12,Descripcion = "¿El piso de la zona de cajeros automáticos exterior se encuentra limpio?"},
                new SupervisionPregunta { IdPregunta=92,  IdSeccion =12,Descripcion = "¿Las paredes de la zona de cajeros automáticos exterior se encuentran limpias?"},
                new SupervisionPregunta { IdPregunta=93,  IdSeccion =12,Descripcion = "¿Los cristales de la zona de cajeros automáticos exterior se encuentran limpios?"},
                new SupervisionPregunta { IdPregunta=94,  IdSeccion =12,Descripcion = "¿El cajero automático exterior se encuentra limpio?"},
                new SupervisionPregunta { IdPregunta=95,  IdSeccion =12,Descripcion = "¿El bote de basura de la zona de cajeros automáticos exterior se encuentra limpio?"},
                new SupervisionPregunta { IdPregunta=96,  IdSeccion =12,Descripcion = "¿El piso de la zona de cajeros automáticos interior se encuentra limpio?"},
                new SupervisionPregunta { IdPregunta=97,  IdSeccion =12,Descripcion = "¿Las paredes de la zona de cajeros automáticos interior se encuentran limpias?"},
                new SupervisionPregunta { IdPregunta=98,  IdSeccion =12,Descripcion = "¿El cajero automático interior se encuentra limpio?"},
                new SupervisionPregunta { IdPregunta=99,  IdSeccion =12,Descripcion = "¿El bote de basura de la zona de cajeros automáticos interior se encuentra limpio?"},
                new SupervisionPregunta { IdPregunta=100,  IdSeccion =12,Descripcion = "¿Los pisos de la zona de Asesores financieros se encuentran limpios? "},
                new SupervisionPregunta { IdPregunta=101,  IdSeccion =12,Descripcion = "¿Los escritorios de los Asesores financieros se encuentran limpios?"},
                new SupervisionPregunta { IdPregunta=102,  IdSeccion =12,Descripcion = " ¿Los botes de basura de la zona de Asesores financieros se encuentran limpios?"},
                new SupervisionPregunta { IdPregunta=103,  IdSeccion =12,Descripcion = "¿Las sillas de los Asesores financieros se encuentran limpias? "},
                new SupervisionPregunta { IdPregunta=104,  IdSeccion =12,Descripcion = "¿Las sillas de los clientes en patio bancario se encuentran limpias?"},
                new SupervisionPregunta { IdPregunta=105,  IdSeccion =12,Descripcion = "¿Los postes de la unifila se encuentran sin polvo o basura?"},
                new SupervisionPregunta { IdPregunta=106,  IdSeccion =12,Descripcion = "¿Las ventanillas de se encuentran limpias? "},
                new SupervisionPregunta { IdPregunta=107,  IdSeccion =12,Descripcion = "¿Los pisos de la zona de cajas se encuentran limpios?"},
                new SupervisionPregunta { IdPregunta=108,  IdSeccion =13,Descripcion = "Cumplimiento de la programación de los trabajos"},
                new SupervisionPregunta { IdPregunta=109,  IdSeccion =13,Descripcion = "Cumplimiento de los horarios del personal "},
                new SupervisionPregunta { IdPregunta=110,  IdSeccion =13,Descripcion = "Disponibilidad de supervisores"},
                new SupervisionPregunta { IdPregunta=111,  IdSeccion =13,Descripcion = "Trato del personal de la empresa de limpieza a los clientes"},
                new SupervisionPregunta { IdPregunta=112,  IdSeccion =13,Descripcion = "Aspecto general de la persona (UNIFORMES)"},
                new SupervisionPregunta { IdPregunta=113,  IdSeccion =13,Descripcion = "Botas de seguridad, chaleco reflejante y casco"},
                new SupervisionPregunta { IdPregunta=114,  IdSeccion =14,Descripcion = "Zona atención al cliente y recepcion"},
                new SupervisionPregunta { IdPregunta=115,  IdSeccion =14,Descripcion = "Baños en general"},
                new SupervisionPregunta { IdPregunta=116,  IdSeccion =14,Descripcion = "Equipo de telefonia y computo"},
                new SupervisionPregunta { IdPregunta=117,  IdSeccion =14,Descripcion = "Limpieza de Cajas"},
                new SupervisionPregunta { IdPregunta=118,  IdSeccion =14,Descripcion = "pasillos y andadores "},
                new SupervisionPregunta { IdPregunta=119,  IdSeccion =14,Descripcion = "Espacios Comunes"},
                new SupervisionPregunta { IdPregunta=120,  IdSeccion =14,Descripcion = "Area de comedor o cocinetas"},
                new SupervisionPregunta { IdPregunta=121,  IdSeccion =14,Descripcion = "Estacionamientos y area de carga  ( area de proovedores )"},
                new SupervisionPregunta { IdPregunta=122,  IdSeccion =14,Descripcion = "Area de maquinas "},
                new SupervisionPregunta { IdPregunta=123,  IdSeccion =15,Descripcion = "Mobiliario (Mesas, sillones y sillas)"},
                new SupervisionPregunta { IdPregunta=124,  IdSeccion =15,Descripcion = "Limpieza de bodega"},
                new SupervisionPregunta { IdPregunta=125,  IdSeccion =15,Descripcion = "Cristales Externos"},
                new SupervisionPregunta { IdPregunta=126,  IdSeccion =15,Descripcion = "Espejos, carteles y acabados"},
                new SupervisionPregunta { IdPregunta=127,  IdSeccion =15,Descripcion = "Limipeza de piso (loceta vinilica e interceramica, marmol, duela laminada, azulejo, cemento pulido)"},
                new SupervisionPregunta { IdPregunta=128,  IdSeccion =15,Descripcion = "Exhibidores"},
                new SupervisionPregunta { IdPregunta=129,  IdSeccion =16,Descripcion = "¿El piso de la zona de cajeros automáticos exterior se encuentra limpio?"},
                new SupervisionPregunta { IdPregunta=130,  IdSeccion =16,Descripcion = "¿Las paredes de la zona de cajeros automáticos exterior se encuentran limpias?"},
                new SupervisionPregunta { IdPregunta=131,  IdSeccion =16,Descripcion = "¿Los cristales de la zona de cajeros automáticos exterior se encuentran limpios?"},
                new SupervisionPregunta { IdPregunta=132,  IdSeccion =16,Descripcion = "¿El cajero automático exterior se encuentra limpio?"},
                new SupervisionPregunta { IdPregunta=133,  IdSeccion =16,Descripcion = "¿El bote de basura de la zona de cajeros automáticos exterior se encuentra limpio?"},
                new SupervisionPregunta { IdPregunta=134,  IdSeccion =16,Descripcion = "¿El piso de la zona de cajeros automáticos interior se encuentra limpio?"},
                new SupervisionPregunta { IdPregunta=135,  IdSeccion =16,Descripcion = "¿Las paredes de la zona de cajeros automáticos interior se encuentran limpias?"},
                new SupervisionPregunta { IdPregunta=136,  IdSeccion =16,Descripcion = "¿El cajero automático interior se encuentra limpio?"},
                new SupervisionPregunta { IdPregunta=137,  IdSeccion =16,Descripcion = "¿El bote de basura de la zona de cajeros automáticos interior se encuentra limpio?"},
                new SupervisionPregunta { IdPregunta=138,  IdSeccion =16,Descripcion = "¿Los pisos de la zona de Asesores financieros se encuentran limpios? "},
                new SupervisionPregunta { IdPregunta=139,  IdSeccion =16,Descripcion = "¿Los escritorios de los Asesores financieros se encuentran limpios?"},
                new SupervisionPregunta { IdPregunta=140,  IdSeccion =16,Descripcion = "¿Los botes de basura de la zona de Asesores financieros se encuentran limpios?"},
                new SupervisionPregunta { IdPregunta=141,  IdSeccion =16,Descripcion = "¿Las sillas de los Asesores financieros se encuentran limpias? "},
                new SupervisionPregunta { IdPregunta=142,  IdSeccion =16,Descripcion = "¿Las sillas de los clientes en patio bancario se encuentran limpias?"},
                new SupervisionPregunta { IdPregunta=143,  IdSeccion =16,Descripcion = "¿Los postes de la unifila se encuentran sin polvo o basura?"},
                new SupervisionPregunta { IdPregunta=144,  IdSeccion =16,Descripcion = "¿Las ventanillas se encuentran limpias? "},
                new SupervisionPregunta { IdPregunta=145,  IdSeccion =16,Descripcion = "¿Los pisos de la zona de cajas se encuentran limpios?"},

            };
                return preguntas.Where(p => p.IdSeccion == idSeccion).ToList();
            }
        }
    }
    public static async Task<List<SupervisionPregunta>> ObtenerPreguntasManenimientoPorIdSeccion(int idSeccion) {
        //if(!Utils.InternetUtil.IsConnectedInternet()) {
            List<SupervisionPregunta> preguntas = new List<SupervisionPregunta> {
                new SupervisionPregunta { IdPregunta=1,  IdSeccion =1,Descripcion = "1.- Realizar inspección visual a los W.C. (inodoros), que desoció el sensor, que no presenten fugas en conexiones y que el W.C. no esté tapado."},
                new SupervisionPregunta { IdPregunta=2,  IdSeccion =1,Descripcion = "2.- Realizar inspección visual a los mingitorios (urinarios), el sensor, push de funcionamiento que no presenten fugas en conexiones."},
                new SupervisionPregunta { IdPregunta=3,  IdSeccion =1,Descripcion = "3.- Realizar inspección visual a los lavabos, grifos que no se encuentren tapados y que no presenten fugas"},
                new SupervisionPregunta { IdPregunta=4,  IdSeccion =1,Descripcion = "4.- Realizar inspección visual a coladeras, que no se encuentren sucias o tapadas."},
                new SupervisionPregunta { IdPregunta=5,  IdSeccion =1,Descripcion = "5.- Realizar inspección visual del sistema eléctrico; luminarias que no se encuentren fundidas, canaletas y funcionamiento."},
                new SupervisionPregunta { IdPregunta=6,  IdSeccion =1,Descripcion = "6.- Revisar funcionamiento y revisión de extractores."},
                new SupervisionPregunta { IdPregunta=7,  IdSeccion =1,Descripcion = "7.- Revisión de puertas, chapas y bisagras."},
                new SupervisionPregunta { IdPregunta=8,  IdSeccion =1,Descripcion = "8.- Revisar apagadores y/o sensores de presencia."},
                new SupervisionPregunta { IdPregunta=9,  IdSeccion =1,Descripcion = "9.- Fijación y funcionamiento de accesorios."},
                new SupervisionPregunta { IdPregunta=10,  IdSeccion =1,Descripcion = "10.- Revisión de acabados en pintura, azulejos, lambrines, pisos, barras y rejillas."},

                new SupervisionPregunta { IdPregunta=11,  IdSeccion =2,Descripcion = "1.- Inspección visual de limpieza interior y exterior."},
                new SupervisionPregunta { IdPregunta=12,  IdSeccion =2,Descripcion = "2.- Revisión de micas y gabinetes."},
                new SupervisionPregunta { IdPregunta=13,  IdSeccion =2,Descripcion = "3.- Verificar elementos de sujeción."},
                new SupervisionPregunta { IdPregunta=14,  IdSeccion =2,Descripcion = "4.- Verificar el nivel de luminosidad."},
                new SupervisionPregunta { IdPregunta=15,  IdSeccion =2,Descripcion = "5.- Verificar carga de baterías en luminarias de emergencia."},
                new SupervisionPregunta { IdPregunta=16,  IdSeccion =2,Descripcion = "6.- Verificar que la cantidad instalada corresponda al plano de distribución."},

                new SupervisionPregunta { IdPregunta=17,  IdSeccion =3,Descripcion = "1.- Inspeccion visual que las llaves de toma de llenado y salida operen correctamente."},
                new SupervisionPregunta { IdPregunta=18,  IdSeccion =3,Descripcion = "2.- Inspección visual que los equipos de bombeo y electroniveles se encuentren instalados."},
                new SupervisionPregunta { IdPregunta=19,  IdSeccion =3,Descripcion = "3.- Inspección visual que los tableros esten instalados cableados y en operación."},
                new SupervisionPregunta { IdPregunta=20,  IdSeccion =3,Descripcion = "4.- Revisión que la capacidad de tinacos corresponda a la memoria del cálculo."},
                new SupervisionPregunta { IdPregunta=21,  IdSeccion =3,Descripcion = "5.- Revisió que la cantidad de tinacos corresponda a la memoria del cálculo."},
                new SupervisionPregunta { IdPregunta=22,  IdSeccion =3,Descripcion = "6.- Revisar que el tinaco no tenga fisuras o este agrietado."},
                new SupervisionPregunta { IdPregunta=23,  IdSeccion =3,Descripcion = "7.- Realizar pruebas de apertura y cierre de válvulas."},
                new SupervisionPregunta { IdPregunta=24,  IdSeccion =3,Descripcion = "8.- Revisar que los electroniveles realicen la función de apertura y cierre del sistema de llenado."},
                new SupervisionPregunta { IdPregunta=25,  IdSeccion =3,Descripcion = "9.- Revisar que no se tengan cuarteaduras o fisuras en las cisternas."},
                new SupervisionPregunta { IdPregunta=26,  IdSeccion =3,Descripcion = "10.- Revisar que tengan rotulado la capacidad de las cisternas y el tipo de agua: pluvial, tratada y potable"},
                new SupervisionPregunta { IdPregunta=27,  IdSeccion =3,Descripcion = "11.- Limpieza del cuarto de cisternas, pintura, iluminación y señalética"},
                new SupervisionPregunta { IdPregunta=28,  IdSeccion =4,Descripcion = "¿Los pisos de la zona de Asesores financieros se encuentran limpios? "},
                new SupervisionPregunta { IdPregunta=29,  IdSeccion =4,Descripcion = "¿Los escritorios de los Asesores financieros se encuentran limpios?"},
                new SupervisionPregunta { IdPregunta=30,  IdSeccion =4,Descripcion = " ¿Los botes de basura de la zona de Asesores financieros se encuentran limpios?"},
                new SupervisionPregunta { IdPregunta=31,  IdSeccion =4,Descripcion = "¿Las sillas de los Asesores financieros se encuentran limpias? "},
                new SupervisionPregunta { IdPregunta=32,  IdSeccion =4,Descripcion = "¿Las sillas de los clientes en patio bancario se encuentran limpias?"},
                new SupervisionPregunta { IdPregunta=33,  IdSeccion =4,Descripcion = "¿Los postes de la unifila se encuentran sin polvo o basura?"},
                new SupervisionPregunta { IdPregunta=34,  IdSeccion =4,Descripcion = "¿Las ventanillas se encuentran limpias? "},
                new SupervisionPregunta { IdPregunta=35,  IdSeccion =4,Descripcion = "¿Los pisos de la zona de cajas se encuentran limpios?"},
                new SupervisionPregunta { IdPregunta=36,  IdSeccion =5,Descripcion = "Cumplimiento de la programación de los trabajos"},
                new SupervisionPregunta { IdPregunta=37,  IdSeccion =5,Descripcion = "Cumplimiento de los horarios del personal "},
                new SupervisionPregunta { IdPregunta=38,  IdSeccion =5,Descripcion = "Disponibilidad de supervisores"},
                new SupervisionPregunta { IdPregunta=39,  IdSeccion =5,Descripcion = "Trato del personal de la empresa de limpieza a los clientes"},
                new SupervisionPregunta { IdPregunta=40,  IdSeccion =5,Descripcion = "Aspecto general de la persona (UNIFORMES)"},
                new SupervisionPregunta { IdPregunta=41,  IdSeccion =6,Descripcion = "Zona atención al cliente y recepcion"},
                new SupervisionPregunta { IdPregunta=42,  IdSeccion =6,Descripcion = "Baños en general"},
                new SupervisionPregunta { IdPregunta=43,  IdSeccion =6,Descripcion = "Equipo de telefonia y computo"},
                new SupervisionPregunta { IdPregunta=44,  IdSeccion =6,Descripcion = "Limpieza de Cajas"},
                new SupervisionPregunta { IdPregunta=45,  IdSeccion =6,Descripcion = "Piso de Venta"},
                new SupervisionPregunta { IdPregunta=46,  IdSeccion =6,Descripcion = "Espacios Comunes"},
                new SupervisionPregunta { IdPregunta=47,  IdSeccion =6,Descripcion = "Area de comedor o cocinetas"},
                new SupervisionPregunta { IdPregunta=48,  IdSeccion =7,Descripcion = "Mobiliario (Mesas, sillones y sillas)"},
                new SupervisionPregunta { IdPregunta=49,  IdSeccion =7,Descripcion = "Limpieza de bodega"},
                new SupervisionPregunta { IdPregunta=50,  IdSeccion =7,Descripcion = "Cristales Externos"},
                new SupervisionPregunta { IdPregunta=51,  IdSeccion =7,Descripcion = "Espejos, carteles y acabados"},
                new SupervisionPregunta { IdPregunta=52,  IdSeccion =7,Descripcion = "Limipeza de piso (loceta vinilica e interceramica, marmol, duela laminada, azulejo, cemento pulido)"},
                new SupervisionPregunta { IdPregunta=53,  IdSeccion =7,Descripcion = "Exhibidores"},
                new SupervisionPregunta { IdPregunta=54,  IdSeccion =7,Descripcion = "Inventarios de material y maquinaria "},
                new SupervisionPregunta { IdPregunta=55,  IdSeccion =8,Descripcion = "¿El piso de la zona de cajeros automáticos exterior se encuentra limpio?"},
                new SupervisionPregunta { IdPregunta=56,  IdSeccion =8,Descripcion = "¿Las paredes de la zona de cajeros automáticos exterior se encuentran limpias?"},
                new SupervisionPregunta { IdPregunta=57,  IdSeccion =8,Descripcion = "¿Los cristales de la zona de cajeros automáticos exterior se encuentran limpios?"},
                new SupervisionPregunta { IdPregunta=58,  IdSeccion =8,Descripcion = "¿El cajero automático exterior se encuentra limpio?"},
                new SupervisionPregunta { IdPregunta=59,  IdSeccion =8,Descripcion = "¿El bote de basura de la zona de cajeros automáticos exterior se encuentra limpio?"},
                new SupervisionPregunta { IdPregunta=60,  IdSeccion =8,Descripcion = "¿El piso de la zona de cajeros automáticos interior se encuentra limpio?"},
                new SupervisionPregunta { IdPregunta=61,  IdSeccion =8,Descripcion = "¿Las paredes de la zona de cajeros automáticos interior se encuentran limpias?"},
                new SupervisionPregunta { IdPregunta=62,  IdSeccion =8,Descripcion = "¿El cajero automático interior se encuentra limpio?"},
                new SupervisionPregunta { IdPregunta=63,  IdSeccion =8,Descripcion = "¿El bote de basura de la zona de cajeros automáticos interior se encuentra limpio?"},
                new SupervisionPregunta { IdPregunta=64,  IdSeccion =8,Descripcion = "¿Los pisos de la zona de Asesores financieros se encuentran limpios? "},
                new SupervisionPregunta { IdPregunta=65,  IdSeccion =8,Descripcion = "¿Los escritorios de los Asesores financieros se encuentran limpios?"},
                new SupervisionPregunta { IdPregunta=66,  IdSeccion =8,Descripcion = " ¿Los botes de basura de la zona de Asesores financieros se encuentran limpios?"},
                new SupervisionPregunta { IdPregunta=67,  IdSeccion =8,Descripcion = "¿Las sillas de los Asesores financieros se encuentran limpias? "},
                new SupervisionPregunta { IdPregunta=68,  IdSeccion =8,Descripcion = "¿Las sillas de los clientes en patio bancario se encuentran limpias?"},
                new SupervisionPregunta { IdPregunta=69,  IdSeccion =8,Descripcion = "¿Los postes de la unifila se encuentran sin polvo o basura?"},
                new SupervisionPregunta { IdPregunta=70,  IdSeccion =8,Descripcion = "¿Las ventanillas de se encuentran limpias? "},
                new SupervisionPregunta { IdPregunta=71,  IdSeccion =8,Descripcion = "¿Los pisos de la zona de cajas se encuentran limpios?"},
                new SupervisionPregunta { IdPregunta=72,  IdSeccion =9,Descripcion = "Cumplimiento de la programación de los trabajos"},
                new SupervisionPregunta { IdPregunta=73,  IdSeccion =9,Descripcion = "Cumplimiento de los horarios del personal "},
                new SupervisionPregunta { IdPregunta=74,  IdSeccion =9,Descripcion = "Disponibilidad de supervisores"},
                new SupervisionPregunta { IdPregunta=75,  IdSeccion =9,Descripcion = "Trato del personal de la empresa de limpieza a los clientes"},
                new SupervisionPregunta { IdPregunta=76,  IdSeccion =9,Descripcion = "Aspecto general de la persona (UNIFORMES)"},
                new SupervisionPregunta { IdPregunta=77,  IdSeccion =10,Descripcion = "Pasillos "},
                new SupervisionPregunta { IdPregunta=78,  IdSeccion =10,Descripcion = "Baños en general"},
                new SupervisionPregunta { IdPregunta=79,  IdSeccion =10,Descripcion = "Equipo de telefonia y computo"},
                new SupervisionPregunta { IdPregunta=80,  IdSeccion =10,Descripcion = "Limpieza de Cajas"},
                new SupervisionPregunta { IdPregunta=81,  IdSeccion =10,Descripcion = "Piso de Venta"},
                new SupervisionPregunta { IdPregunta=82,  IdSeccion =10,Descripcion = "Espacios Comunes"},
                new SupervisionPregunta { IdPregunta=83,  IdSeccion =10,Descripcion = "Area de comedor o cocinetas"},
                new SupervisionPregunta { IdPregunta=84,  IdSeccion =11,Descripcion = "Mobiliario (Mesas, sillones y sillas)"},
                new SupervisionPregunta { IdPregunta=85,  IdSeccion =11,Descripcion = "Limpieza de bodega"},
                new SupervisionPregunta { IdPregunta=86,  IdSeccion =11,Descripcion = "Cristales Externos"},
                new SupervisionPregunta { IdPregunta=87,  IdSeccion =11,Descripcion = "Espejos, carteles y acabados"},
                new SupervisionPregunta { IdPregunta=88,  IdSeccion =11,Descripcion = "Limipeza de piso (loceta vinilica e interceramica, marmol, duela laminada, azulejo, cemento pulido)"},
                new SupervisionPregunta { IdPregunta=89,  IdSeccion =11,Descripcion = "Exhibidores"},
                new SupervisionPregunta { IdPregunta=90,  IdSeccion =11,Descripcion = "limpieza de covacha y inventarios de materiales y maquinaria "},
                new SupervisionPregunta { IdPregunta=91,  IdSeccion =12,Descripcion = "¿El piso de la zona de cajeros automáticos exterior se encuentra limpio?"},
                new SupervisionPregunta { IdPregunta=92,  IdSeccion =12,Descripcion = "¿Las paredes de la zona de cajeros automáticos exterior se encuentran limpias?"},
                new SupervisionPregunta { IdPregunta=93,  IdSeccion =12,Descripcion = "¿Los cristales de la zona de cajeros automáticos exterior se encuentran limpios?"},
                new SupervisionPregunta { IdPregunta=94,  IdSeccion =12,Descripcion = "¿El cajero automático exterior se encuentra limpio?"},
                new SupervisionPregunta { IdPregunta=95,  IdSeccion =12,Descripcion = "¿El bote de basura de la zona de cajeros automáticos exterior se encuentra limpio?"},
                new SupervisionPregunta { IdPregunta=96,  IdSeccion =12,Descripcion = "¿El piso de la zona de cajeros automáticos interior se encuentra limpio?"},
                new SupervisionPregunta { IdPregunta=97,  IdSeccion =12,Descripcion = "¿Las paredes de la zona de cajeros automáticos interior se encuentran limpias?"},
                new SupervisionPregunta { IdPregunta=98,  IdSeccion =12,Descripcion = "¿El cajero automático interior se encuentra limpio?"},
                new SupervisionPregunta { IdPregunta=99,  IdSeccion =12,Descripcion = "¿El bote de basura de la zona de cajeros automáticos interior se encuentra limpio?"},
                new SupervisionPregunta { IdPregunta=100,  IdSeccion =12,Descripcion = "¿Los pisos de la zona de Asesores financieros se encuentran limpios? "},
                new SupervisionPregunta { IdPregunta=101,  IdSeccion =12,Descripcion = "¿Los escritorios de los Asesores financieros se encuentran limpios?"},
                new SupervisionPregunta { IdPregunta=102,  IdSeccion =12,Descripcion = " ¿Los botes de basura de la zona de Asesores financieros se encuentran limpios?"},
                new SupervisionPregunta { IdPregunta=103,  IdSeccion =12,Descripcion = "¿Las sillas de los Asesores financieros se encuentran limpias? "},
                new SupervisionPregunta { IdPregunta=104,  IdSeccion =12,Descripcion = "¿Las sillas de los clientes en patio bancario se encuentran limpias?"},
                new SupervisionPregunta { IdPregunta=105,  IdSeccion =12,Descripcion = "¿Los postes de la unifila se encuentran sin polvo o basura?"},
                new SupervisionPregunta { IdPregunta=106,  IdSeccion =12,Descripcion = "¿Las ventanillas de se encuentran limpias? "},
                new SupervisionPregunta { IdPregunta=107,  IdSeccion =12,Descripcion = "¿Los pisos de la zona de cajas se encuentran limpios?"},
                new SupervisionPregunta { IdPregunta=108,  IdSeccion =13,Descripcion = "Cumplimiento de la programación de los trabajos"},
                new SupervisionPregunta { IdPregunta=109,  IdSeccion =13,Descripcion = "Cumplimiento de los horarios del personal "},
                new SupervisionPregunta { IdPregunta=110,  IdSeccion =13,Descripcion = "Disponibilidad de supervisores"},
                new SupervisionPregunta { IdPregunta=111,  IdSeccion =13,Descripcion = "Trato del personal de la empresa de limpieza a los clientes"},
                new SupervisionPregunta { IdPregunta=112,  IdSeccion =13,Descripcion = "Aspecto general de la persona (UNIFORMES)"},
                new SupervisionPregunta { IdPregunta=113,  IdSeccion =13,Descripcion = "Botas de seguridad, chaleco reflejante y casco"},
                new SupervisionPregunta { IdPregunta=114,  IdSeccion =14,Descripcion = "Zona atención al cliente y recepcion"},
                new SupervisionPregunta { IdPregunta=115,  IdSeccion =14,Descripcion = "Baños en general"},
                new SupervisionPregunta { IdPregunta=116,  IdSeccion =14,Descripcion = "Equipo de telefonia y computo"},
                new SupervisionPregunta { IdPregunta=117,  IdSeccion =14,Descripcion = "Limpieza de Cajas"},
                new SupervisionPregunta { IdPregunta=118,  IdSeccion =14,Descripcion = "pasillos y andadores "},
                new SupervisionPregunta { IdPregunta=119,  IdSeccion =14,Descripcion = "Espacios Comunes"},
                new SupervisionPregunta { IdPregunta=120,  IdSeccion =14,Descripcion = "Area de comedor o cocinetas"},
                new SupervisionPregunta { IdPregunta=121,  IdSeccion =14,Descripcion = "Estacionamientos y area de carga  ( area de proovedores )"},
                new SupervisionPregunta { IdPregunta=122,  IdSeccion =14,Descripcion = "Area de maquinas "},
                new SupervisionPregunta { IdPregunta=123,  IdSeccion =15,Descripcion = "Mobiliario (Mesas, sillones y sillas)"},
                new SupervisionPregunta { IdPregunta=124,  IdSeccion =15,Descripcion = "Limpieza de bodega"},
                new SupervisionPregunta { IdPregunta=125,  IdSeccion =15,Descripcion = "Cristales Externos"},
                new SupervisionPregunta { IdPregunta=126,  IdSeccion =15,Descripcion = "Espejos, carteles y acabados"},
                new SupervisionPregunta { IdPregunta=127,  IdSeccion =15,Descripcion = "Limipeza de piso (loceta vinilica e interceramica, marmol, duela laminada, azulejo, cemento pulido)"},
                new SupervisionPregunta { IdPregunta=128,  IdSeccion =15,Descripcion = "Exhibidores"},
                new SupervisionPregunta { IdPregunta=129,  IdSeccion =16,Descripcion = "¿El piso de la zona de cajeros automáticos exterior se encuentra limpio?"},
                new SupervisionPregunta { IdPregunta=130,  IdSeccion =16,Descripcion = "¿Las paredes de la zona de cajeros automáticos exterior se encuentran limpias?"},
                new SupervisionPregunta { IdPregunta=131,  IdSeccion =16,Descripcion = "¿Los cristales de la zona de cajeros automáticos exterior se encuentran limpios?"},
                new SupervisionPregunta { IdPregunta=132,  IdSeccion =16,Descripcion = "¿El cajero automático exterior se encuentra limpio?"},
                new SupervisionPregunta { IdPregunta=133,  IdSeccion =16,Descripcion = "¿El bote de basura de la zona de cajeros automáticos exterior se encuentra limpio?"},
                new SupervisionPregunta { IdPregunta=134,  IdSeccion =16,Descripcion = "¿El piso de la zona de cajeros automáticos interior se encuentra limpio?"},
                new SupervisionPregunta { IdPregunta=135,  IdSeccion =16,Descripcion = "¿Las paredes de la zona de cajeros automáticos interior se encuentran limpias?"},
                new SupervisionPregunta { IdPregunta=136,  IdSeccion =16,Descripcion = "¿El cajero automático interior se encuentra limpio?"},
                new SupervisionPregunta { IdPregunta=137,  IdSeccion =16,Descripcion = "¿El bote de basura de la zona de cajeros automáticos interior se encuentra limpio?"},
                new SupervisionPregunta { IdPregunta=138,  IdSeccion =16,Descripcion = "¿Los pisos de la zona de Asesores financieros se encuentran limpios? "},
                new SupervisionPregunta { IdPregunta=139,  IdSeccion =16,Descripcion = "¿Los escritorios de los Asesores financieros se encuentran limpios?"},
                new SupervisionPregunta { IdPregunta=140,  IdSeccion =16,Descripcion = "¿Los botes de basura de la zona de Asesores financieros se encuentran limpios?"},
                new SupervisionPregunta { IdPregunta=141,  IdSeccion =16,Descripcion = "¿Las sillas de los Asesores financieros se encuentran limpias? "},
                new SupervisionPregunta { IdPregunta=142,  IdSeccion =16,Descripcion = "¿Las sillas de los clientes en patio bancario se encuentran limpias?"},
                new SupervisionPregunta { IdPregunta=143,  IdSeccion =16,Descripcion = "¿Los postes de la unifila se encuentran sin polvo o basura?"},
                new SupervisionPregunta { IdPregunta=144,  IdSeccion =16,Descripcion = "¿Las ventanillas se encuentran limpias? "},
                new SupervisionPregunta { IdPregunta=145,  IdSeccion =16,Descripcion = "¿Los pisos de la zona de cajas se encuentran limpios?"},

            };
            return preguntas.Where(p => p.IdSeccion == idSeccion).ToList();
        //} else {
        //    List<SupervisionPregunta> preguntasRemoto = new List<SupervisionPregunta>();
        //    string url = $"{Constants.GET_SUPERVISION_PREGUNTAS}/{idSeccion}";
        //    HttpHelper httpHelper = new HttpHelper();
        //    preguntasRemoto = await httpHelper.GetAsync<List<SupervisionPregunta>>(url);
        //    if(preguntasRemoto != null) {
        //        return preguntasRemoto;
        //    } else {
        //        List<SupervisionPregunta> preguntas = new List<SupervisionPregunta> {
        //        new SupervisionPregunta { IdPregunta=1,  IdSeccion =1,Descripcion = "Cumplimiento de la programación de los trabajos"},
        //        new SupervisionPregunta { IdPregunta=2,  IdSeccion =1,Descripcion = "Cumplimiento de los horarios del personal "},
        //        new SupervisionPregunta { IdPregunta=3,  IdSeccion =1,Descripcion = "Disponibilidad de supervisores"},
        //        new SupervisionPregunta { IdPregunta=4,  IdSeccion =1,Descripcion = "Trato del personal de la empresa de limpieza a los clientes"},
        //        new SupervisionPregunta { IdPregunta=5,  IdSeccion =1,Descripcion = "Aspecto general de la persona (UNIFORMES)"},
        //        new SupervisionPregunta { IdPregunta=6,  IdSeccion =2,Descripcion = "Zona atención al cliente y recepcion"},
        //        new SupervisionPregunta { IdPregunta=7,  IdSeccion =2,Descripcion = "Baños en general"},
        //        new SupervisionPregunta { IdPregunta=8,  IdSeccion =2,Descripcion = "Equipo de telefonia y computo"},
        //        new SupervisionPregunta { IdPregunta=9,  IdSeccion =2,Descripcion = "Limpieza de vidrios "},
        //        new SupervisionPregunta { IdPregunta=10,  IdSeccion =2,Descripcion = "pasillos y andadores "},
        //        new SupervisionPregunta { IdPregunta=11,  IdSeccion =2,Descripcion = "Espacios Comunes"},
        //        new SupervisionPregunta { IdPregunta=12,  IdSeccion =2,Descripcion = "Area de comedor o cocinetas"},
        //        new SupervisionPregunta { IdPregunta=13,  IdSeccion =3,Descripcion = "Mobiliario (Mesas, sillones y sillas, partes bajas )"},
        //        new SupervisionPregunta { IdPregunta=14,  IdSeccion =3,Descripcion = "Limpieza de bodega"},
        //        new SupervisionPregunta { IdPregunta=15,  IdSeccion =3,Descripcion = "Cristales Externos"},
        //        new SupervisionPregunta { IdPregunta=16,  IdSeccion =3,Descripcion = "Espejos, carteles y acabados"},
        //        new SupervisionPregunta { IdPregunta=17,  IdSeccion =3,Descripcion = "Limipeza de piso (loceta vinilica e interceramica, marmol, duela laminada, azulejo, cemento pulido)"},
        //        new SupervisionPregunta { IdPregunta=18,  IdSeccion =3,Descripcion = "Inventarios de material y maquinaria "},
        //        new SupervisionPregunta { IdPregunta=19,  IdSeccion =4,Descripcion = "¿El piso de la zona de cajeros automáticos exterior se encuentra limpio?"},
        //        new SupervisionPregunta { IdPregunta=20,  IdSeccion =4,Descripcion = "¿Las paredes de la zona de cajeros automáticos exterior se encuentran limpias?"},
        //        new SupervisionPregunta { IdPregunta=21,  IdSeccion =4,Descripcion = "¿Los cristales de la zona de cajeros automáticos exterior se encuentran limpios?"},
        //        new SupervisionPregunta { IdPregunta=22,  IdSeccion =4,Descripcion = "¿El cajero automático exterior se encuentra limpio?"},
        //        new SupervisionPregunta { IdPregunta=23,  IdSeccion =4,Descripcion = "¿El bote de basura de la zona de cajeros automáticos exterior se encuentra limpio?"},
        //        new SupervisionPregunta { IdPregunta=24,  IdSeccion =4,Descripcion = "¿El piso de la zona de cajeros automáticos interior se encuentra limpio?"},
        //        new SupervisionPregunta { IdPregunta=25,  IdSeccion =4,Descripcion = "¿Las paredes de la zona de cajeros automáticos interior se encuentran limpias?"},
        //        new SupervisionPregunta { IdPregunta=26,  IdSeccion =4,Descripcion = "¿El cajero automático interior se encuentra limpio?"},
        //        new SupervisionPregunta { IdPregunta=27,  IdSeccion =4,Descripcion = "¿El bote de basura de la zona de cajeros automáticos interior se encuentra limpio?"},
        //        new SupervisionPregunta { IdPregunta=28,  IdSeccion =4,Descripcion = "¿Los pisos de la zona de Asesores financieros se encuentran limpios? "},
        //        new SupervisionPregunta { IdPregunta=29,  IdSeccion =4,Descripcion = "¿Los escritorios de los Asesores financieros se encuentran limpios?"},
        //        new SupervisionPregunta { IdPregunta=30,  IdSeccion =4,Descripcion = " ¿Los botes de basura de la zona de Asesores financieros se encuentran limpios?"},
        //        new SupervisionPregunta { IdPregunta=31,  IdSeccion =4,Descripcion = "¿Las sillas de los Asesores financieros se encuentran limpias? "},
        //        new SupervisionPregunta { IdPregunta=32,  IdSeccion =4,Descripcion = "¿Las sillas de los clientes en patio bancario se encuentran limpias?"},
        //        new SupervisionPregunta { IdPregunta=33,  IdSeccion =4,Descripcion = "¿Los postes de la unifila se encuentran sin polvo o basura?"},
        //        new SupervisionPregunta { IdPregunta=34,  IdSeccion =4,Descripcion = "¿Las ventanillas se encuentran limpias? "},
        //        new SupervisionPregunta { IdPregunta=35,  IdSeccion =4,Descripcion = "¿Los pisos de la zona de cajas se encuentran limpios?"},
        //        new SupervisionPregunta { IdPregunta=36,  IdSeccion =5,Descripcion = "Cumplimiento de la programación de los trabajos"},
        //        new SupervisionPregunta { IdPregunta=37,  IdSeccion =5,Descripcion = "Cumplimiento de los horarios del personal "},
        //        new SupervisionPregunta { IdPregunta=38,  IdSeccion =5,Descripcion = "Disponibilidad de supervisores"},
        //        new SupervisionPregunta { IdPregunta=39,  IdSeccion =5,Descripcion = "Trato del personal de la empresa de limpieza a los clientes"},
        //        new SupervisionPregunta { IdPregunta=40,  IdSeccion =5,Descripcion = "Aspecto general de la persona (UNIFORMES)"},
        //        new SupervisionPregunta { IdPregunta=41,  IdSeccion =6,Descripcion = "Zona atención al cliente y recepcion"},
        //        new SupervisionPregunta { IdPregunta=42,  IdSeccion =6,Descripcion = "Baños en general"},
        //        new SupervisionPregunta { IdPregunta=43,  IdSeccion =6,Descripcion = "Equipo de telefonia y computo"},
        //        new SupervisionPregunta { IdPregunta=44,  IdSeccion =6,Descripcion = "Limpieza de Cajas"},
        //        new SupervisionPregunta { IdPregunta=45,  IdSeccion =6,Descripcion = "Piso de Venta"},
        //        new SupervisionPregunta { IdPregunta=46,  IdSeccion =6,Descripcion = "Espacios Comunes"},
        //        new SupervisionPregunta { IdPregunta=47,  IdSeccion =6,Descripcion = "Area de comedor o cocinetas"},
        //        new SupervisionPregunta { IdPregunta=48,  IdSeccion =7,Descripcion = "Mobiliario (Mesas, sillones y sillas)"},
        //        new SupervisionPregunta { IdPregunta=49,  IdSeccion =7,Descripcion = "Limpieza de bodega"},
        //        new SupervisionPregunta { IdPregunta=50,  IdSeccion =7,Descripcion = "Cristales Externos"},
        //        new SupervisionPregunta { IdPregunta=51,  IdSeccion =7,Descripcion = "Espejos, carteles y acabados"},
        //        new SupervisionPregunta { IdPregunta=52,  IdSeccion =7,Descripcion = "Limipeza de piso (loceta vinilica e interceramica, marmol, duela laminada, azulejo, cemento pulido)"},
        //        new SupervisionPregunta { IdPregunta=53,  IdSeccion =7,Descripcion = "Exhibidores"},
        //        new SupervisionPregunta { IdPregunta=54,  IdSeccion =7,Descripcion = "Inventarios de material y maquinaria "},
        //        new SupervisionPregunta { IdPregunta=55,  IdSeccion =8,Descripcion = "¿El piso de la zona de cajeros automáticos exterior se encuentra limpio?"},
        //        new SupervisionPregunta { IdPregunta=56,  IdSeccion =8,Descripcion = "¿Las paredes de la zona de cajeros automáticos exterior se encuentran limpias?"},
        //        new SupervisionPregunta { IdPregunta=57,  IdSeccion =8,Descripcion = "¿Los cristales de la zona de cajeros automáticos exterior se encuentran limpios?"},
        //        new SupervisionPregunta { IdPregunta=58,  IdSeccion =8,Descripcion = "¿El cajero automático exterior se encuentra limpio?"},
        //        new SupervisionPregunta { IdPregunta=59,  IdSeccion =8,Descripcion = "¿El bote de basura de la zona de cajeros automáticos exterior se encuentra limpio?"},
        //        new SupervisionPregunta { IdPregunta=60,  IdSeccion =8,Descripcion = "¿El piso de la zona de cajeros automáticos interior se encuentra limpio?"},
        //        new SupervisionPregunta { IdPregunta=61,  IdSeccion =8,Descripcion = "¿Las paredes de la zona de cajeros automáticos interior se encuentran limpias?"},
        //        new SupervisionPregunta { IdPregunta=62,  IdSeccion =8,Descripcion = "¿El cajero automático interior se encuentra limpio?"},
        //        new SupervisionPregunta { IdPregunta=63,  IdSeccion =8,Descripcion = "¿El bote de basura de la zona de cajeros automáticos interior se encuentra limpio?"},
        //        new SupervisionPregunta { IdPregunta=64,  IdSeccion =8,Descripcion = "¿Los pisos de la zona de Asesores financieros se encuentran limpios? "},
        //        new SupervisionPregunta { IdPregunta=65,  IdSeccion =8,Descripcion = "¿Los escritorios de los Asesores financieros se encuentran limpios?"},
        //        new SupervisionPregunta { IdPregunta=66,  IdSeccion =8,Descripcion = " ¿Los botes de basura de la zona de Asesores financieros se encuentran limpios?"},
        //        new SupervisionPregunta { IdPregunta=67,  IdSeccion =8,Descripcion = "¿Las sillas de los Asesores financieros se encuentran limpias? "},
        //        new SupervisionPregunta { IdPregunta=68,  IdSeccion =8,Descripcion = "¿Las sillas de los clientes en patio bancario se encuentran limpias?"},
        //        new SupervisionPregunta { IdPregunta=69,  IdSeccion =8,Descripcion = "¿Los postes de la unifila se encuentran sin polvo o basura?"},
        //        new SupervisionPregunta { IdPregunta=70,  IdSeccion =8,Descripcion = "¿Las ventanillas de se encuentran limpias? "},
        //        new SupervisionPregunta { IdPregunta=71,  IdSeccion =8,Descripcion = "¿Los pisos de la zona de cajas se encuentran limpios?"},
        //        new SupervisionPregunta { IdPregunta=72,  IdSeccion =9,Descripcion = "Cumplimiento de la programación de los trabajos"},
        //        new SupervisionPregunta { IdPregunta=73,  IdSeccion =9,Descripcion = "Cumplimiento de los horarios del personal "},
        //        new SupervisionPregunta { IdPregunta=74,  IdSeccion =9,Descripcion = "Disponibilidad de supervisores"},
        //        new SupervisionPregunta { IdPregunta=75,  IdSeccion =9,Descripcion = "Trato del personal de la empresa de limpieza a los clientes"},
        //        new SupervisionPregunta { IdPregunta=76,  IdSeccion =9,Descripcion = "Aspecto general de la persona (UNIFORMES)"},
        //        new SupervisionPregunta { IdPregunta=77,  IdSeccion =10,Descripcion = "Pasillos "},
        //        new SupervisionPregunta { IdPregunta=78,  IdSeccion =10,Descripcion = "Baños en general"},
        //        new SupervisionPregunta { IdPregunta=79,  IdSeccion =10,Descripcion = "Equipo de telefonia y computo"},
        //        new SupervisionPregunta { IdPregunta=80,  IdSeccion =10,Descripcion = "Limpieza de Cajas"},
        //        new SupervisionPregunta { IdPregunta=81,  IdSeccion =10,Descripcion = "Piso de Venta"},
        //        new SupervisionPregunta { IdPregunta=82,  IdSeccion =10,Descripcion = "Espacios Comunes"},
        //        new SupervisionPregunta { IdPregunta=83,  IdSeccion =10,Descripcion = "Area de comedor o cocinetas"},
        //        new SupervisionPregunta { IdPregunta=84,  IdSeccion =11,Descripcion = "Mobiliario (Mesas, sillones y sillas)"},
        //        new SupervisionPregunta { IdPregunta=85,  IdSeccion =11,Descripcion = "Limpieza de bodega"},
        //        new SupervisionPregunta { IdPregunta=86,  IdSeccion =11,Descripcion = "Cristales Externos"},
        //        new SupervisionPregunta { IdPregunta=87,  IdSeccion =11,Descripcion = "Espejos, carteles y acabados"},
        //        new SupervisionPregunta { IdPregunta=88,  IdSeccion =11,Descripcion = "Limipeza de piso (loceta vinilica e interceramica, marmol, duela laminada, azulejo, cemento pulido)"},
        //        new SupervisionPregunta { IdPregunta=89,  IdSeccion =11,Descripcion = "Exhibidores"},
        //        new SupervisionPregunta { IdPregunta=90,  IdSeccion =11,Descripcion = "limpieza de covacha y inventarios de materiales y maquinaria "},
        //        new SupervisionPregunta { IdPregunta=91,  IdSeccion =12,Descripcion = "¿El piso de la zona de cajeros automáticos exterior se encuentra limpio?"},
        //        new SupervisionPregunta { IdPregunta=92,  IdSeccion =12,Descripcion = "¿Las paredes de la zona de cajeros automáticos exterior se encuentran limpias?"},
        //        new SupervisionPregunta { IdPregunta=93,  IdSeccion =12,Descripcion = "¿Los cristales de la zona de cajeros automáticos exterior se encuentran limpios?"},
        //        new SupervisionPregunta { IdPregunta=94,  IdSeccion =12,Descripcion = "¿El cajero automático exterior se encuentra limpio?"},
        //        new SupervisionPregunta { IdPregunta=95,  IdSeccion =12,Descripcion = "¿El bote de basura de la zona de cajeros automáticos exterior se encuentra limpio?"},
        //        new SupervisionPregunta { IdPregunta=96,  IdSeccion =12,Descripcion = "¿El piso de la zona de cajeros automáticos interior se encuentra limpio?"},
        //        new SupervisionPregunta { IdPregunta=97,  IdSeccion =12,Descripcion = "¿Las paredes de la zona de cajeros automáticos interior se encuentran limpias?"},
        //        new SupervisionPregunta { IdPregunta=98,  IdSeccion =12,Descripcion = "¿El cajero automático interior se encuentra limpio?"},
        //        new SupervisionPregunta { IdPregunta=99,  IdSeccion =12,Descripcion = "¿El bote de basura de la zona de cajeros automáticos interior se encuentra limpio?"},
        //        new SupervisionPregunta { IdPregunta=100,  IdSeccion =12,Descripcion = "¿Los pisos de la zona de Asesores financieros se encuentran limpios? "},
        //        new SupervisionPregunta { IdPregunta=101,  IdSeccion =12,Descripcion = "¿Los escritorios de los Asesores financieros se encuentran limpios?"},
        //        new SupervisionPregunta { IdPregunta=102,  IdSeccion =12,Descripcion = " ¿Los botes de basura de la zona de Asesores financieros se encuentran limpios?"},
        //        new SupervisionPregunta { IdPregunta=103,  IdSeccion =12,Descripcion = "¿Las sillas de los Asesores financieros se encuentran limpias? "},
        //        new SupervisionPregunta { IdPregunta=104,  IdSeccion =12,Descripcion = "¿Las sillas de los clientes en patio bancario se encuentran limpias?"},
        //        new SupervisionPregunta { IdPregunta=105,  IdSeccion =12,Descripcion = "¿Los postes de la unifila se encuentran sin polvo o basura?"},
        //        new SupervisionPregunta { IdPregunta=106,  IdSeccion =12,Descripcion = "¿Las ventanillas de se encuentran limpias? "},
        //        new SupervisionPregunta { IdPregunta=107,  IdSeccion =12,Descripcion = "¿Los pisos de la zona de cajas se encuentran limpios?"},
        //        new SupervisionPregunta { IdPregunta=108,  IdSeccion =13,Descripcion = "Cumplimiento de la programación de los trabajos"},
        //        new SupervisionPregunta { IdPregunta=109,  IdSeccion =13,Descripcion = "Cumplimiento de los horarios del personal "},
        //        new SupervisionPregunta { IdPregunta=110,  IdSeccion =13,Descripcion = "Disponibilidad de supervisores"},
        //        new SupervisionPregunta { IdPregunta=111,  IdSeccion =13,Descripcion = "Trato del personal de la empresa de limpieza a los clientes"},
        //        new SupervisionPregunta { IdPregunta=112,  IdSeccion =13,Descripcion = "Aspecto general de la persona (UNIFORMES)"},
        //        new SupervisionPregunta { IdPregunta=113,  IdSeccion =13,Descripcion = "Botas de seguridad, chaleco reflejante y casco"},
        //        new SupervisionPregunta { IdPregunta=114,  IdSeccion =14,Descripcion = "Zona atención al cliente y recepcion"},
        //        new SupervisionPregunta { IdPregunta=115,  IdSeccion =14,Descripcion = "Baños en general"},
        //        new SupervisionPregunta { IdPregunta=116,  IdSeccion =14,Descripcion = "Equipo de telefonia y computo"},
        //        new SupervisionPregunta { IdPregunta=117,  IdSeccion =14,Descripcion = "Limpieza de Cajas"},
        //        new SupervisionPregunta { IdPregunta=118,  IdSeccion =14,Descripcion = "pasillos y andadores "},
        //        new SupervisionPregunta { IdPregunta=119,  IdSeccion =14,Descripcion = "Espacios Comunes"},
        //        new SupervisionPregunta { IdPregunta=120,  IdSeccion =14,Descripcion = "Area de comedor o cocinetas"},
        //        new SupervisionPregunta { IdPregunta=121,  IdSeccion =14,Descripcion = "Estacionamientos y area de carga  ( area de proovedores )"},
        //        new SupervisionPregunta { IdPregunta=122,  IdSeccion =14,Descripcion = "Area de maquinas "},
        //        new SupervisionPregunta { IdPregunta=123,  IdSeccion =15,Descripcion = "Mobiliario (Mesas, sillones y sillas)"},
        //        new SupervisionPregunta { IdPregunta=124,  IdSeccion =15,Descripcion = "Limpieza de bodega"},
        //        new SupervisionPregunta { IdPregunta=125,  IdSeccion =15,Descripcion = "Cristales Externos"},
        //        new SupervisionPregunta { IdPregunta=126,  IdSeccion =15,Descripcion = "Espejos, carteles y acabados"},
        //        new SupervisionPregunta { IdPregunta=127,  IdSeccion =15,Descripcion = "Limipeza de piso (loceta vinilica e interceramica, marmol, duela laminada, azulejo, cemento pulido)"},
        //        new SupervisionPregunta { IdPregunta=128,  IdSeccion =15,Descripcion = "Exhibidores"},
        //        new SupervisionPregunta { IdPregunta=129,  IdSeccion =16,Descripcion = "¿El piso de la zona de cajeros automáticos exterior se encuentra limpio?"},
        //        new SupervisionPregunta { IdPregunta=130,  IdSeccion =16,Descripcion = "¿Las paredes de la zona de cajeros automáticos exterior se encuentran limpias?"},
        //        new SupervisionPregunta { IdPregunta=131,  IdSeccion =16,Descripcion = "¿Los cristales de la zona de cajeros automáticos exterior se encuentran limpios?"},
        //        new SupervisionPregunta { IdPregunta=132,  IdSeccion =16,Descripcion = "¿El cajero automático exterior se encuentra limpio?"},
        //        new SupervisionPregunta { IdPregunta=133,  IdSeccion =16,Descripcion = "¿El bote de basura de la zona de cajeros automáticos exterior se encuentra limpio?"},
        //        new SupervisionPregunta { IdPregunta=134,  IdSeccion =16,Descripcion = "¿El piso de la zona de cajeros automáticos interior se encuentra limpio?"},
        //        new SupervisionPregunta { IdPregunta=135,  IdSeccion =16,Descripcion = "¿Las paredes de la zona de cajeros automáticos interior se encuentran limpias?"},
        //        new SupervisionPregunta { IdPregunta=136,  IdSeccion =16,Descripcion = "¿El cajero automático interior se encuentra limpio?"},
        //        new SupervisionPregunta { IdPregunta=137,  IdSeccion =16,Descripcion = "¿El bote de basura de la zona de cajeros automáticos interior se encuentra limpio?"},
        //        new SupervisionPregunta { IdPregunta=138,  IdSeccion =16,Descripcion = "¿Los pisos de la zona de Asesores financieros se encuentran limpios? "},
        //        new SupervisionPregunta { IdPregunta=139,  IdSeccion =16,Descripcion = "¿Los escritorios de los Asesores financieros se encuentran limpios?"},
        //        new SupervisionPregunta { IdPregunta=140,  IdSeccion =16,Descripcion = "¿Los botes de basura de la zona de Asesores financieros se encuentran limpios?"},
        //        new SupervisionPregunta { IdPregunta=141,  IdSeccion =16,Descripcion = "¿Las sillas de los Asesores financieros se encuentran limpias? "},
        //        new SupervisionPregunta { IdPregunta=142,  IdSeccion =16,Descripcion = "¿Las sillas de los clientes en patio bancario se encuentran limpias?"},
        //        new SupervisionPregunta { IdPregunta=143,  IdSeccion =16,Descripcion = "¿Los postes de la unifila se encuentran sin polvo o basura?"},
        //        new SupervisionPregunta { IdPregunta=144,  IdSeccion =16,Descripcion = "¿Las ventanillas se encuentran limpias? "},
        //        new SupervisionPregunta { IdPregunta=145,  IdSeccion =16,Descripcion = "¿Los pisos de la zona de cajas se encuentran limpios?"},

        //    };
        //        return preguntas.Where(p => p.IdSeccion == idSeccion).ToList();
        //    }
        //}
    }
}

public class Evaluacion() : BaseNotify
{
    [JsonIgnore]
    public int IdSupervision { get; set; }
    public int IdPregunta { get; set; }
    public string Descripcion { get; set; }

    float? _valor;
    public float? Valor
    {
        get => _valor;
        set
        {
            if (_valor == value)
            {
                return;
            }

            _valor = value;
            Observaciones = string.Empty;
            OnPropertyChanged();
        }
    }

    string _observaciones;
    public string Observaciones
    {
        get => _observaciones;
        set
        {
            if (_observaciones == value)
            {
                return;
            }

            _observaciones = value;
            OnPropertyChanged();
        }
    }

    public static async Task<List<Evaluacion>> ObtenerPreguntas()
    {
        if (!Utils.InternetUtil.IsConnectedInternet())
        {
            return new List<Evaluacion> {
                new Evaluacion {
                    IdPregunta = 1,
                    Descripcion = "1.- ¿Cómo es el estado de la limpieza en el lugar?"
                },
                new Evaluacion {
                    IdPregunta = 2,
                    Descripcion = "2.- ¿Los materiales se encuentran acomodados?"
                },
                new Evaluacion {
                    IdPregunta = 3,
                    Descripcion = "3.- ¿Existe material visual?"
                },
                new Evaluacion {
                    IdPregunta = 4,
                    Descripcion = "4.- Estado de los materiales actuales"
                },
                new Evaluacion {
                    IdPregunta = 5,
                    Descripcion = "5.- ¿El material esta debidammente etiquetado?"
                }
            };
        }

        List<Evaluacion> preguntasRemoto = new List<Evaluacion>();
        HttpHelper httpHelper = new HttpHelper();
        preguntasRemoto = await httpHelper.GetAsync<List<Evaluacion>>(Constants.GET_SUPERVISION_PREGUNTAS_EVALUACION);
        if (preguntasRemoto != null)
        {
            return preguntasRemoto;
        }
        else
        {
            return new List<Evaluacion> {
                new Evaluacion {
                    IdPregunta = 1,
                    Descripcion = "1.- ¿Cómo es el estado de la limpieza en el lugar?"
                },
                new Evaluacion {
                    IdPregunta = 2,
                    Descripcion = "2.- ¿Los materiales se encuentran acomodados?"
                },
                new Evaluacion {
                    IdPregunta = 3,
                    Descripcion = "3.- ¿Existe material visual?"
                },
                new Evaluacion {
                    IdPregunta = 4,
                    Descripcion = "4.- Estado de los materiales actuales"
                },
                new Evaluacion {
                    IdPregunta = 5,
                    Descripcion = "5.- ¿El material esta debidammente etiquetado?"
                }
            };
        }
    }
}

public class ChecklistPregunta : BaseNotify
{
    [JsonIgnore]
    public int IdSupervision { get; set; }
    public int IdPregunta { get; set; }
    public string Descripcion { get; set; }
    bool _valor;
    public bool Valor
    {
        get => _valor;
        set
        {
            if (_valor == value)
            {
                return;
            }

            _valor = value;
            OnPropertyChanged();

            if (_valor)
            {
                Observaciones = null;
            }
        }
    }
    public string Observaciones { get; set; }

    public static async Task<List<ChecklistPregunta>> ObtenerPreguntas()
    {

        //List<ChecklistPregunta> preguntasRemoto = new List<ChecklistPregunta>();
        //HttpHelper httpHelper = new HttpHelper();
        //preguntasRemoto = await httpHelper.GetAsync<List<ChecklistPregunta>>(Constants.GET_SUPERVISION_PREGUNTAS_OPERADOR);
        //if(preguntasRemoto != null) {
        //    return preguntasRemoto;
        //} else {
        return new List<ChecklistPregunta> {
                new ChecklistPregunta {
                    IdPregunta= 1,
                    Descripcion = "¿El personal asignado porta su uniforme?"
                },
                new ChecklistPregunta {
                    IdPregunta= 2,
                    Descripcion = "¿El personal asignado porta su gafete?"
                },
                //new ChecklistPregunta {
                //    IdPregunta= 3,
                //    Descripcion = "¿El personal cuenta con carta patronal?"
                //},
                new ChecklistPregunta {
                    IdPregunta= 3,
                    Descripcion = "¿El personal conoce el rombo de seguridad?"
                },
                new ChecklistPregunta {
                    IdPregunta= 4,
                    Descripcion = "¿El personal conoce numéricamente las 5 divisiones del rombo de seguridad?"
                },
                new ChecklistPregunta {
                    IdPregunta= 5,
                    Descripcion = "¿El personal conoce el manejo de los químicos?"
                },
                 new ChecklistPregunta {
                    IdPregunta= 6,
                    Descripcion = "¿El personal utiliza correctamente los químicos?"
                },
                new ChecklistPregunta {
                    IdPregunta= 7,
                    Descripcion = "¿El personal cuenta con sus herramientas de trabajo?"
                },
                new ChecklistPregunta {
                    IdPregunta= 8,
                    Descripcion = "¿El personal se presento en su hora de labores?"
                },
                new ChecklistPregunta {
                    IdPregunta= 9,
                    Descripcion = "¿El personal tiene limpias sus áreas?"
                },
                new ChecklistPregunta {
                    IdPregunta= 10,
                    Descripcion = "¿El personal recibió capacitación del manejo correcto de químicos?"
                }
            };
        //}
    }
}

public abstract class BaseNotify : INotifyPropertyChanged
{

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}