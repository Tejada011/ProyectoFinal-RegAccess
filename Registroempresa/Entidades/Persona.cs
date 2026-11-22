using System;
using System.Collections.Generic;
using System.Text;

namespace Registroempresa.Entidades
{
    public abstract class Persona
    {
        public string Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }

        protected Persona() { }

        protected Persona(string id, string nombre, string apellido)
        {
            Id = id;
            Nombre = nombre;
            Apellido = apellido;
        }

        public abstract string ObtenerRol();
    }
}