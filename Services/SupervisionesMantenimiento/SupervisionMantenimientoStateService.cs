using BatiaSuite.Models;
using BatiaSuite.Models.OrdenesTrabajo;
using BatiaSuite.Models.SupervisionMantenimiento;
using BatiaSuite.Models.SupervisionMantenimiento.Operarios;
using BatiaSuite.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatiaSuite.Services.SupervisionesMantenimiento {

    public class SupervisionStateService {
        public OrdenTrabajoModel? OrdenActual { get; set; }
        public int IdSupervisionActual { get; set; }
        public DateTime FechaInicio { get; set; } = DateTime.Now;
        public int IdTipoServicio { get; set; }
        public int IdCliente { get; set; }
        public InmuebleModel Inmueble { get; set; }
        public PisoModel? PisoActual { get; set; }
        public SeccionModel? SeccionActual { get; set; }
        public IteracionModel? IteracionActual { get; set; }

        public List<SeccionModel> PlantillaBaseSecciones { get; set; } = new();

        public ObservableCollection<PisoModel> Pisos { get; set; } = new();

        /// <summary>
        /// Genera una copia 100% nueva e independiente de las secciones y preguntas base
        /// para ser asignada a un nuevo piso creado dinámicamente.
        /// </summary>
        public ObservableCollection<SeccionModel> GenerarSeccionesParaNuevoPiso() {
            var seccionesClonadas = new ObservableCollection<SeccionModel>();

            foreach(var seccionBase in PlantillaBaseSecciones) {
                var nuevaSeccion = new SeccionModel {
                    IdSeccion = seccionBase.IdSeccion,
                    Seccion = seccionBase.Seccion,
                    Preguntas = new ObservableCollection<PreguntaModel>(),
                };

                foreach(var pregBase in seccionBase.Preguntas) {
                    nuevaSeccion.Preguntas.Add(new PreguntaModel {
                        IdPregunta = pregBase.IdPregunta,
                        Pregunta = pregBase.Pregunta,
                        Respuesta = -1, // Inicializado sin responder
                        Observaciones = string.Empty
                    });
                }

                seccionesClonadas.Add(nuevaSeccion);
            }

            return seccionesClonadas;
        }

        /// <summary>
        /// Limpia todo el estado de la sesión para preparar la app para una nueva orden.
        /// </summary>
        public void LimpiarSesion() {
            OrdenActual = null;
            PisoActual = null;
            SeccionActual = null;
            IdSupervisionActual = 0;
            FechaInicio = DateTime.Now;
            PlantillaBaseSecciones.Clear();
            Pisos.Clear();
        }
    }
}