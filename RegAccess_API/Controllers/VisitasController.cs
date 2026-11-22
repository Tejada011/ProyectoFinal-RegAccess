using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Registroempresa.Entidades;
using Registroempresa.Negocio;

namespace RegAccess_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VisitasController : ControllerBase
    {
        VisitaNegocio negocio = new VisitaNegocio();

        [HttpGet("listar")]
        public List<string> Listar()
        {
            return negocio.ObtenerTodas();
        }

        [HttpGet("buscarTicket")]
        public List<string> BuscarPorTicket(int id)
        {
            return negocio.Buscar(id);
        }

        [HttpGet("buscarNombre")]
        public List<string> BuscarPorNombre(string nombre)
        {
            return negocio.Buscar(nombre);
        }

        [HttpGet("activas")]
        public List<string> VerActivas()
        {
            return negocio.Buscar(true);
        }

        [HttpPost("registrar")]
        public int Registrar(string idVis, string nomVis, string apeVis, string empVis, string idEmp, string nomEmp, string apeEmp, string depEmp)
        {
            Visitante v = new Visitante(idVis, nomVis, apeVis, empVis);
            Empleado e = new Empleado(idEmp, nomEmp, apeEmp, depEmp);
            return negocio.ProcesarEntrada(v, e);
        }

        [HttpPut("salida")]
        public string Salida(int id)
        {
            return negocio.MarcarSalida(id);
        }

        [HttpDelete("eliminar")]
        public bool Eliminar(int id)
        {
            return negocio.Eliminar(id);
        }
    }
}