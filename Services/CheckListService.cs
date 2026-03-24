using BatiaSuite.Models.CheckListAparadores;
using BatiaSuite.ViewModel.CheckListAparadores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatiaSuite.Services {
    public class CheckListService {
        private List<CheckListPreguntasModel> _preguntas = new();
        private List<CheckListFoto> _fotos = new();
        public void SetPreguntas(List<CheckListPreguntasModel> preguntas) {
            _preguntas = preguntas;
        }

        public List<CheckListPreguntasModel> GetPreguntas() {
            return _preguntas;
        }

        public List<CheckListPreguntasModel> GetPreguntasBySeccion(int seccion) {
            return _preguntas.Where(x => x.IdSeccion == seccion).ToList();
        }


        public List<CheckListPreguntasModel> GetTotalCheckList() {
            return _preguntas;
        }
        public void UpdatePregunta(CheckListPreguntasModel preguntaActualizada) {
            var existente = _preguntas
                .FirstOrDefault(x =>
                    x.IdSeccion == preguntaActualizada.IdSeccion &&
                    x.IdPregunta == preguntaActualizada.IdPregunta);

            if(existente != null) {
                existente.Valor1 = preguntaActualizada.Valor1;
                existente.Valor2 = preguntaActualizada.Valor2;
                existente.Valor3 = preguntaActualizada.Valor3;
                existente.Comentarios = preguntaActualizada.Comentarios;
            }
        }
        public bool GuardarFotoSeccion(CheckListFoto fotosSeccion) {
            _fotos.Add(fotosSeccion);
            return true;
        }
        public  bool EliminarFotoSeccion(CheckListFoto fotoSeccion) {
            var fotoAEliminar = _fotos
                .FirstOrDefault(x =>
                    x.IdSeccion == fotoSeccion.IdSeccion &&
                    x.Path == fotoSeccion.Path);
            if(fotoAEliminar != null) {
                _fotos.Remove(fotoAEliminar);
                return true;
            }
            return false;
        }
        
        public List<CheckListFoto> ObtenerFotosPorSeccion(int seccion) {
            var fotosSeccion =  _fotos.Where(x => x.IdSeccion == seccion).ToList();
            return fotosSeccion;
        }

        public List<CheckListFoto> GetAllFotos() {
            return _fotos;
        }

        public bool LimpiarDatos() {
            _fotos = new List<CheckListFoto>();
            return true;
        }


    }

}
