using BatiaSuite.Models.CheckListAparadores;
using BatiaSuite.Models.SupervisionMantenimiento;
using BatiaSuite.ViewModel.CheckListAparadores;
using BatiaSuite.ViewModel.Supervisionmantenimiento;
using DynamicData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.Maui.ApplicationModel.Permissions;
using static System.Collections.Specialized.BitVector32;

namespace BatiaSuite.Services {
    public class SupervisionMantenimientoService {



        private SupervisionMantenimientoModel _supervisionModel = new SupervisionMantenimientoModel();
        private int _currentSectionIndex = 0;
        private List<SupervisionMantenimientoSeccionesModel> _activeSections = new();



        public SupervisionMantenimientoSeccionesModel GetCurrentSection() {
            if(_activeSections == null || _activeSections.Count == 0)
                return null;

            if(_currentSectionIndex >= 0 && _currentSectionIndex < _activeSections.Count)
                return _activeSections[_currentSectionIndex];

            return null;
        }

        public SupervisionMantenimientoSeccionesModel GetNextSection() {
            if(_currentSectionIndex < _activeSections.Count - 1) {
                _currentSectionIndex++;
                return GetCurrentSection();
            }
            return null; // No hay más secciones
        }

        public SupervisionMantenimientoSeccionesModel GetPreviousSection() {
            if(_currentSectionIndex > 0) {
                _currentSectionIndex--;
                return GetCurrentSection();
            }
            return null;
        }

        public bool HasNextSection() {
            return _currentSectionIndex < _activeSections.Count - 1;
        }

        public bool HasPreviousSection() {
            return _currentSectionIndex > 0;
        }

        public int GetCurrentSectionIndex() {
            return _currentSectionIndex + 1; // Para mostrar 1-based
        }

        public int GetTotalSections() {
            return _activeSections.Count;
        }



        public SupervisionMantenimientoModel GetSupervisionModel() {
            return _supervisionModel;
        }

        public List<SupervisionMantenimientoPreguntasModel> GetPreguntasBySeccion(int seccion) {
            if(_supervisionModel != null && _supervisionModel.Preguntas != null && _supervisionModel.Preguntas.Count > 0) {
                return _supervisionModel.Preguntas.Where(x => x.IdSeccion == seccion).ToList();
            } else {
                return new List<SupervisionMantenimientoPreguntasModel>();
            }
        }

        public List<SupervisionMantenimientoSeccionesModel> ObtenerSecciones() {
            try {
                if(_supervisionModel != null && _supervisionModel.Secciones != null && _supervisionModel.Secciones.Count > 0) {
                    return _supervisionModel.Secciones;
                } else {
                    return new List<SupervisionMantenimientoSeccionesModel>();
                }
            } catch(Exception ex) {
                Console.WriteLine($"Error al obtener secciones (Metodo ObtenerSecciones en Service): {ex.Message}");
                return new List<SupervisionMantenimientoSeccionesModel>();
            }
        }

        public bool InicioSupervision(int idCliente, int idInmueble) {
            try {
                _supervisionModel.IdCliente = idCliente;
                _supervisionModel.IdInmueble = idInmueble;
                _supervisionModel.FechaInicio = DateTime.Now;
                return true;
            } catch(Exception ex) {
                Console.WriteLine($"Error al iniciar supervisión (Metodo InicioSupervision en Service): {ex.Message}");
                return false;
            }
        }

        public bool InitModel() {
            try {
                _supervisionModel = new SupervisionMantenimientoModel();
                _supervisionModel.Secciones = new List<SupervisionMantenimientoSeccionesModel>();
                _supervisionModel.Preguntas = new List<SupervisionMantenimientoPreguntasModel>();
                _supervisionModel.FotosSeccion = new List<SupervisionMantenimientoFotosSeccionModel>();
                _supervisionModel.FirmasBytes = new List<FirmaSupervisionMantenimientoModel>();
                _supervisionModel.HidrantesyAspersoresPreguntas = new List<SupervisionMantenimientoHidrantesPreguntasModel>();
                _supervisionModel.ExtintoresPreguntas = new List<SupervisionMantenimientoExtintoresPreguntasModel>();
                _supervisionModel.HidrantesyAspersoresObjects = new List<SupervisionMantenimientoHidrantesObjectModel>();
                _supervisionModel.ExtintoresObjects = new List<SupervisionMantenimientoExtintoresObjectModel>();
                //IniciarValores();
                return true;
            } catch(Exception ex) {
                Console.WriteLine($"Error al iniciar supervisión (Metodo InitModel en Service): {ex.Message}");
                return false;
            }
        }

        public bool GuardarSeccionesPreguntas(SupervisionMantenimientoSeccionPreguntaModel seccionPregunta) {
            if(seccionPregunta != null && seccionPregunta.Preguntas != null && seccionPregunta.Secciones != null) {
                _supervisionModel.Secciones = seccionPregunta.Secciones;

                //
                if(_supervisionModel.HidrantesyAspersoresPreguntas != null && _supervisionModel.ExtintoresPreguntas != null && _supervisionModel.Preguntas != null) {
                    foreach(var sec in seccionPregunta.Preguntas) {
                        if(sec.IdSeccion == 7) {
                            var pre = new SupervisionMantenimientoHidrantesPreguntasModel {
                                IdPregunta = sec.IdPregunta,
                                Pregunta = sec.Pregunta,
                                Comentarios = sec.Comentarios,
                                Valor = 0
                            };

                            _supervisionModel.HidrantesyAspersoresPreguntas.Add(pre);
                        } else if(sec.IdSeccion == 10) {
                            var pre = new SupervisionMantenimientoExtintoresPreguntasModel {
                                IdPregunta = sec.IdPregunta,
                                Pregunta = sec.Pregunta,
                                Comentarios = sec.Comentarios,
                                Valor = 0
                            };
                            _supervisionModel.ExtintoresPreguntas.Add(pre);
                        } else if(sec.IdSeccion != 7 && sec.IdSeccion != 10) {
                            _supervisionModel.Preguntas.Add(sec);
                        }
                    }
                }



                if(_supervisionModel.Secciones != null) {
                    _activeSections = _supervisionModel.Secciones;
                }
                return true;
            } else {
                return false;
            }
        }

        public bool IniciarValores() {
            _supervisionModel.Secciones = new List<SupervisionMantenimientoSeccionesModel> {
                new SupervisionMantenimientoSeccionesModel { IdSeccion = 1, Seccion = "Sanitarios", Terminada = false, EsSeccionDeObjetos = false },
                new SupervisionMantenimientoSeccionesModel { IdSeccion = 2, Seccion = "Luminarias", Terminada = false, EsSeccionDeObjetos = false },
                new SupervisionMantenimientoSeccionesModel { IdSeccion = 5, Seccion = "Tableros de control", Terminada = false, EsSeccionDeObjetos = false },
                new SupervisionMantenimientoSeccionesModel { IdSeccion = 7, Seccion = "Hidrantes y Aspersores", Terminada = false, EsSeccionDeObjetos = true },
                new SupervisionMantenimientoSeccionesModel { IdSeccion = 9, Seccion = "Planta de emergencia", Terminada = false, EsSeccionDeObjetos = false },
                new SupervisionMantenimientoSeccionesModel { IdSeccion = 10, Seccion = "Extintores", Terminada = false, EsSeccionDeObjetos = true },
                new SupervisionMantenimientoSeccionesModel { IdSeccion = 12, Seccion = "Conservación", Terminada = false, EsSeccionDeObjetos = false },
                new SupervisionMantenimientoSeccionesModel { IdSeccion = 13, Seccion = "Acceso", Terminada = false, EsSeccionDeObjetos = false },
                new SupervisionMantenimientoSeccionesModel { IdSeccion = 14, Seccion = "BMS(Monitoreo)", Terminada = false, EsSeccionDeObjetos = false },
                new SupervisionMantenimientoSeccionesModel { IdSeccion = 15, Seccion = "Elevadores", Terminada = false, EsSeccionDeObjetos = false },
                new SupervisionMantenimientoSeccionesModel { IdSeccion = 20, Seccion = "HVAC y extracción", Terminada = false, EsSeccionDeObjetos = false }
            };

            _supervisionModel.Preguntas = new List<SupervisionMantenimientoPreguntasModel> {
                new SupervisionMantenimientoPreguntasModel { IdSeccion = 1, IdPregunta = 1, Pregunta = "Pregunta 1.1" },
                new SupervisionMantenimientoPreguntasModel { IdSeccion = 1, IdPregunta = 2, Pregunta = "Pregunta 1.2" },
                //new SupervisionMantenimientoPreguntasModel { IdSeccion = 1, IdPregunta = 3, Pregunta = "Pregunta 1.3" },
                //new SupervisionMantenimientoPreguntasModel { IdSeccion = 1, IdPregunta = 4, Pregunta = "Pregunta 1.4" },
                //new SupervisionMantenimientoPreguntasModel { IdSeccion = 1, IdPregunta = 5, Pregunta = "Pregunta 1.5" },
                //new SupervisionMantenimientoPreguntasModel { IdSeccion = 1, IdPregunta = 6, Pregunta = "Pregunta 1.6" },
                new SupervisionMantenimientoPreguntasModel { IdSeccion = 2, IdPregunta = 1, Pregunta = "Pregunta 2.1" },
                new SupervisionMantenimientoPreguntasModel { IdSeccion = 2, IdPregunta = 2, Pregunta = "Pregunta 2.2" },
                new SupervisionMantenimientoPreguntasModel { IdSeccion = 5, IdPregunta = 1, Pregunta = "Pregunta 5.1" },
                new SupervisionMantenimientoPreguntasModel { IdSeccion = 5, IdPregunta = 2, Pregunta = "Pregunta 5.2" },
                new SupervisionMantenimientoPreguntasModel { IdSeccion = 9, IdPregunta = 1, Pregunta = "Pregunta 9.1" },
                new SupervisionMantenimientoPreguntasModel { IdSeccion = 9, IdPregunta = 2, Pregunta = "Pregunta 9.2" },
                new SupervisionMantenimientoPreguntasModel { IdSeccion = 12, IdPregunta = 1, Pregunta = "Pregunta 12.1" },
                new SupervisionMantenimientoPreguntasModel { IdSeccion = 12, IdPregunta = 2, Pregunta = "Pregunta 12.2" },
                new SupervisionMantenimientoPreguntasModel { IdSeccion = 13, IdPregunta = 1, Pregunta = "Pregunta 13.1" },
                new SupervisionMantenimientoPreguntasModel { IdSeccion = 13, IdPregunta = 2, Pregunta = "Pregunta 13.2" },
                new SupervisionMantenimientoPreguntasModel { IdSeccion = 14, IdPregunta = 1, Pregunta = "Pregunta 14.1" },
                new SupervisionMantenimientoPreguntasModel { IdSeccion = 14, IdPregunta = 2, Pregunta = "Pregunta 14.2" },
                new SupervisionMantenimientoPreguntasModel { IdSeccion = 20, IdPregunta = 1, Pregunta = "Pregunta 20.1" },
                new SupervisionMantenimientoPreguntasModel { IdSeccion = 20, IdPregunta = 2, Pregunta = "Pregunta 20.2" },
            };

            _supervisionModel.HidrantesyAspersoresPreguntas = new List<SupervisionMantenimientoHidrantesPreguntasModel> {
                new SupervisionMantenimientoHidrantesPreguntasModel { IdPregunta = 1, Pregunta = "Estado de Gabinete, Chapa, Cristal en buen estado, rack y manguera", Valor = 0 },
                new SupervisionMantenimientoHidrantesPreguntasModel { IdPregunta = 2, Pregunta = "Revision de etiquete de Mantenimiento", Valor = 0 },
                //new SupervisionMantenimientoHidrantesPreguntasModel { IdPregunta = 3, Pregunta = "Inspeccion del estado de la manguera que no presente fuga o este fisurada", Valor = 0 },
                //new SupervisionMantenimientoHidrantesPreguntasModel { IdPregunta = 4, Pregunta = "Verificar el buen estado del chiflon", Valor = 0 },
                //new SupervisionMantenimientoHidrantesPreguntasModel { IdPregunta = 5, Pregunta = "Revision de valvula", Valor = 0 },
                //new SupervisionMantenimientoHidrantesPreguntasModel { IdPregunta = 6, Pregunta = "Soporteria en buen estado", Valor = 0 },
                //new SupervisionMantenimientoHidrantesPreguntasModel { IdPregunta = 7, Pregunta = "Señaletica", Valor = 0 },
                //new SupervisionMantenimientoHidrantesPreguntasModel { IdPregunta = 8, Pregunta = "Verificar llave angular", Valor = 0 },
                //new SupervisionMantenimientoHidrantesPreguntasModel { IdPregunta = 9, Pregunta = "Verificar su cuenta con manometro", Valor = 0 },
                //new SupervisionMantenimientoHidrantesPreguntasModel { IdPregunta = 10, Pregunta = "Revisar su cuenta con llave universal pata de cabra", Valor = 0 },
                //new SupervisionMantenimientoHidrantesPreguntasModel { IdPregunta = 11, Pregunta = "Verificar que cuente con llave universal para apertura de gabinetes", Valor = 0 },
                //new SupervisionMantenimientoHidrantesPreguntasModel { IdPregunta = 12, Pregunta = "Verificar que no este obstruido el gabinete", Valor = 0 },
                //new SupervisionMantenimientoHidrantesPreguntasModel { IdPregunta = 13, Pregunta = "Verificar que no goteen los aspersores", Valor = 0 },
                //new SupervisionMantenimientoHidrantesPreguntasModel { IdPregunta = 14, Pregunta = "Verificar que tengan tapa los aspersores", Valor = 0 },
                //new SupervisionMantenimientoHidrantesPreguntasModel { IdPregunta = 15, Pregunta = "Ubicacion y/o ID", Valor = 0 },
            };
            _supervisionModel.ExtintoresPreguntas = new List<SupervisionMantenimientoExtintoresPreguntasModel> {
                new SupervisionMantenimientoExtintoresPreguntasModel { IdPregunta = 1, Pregunta = "Estado del cilindro", Valor = 0 },
                new SupervisionMantenimientoExtintoresPreguntasModel { IdPregunta = 2, Pregunta = "Revision de boquilla", Valor = 0 },
                //new SupervisionMantenimientoExtintoresPreguntasModel { IdPregunta = 3, Pregunta = "Capacidad", Valor = 0 },
                //new SupervisionMantenimientoExtintoresPreguntasModel { IdPregunta = 4, Pregunta = "Inspeccion del estado de la pintura", Valor = 0 },
                //new SupervisionMantenimientoExtintoresPreguntasModel { IdPregunta = 5, Pregunta = "Tipo de agente extintor", Valor = 0 },
                //new SupervisionMantenimientoExtintoresPreguntasModel { IdPregunta = 6, Pregunta = "Verificar que cuente con seguro de manguera este en buen estado", Valor = 0 },
                //new SupervisionMantenimientoExtintoresPreguntasModel { IdPregunta = 7, Pregunta = "Inspeccion del manometro presion", Valor = 0 },
                //new SupervisionMantenimientoExtintoresPreguntasModel { IdPregunta = 8, Pregunta = "Soporteria en buen estado", Valor = 0 },
                //new SupervisionMantenimientoExtintoresPreguntasModel { IdPregunta = 9, Pregunta = "Verificar la altura del extintor y señaletica", Valor = 0 },
                //new SupervisionMantenimientoExtintoresPreguntasModel { IdPregunta = 10, Pregunta = "Fecha de ultima recarga y etiqueta de inspeccion", Valor = 0 },
                //new SupervisionMantenimientoExtintoresPreguntasModel { IdPregunta = 11, Pregunta = "Ubicacion y/o ID", Valor = 0 },
            };
            _activeSections = _supervisionModel.Secciones;
            return true;
        }

        public List<SupervisionMantenimientoHidrantesPreguntasModel> GetHidrantesyAspersoresPreguntas() {
            if(_supervisionModel != null && _supervisionModel.HidrantesyAspersoresPreguntas != null && _supervisionModel.HidrantesyAspersoresPreguntas.Count > 0) {
                return _supervisionModel.HidrantesyAspersoresPreguntas;
            } else {
                return new List<SupervisionMantenimientoHidrantesPreguntasModel>();
            }
        }

        public List<SupervisionMantenimientoExtintoresPreguntasModel> GetExtintoresPreguntas() {
            if(_supervisionModel != null && _supervisionModel.ExtintoresPreguntas != null && _supervisionModel.ExtintoresPreguntas.Count > 0) {
                return _supervisionModel.ExtintoresPreguntas;
            } else {
                return new List<SupervisionMantenimientoExtintoresPreguntasModel>();
            }
        }

        public bool GuardarRespuestasPorSeccion(List<SupervisionMantenimientoPreguntasModel> respuestas) {
            try {
                if(respuestas != null && respuestas.Count > 0 && _supervisionModel.Preguntas != null) {
                    foreach(var obj in respuestas) {
                        var existente = _supervisionModel.Preguntas
                       .FirstOrDefault(x =>
                           x.IdSeccion == obj.IdSeccion &&
                           x.IdPregunta == obj.IdPregunta);

                        if(existente != null) {
                            existente.DispositivosPorNivel = obj.DispositivosPorNivel;
                            existente.Estado = obj.Estado;
                            existente.Comentarios = obj.Comentarios;
                        }
                    }
                }
                return true;
            } catch(Exception ex) {
                Console.WriteLine($"Error al guardar preguntas de sección (Metodo GuardarRespuestasPorSeccion en Service): {ex.Message}");
                return false;
            }
        }
        public bool GuardarRespuestaHidrante(List<SupervisionMantenimientoHidrantesPreguntasModel> respuestas, string comentarios, string fotoPath) {
            try {
                var hid = new SupervisionMantenimientoHidrantesObjectModel {
                    IdConsec = _supervisionModel.HidrantesyAspersoresObjects != null ? _supervisionModel.HidrantesyAspersoresObjects.Count + 1 : 1,
                    ComentarioGeneral = comentarios,
                    Respuestas = new List<SupervisionMantenimientoHidrantesObject>(),
                    FotoPath = fotoPath
                };
                if(respuestas != null && respuestas.Count > 0) {
                    foreach(var obj in respuestas) {
                        var hidranteObj = new SupervisionMantenimientoHidrantesObject {
                            IdPregunta = obj.IdPregunta,
                            Pregunta = obj.Pregunta,
                            Estado = obj.Valor,
                            Comentarios = obj.Comentarios
                        };
                        hid.Respuestas.Add(hidranteObj);
                    }
                }
                if(_supervisionModel.HidrantesyAspersoresObjects != null) {
                    //_supervisionModel.HidrantesyAspersoresObjects.Add(hid);
                    _supervisionModel.HidrantesyAspersoresObjects.Insert(0, hid);
                }

                return true;
            } catch(Exception ex) {
                Console.WriteLine($"Error al guardar objeto hidrante (Metodo GuardarRespuestaHidrante en Service): {ex.Message}");
                return false;
            }
        }

        public bool GuardarRespuestaExtintor(List<SupervisionMantenimientoExtintoresPreguntasModel> respuestas, string comentarios, string fotoPath) {
            try {
                var hid = new SupervisionMantenimientoExtintoresObjectModel {
                    IdConsec = _supervisionModel.ExtintoresObjects != null ? _supervisionModel.ExtintoresObjects.Count + 1 : 1,
                    ComentarioGeneral = comentarios,
                    Respuestas = new List<SupervisionMantenimientoExtintoresObject>(),
                    FotoPath = fotoPath
                };
                if(respuestas != null && respuestas.Count > 0) {
                    foreach(var obj in respuestas) {
                        var hidranteObj = new SupervisionMantenimientoExtintoresObject {
                            IdPregunta = obj.IdPregunta,
                            Pregunta = obj.Pregunta,
                            Estado = obj.Valor,
                            Comentarios = obj.Comentarios
                        };
                        hid.Respuestas.Add(hidranteObj);
                    }
                }
                if(_supervisionModel.ExtintoresObjects != null) {
                    //_supervisionModel.ExtintoresObjects.Add(hid);
                    //INSERTAR EN POSICION 0 PARA RECORRER EL ARREGLO
                    _supervisionModel.ExtintoresObjects.Insert(0, hid);
                }

                return true;
            } catch(Exception ex) {
                Console.WriteLine($"Error al guardar objeto hidrante (Metodo GuardarRespuestaExtintor en Service): {ex.Message}");
                return false;
            }
        }

        public bool MarcarSeccionTerminada(int idSeccion) {
            try {
                if(_supervisionModel.Secciones != null && _supervisionModel.Secciones.Count > 0) {
                    var existente = _supervisionModel.Secciones
                       .FirstOrDefault(x =>
                           x.IdSeccion == idSeccion);

                    if(existente != null) {
                        existente.Terminada = true;
                    }
                }
                return true;

            } catch(Exception ex) {
                Console.WriteLine($"Error al marcar sección como terminada (Metodo MarcarSeccionTerminada en Service): {ex.Message}");
                return false;
            }
        }

        public bool GuardarFotoSeccion(SupervisionMantenimientoFotosSeccionModel fotosSeccion) {
            if(_supervisionModel.FotosSeccion != null) {
                _supervisionModel.FotosSeccion.Add(fotosSeccion);
                return true;
            } else {
                return false;
            }
        }

        public bool EliminarFotoSeccion(SupervisionMantenimientoFotosSeccionModel fotoSeccion) {
            if(_supervisionModel.FotosSeccion != null) {
                var fotoAEliminar = _supervisionModel.FotosSeccion
                .FirstOrDefault(x =>
                    x.IdSeccion == fotoSeccion.IdSeccion &&
                    x.FotoPath == fotoSeccion.FotoPath);
                if(fotoAEliminar != null) {
                    _supervisionModel.FotosSeccion.Remove(fotoAEliminar);
                    return true;
                }
                return false;
            } else {
                return false;
            }

        }

        public List<SupervisionMantenimientoFotosSeccionModel> ObtenerFotosPorSeccion(int seccion) {
            if(_supervisionModel.FotosSeccion != null && _supervisionModel.FotosSeccion.Count > 0) {
                var fotosSeccion = _supervisionModel.FotosSeccion.Where(x => x.IdSeccion == seccion).ToList();
                return fotosSeccion;
            } else {
                return new List<SupervisionMantenimientoFotosSeccionModel>();
            }
        }

        public List<SupervisionMantenimientoFotosSeccionModel> GetAllFotos() {
            if(_supervisionModel.FotosSeccion != null && _supervisionModel.FotosSeccion.Count > 0) {
                return _supervisionModel.FotosSeccion;
            } else {
                return new List<SupervisionMantenimientoFotosSeccionModel>();
            }
        }

        public bool LimpiarDatos() {
            _supervisionModel.FotosSeccion = new List<SupervisionMantenimientoFotosSeccionModel>();
            return true;
        }

        public int ContarHidrantesoAspersoresGuardados() {
            if(_supervisionModel.HidrantesyAspersoresObjects != null) {
                return _supervisionModel.HidrantesyAspersoresObjects.Count;
            } else {
                return 0;
            }
        }
        public int ContarExtintoresGuardados() {
            if(_supervisionModel.ExtintoresObjects != null) {
                return _supervisionModel.ExtintoresObjects.Count;
            } else {
                return 0;
            }
        }

        public List<SupervisionMantenimientoExtintoresObjectModel> ObtenerExtintoresGuardados() {
            try {
                if(_supervisionModel.ExtintoresObjects != null && _supervisionModel.ExtintoresObjects.Count > 0) {
                    return _supervisionModel.ExtintoresObjects;
                } else {
                    return new List<SupervisionMantenimientoExtintoresObjectModel>();
                }
            } catch(Exception ex) {
                Console.WriteLine($"Error al obtener extintores guardados (Metodo ObtenerExtintoresGuardados en Service): {ex.Message}");
                return new List<SupervisionMantenimientoExtintoresObjectModel>();
            }
        }

        public List<SupervisionMantenimientoHidrantesObjectModel> ObtenerHidrantesoAspersoresGuardados() {
            try {
                if(_supervisionModel.HidrantesyAspersoresObjects != null && _supervisionModel.HidrantesyAspersoresObjects.Count > 0) {
                    return _supervisionModel.HidrantesyAspersoresObjects;
                } else {
                    return new List<SupervisionMantenimientoHidrantesObjectModel>();
                }
            } catch(Exception ex) {
                Console.WriteLine($"Error al obtener hidrantes y aspersores guardados (Metodo ObtenerHidrantesoAspersoresGuardados en Service): {ex.Message}");
                return new List<SupervisionMantenimientoHidrantesObjectModel>();
            }
        }

        public SupervisionMantenimientoExtintoresObjectModel GetExtintorById(int idConsec) {
            try {
                if(_supervisionModel.ExtintoresObjects != null && _supervisionModel.ExtintoresObjects.Count > 0) {
                    var ext = _supervisionModel.ExtintoresObjects.Where(x => x.IdConsec == idConsec).FirstOrDefault();
                    return ext;
                } else {
                    return new SupervisionMantenimientoExtintoresObjectModel();
                }
            } catch(Exception ex) {
                Console.WriteLine($"Error al obtener extintor por ID (Metodo GetExtintorById en Service): {ex.Message}");
                return new SupervisionMantenimientoExtintoresObjectModel();
            }
        }

        public SupervisionMantenimientoHidrantesObjectModel GetHidranteById(int idConsec) {
            try {
                if(_supervisionModel.HidrantesyAspersoresObjects != null && _supervisionModel.HidrantesyAspersoresObjects.Count > 0) {
                    var hid = _supervisionModel.HidrantesyAspersoresObjects.Where(x => x.IdConsec == idConsec).FirstOrDefault();
                    return hid;
                } else {
                    return new SupervisionMantenimientoHidrantesObjectModel();
                }
            } catch(Exception ex) {
                Console.WriteLine($"Error al obtener extintor por ID (Metodo GetExtintorById en Service): {ex.Message}");
                return new SupervisionMantenimientoHidrantesObjectModel();
            }
        }

        public bool ActualizarHidrante(SupervisionMantenimientoHidrantesObjectModel hidranteActualizado) {
            try {
                if(_supervisionModel.HidrantesyAspersoresObjects != null && _supervisionModel.HidrantesyAspersoresObjects.Count > 0) {
                    var index = _supervisionModel.HidrantesyAspersoresObjects.FindIndex(x => x.IdConsec == hidranteActualizado.IdConsec);
                    if(index >= 0) {
                        _supervisionModel.HidrantesyAspersoresObjects[index] = hidranteActualizado;
                        return true;
                    }
                }
                return false;
            } catch(Exception ex) {
                Console.WriteLine($"Error al actualizar hidrante (Metodo ActualizarHidrante en Service): {ex.Message}");
                return false;
            }
        }

        public bool ActualizarExtintor(SupervisionMantenimientoExtintoresObjectModel extintorActualizado) {
            try {
                if(_supervisionModel.ExtintoresObjects != null && _supervisionModel.ExtintoresObjects.Count > 0) {
                    var index = _supervisionModel.ExtintoresObjects.FindIndex(x => x.IdConsec == extintorActualizado.IdConsec);
                    if(index >= 0) {
                        _supervisionModel.ExtintoresObjects[index] = extintorActualizado;
                        return true;
                    }
                }
                return false;
            } catch(Exception ex) {
                Console.WriteLine($"Error al actualizar extintor (Metodo ActualizarExtintor en Service): {ex.Message}");
                return false;
            }
        }
    }
}
