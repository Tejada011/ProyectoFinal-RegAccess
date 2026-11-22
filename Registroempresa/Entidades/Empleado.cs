using System;
using System.Collections.Generic;
using System.Text;

namespace Registroempresa.Entidades
{
    public class Empleado : Persona
    {
        public string Departamento { get; set; }

        public Empleado() : base() { }

        public Empleado(string id, string nombre, string apellido, string departamento)
            : base(id, nombre, apellido)
        {
            Departamento = departamento;
        }

        public override string ObtenerRol()
        {
            return $"Empleado - Depto: {Departamento}";
        }
    }
}
