using Registroempresa.Entidades;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Registroempresa.Datos
{
    public class VisitaDatos
    {
        private string connectionString = @"Server=DESKTOP-Q1OJ2CN\SQLEXPRESS;Database=Registroempresa;Trusted_Connection=True;TrustServerCertificate=True;";

        public int RegistrarEntrada(Visitante visitante, Empleado empleado, Visita visita)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                string sqlCheckVis = "SELECT COUNT(*) FROM Visitantes WHERE Id = @Id";
                SqlCommand cmdCheckVis = new SqlCommand(sqlCheckVis, con);
                cmdCheckVis.Parameters.AddWithValue("@Id", visitante.Id);
                if ((int)cmdCheckVis.ExecuteScalar() == 0)
                {
                    string sqlInsertVis = "INSERT INTO Visitantes (Id, Nombre, Apellido, Empresa) VALUES (@Id, @Nombre, @Apellido, @Empresa)";
                    SqlCommand cmdInsertVis = new SqlCommand(sqlInsertVis, con);
                    cmdInsertVis.Parameters.AddWithValue("@Id", visitante.Id);
                    cmdInsertVis.Parameters.AddWithValue("@Nombre", visitante.Nombre);
                    cmdInsertVis.Parameters.AddWithValue("@Apellido", visitante.Apellido);
                    cmdInsertVis.Parameters.AddWithValue("@Empresa", visitante.Empresa);
                    cmdInsertVis.ExecuteNonQuery();
                }

                string sqlCheckEmp = "SELECT COUNT(*) FROM Empleados WHERE Id = @Id";
                SqlCommand cmdCheckEmp = new SqlCommand(sqlCheckEmp, con);
                cmdCheckEmp.Parameters.AddWithValue("@Id", empleado.Id);
                if ((int)cmdCheckEmp.ExecuteScalar() == 0)
                {
                    string sqlInsertEmp = "INSERT INTO Empleados (Id, Nombre, Apellido, Departamento) VALUES (@Id, @Nombre, @Apellido, @Departamento)";
                    SqlCommand cmdInsertEmp = new SqlCommand(sqlInsertEmp, con);
                    cmdInsertEmp.Parameters.AddWithValue("@Id", empleado.Id);
                    cmdInsertEmp.Parameters.AddWithValue("@Nombre", empleado.Nombre);
                    cmdInsertEmp.Parameters.AddWithValue("@Apellido", empleado.Apellido);
                    cmdInsertEmp.Parameters.AddWithValue("@Departamento", empleado.Departamento);
                    cmdInsertEmp.ExecuteNonQuery();
                }

                string sqlInsertVisita = @"
                    INSERT INTO Visitas (IdVisitante, IdEmpleado, HoraEntrada, HoraSalida, Activa) 
                    VALUES (@IdVis, @IdEmp, @Entrada, @Salida, @Activa);
                    SELECT CAST(SCOPE_IDENTITY() AS INT);";

                SqlCommand cmdVisita = new SqlCommand(sqlInsertVisita, con);
                cmdVisita.Parameters.AddWithValue("@IdVis", visita.IdVisitante);
                cmdVisita.Parameters.AddWithValue("@IdEmp", visita.IdEmpleado);
                cmdVisita.Parameters.AddWithValue("@Entrada", visita.HoraEntrada);
                cmdVisita.Parameters.AddWithValue("@Salida", visita.HoraSalida);
                cmdVisita.Parameters.AddWithValue("@Activa", visita.Activa);

                return (int)cmdVisita.ExecuteScalar();
            }
        }

        public bool ExisteVisitante(string id)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Visitantes WHERE Id = @Id", con);
                cmd.Parameters.AddWithValue("@Id", id);
                return (int)cmd.ExecuteScalar() > 0;
            }
        }

        public bool ExisteEmpleado(string id)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Empleados WHERE Id = @Id", con);
                cmd.Parameters.AddWithValue("@Id", id);
                return (int)cmd.ExecuteScalar() > 0;
            }
        }

        public List<string> BuscarPorTicket(int idTicket)
        {
            List<string> resultados = new List<string>();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string sql = @"
                    SELECT V.IdVisita, V.HoraEntrada, V.HoraSalida, V.Activa,
                           Vis.Nombre AS VisNombre, Vis.Apellido AS VisApellido, Vis.Empresa,
                           Emp.Id AS EmpId, Emp.Nombre AS EmpNombre, Emp.Apellido AS EmpApellido, Emp.Departamento
                    FROM Visitas V
                    INNER JOIN Visitantes Vis ON V.IdVisitante = Vis.Id
                    INNER JOIN Empleados Emp ON V.IdEmpleado = Emp.Id
                    WHERE V.IdVisita = @Id";

                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@Id", idTicket);
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    string estado = (bool)reader["Activa"] ? "En el edificio" : "Fuera";
                    resultados.Add($"Ticket ID: {reader["IdVisita"]}");
                    resultados.Add($"Visitante: {reader["VisNombre"]} {reader["VisApellido"]} ({reader["Empresa"]})");
                    resultados.Add($"Atendido por: [{reader["EmpId"]}] {reader["EmpNombre"]} {reader["EmpApellido"]} - Depto: {reader["Departamento"]}");
                    resultados.Add($"Entrada: {reader["HoraEntrada"]} | Salida: {reader["HoraSalida"]}");
                    resultados.Add($"Estado: {estado}");
                }
            }
            return resultados;
        }

        public List<string> BuscarPorNombre(string nombre)
        {
            List<string> resultados = new List<string>();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string sql = @"
                    SELECT V.IdVisita, V.Activa,
                           Vis.Nombre AS VisNombre, Vis.Apellido AS VisApellido,
                           Emp.Nombre AS EmpNombre
                    FROM Visitas V
                    INNER JOIN Visitantes Vis ON V.IdVisitante = Vis.Id
                    INNER JOIN Empleados Emp ON V.IdEmpleado = Emp.Id
                    WHERE LOWER(Vis.Nombre) LIKE @Nombre OR LOWER(Vis.Apellido) LIKE @Nombre";

                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@Nombre", "%" + nombre.ToLower() + "%");
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    string estado = (bool)reader["Activa"] ? "En el edificio" : "Fuera";
                    resultados.Add($"Ticket: {reader["IdVisita"]} | Visitante: {reader["VisNombre"]} {reader["VisApellido"]} | Empleado: {reader["EmpNombre"]} - Estado: {estado}");
                }
            }
            return resultados;
        }

        public List<string> ObtenerActivas()
        {
            List<string> resultados = new List<string>();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string sql = @"
                    SELECT V.IdVisita, V.HoraEntrada,
                           Vis.Nombre AS VisNombre, Vis.Apellido AS VisApellido,
                           Emp.Nombre AS EmpNombre
                    FROM Visitas V
                    INNER JOIN Visitantes Vis ON V.IdVisitante = Vis.Id
                    INNER JOIN Empleados Emp ON V.IdEmpleado = Emp.Id
                    WHERE V.Activa = 1";

                SqlCommand cmd = new SqlCommand(sql, con);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    resultados.Add($"Ticket: {reader["IdVisita"]} | Visitante: {reader["VisNombre"]} {reader["VisApellido"]} | Empleado: {reader["EmpNombre"]} | Entrada: {reader["HoraEntrada"]}");
                }
            }
            return resultados;
        }

        public bool ModificarNombres(int idTicket, string nuevoNombreVis, string nuevoNombreEmp)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string sqlSelect = "SELECT IdVisitante, IdEmpleado FROM Visitas WHERE IdVisita = @Id";
                SqlCommand cmdSelect = new SqlCommand(sqlSelect, con);
                cmdSelect.Parameters.AddWithValue("@Id", idTicket);
                SqlDataReader reader = cmdSelect.ExecuteReader();

                if (reader.Read())
                {
                    string idVis = reader["IdVisitante"].ToString();
                    string idEmp = reader["IdEmpleado"].ToString();
                    reader.Close();

                    if (!string.IsNullOrWhiteSpace(nuevoNombreVis))
                    {
                        SqlCommand cmdUpdVis = new SqlCommand("UPDATE Visitantes SET Nombre = @Nombre WHERE Id = @Id", con);
                        cmdUpdVis.Parameters.AddWithValue("@Nombre", nuevoNombreVis.Trim());
                        cmdUpdVis.Parameters.AddWithValue("@Id", idVis);
                        cmdUpdVis.ExecuteNonQuery();
                    }

                    if (!string.IsNullOrWhiteSpace(nuevoNombreEmp))
                    {
                        SqlCommand cmdUpdEmp = new SqlCommand("UPDATE Empleados SET Nombre = @Nombre WHERE Id = @Id", con);
                        cmdUpdEmp.Parameters.AddWithValue("@Nombre", nuevoNombreEmp.Trim());
                        cmdUpdEmp.Parameters.AddWithValue("@Id", idEmp);
                        cmdUpdEmp.ExecuteNonQuery();
                    }
                    return true;
                }
                return false;
            }
        }
        public string RegistrarSalida(int idTicket)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlCommand cmdCheck = new SqlCommand("SELECT Activa, HoraSalida FROM Visitas WHERE IdVisita = @Id", con);
                cmdCheck.Parameters.AddWithValue("@Id", idTicket);
                SqlDataReader reader = cmdCheck.ExecuteReader();
                if (reader.Read())
                {
                    bool activa = (bool)reader["Activa"];
                    string horaAnterior = reader["HoraSalida"].ToString();
                    reader.Close();
                    if (activa)
                    {
                        string horaActual = DateTime.Now.ToString("hh:mm tt");
                        SqlCommand cmdUpd = new SqlCommand("UPDATE Visitas SET HoraSalida = @Salida, Activa = 0 WHERE IdVisita = @Id", con);
                        cmdUpd.Parameters.AddWithValue("@Salida", horaActual);
                        cmdUpd.Parameters.AddWithValue("@Id", idTicket);
                        cmdUpd.ExecuteNonQuery();
                        return $"Salida marcada de forma correcta a las {horaActual}";
                    }
                    return $"Este ticket ya registro su salida a las: {horaAnterior}";
                }
                return "Ticket no encontrado.";
            }
        }
        public List<string> ListarTodas()
        {
            List<string> lista = new List<string>();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string sql = @"
SELECT V.IdVisita, V.HoraEntrada, V.HoraSalida, V.Activa,
Vis.Nombre AS VisNombre, Vis.Apellido AS VisApellido,
Emp.Nombre AS EmpNombre
FROM Visitas V
INNER JOIN Visitantes Vis ON V.IdVisitante = Vis.Id
INNER JOIN Empleados Emp ON V.IdEmpleado = Emp.Id";
                SqlCommand cmd = new SqlCommand(sql, con);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string estado = (bool)reader["Activa"] ? "En el edificio" : "Fuera";
                    lista.Add($"Ticket: {reader["IdVisita"]} | Visitante: {reader["VisNombre"]} {reader["VisApellido"]} | Empleado: {reader["EmpNombre"]} | Entrada: {reader["HoraEntrada"]} | Salida: {reader["HoraSalida"]} | Estado: {estado}");
                }
            }
            return lista;
        }
        public bool Eliminar(int idTicket)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM Visitas WHERE IdVisita = @Id", con);
                cmd.Parameters.AddWithValue("@Id", idTicket);
                return cmd.ExecuteNonQuery() > 0;
            }
        }
    }
}