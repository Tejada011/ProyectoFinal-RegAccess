using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Registroempresa.Negocio;

namespace Registroempresa
{
    class Program
    {
        static HttpClient client = new HttpClient();
        static string urlBase = "https://localhost:7016/api/Visitas";
        static VisitaNegocio negocio = new VisitaNegocio();

        static async Task Main(string[] args)
        {
            DisplayHeader();
            bool wantToContinue = true;

            while (wantToContinue == true)
            {
                try
                {
                    DisplayMenu();
                    int chosenOption = Convert.ToInt32(Console.ReadLine());

                    switch (chosenOption)
                    {
                        case 1:
                            await RegistrarEntrada();
                            break;
                        case 2:
                            await BuscarVisitas();
                            break;
                        case 3:
                            ModificarVisita();
                            break;
                        case 4:
                            await RegistrarSalida();
                            break;
                        case 5:
                            await ListarVisitas();
                            break;
                        case 6:
                            await EliminarVisita();
                            break;
                        case 7:
                            wantToContinue = false;
                            Console.WriteLine("Cerrando el sistema...");
                            break;
                        default:
                            Console.WriteLine("Opcion no valida. Intente de nuevo.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\nError: {ex.Message}");
                }

                if (wantToContinue == true)
                {
                    Console.WriteLine("\nPresione ENTER para continuar...");
                    Console.ReadLine();
                    Console.Clear();
                }
            }
        }

        static void DisplayHeader()
        {
            Console.WriteLine("=== CONTROL DE ACCESO ===");
            Console.WriteLine("Sistema de gestion de visitas");
        }

        static void DisplayMenu()
        {
            Console.WriteLine("\n=== MENÚ PRINCIPAL ===");
            Console.WriteLine("1. Registrar Entrada");
            Console.WriteLine("2. Buscar Visitas");
            Console.WriteLine("3. Modificar Informacion");
            Console.WriteLine("4. Registrar Salida");
            Console.WriteLine("5. Listar Todas las Visitas");
            Console.WriteLine("6. Eliminar Visita");
            Console.WriteLine("7. Salir");
            Console.Write("Seleccione una opción: ");
        }

        static async Task RegistrarEntrada()
        {
            Console.Clear();
            Console.WriteLine("--- AGREGAR NUEVA ENTRADA ---");

            Console.Write("Cédula/ID del visitante: ");
            string idVis = Console.ReadLine().Trim();
            string nomVis = "", apeVis = "", empVis = "";

            if (negocio.ValidarVisitanteExiste(idVis) == false)
            {
                Console.WriteLine("Visitante nuevo detectado. Ingrese sus datos:");
                Console.Write("Nombre: "); nomVis = Console.ReadLine().Trim();
                Console.Write("Apellido: "); apeVis = Console.ReadLine().Trim();
                Console.Write("Empresa: "); empVis = Console.ReadLine().Trim();
            }

            Console.Write("ID Numérico del Empleado que lo recibe: ");
            string idEmp = Convert.ToInt32(Console.ReadLine()).ToString();
            string nomEmp = "", apeEmp = "", depEmp = "";

            if (negocio.ValidarEmpleadoExiste(idEmp) == false)
            {
                Console.WriteLine("Empleado nuevo detectado. Ingrese sus datos:");
                Console.Write("Nombre: "); nomEmp = Console.ReadLine().Trim();
                Console.Write("Apellido: "); apeEmp = Console.ReadLine().Trim();
                Console.Write("Departamento: "); depEmp = Console.ReadLine().Trim();
            }

            string url = $"{urlBase}/registrar?idVis={idVis}&nomVis={nomVis}&apeVis={apeVis}&empVis={empVis}&idEmp={idEmp}&nomEmp={nomEmp}&apeEmp={apeEmp}&depEmp={depEmp}";
            HttpResponseMessage respuesta = await client.PostAsync(url, null);
            string ticket = await respuesta.Content.ReadAsStringAsync();

            Console.WriteLine($"\n¡Visita registrada! Ticket de visita asignado: {ticket}");
        }

        static async Task BuscarVisitas()
        {
            Console.Clear();
            Console.WriteLine("--- BUSCAR VISITAS ---");
            Console.WriteLine("1. Buscar por Ticket de Visita");
            Console.WriteLine("2. Buscar por Nombre del Visitante");
            Console.WriteLine("3. Ver visitas que siguen en el edificio");
            Console.Write("Seleccione (1-3): ");
            string criterio = Console.ReadLine();

            string url = "";
            if (criterio == "1")
            {
                Console.Write("Ingrese el numero de ticket: ");
                string id = Console.ReadLine();
                url = $"{urlBase}/buscarTicket?id={id}";
            }
            else if (criterio == "2")
            {
                Console.Write("Ingrese el nombre del visitante: ");
                string nombre = Console.ReadLine();
                url = $"{urlBase}/buscarNombre?nombre={nombre}";
            }
            else if (criterio == "3")
            {
                url = $"{urlBase}/activas";
            }

            string json = await client.GetStringAsync(url);
            List<string> resultados = JsonSerializer.Deserialize<List<string>>(json);

            if (resultados.Count == 0)
            {
                Console.WriteLine("No se encontraron registros.");
            }
            else
            {
                for (int i = 0; i < resultados.Count; i++)
                {
                    Console.WriteLine(resultados[i]);
                }
            }
        }

        static void ModificarVisita()
        {
            Console.Clear();
            Console.WriteLine("--- MODIFICAR DATOS ---");
            Console.Write("Ingrese el Ticket de la visita a modificar: ");
            int idBuscado = Convert.ToInt32(Console.ReadLine());

            Console.Write("Nuevo Nombre Visitante (deje vacio para no cambiar): ");
            string nuevoVis = Console.ReadLine();

            Console.Write("Nuevo Nombre Empleado (deje vacio para no cambiar): ");
            string nuevoEmp = Console.ReadLine();

            if (negocio.Modificar(idBuscado, nuevoVis, nuevoEmp) == true)
            {
                Console.WriteLine("¡Informacion actualizada exitosamente!");
            }
            else
            {
                Console.WriteLine("El ticket ingresado no existe.");
            }
        }

        static async Task RegistrarSalida()
        {
            Console.Clear();
            Console.WriteLine("--- REGISTRAR SALIDA ---");
            Console.Write("Ingrese el Ticket de la Visita: ");
            string id = Console.ReadLine();

            HttpResponseMessage respuesta = await client.PutAsync($"{urlBase}/salida?id={id}", null);
            string mensaje = await respuesta.Content.ReadAsStringAsync();
            Console.WriteLine(mensaje.Replace("\"", ""));
        }

        static async Task ListarVisitas()
        {
            Console.Clear();
            Console.WriteLine("--- LISTA DE VISITAS ---");

            string json = await client.GetStringAsync($"{urlBase}/listar");
            List<string> lista = JsonSerializer.Deserialize<List<string>>(json);

            if (lista.Count == 0)
            {
                Console.WriteLine("No hay visitas registradas.");
            }
            else
            {
                for (int i = 0; i < lista.Count; i++)
                {
                    Console.WriteLine(lista[i]);
                }
            }
        }

        static async Task EliminarVisita()
        {
            Console.Clear();
            Console.WriteLine("--- ELIMINAR VISITA ---");
            Console.Write("Ingrese el Ticket de la visita a eliminar: ");
            string id = Console.ReadLine();

            HttpResponseMessage respuesta = await client.DeleteAsync($"{urlBase}/eliminar?id={id}");
            string resultado = await respuesta.Content.ReadAsStringAsync();

            if (resultado == "true")
            {
                Console.WriteLine("Visita eliminada correctamente.");
            }
            else
            {
                Console.WriteLine("Ticket no encontrado.");
            }
        }
    }
}