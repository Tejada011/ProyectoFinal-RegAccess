using System;
using System.Collections.Generic;
using Registroempresa.Datos;
using Registroempresa.Entidades;

namespace Registroempresa.Negocio
{
    public class VisitaNegocio
    {
        private VisitaDatos datos = new VisitaDatos();

        public bool ValidarVisitanteExiste(string id) => datos.ExisteVisitante(id);
        public bool ValidarEmpleadoExiste(string id) => datos.ExisteEmpleado(id);

        public int ProcesarEntrada(Visitante visitante, Empleado empleado)
        {
            if (string.IsNullOrWhiteSpace(visitante.Id) || string.IsNullOrWhiteSpace(empleado.Id))
            {
                throw new Exception("Los IDs no pueden estar vacios.");
            }

            Visita nuevaVisita = new Visita(0, visitante.Id, empleado.Id);
            return datos.RegistrarEntrada(visitante, empleado, nuevaVisita);
        }

      
        public List<string> Buscar(int idTicket)
        {
            return datos.BuscarPorTicket(idTicket);
        }

      
        public List<string> Buscar(string nombreVisitante)
        {
            return datos.BuscarPorNombre(nombreVisitante);
        }

        public List<string> Buscar(bool soloActivas)
        {
            return datos.ObtenerActivas();
        }

        public bool Modificar(int idTicket, string nuevoVis, string nuevoEmp)
        {
            return datos.ModificarNombres(idTicket, nuevoVis, nuevoEmp);
        }

        public string MarcarSalida(int idTicket)
        {
            return datos.RegistrarSalida(idTicket);
        }

        public List<string> ObtenerTodas()
        {
            return datos.ListarTodas();
        }

        public bool Eliminar(int idTicket)
        {
            return datos.Eliminar(idTicket);
        }
    }
}