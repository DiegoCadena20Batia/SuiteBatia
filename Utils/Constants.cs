using BatiaSuite.Data;
using BatiaSuite.Models;
using BatiaSuite.Models.IncidenciasBiometa;
using BatiaSuite.Models.OrdenesTrabajo;
using BatiaSuite.Models.Supervision;
using BatiaSuite.Models.Vacantes;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Shiny;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text.RegularExpressions;

namespace BatiaSuite.Utils;

public static class Constants {

    public static bool IS_TABLET = DeviceInfo.Current.Idiom == DeviceIdiom.Tablet;
    public static bool IS_IOS = DeviceInfo.Current.Platform == DevicePlatform.iOS;

    #region GOTO
    public static async Task GoToAsync(string route) {
        await Shell.Current.GoToAsync(route, true);
    }

    public static async Task GoToAsync(string route, IDictionary<string, object> parameters) {
        await Shell.Current.GoToAsync(route, true, parameters);
    }
    #endregion

    #region API URI´s 
    //URL BASE PARA PRODUCCIÓN
    public const string API_BASE_URL = "https://www.singa.com.mx:5500/api/";
    //URL BASE PARA PRUEBAS
    //public const string API_BASE_URL = "https://www.singa.com.mx:8086/api/";
    public const string ENT_GET_PRECARGA_API = "entregaprecarga";
    public const string GET_MODULOS_MENU = "ObtenerModulos";
    public const string GET_TECNICOS_ORDENES_TRABAJO_API = "OTTecnico";
    public const string GET_TIPO_ORDENES_TRABAJO_API = "OTTipoOrdenes";
    public const string ORDENES_TRABAJO_API = "OTLista";
    public const string ORDEN_TRABAJO_B = "OrdenTrabajoB";
    public const string OT_TECNICO_API = "OTtecnicoApp";
    public const string OT_UNIDAD_MEDIDA_API = "OTUnidadMedida";
    public const string OT_ALMACEN_API = "OTAlmacen";
    public const string OT_MATERIAL_API = "OTMaterial";
    public const string OT_MATERIAL_COMPLETO_API = "OTMaterialCompleto";
    public const string OT_PRODUCTOS_API = "OTProductos";
    public const string OT_FILES_API = "FilesOrdenesTrabajo";
    public const string OT_ENVIAR_ORDEN_EJECUTADA_API = "OrdenTrabajoB";

    public const string SUP_GET_ORDENES_API = "OrdenSupervision";
    public const string SUP_GET_ORDENES_TOTALES_API = "OrdenSupervisionTotales";
    public const string SUP_GET_MATERIALES_API = "ListadoMatSupervision";
    public const string SUP_PRODUCTOS_CLIENTE_API = "OTProductosCliente";
    //public const string SUP_POST_FOTOS = "Carga/CargaMul";
    public const string SUP_POST_FOTOS = "Carga/CargaMultiple";
    public const string SUP_POST_FOTOS_STREAMING = "Carga/Streaming";
    public const string SUP_POST_ENVIAR_SUPERVISION = "SupervisionApp";

    public const string VAC_GET_VACANTES = "RHVacanteApp";
    public const string VAC_POST_RFC = "util/getrfc";
    public const string VAC_POST_CURP = "util/getcurp";
    public const string VAC_GET_VALIDA_DATOS = "candidatosapp/existevalida";
    public const string VAC_GET_VALIDA_DATOS_BANCO = "ValidaBancoCandidato";
    public const string VAC_GET_CANDIDATO_DOCUMENTOS = "CandidatoDocumentos";
    public const string VAC_POST_VACANTE_DATA = "CandidatosAppN";

    public const string SAN_POST_SANITIZACION_DATA = "SanitizacionApp";

    public const string GET_CLIENTES_API = "Cliente";
    public const string GET_CLIENTE_SUPERVISIONES_API = "ClienteSupervisiones";
    public const string GET_ESTADOS_API = "Estado";
    public const string GET_INMUEBLES_API = "Sucursales";
    public const string GET_TOTAL_INMUEBLES_API = "TotalSucursales";
    public const string GET_BANCOS_API = "Banco";

    public const string GET_SUPERVISION_SECCIONTIPO = "SupervisionPreguntas/SeccionTipoByInmueble";
    public const string GET_SUPERVISION_PREGUNTAS = "SupervisionPreguntas/Preguntas";
    public const string GET_SUPERVISION_PREGUNTAS_EVALUACION = "SupervisionPreguntas/EvaluacionP";
    public const string GET_SUPERVISION_PREGUNTAS_OPERADOR = "SupervisionPreguntas/EvaluacionOperador";

    public const string GET_ENTREGA_PRECARGA_API = "EntregaPrecarga";

    #endregion

    #region Literals
    public const string ENTREGAS = "Entregas";
    public const string ENTREGAS_INTELIGENTES = "Entregas inteligentes";
    public const string CORRECTIVOS_MAYORES = "Correctivos mayores";
    public const string NO_EXISTEN_SECCIONES = "No existen secciones cargadas";

    public const string MANTENIMIENTO = "Mantenimiento";
    public const string ORDENES_TRABAJO = "Órdenes de trabajo";
    public const string GENERAR_ORDEN_TRABAJO = "Generar órden de trabajo";
    public const string MATERIALES_UTILIZADOS = "Materiales utilizados";
    public const string FOTOS_EVIDENCIA = "Fotos y evidencias";
    public const string TITULO_ENCUESTA = "Evaluación de los trabajos";
    public const string SUPERVISION_MANTENIMIENTO = "Supervisión mantenimiento";
    public const string SOLICITUD_COTIZACION = "Solicitud de cótizacion";
    public const string SOLICITUD_COTIZACION_DETALLE = "Crea solicitudes de cotización de forma rápida y organizada.";

    public const string SUPERVISION = "Supervisión";
    public const string SUPERVISIONES = "Supervisiones";
    public const string SUPERVISIONES_MANTENIMIENTO = "Supervisión de mantenimiento";
    public const string SUPERVISION_NO_PROGRAMADA = "Supervisión no programada";
    public const string SUPERVISION_MANTENIMIENTO_NO_PROGRAMADA = "Supervisión de mantenimiento no programada";
    
    
    public const string SUPERVISION_ALDO_CONTI = "Supervisión de Aldo Conti";
    public const string APARADORISTAS_ALDO_CONTI = "Supervisión aparadoristas de Aldo Conti";
    public const string LIMPIEZA_ALDO_CONTI = "Supervisión limpieza de Aldo Conti";
    public const string MANTENIMIENTO_ALDO_CONTI = "Supervisión mantenimiento de Aldo Conti";
    public const string MONITOREO_ALDO_CONTI = "Supervisión monitoreo de Aldo Conti";
    public const string DIARIO_GERENTE_ALDO_CONTI = "Diario Gerente de Aldo Conti";
    public const string REPORTE_MANTENIMIENTO_ALDO_CONTI = "Reporte de Mantenimiento de Aldo Conti";
    public const string DIARIO_LIMPIEZA_ALDO_CONTI = "Checklist diario de limpieza de Aldo Conti";

    public const string VACANTES = "Vacantes";
    public const string DATOS_GENERALES = "Datos generales";
    public const string DATOS_SUELDO = "Datos de sueldo";
    public const string DIRECCION = "Dirección";
    public const string DIRECCION_FISCAL = "Dirección fiscal";
    public const string DATOS_COMPLEMENTARIOS = "Datos complementarios";
    public const string DOCUMENTOS = "Documentos";

    public const string SANITIZACION = "Sanitización y desinfección";
    public const string EVIDENCIAS = "Evidencias";

    public const string EVALUACION = "Evaluación general";
    public const string CHECKLIST_OPERADOR = "Checklist Operador";

    public const string INGRESE = "Ingrese ";
    public const string INGRESE_TRABAJOS_REALIZADOS = "Ingrese trabajos realizados";
    public const string INGRESE_CANTIDAD = "Ingrese cantidad";
    public const string INGRESE_CANTIDAD_COBRAR = "Ingrese la cantidad a cobrar";
    public const string INGRESE_CANTIDAD_COMPRADA = "Ingrese la cantidad comprada";
    public const string INGRESE_CANTIDAD_UTILIZADA = "Ingrese la cantidad utilizada";
    public const string INGRESE_COSTO_UNITARIO = "Ingrese el costo unitario del producto";
    public const string INGRESE_HORAS_TRABAJADAS = "Ingrese horas trabajadas";
    public const string INGRESE_CANTIDAD_SUGERIDA = "Ingrese la cantidad sugerida";
    public const string INGRESE_OBSERVACIONES = "Ingrese observaciones en la pregunta";

    public const string SELECCIONE_PERSONAL = "Seleccione personal";
    public const string SELECCIONE_UNIDAD = "Seleccione unidad";
    public const string SELECCIONE_ARCHIVOS = "Seleccione archivos";
    public const string SELECCIONE_CLIENTE = "Seleccione un cliente";
    public const string SELECCIONE_INMUEBLE = "Seleccione un inmueble";
    public const string SELECCIONE_TECNICO = "Seleccione un técnico";
    public const string SELECCIONE_ESTADO = "Seleccione Estado";
    public const string SELECCIONE_PUNTO_ATENCION = "Seleccione el punto de atención";
    public const string SELECCIONE_TIPO_ORDEN = "Seleccione el tipo de orden";
    public const string INGRESE_REPORTE_CLIENTE = "Ingrese un folio de reporte";
    public const string INGRESE_TRABAJOS_A_EJECUTAR = "Ingrese los trabajos a realizar";
    public const string CAPTURE_FOTOGRAFIA = "Capture una fotografía";

    public const string SUMINISTRADOS_CLIENTE = "Suministrados por el cliente";
    public const string SUMINISTRADOS_ALMACEN = "Suministrados por almacén";
    public const string SUMINISTRADOS_COMPRA = "Suministrados por compra directa";
    public const string QUITAR = "Quitar";
    public const string CARGAR_FOTOS = "Cargar fotos";
    public const string CARGAR_REPORTE = "Cargar reporte";
    public const string DESCRIPCION = "Ingrese descripción";
    public const string CANCELAR = "Cancelar";
    public const string ACEPTAR = "Aceptar";
    public const string PERSONAL_AGREGADO = "El personal seleccionado ya fue agregado";
    public const string MATERIAL_AGREGADO = "El material seleccionado ya fue agregado";

    public const string ES_PENSIONADO = "¿Es pensionado(a)?";
    public const string NSS = "NSS";
    public const string CURP = "CURP";
    public const string RFC = "RFC";
    public const string NOMBRE = "Nombre(s)";
    public const string APELLIDO_PATERNO = "Apellido paterno";
    public const string APELLIDO_MATERNO = "Apellido materno";
    public const string FECHA_NACIMIENTO = "Fecha de nacimiento";
    public const string LUGAR_NACIMIENTO = "Lugar de nacimiento";
    public const string NACIONALIDAD = "Nacionalidad";
    public const string GENERO = "Genero";
    public const string ESTADO_CIVIL = "Estado civil";
    public const string TALLA_UNIFORME = "Talla uniforme";
    public const string TALLA_CALZADO = "Talla calzado";
    public const string FUENTE_RECLUTAMIENTO = "Fuente de reclutamiento";
    public const string SALARIO_MENSUAL = "Salario mensual";
    public const string SALARIO_DIARIO = "Salario diario";
    public const string SALARIO_DIARIO_INTEGRADO = "Salario diario integrado";
    public const string FECHA_INGRESO = "Fecha de ingreso";
    public const string BANCO = "Banco";
    public const string CLABE = "CLABE";
    public const string NO_CUENTA = "n° de cuenta";
    public const string NO_TARJETA = "n° de tarjeta";
    public const string CALLE = "Calle";
    public const string NO_INTERIOR = "No. interior";
    public const string NO_EXTERIOR = "No. exterior";
    public const string COLONIA = "Colonia";
    public const string CODIGO_POSTAL = "Código postal";
    public const string MUNICIPIO = "Municipio";
    public const string ESTADO = "Estado";
    public const string TELEFONO = "Teléfono";
    public const string CORREO_ELECTRONICO = "Correo electrónico (personal)";
    public const string CONTACTO_EMERGENCIA = "Contacto emergencia";
    public const string TELEFONO_EMERGENCIA = "Teléfono emergencia";
    public const string CALLE_NUMERO = "Calle y número";
    public const string OTRO = "Otro";
    public const string ULTIMO_GRADO_ESTUDIOS = "Último grado de estudios";
    public const string TIENE_HIJOS = "¿Tiene hijos?";
    public const string CUANTOS_HIJOS = "¿Cuántos hijos?";
    public const string DEPENDEN_ECONOMICAMENTE = "¿Dependen económicamente de él/ella?";
    public const string SABE_LEER = "¿Sabe leer?";
    public const string SABE_ESCRIBIR = "¿Sabe escribir?";
    public const string TIENE_TELEFONO = "¿Tiene teléfono inteligente propio?";
    public const string COMO_COMUNICA = "¿Cómo se comunica con su lugar de trabajo?";
    public const string ESPECIFIQUE_COMUNICA = "Especifique otra forma de comunicarse";
    public const string QUE_TRANSPORTE = "¿Qué transporte usa para llegar al trabajo?";
    public const string ESPECIFIQUE_TRANSPORTE = "Especifique otro transporte";
    public const string CUANTO_UNIDADES_TRANSPORTE = "¿Cuántas unidades de transporte usa?";
    public const string CUENTA_BONO = "¿Cuenta con bono de trasporte?";
    public const string CUANTO_GASTA = "¿Cuánto gasta de transporte a la semana?";
    public const string SE_OFRECE_BONO = "Se ofrece bono de puntualidad en su servicio";
    public const string AREA = "Área";
    public const string PROCEDIMIENTO = "Procedimiento";
    public const string RECIBE = "Quién recibe";
    public const string CAPTURAR_FOTOGRAFIAS = "Capturar fotografías";
    public const string PERSONAL_QUE_RECIBIO_CAPACITACION = "Personal que recibió capacitación";

    public const string NO_HAY_REGISTROS = "No se encontraron elementos con ese criterio de búsqueda.";
    public const string NO_HAY_ORDENES_MANTENIMIENTO = "No hay órdenes de trabajo.";
    public const string NO_HAY_ORDENES_SUPERVISION = "No hay órdenes de Supervisión con estos datos.";
    public const string NO_HAY_ORDENES_SUPERVISION_LOCAL = "No hay órdenes de Supervisión para enviar";
    public const string NO_HAY_INVENTARIO_MATERIALES = "No hay inventario de materiales para la fecha en cuestión.";
    public const string NO_HAY_VACANTES = "No hay Vacantes por mostrar.";
    public const string DATOS_PRECARGADOS = "Datos precargados correctamente";
    public const string BUSCAR = "Buscar";
    public const string BUSCAR_PERSONAL = "Buscar personal";
    public const string BUSCAR_ALMACEN = "Buscar almacén";
    public const string BUSCAR_MATERIAL = "Buscar material";
    public const string BUSCAR_SUCURSAL = "Buscar sucursal";

    public const string USER_PASS_INCORRECTOS = "Usuario y/o Contraseña incorrectos";

    public const string OBSERVACIONES = "Observaciones";
    public const string CONTESTE_PREGUNTAS = "Conteste la pregunta";
    public const string NUMERO_MAXIMO = "Número máximo de fotos :";
    public const string GRABAR_VIDEO = "Grabar video";
    public const string VIDEO_NO_GRABADO = "Video no grabado";
    public const string AGREGUE_VIDEO = "Agregue un video";
    public const string VALIDAR_VIDEO = "Validando video ...";
    public const string CARGANDO = "Cargando";
    #endregion

    #region Encuestas
    public const string PREGUNTA_0 = "¿Se pudo entrevistar con el cliente?";
    public const string PREGUNTA_1 = "¿Cómo evalúa los trabajos en general?";
    public const string PREGUNTA_2 = "¿El(los) técnico(s) portan uniforme y gafete?";
    public const string PREGUNTA_3 = "El trato de nuestro(s) técnico(s) fue";
    public const string PREGUNTA_4 = "¿Los trabajos fueron realizados con orden y limpieza?";
    public const string PREGUNTA_5 = "¿Los materiales utilizados fueron los adecuados?";
    public const string BUENO = "Bueno";
    public const string REGULAR = "Regular";
    public const string MALO = "Malo";
    public const string SI = "Sí";
    public const string NO = "No";
    public const string GENERAL = "GENERAL";
    public const string PERSONAL_ASIGNADO = "PERSONAL ASIGNADO";
    public const string CALIDAD_TRABAJOS = "CALIDAD DE LOS TRABAJOS";
    public const string AGRADECIMIENTOS = "Sus respuestas son realmente valiosas para nosotros\n¡Gracias por ayudarnos a mejorar!";
    public const string RESPONDA_TODAS_PREGUNTAS = "Responda todas las preguntas";
    public const string INGRESE_ENCUESTADO = "Ingrese nombre del encuestado";
    public const string INGRESE_FIRMA = "Ingrese una firma";
    public const string ERROR_API = "Ocurrió un error al enviar los datos, vuelva a intentarlo";
    public const string ERROR_API_GET = "Ocurrió un error al obtener los datos, vuelva a intentarlo";
    public const string ERROR_OFFLINE = "Sin conexión a internet, conectese a una red y precargue los datos para poder realizar supervisiones sin internet.";
    public const string ERROR_INTERNET = "Sin conexión a internet, verifique e inténtelo nuevamente";
    public const string USANDO_DATOS_PRECARGADOS = "Sin internet, usando datos precargados";
    public const string ORDEN_TRABAJO_ENVIADA = "Orden de trabajo enviada correctamente";
    public const string ENVIANDO_DATOS = "Enviando datos ...";
    public const string SUPERVISION_ENVIADA = "Supervisión enviada correctamente";
    public const string VACANTE_ENVIADA = "Vacante enviada correctamente";
    public const string DATOS_ENVIADOS_CORRECTAMENTE = "Datos enviados correctamente.";
    public const string FIRME_AQUI = "Firme aquí";

    public const string ENCUESTA_SATISFACCION = "Encuesta de satisfacción";
    public const string PERSONAL_ASIGNADDO_SERVICIO = "PERSONAL ASIGNADO AL SERVICIO";
    public const string SUP_PREGUNTA_2 = "La calidad de los trabajos realizados es :";
    public const string SUP_PREGUNTA_3 = "El personal tiene uniforme completo";
    public const string SUP_PREGUNTA_4 = "El trato del personal al cliente es :";
    public const string OPERACIONES = "OPERACIONES";
    public const string SUP_PREGUNTA_5 = "¿El supervisor realizó el recorrido adecuadamente?";
    public const string SUP_PREGUNTA_6 = "¿El supervisor le notificó las áreas de oportunidad del servicio?";
    public const string SUP_PREGUNTA_7 = "¿Se le informó el plan correctivo y/o fecha de realización?";
    public const string SUP_PREGUNTA_8 = "¿Cómo califica la atención del supervisor?";
    //public const string CGO = "CGO (Centro de Gestión Operativa)";
    //public const string SUP_PREGUNTA_9 = "¿Cómo califica la atención de su ejecutivo de CGO?";
    public const string SUP_PREGUNTA_10 = "¿Recibe el reporte de asistencia por parte de supervisor?";
    public const string MATERIALES = "MATERIALES";
    public const string SUP_PREGUNTA_11 = "¿Los materiales se entregaron correctamente etiquetados y envasados?";
    public const string SUP_PREGUNTA_12 = "¿El material de limpieza cumple con los requerimientos del servicio?";
    public const string COMENTARIOS = "Comentarios";
    #endregion

    #region PERMISOS
    public const string GRANT_PERMISSIONS = "Otorgar permisos";
    public const string PERMISSIONS_ERROR_CAM = "Es necesario otorgar permisos para acceder a la cámara";
    public const string PERMISSIONS_CAM_CONFIG = "Para acceder a la cámara es necesario otorgar permisos desde los ajustes del dispositivo";
    public const string PERMISSIONS_LOCATION_TITLE = "📍 Uso de ubicación en segundo plano";

    public const string PERMISSIONS_LOCATION_BODY =
    "✅ Esta aplicación requiere acceder a tu ubicación incluso en segundo plano.\r\n\r\n" +
    "🚚 ¿Para qué se usa?\r\n" +
    "- Registrar las rutas de entrega.\r\n" +
    "- Permitir a la empresa visualizar tu recorrido en tiempo real.\r\n\r\n" +
    "🔒 Privacidad garantizada:\r\n" +
    "- Solo se recopila tu ubicación mientras la ruta está activa.\r\n" +
    "- El seguimiento se detiene automáticamente al finalizarla.";
    public const string PERMISSIONS_LOCATION_ACEPTAR = "Aceptar";
    public const string PERMISSIONS_LOCATION_CANCELAR = "Cancelar";
    //OCURRIO UN ERROR AL ENVIAR LA INFORMACION
    #endregion

    #region Keys
    public const string ORDEN_TRABAJO_KEY = "Orden trabajo key";
    public const string SUPERVISION_REQUEST_DATA_KEY = "Supervisión Request Data key";
    public const string VACANTES_LIST_DATA_KEY = "Vacantes list data key";
    public const string VACANTE_DATA_KEY = "Vacante data key";
    public const string SANITIZACION_DATA_KEY = "Sanitización data key";
    public const string SECCIONES_KEY = "Secciones key";
    public const string INDICE_KEY = "Indice key";
    #endregion

    #region DETALLES 
    public const string DETALLES_ENTREGAS = "Registro de entregas de material, seguimiento a rutas de entrega, envío de acuses.";
    public const string DETALLES_CORRECTIVOS_MAYORES = "Registro de evidencias de los trabajos realizados por correctivos mayores.";
    public const string DETALLES_CORRECTIVOS_MENORES = "Registro de evidencias de los trabajos realizados por órdenes de trabajo.";
    public const string DETALLES_SUPERVISION = "Registro de visita a puntos de atención de nuestros clientes, recorridos dentro de sus instalaciones.";
    public const string DETALLES_SUPERVISION_MANTENIMIENTO = "Registra la supervisión de mantenimiento mediante un checklist detallado.";
    public const string DETALLES_VACANTES = "Registro de candidatos para cobertura de vacantes.";
    public const string DETALLES_SANITIZACION = "Registro de visitas a puntos de atención para sanitización y desinfección del inmueble.";
    public const string DETALLES_SUPERVISION_ALDO_CONTI = "Se realiza la supervisión de Aldo Conti mediante un checklist detallado.";
    public const string DETALLES_APARADORISTAS_ALDO_CONTI = "Se realiza la supervisión en Aldo Conti a aparadoristas mediante un checklist detallado.";
    public const string DETALLES_LIMPIEZA_ALDO_CONTI = "Se realiza la supervisión en Aldo Conti a limpieza mediante un checklist detallado.";
    public const string DETALLES_MANTENIMIENTO_ALDO_CONTI = "Se realiza la supervisión en Aldo Conti a mantenimiento mediante un checklist detallado.";
    public const string DETALLES_MONITOREO_ALDO_CONTI = "Se realiza la supervisión en Aldo Conti a monitoreo mediante un checklist detallado.";
    public const string DETALLES_DIARIO_GERENTE_ALDO_CONTI = "Se realiza el diario de gerente mediante un checklist detallado.";
    public const string DETALLES_REPORTE_MANTENIMIENTO_ALDO_CONTI = "Se realiza el reporte de mantenimiento mediante un checklist detallado.";
    public const string DETALLES_DIARIO_LIMPIEZA_ALDO_CONTI = "Se realiza el reporte de limpieza diario mediante un checklist detallado.";

    #region MONTHS
    public static List<object> MonthList = ["Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"];

    public static int GetMonthNumber(string monthName) {
        for(int i = 0; i < MonthList.Count; i++) {
            if(MonthList[i].Equals(monthName)) {
                return i + 1;
            }
        }
        return 0;
    }

    public static string GetMonthName(int monthNumber) {
        return (string)MonthList[monthNumber - 1];
    }
    #endregion

    #region ESTADOS 
    static List<EstadoModel> _estadoList;
    static List<object> _estadoListAux = new List<object>();

    public static async Task LoadEstadosAsync() {
        try {
            if(!InternetUtil.IsConnectedInternet()) {
                var _dbContext = new DbContext();
                _estadoList = await _dbContext.GetEstadosLocal();
                if(_estadoList == null || _estadoList.Count == 0) {
                    await Toast.Make(Constants.ERROR_OFFLINE, ToastDuration.Short).Show();
                    return;
                }
            } else {


                if(_estadoList is not null && _estadoList.Count > 0) {
                    return;
                }
                HttpHelper _httpHelper = new HttpHelper();
                var est = await _httpHelper.GetAsync<List<EstadoModel>>(GET_ESTADOS_API);
                _estadoList = est;
                if(_estadoList == null) {
                    var _dbContext = new DbContext();
                    _estadoList = await _dbContext.GetEstadosLocal();
                    if(_estadoList == null || _estadoList.Count == 0) {
                        await Toast.Make(Constants.ERROR_OFFLINE, ToastDuration.Short).Show();
                        return;
                    }



                    //await Toast.Make(Constants.ERROR_API_GET, ToastDuration.Short).Show();
                    return;
                }
                _estadoList.RemoveAt(0);
                foreach(EstadoModel estado in _estadoList) {
                    _estadoListAux.Add(estado.descripcion);
                }
            }
        }
        catch(Exception ex) {
            Console.WriteLine("Error LoadEstadosAsync: " + ex.Message);
        }
        
    }

    public static async Task<EstadoModel> GetEstadoAsync(EstadoModel currentEstado) {

        if(!InternetUtil.IsConnectedInternet()) {
            _estadoListAux = new List<object>();
            _estadoList = new List<EstadoModel>();
            var _dbContext = new DbContext();
            var est = await _dbContext.GetEstadosLocal();
            if (est != null && est.Count > 0) {
                foreach(var es in est) {
                    var data = new EstadoModel {
                        id_estado = es.id_estado,
                        abreviatura = es.abreviatura,
                        descripcion = es.descripcion
                    };
                    _estadoList.Add(data);
                }
                //_estadoList.RemoveAt(0);
                foreach(EstadoModel estado in _estadoList) {
                    _estadoListAux.Add(estado.descripcion);
                }
            } else {
                await Toast.Make(Constants.ERROR_OFFLINE, ToastDuration.Short).Show();

            }
        }

            double size = IS_IOS ? IS_TABLET ? 3.5 : 5.5 : IS_TABLET ? 2.5 : 4;
        //double size = IS_IOS ? IS_TABLET ? 2.5 : 4.5 : IS_TABLET ? 1.5 : 2.5;
        string value = (string)await PopupUtil.GetObjectAsync(currentEstado.descripcion is null ? "" : currentEstado.descripcion, _estadoListAux, size, true);

        if(string.IsNullOrWhiteSpace(value) || (currentEstado.descripcion is not null && currentEstado.descripcion.Equals(value))) {
            return currentEstado;
        }

        foreach(EstadoModel estado in _estadoList) {
            if(estado.descripcion.Equals(value)) {
                return estado;
            }
        }

        return currentEstado;
    }
    #endregion

    #region CLIENTES
    static List<ClientsModel> _clienteList;
    static List<object> _clienteListAux = new List<object>();
    public static async Task<ClientsModel> SetCliente(int idCliente) {
        var cliente = new ClientsModel();
        if (_clienteList != null && _clienteList .Count > 0) {
            cliente = _clienteList.FirstOrDefault(c => c.idCliente == idCliente);
        }
        return cliente;
    }

    public static async Task LoadClientesAsync() {
        _clienteList = new List<ClientsModel>();
        _clienteListAux = new List<object>();
        if(_clienteList is not null && _clienteList.Count > 0) {
            return;
        }

        if(!InternetUtil.IsConnectedInternet()) {
            await Toast.Make(Constants.USANDO_DATOS_PRECARGADOS, ToastDuration.Short).Show();
            //OBTENER CLIENTES DE LOCAL
            var _dbContext = new DbContext();
            _clienteList = await _dbContext.GetClientesLocal();
            if(_clienteList == null || _clienteList.Count == 0) {
                await Toast.Make(Constants.ERROR_OFFLINE, ToastDuration.Short).Show();
                return;
            }
            foreach(ClientsModel cliente in _clienteList) {
                _clienteListAux.Add(cliente.nombre);
            }
        } else {
            //OBTENER POR API
            HttpHelper httpHelper = new HttpHelper();
            _clienteList = await httpHelper.GetAsync<List<ClientsModel>>(GET_CLIENTE_SUPERVISIONES_API + "?idcliente=" + UserSession.Cliente);
            if(_clienteList == null) {
                //SI EL ENDPOINT FALLA OBTENER DEL LOCAL
                var _dbContext = new DbContext();
                _clienteList = await _dbContext.GetClientesLocal();
                //SI EL LOCAL NO FUNCIONA O NO HAY REGISTROS MOSTRAR ERROR
                if(_clienteList == null || _clienteList.Count == 0) {
                    await Toast.Make(Constants.ERROR_API_GET, ToastDuration.Short).Show();
                    return;
                }
            }
            foreach(ClientsModel cliente in _clienteList) {
                _clienteListAux.Add(cliente.nombre);
            }
        }

    }

    public static async Task<ClientsModel> GetClienteAsync(ClientsModel currentCliente) {
        double size = IS_IOS ? IS_TABLET ? 3.5 : 5.5 : IS_TABLET ? 2.5 : 4;
        //double size = IS_IOS ? IS_TABLET ? 2.5 : 4.2 : IS_TABLET ? 1.5 : 3;
        string value = (string)await PopupUtil.GetObjectAsync(currentCliente.nombre is null ? "" : currentCliente.nombre, _clienteListAux, size, true);

        if(string.IsNullOrWhiteSpace(value) || (currentCliente.nombre is not null && currentCliente.nombre.Equals(value))) {
            return currentCliente;
        }

        foreach(ClientsModel cliente in _clienteList) {
            if(cliente.nombre.Equals(value)) {
                return cliente;
            }
        }

        return currentCliente;
    }


    public static async Task<Turno> GetTurnoAsync(Turno currentTurno) {
        try {

        
        double size = IS_IOS ? IS_TABLET ? 3.5 : 5.5 : IS_TABLET ? 2.5 : 4;
        var listaturnos = new List<Turno>
        {
                new Turno { IdTurno =1, Nombre ="MATUTINO" },
                new Turno { IdTurno =2, Nombre ="VESPERTINO" },
                new Turno { IdTurno =3, Nombre ="NOCTURNO" }
        };
        var listaTurnosSelect = new List<Object>();
        foreach(var turno in listaturnos) {
                       listaTurnosSelect.Add(turno.Nombre);
        }
        currentTurno.Nombre = currentTurno.Nombre == null ? currentTurno.Nombre = "" : currentTurno.Nombre;
        string value = (string)await PopupUtil.GetObjectAsync(currentTurno, listaTurnosSelect, size, true);

        if(string.IsNullOrWhiteSpace(value) || (currentTurno.Nombre is not null && currentTurno.Nombre.Equals(value))) {
            return currentTurno;
        }

        foreach(Turno turno in listaturnos) {
            if(turno.Nombre.Equals(value)) {
                return turno;
            }
        }

        return currentTurno;
        }
        catch(Exception ex) {
            Console.WriteLine("Error GetTurnoAsync: " + ex.Message);
            return currentTurno;
        }
    }


    #endregion

    #region BANCO
    static List<BancoModel> _bancoList;
    static List<object> _bancoListAAux = new List<object>();

    public static async Task LoadBancosAsync() {
        if(_bancoList is not null && _bancoList.Count > 0) {
            return;
        }

        HttpHelper _httpHelper = new HttpHelper();
        _bancoList = await _httpHelper.GetAsync<List<BancoModel>>(BANCO);

        foreach(BancoModel banco in _bancoList) {
            _bancoListAAux.Add(banco.Descripcion);
        }
    }

    public static async Task<BancoModel> GetBancoAsync(BancoModel currentBanco) {
        double size = IS_IOS ? IS_TABLET ? 3 : 4.5 : IS_TABLET ? 1.5 : 3;
        string value = (string)await PopupUtil.GetObjectAsync(currentBanco.Descripcion is null ? "" : currentBanco.Descripcion, _bancoListAAux, size, true);

        if(string.IsNullOrWhiteSpace(value) || (currentBanco.Descripcion is not null && currentBanco.Descripcion.Equals(value))) {
            return currentBanco;
        }

        foreach(BancoModel banco in _bancoList) {
            if(banco.Descripcion.Equals(value)) {
                return banco;
            }
        }

        return currentBanco;
    }
    #endregion

    #region GENERO
    static List<CatalogoModel> _generoList;
    static List<object> _generoListAux = new List<object>();

    public static void LoadGenerosAsync() {
        if(_generoList is not null && _generoList.Count > 0) {
            return;
        }

        _generoList = new List<CatalogoModel> {
            new CatalogoModel{ Id = 1, Descripcion = "Hombre"},
            new CatalogoModel{ Id = 2, Descripcion = "Mujer"}
        };

        foreach(CatalogoModel genero in _generoList) {
            _generoListAux.Add(genero.Descripcion);
        }
    }
    #endregion

    #region ESTADO CIVIL
    static List<CatalogoModel> _estadoCivilList;
    static List<object> _estadoCivilListAux = new List<object>();

    public static void LoadEstadosCivilAsync() {
        if(_estadoCivilList is not null && _estadoCivilList.Count > 0) {
            return;
        }

        _estadoCivilList = new List<CatalogoModel> {
            new CatalogoModel{Id = 1, Descripcion = "Casado(a)"},
            new CatalogoModel{Id = 2, Descripcion = "Divorciado(a)"},
            new CatalogoModel{Id = 3, Descripcion = "Soltero(a)"},
            new CatalogoModel{Id = 4, Descripcion = "Unión libre"},
            new CatalogoModel{Id = 5, Descripcion = "Viudo(a)"}
        };

        foreach(CatalogoModel estadoCivil in _estadoCivilList) {
            _estadoCivilListAux.Add(estadoCivil.Descripcion);
        }
    }
    #endregion

    #region GRADO DE ESTUDIOS
    static List<CatalogoModel> _gradoEstudiosList;
    static List<object> _gradoEstudiosListAux = new List<object>();

    public static void LoadGradoEstudiosAsync() {
        if(_gradoEstudiosList is not null && _gradoEstudiosList.Count > 0) {
            return;
        }

        _gradoEstudiosList = new List<CatalogoModel> {
            new CatalogoModel{Id= 1, Descripcion = "Primaria trunca"},
            new CatalogoModel{Id= 2, Descripcion = "Primaria terminada"},
            new CatalogoModel{Id= 3, Descripcion = "Secundaria trunca"},
            new CatalogoModel{Id= 4, Descripcion = "Secundaria terminada"},
            new CatalogoModel{Id= 5, Descripcion = "Bachillerato trunco"},
            new CatalogoModel{Id= 6, Descripcion = "Bachillerato terminado"},
            new CatalogoModel{Id= 7, Descripcion = "Escuela técnica trunca"},
            new CatalogoModel{Id= 8, Descripcion = "Escuela técnica terminada"},
            new CatalogoModel{Id= 9, Descripcion = "Universidad trunca"},
            new CatalogoModel{Id= 10, Descripcion = "Universidad terminada"},
        };

        foreach(CatalogoModel gradoEstudios in _gradoEstudiosList) {
            _gradoEstudiosListAux.Add(gradoEstudios.Descripcion);
        }
    }
    #endregion

    #region MÉTODOS COMUNICA
    static List<CatalogoModel> _metodosComunicaList;
    static List<object> _metodosComunicaListAux = new List<object>();

    public static void LoadMetodosComunicaAsync() {
        if(_metodosComunicaList is not null && _metodosComunicaList.Count > 0) {
            return;
        }

        _metodosComunicaList = new List<CatalogoModel> {
            new CatalogoModel{Id = 1, Descripcion = "Celular no inteligente"},
            new CatalogoModel{Id = 2, Descripcion = "Celular inteligente prestado"},
            new CatalogoModel{Id = 3, Descripcion = OTRO}
        };

        foreach(CatalogoModel metodoComunica in _metodosComunicaList) {
            _metodosComunicaListAux.Add(metodoComunica.Descripcion);
        }
    }
    #endregion

    #region TRANSPORTE
    static List<CatalogoModel> _transporteList;
    static List<object> _transporteListAux = new List<object>();

    public static void LoadTransportesAsync() {
        if(_transporteList is not null && _transporteList.Count > 0) {
            return;
        }

        _transporteList = new List<CatalogoModel> {
            new CatalogoModel{Id = 1, Descripcion = "Caminando"},
            new CatalogoModel{Id = 2, Descripcion = "Bicicleta"},
            new CatalogoModel{Id = 3, Descripcion = "Metro, tren ligero, tren suburbano"},
            new CatalogoModel{Id = 4, Descripcion = "Trolebús"},
            new CatalogoModel{Id = 5, Descripcion = "Metrobús"},
            new CatalogoModel{Id = 6, Descripcion = "Camión, autobús, combi, colectivo"},
            new CatalogoModel{Id = 7, Descripcion = "Transporte de personal"},
            new CatalogoModel{Id = 8, Descripcion = "Taxi"},
            new CatalogoModel{Id = 9, Descripcion = "Taxi de aplicación"},
            new CatalogoModel{Id = 10, Descripcion = "Motocicleta"},
            new CatalogoModel{Id = 11, Descripcion = "Automóvil o camioneta"},
            new CatalogoModel{Id = 12, Descripcion = OTRO},
        };

        foreach(CatalogoModel transporte in _transporteList) {
            _transporteListAux.Add(transporte.Descripcion);
        }
    }
    #endregion

    #region CANTIDAD TRANSPORTE 
    static List<CatalogoModel> _cantidadTransporteList;
    static List<object> _cantidadTransporteListAux = new List<object>();

    public static void LoadCantidadTransporteAsync() {
        if(_cantidadTransporteList is not null && _cantidadTransporteList.Count > 0) {
            return;
        }

        _cantidadTransporteList = new List<CatalogoModel> {
            new CatalogoModel{Id = 1, Descripcion = "Una"},
            new CatalogoModel{Id = 2, Descripcion = "Dos"},
            new CatalogoModel{Id = 3, Descripcion = "Tres"},
            new CatalogoModel{Id = 4, Descripcion = "Más de tres"}
        };

        foreach(CatalogoModel cantidadTransporte in _cantidadTransporteList) {
            _cantidadTransporteListAux.Add(cantidadTransporte.Descripcion);
        }
    }
    #endregion

    #region PROCEDIMIENTO
    static List<CatalogoModel> _procedimientoList;
    static List<object> _procedimientoListAux = new List<object>();

    public static void LoadProcedimientosAsync() {
        if(_procedimientoList is not null && _procedimientoList.Count > 0) {
            return;
        }

        _procedimientoList = new List<CatalogoModel>{
            new CatalogoModel{Id = 1, Descripcion = "Sanitización"},
            new CatalogoModel{Id = 2, Descripcion = "Desinfección"},
            new CatalogoModel{Id = 3, Descripcion = "Sanitización y desinfección"}
        };

        foreach(CatalogoModel procedimiento in _procedimientoList) {
            _procedimientoListAux.Add(procedimiento.Descripcion);
        }
    }
    #endregion

    #region SQLite
    //public const string DATABASE_FILE_NAME = "SuiteBatiaDB.db3";
    //public const SQLite.SQLiteOpenFlags FLAGS = SQLite.SQLiteOpenFlags.ReadWrite | SQLite.SQLiteOpenFlags.Create | SQLite.SQLiteOpenFlags.SharedCache;
    //public static string DATABASE_PATH = Path.Combine(FileSystem.AppDataDirectory, DATABASE_FILE_NAME);
    #region SQLite
    public const string DATABASE_FILE_NAME = "SuiteBatiaDB.db3";
    public static string DATABASE_PATH = Path.Combine(FileSystem.AppDataDirectory, DATABASE_FILE_NAME);
    #endregion
    #endregion

    #region Catalogos
    static List<TipoOrdenTrabajoModel> _tipoOrdenList;
    static List<object> _tipoOrdenListAux = new List<object>();
    public static async Task LoadTipoOrdenAsync() {
        if(_tipoOrdenList is not null && _tipoOrdenList.Count > 0) {
            return;
        }

        if(!InternetUtil.IsConnectedInternet()) {
            //await Toast.Make(Constants.USANDO_DATOS_PRECARGADOS, ToastDuration.Short).Show();
            ////OBTENER CLIENTES DE LOCAL
            //var _dbContext = new DbContext();
            //_clienteList = await _dbContext.GetClientesLocal();
            //if(_clienteList == null || _clienteList.Count == 0) {
            //    await Toast.Make(Constants.ERROR_OFFLINE, ToastDuration.Short).Show();
            //    return;
            //}
            //foreach(ClientsModel cliente in _clienteList) {
            //    _clienteListAux.Add(cliente.nombre);
            //}
        } else {
            //OBTENER POR API
            HttpHelper httpHelper = new HttpHelper();
            _tipoOrdenList = await httpHelper.GetAsync<List<TipoOrdenTrabajoModel>>(GET_TIPO_ORDENES_TRABAJO_API);
            if(_tipoOrdenList == null) {
                //SI EL ENDPOINT FALLA OBTENER DEL LOCAL
                //var _dbContext = new DbContext();
                //_tipoOrdenList = await _dbContext.GetClientesLocal();
                ////SI EL LOCAL NO FUNCIONA O NO HAY REGISTROS MOSTRAR ERROR
                //if(_clienteList == null || _clienteList.Count == 0) {
                //    await Toast.Make(Constants.ERROR_API_GET, ToastDuration.Short).Show();
                //    return;
                //}
            } else {
                
            }
            if(_tipoOrdenList != null) {
                foreach(TipoOrdenTrabajoModel tipoOrden in _tipoOrdenList) {
                    if(tipoOrden != null && tipoOrden.Descripcion != null) {
                        _tipoOrdenListAux.Add(tipoOrden.Descripcion);
                    }
                }
            }
        }
    }

    public static async Task<TipoOrdenTrabajoModel> GetTipoOrdenAsync(TipoOrdenTrabajoModel tipoOrdenTrabajo) {
        double size = IS_IOS ? IS_TABLET ? 3.5 : 5.5 : IS_TABLET ? 2.5 : 4;
        //double size = IS_IOS ? IS_TABLET ? 2.5 : 4.2 : IS_TABLET ? 1.5 : 3;
        string value = (string)await PopupUtil.GetObjectAsync(tipoOrdenTrabajo.Descripcion is null ? "" : tipoOrdenTrabajo.Descripcion, _tipoOrdenListAux, size, true);

        if(string.IsNullOrWhiteSpace(value) || (tipoOrdenTrabajo.Descripcion is not null && tipoOrdenTrabajo.Descripcion.Equals(value))) {
            return tipoOrdenTrabajo;
        }

        foreach(TipoOrdenTrabajoModel tipoOrden in _tipoOrdenList) {
            if(tipoOrden.Descripcion != null && tipoOrden.Descripcion.Equals(value)) {
                return tipoOrden;
            }
        }

        return tipoOrdenTrabajo;
    }
    #endregion




    


















    public static async Task<CatalogoModel> GetCatalogoAsync(CatalogoModel currentCatalogo, Opciones opcion, double popupHeight = 4, bool showSearching = false) {
        string value = (string)await PopupUtil.GetObjectAsync(currentCatalogo.Descripcion is null ? "" : currentCatalogo.Descripcion, GetListAux(opcion), popupHeight, showSearching);

        if(string.IsNullOrWhiteSpace(value) || (currentCatalogo.Descripcion is not null && currentCatalogo.Descripcion.Equals(value))) {
            return currentCatalogo;
        }

        foreach(CatalogoModel catalogo in GetList(opcion)) {
            if(catalogo.Descripcion.Equals(value)) {
                return catalogo;
            }
        }

        return currentCatalogo;
    }

    static List<object> GetListAux(Opciones opcion) {
        switch(opcion) {
            case Opciones.GENEROS:
                return _generoListAux;
            case Opciones.ESTADOS_CIVIL:
                return _estadoCivilListAux;
            case Opciones.GRADO_ESTUDIOS:
                return _gradoEstudiosListAux;
            case Opciones.METODO_COMUNICA:
                return _metodosComunicaListAux;
            case Opciones.TRANSPORTE:
                return _transporteListAux;
            case Opciones.CANTIDAD_TRANSPORTE:
                return _cantidadTransporteListAux;
            case Opciones.PROCEDIMIENTOS:
                return _procedimientoListAux;
            default:
                return null;
        }
    }

    static List<CatalogoModel> GetList(Opciones opcion) {
        switch(opcion) {
            case Opciones.GENEROS:
                return _generoList;
            case Opciones.ESTADOS_CIVIL:
                return _estadoCivilList;
            case Opciones.GRADO_ESTUDIOS:
                return _gradoEstudiosList;
            case Opciones.METODO_COMUNICA:
                return _metodosComunicaList;
            case Opciones.TRANSPORTE:
                return _transporteList;
            case Opciones.CANTIDAD_TRANSPORTE:
                return _cantidadTransporteList;
            case Opciones.PROCEDIMIENTOS:
                return _procedimientoList;
            default:
                return null;
        }
    }

    public static bool IsValidEmail(string email) {
        if(string.IsNullOrWhiteSpace(email))
            return false;

        try {
            // Normalize the domain
            email = Regex.Replace(email, @"(@)(.+)$", DomainMapper,
                                  RegexOptions.None, TimeSpan.FromMilliseconds(200));

            // Examines the domain part of the email and normalizes it.
            string DomainMapper(Match match) {
                // Use IdnMapping class to convert Unicode domain names.
                var idn = new IdnMapping();

                // Pull out and process domain name (throws ArgumentException on invalid)
                string domainName = idn.GetAscii(match.Groups[2].Value);

                return match.Groups[1].Value + domainName;
            }
        } catch(RegexMatchTimeoutException e) {
            return false;
        } catch(ArgumentException e) {
            return false;
        }

        try {
            return Regex.IsMatch(email,
                @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
        } catch(RegexMatchTimeoutException) {
            return false;
        }
    }

    public static PickOptions GetPickOptions(bool showImages, bool showPdf) {
        List<string> pickerOptionsAndroid = new List<string>();
        List<string> pickerOptionsIOS = new List<string>();

        if(showImages) {
            pickerOptionsAndroid.Add("image/jpeg");
            pickerOptionsAndroid.Add("image/png");
            pickerOptionsIOS.Add("public.image");
        }

        if(showPdf) {
            pickerOptionsAndroid.Add("application/pdf");
            pickerOptionsIOS.Add("com.adobe.pdf");
        }

        FilePickerFileType filePickerFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>> {
            {DevicePlatform.Android, pickerOptionsAndroid},
            {DevicePlatform.iOS, pickerOptionsIOS}
        });

        return new PickOptions() {
            PickerTitle = SELECCIONE_ARCHIVOS,
            FileTypes = filePickerFileType
        };
    }

    public static async Task<Inmueble> GetInmuebleAsync(int idCliente, int idEstado, Inmueble currentInmueble) {
        try {


            if(!InternetUtil.IsConnectedInternet()) {
                var _dbContext = new DbContext();
                var inmuebles = await _dbContext.GetinmueblesLocal(idCliente, idEstado);

                if(inmuebles == null) {
                    await Toast.Make(Constants.ERROR_API_GET, ToastDuration.Short).Show();
                    var inm = new Inmueble();
                    return inm;
                }
                List<object> inmuebleListAux = new List<object>();

                foreach(Inmueble inmueble in inmuebles) {
                    inmuebleListAux.Add(inmueble.Nombre);
                }

                double size = IS_IOS ? IS_TABLET ? 3.5 : 5.5 : IS_TABLET ? 2.5 : 4;
                string value = (string)await PopupUtil.GetObjectAsync(currentInmueble.Nombre is null ? "" : currentInmueble.Nombre, inmuebleListAux, size);

                if(string.IsNullOrWhiteSpace(value) || (currentInmueble.Nombre is not null && currentInmueble.Nombre.Equals(value))) {
                    return currentInmueble;
                }

                foreach(Inmueble inmueble in inmuebles) {
                    if(inmueble.Nombre.Equals(value)) {
                        return inmueble;
                    }
                }

                return currentInmueble;
            } else {
                string url = $"{GET_INMUEBLES_API}?idcliente={idCliente}&idestado={idEstado}";

                HttpHelper httpHelper = new HttpHelper();
                List<Inmueble> inmuebleList = await httpHelper.GetAsync<List<Inmueble>>(url);
                if(inmuebleList == null) {
                    var _dbContext = new DbContext();
                    inmuebleList = await _dbContext.GetinmueblesLocal(idCliente, idEstado);
                    if(inmuebleList == null) {

                        await Toast.Make(Constants.ERROR_OFFLINE, ToastDuration.Short).Show();
                        var inm = new Inmueble();
                        return inm;
                    }

                }
                List<object> inmuebleListAux = new List<object>();

                foreach(Inmueble inmueble in inmuebleList) {
                    inmuebleListAux.Add(inmueble.Nombre);
                }

                double size = IS_IOS ? IS_TABLET ? 3.5 : 5.5 : IS_TABLET ? 2.5 : 4;
                string value = (string)await PopupUtil.GetObjectAsync(currentInmueble.Nombre is null ? "" : currentInmueble.Nombre, inmuebleListAux, size);

                if(string.IsNullOrWhiteSpace(value) || (currentInmueble.Nombre is not null && currentInmueble.Nombre.Equals(value))) {
                    return currentInmueble;
                }

                foreach(Inmueble inmueble in inmuebleList) {
                    if(inmueble.Nombre.Equals(value)) {
                        return inmueble;
                    }
                }

                return currentInmueble;
            }
        } catch(Exception ex) {
            Console.WriteLine(ex.Message);
            throw ex;
        }
}





    #region Tecnicos

    
    public static async Task<TecnicoModel> GetTecnicoAsync(int idCliente, TecnicoModel currentTecnico) {

        if(!InternetUtil.IsConnectedInternet()) {
            //var _dbContext = new DbContext();
            //var inmuebles = await _dbContext.GetinmueblesLocal(idCliente, idEstado);

            //if(inmuebles == null) {
            //    await Toast.Make(Constants.ERROR_API_GET, ToastDuration.Short).Show();
            //    var inm = new Inmueble();
            //    return inm;
            //}
            //List<object> inmuebleListAux = new List<object>();

            //foreach(Inmueble inmueble in inmuebles) {
            //    inmuebleListAux.Add(inmueble.Nombre);
            //}

            //double size = IS_IOS ? IS_TABLET ? 3.5 : 5.5 : IS_TABLET ? 2.5 : 4;
            //string value = (string)await PopupUtil.GetObjectAsync(currentInmueble.Nombre is null ? "" : currentInmueble.Nombre, inmuebleListAux, size);

            //if(string.IsNullOrWhiteSpace(value) || (currentInmueble.Nombre is not null && currentInmueble.Nombre.Equals(value))) {
            //    return currentInmueble;
            //}

            //foreach(Inmueble inmueble in inmuebles) {
            //    if(inmueble.Nombre.Equals(value)) {
            //        return inmueble;
            //    }
            //}

            //return currentInmueble;
            return new TecnicoModel();
        } else {
            string url = $"{GET_TECNICOS_ORDENES_TRABAJO_API}?idCliente={idCliente}";

            HttpHelper httpHelper = new HttpHelper();
            List<TecnicoModel> tecnicoList = await httpHelper.GetAsync<List<TecnicoModel>>(url);
            //if(tecnicoList == null || tecnicoList.Count == 0) {
            //       await Toast.Make(Constants.SIN_REGISTROS, ToastDuration.Short).Show();

            //    return new TecnicoModel();
            //    //var _dbContext = new DbContext();
            //    //inmuebleList = await _dbContext.GetinmueblesLocal(idCliente, idEstado);
            //    //if(inmuebleList == null) {

            //    //    await Toast.Make(Constants.ERROR_OFFLINE, ToastDuration.Short).Show();
            //    //    var inm = new Inmueble();
            //    //    return inm;
            //    //}

            //}
            List<object> tecnicoListAux = new List<object>();

            if(tecnicoList != null && tecnicoList.Count > 0) {
                foreach(TecnicoModel tecnico in tecnicoList) {
                    tecnicoListAux.Add(tecnico.Nombre);
                }
            } else {

            }


                double size = IS_IOS ? IS_TABLET ? 3.5 : 5.5 : IS_TABLET ? 2.5 : 4;
            string value = (string)await PopupUtil.GetObjectAsync(currentTecnico.Nombre is null ? "" : currentTecnico.Nombre, tecnicoListAux, size);

            if(string.IsNullOrWhiteSpace(value) || (currentTecnico.Nombre is not null && currentTecnico.Nombre.Equals(value))) {
                return currentTecnico;
            }

            foreach(TecnicoModel tecnico in tecnicoList) {
                if(tecnico.Nombre.Equals(value)) {
                    return tecnico;
                }
            }

            return currentTecnico;
        }
    }
    #endregion

}

public enum Opciones {
    GENEROS,
    ESTADOS_CIVIL,
    GRADO_ESTUDIOS,
    METODO_COMUNICA,
    TRANSPORTE,
    CANTIDAD_TRANSPORTE,
    PROCEDIMIENTOS
}
#endregion