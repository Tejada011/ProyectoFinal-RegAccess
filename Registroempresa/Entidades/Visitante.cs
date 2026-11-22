using System;
using System.Collections.Generic;
using System.Text;

namespace Registroempresa.Entidades
{
    public class Visitante : Persona
    {
        public string Empresa { get; set; }

        public Visitante() : base() { }

        public Visitante(string id, string nombre, string apellido, string empresa)
            : base(id, nombre, apellido)
        {
            Empresa = empresa;
        }

        public override string ObtenerRol()
        {
            return $"Visitante - Empresa: {Empresa}";
        }
    }
}
