using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    public class RepositorioContrato
    {
        private readonly string connectionString;
        private readonly IConfiguration configuration;

        public RepositorioContrato(IConfiguration configuration)
        {
            this.configuration = configuration;
            connectionString = configuration["ConnectionStrings:DefaultConnection"];
        }

        public int Alta(Contrato p)
        {
            int res = -1;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = $"INSERT INTO contrato (FechaInicio, FechaFin, Importe, DniGarante, NombreCompletoGarante, TelefonoGarante, EmailGarante, InquilinoId, InmuebleId) " +
                    $"VALUES (@fechaInicio, @fechaFin, @importe, @dniGarante, @nombreGarante, @telGarante, @emailGarante, @inquilinoId, @inmuebleId);" +
                    $"SELECT LAST_INSERT_ID();";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@fechaInicio", p.FechaInicio);
                    command.Parameters.AddWithValue("@fechaFin", p.FechaFin);
                    command.Parameters.AddWithValue("@importe", p.Importe);
                    command.Parameters.AddWithValue("@dniGarante", p.DniGarante);
                    command.Parameters.AddWithValue("@nombreGarante", p.NombreCompletoGarante);
                    command.Parameters.AddWithValue("@telGarante", p.TelefonoGarante);
                    command.Parameters.AddWithValue("@emailGarante", p.EmailGarante);
                    command.Parameters.AddWithValue("@inquilinoId", p.InquilinoId);
                    command.Parameters.AddWithValue("@inmuebleId", p.InmuebleId);
                    connection.Open();
                    res = Convert.ToInt32(command.ExecuteScalar());
                    p.Id = res;
                    connection.Close();
                }
            }
            return res;
        }
        public int Baja(int id)
        {
            int res = -1;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = $"DELETE FROM contrato WHERE Id = {id}";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    res = command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            return res;
        }

        public int Modificacion(Contrato a)
        {
            int res = -1;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = $"UPDATE contrato SET FechaInicio=@fechainicio, FechaFin=@fechafin, Importe=@importe, DniGarante=@dniGarante, NombreCompletoGarante=@nombreGarante, TelefonoGarante=@telGarante, EmailGarante=@emailGarante, InquilinoId=@inquilinoId, InmuebleId=@inmuebleId " +
                    $"WHERE Id = {a.Id}";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.Add("@importe", MySqlDbType.Decimal).Value = a.Importe;
                    command.Parameters.Add("@fechainicio", MySqlDbType.Date).Value = a.FechaInicio;
                    command.Parameters.Add("@fechafin", MySqlDbType.Date).Value = a.FechaFin;
                    command.Parameters.Add("@dniGarante", MySqlDbType.VarChar).Value = a.DniGarante;
                    command.Parameters.Add("@nombreGarante", MySqlDbType.VarChar).Value = a.NombreCompletoGarante;
                    command.Parameters.Add("@telGarante", MySqlDbType.VarChar).Value = a.TelefonoGarante;
                    command.Parameters.Add("@emailGarante", MySqlDbType.VarChar).Value = a.EmailGarante;
                    command.Parameters.Add("@inquilinoId", MySqlDbType.Int32).Value = a.InquilinoId;
                    command.Parameters.Add("@inmuebleId", MySqlDbType.Int32).Value = a.InmuebleId;
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    res = command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            return res;
        }

        public IList<Contrato> ObtenerTodos()
        {
            IList<Contrato> res = new List<Contrato>();
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = $"SELECT contrato.Id, FechaInicio, FechaFin, Importe, DniGarante, " +
                    $" NombreCompletoGarante, TelefonoGarante, EmailGarante, InquilinoId, InmuebleId, " +
                    $" inquilinos.Nombre, inquilinos.Apellido, inmuebles.Direccion " +
                    $" FROM contrato JOIN inquilinos ON(inquilinos.Id = contrato.InquilinoId) " +
                    $"JOIN inmuebles ON(inmuebles.Id = contrato.InmuebleId)";

                using (var command = new MySqlCommand(sql, connection))
                {
                   
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Contrato a = new Contrato
                        {
                            Id = reader.GetInt32(0),
                            FechaInicio = reader.GetDateTime(1),
                            FechaFin = reader.GetDateTime(2),
                            Importe = reader.GetDecimal(3),
                            DniGarante = reader.GetString(4),
                            NombreCompletoGarante = reader.GetString(5),
                            TelefonoGarante = reader.GetString(6),
                            EmailGarante = reader.GetString(7),
                            Inquilino = new Inquilino
                            {
                                Id = reader.GetInt32(8),
                                Nombre = reader.GetString(10),
                                Apellido = reader.GetString(11),
                            },
                            Inmueble = new Inmueble
                            {
                                Id = reader.GetInt32(9),
                                Direccion = reader.GetString(12)
                            }
                        };
                        res.Add(a);
                    }
                    connection.Close();
                }
            }
            return res;
        }

        public IList<Contrato> ObtenerVigentesVencidosPorInmuebleId(int id)
        {
            IList<Contrato> res = new List<Contrato>();
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = $"SELECT contrato.Id, FechaInicio, FechaFin, Importe, DniGarante, " +
                    $" NombreCompletoGarante, TelefonoGarante, EmailGarante, InquilinoId, InmuebleId, " +
                    $" inquilinos.Nombre, inquilinos.Apellido, inmuebles.Direccion, p.Id, p.Apellido, p.Nombre " +
                    $" FROM contrato JOIN inquilinos ON(inquilinos.Id = contrato.InquilinoId) " +
                    $" JOIN inmuebles ON(inmuebles.Id = contrato.InmuebleId) " +
                    $" JOIN propietarios p ON(p.Id = inmuebles.PropietarioId) " +
                    $" WHERE contrato.InmuebleId = @id";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.Add("@id", MySqlDbType.Int32).Value = id;
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Contrato a = new Contrato
                        {
                            Id = reader.GetInt32(0),
                            FechaInicio = reader.GetDateTime(1),
                            FechaFin = reader.GetDateTime(2),
                            Importe = reader.GetDecimal(3),
                            DniGarante = reader.GetString(4),
                            NombreCompletoGarante = reader.GetString(5),
                            TelefonoGarante = reader.GetString(6),
                            EmailGarante = reader.GetString(7),
                            Inquilino = new Inquilino
                            {
                                Id = reader.GetInt32(8),
                                Nombre = reader.GetString(10),
                                Apellido = reader.GetString(11),
                            },
                            Inmueble = new Inmueble
                            {
                                Id = reader.GetInt32(9),
                                Direccion = reader.GetString(12),
                                Duenio = new Propietario
                                {
                                    Id = reader.GetInt32(13),
                                    Apellido = reader.GetString(14),
                                    Nombre = reader.GetString(15)
                                }
                            }
                        };
                        res.Add(a);
                    }
                    connection.Close();
                }
            }
            return res;
        }

        public IList<Contrato> ObtenerTodosVigentes()
        {
            DateTime fecha = DateTime.Now;
            IList<Contrato> res = new List<Contrato>();
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = $"SELECT contrato.Id, FechaInicio, FechaFin, Importe, DniGarante, " +
                    $" NombreCompletoGarante, TelefonoGarante, EmailGarante, InquilinoId, InmuebleId, " +
                    $" inquilinos.Nombre, inquilinos.Apellido, inmuebles.Direccion " +
                    $" FROM contrato JOIN inquilinos ON(inquilinos.Id = contrato.InquilinoId)" +
                    $" JOIN inmuebles ON(inmuebles.Id = contrato.InmuebleId) WHERE FechaFin >= @fecha";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.Add("@fecha", MySqlDbType.DateTime).Value = fecha;
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Contrato a = new Contrato
                        {
                            Id = reader.GetInt32(0),
                            FechaInicio = reader.GetDateTime(1),
                            FechaFin = reader.GetDateTime(2),
                            Importe = reader.GetDecimal(3),
                            DniGarante = reader.GetString(4),
                            NombreCompletoGarante = reader.GetString(5),
                            TelefonoGarante = reader.GetString(6),
                            EmailGarante = reader.GetString(7),
                            Inquilino = new Inquilino
                            {
                                Id = reader.GetInt32(8),
                                Nombre = reader.GetString(10),
                                Apellido = reader.GetString(11),
                            },
                            Inmueble = new Inmueble
                            {
                                Id = reader.GetInt32(9),
                                Direccion = reader.GetString(12)
                            }
                        };
                        res.Add(a);
                    }
                    connection.Close();
                }
            }
            return res;
        }

        public IList<Contrato> ObtenerTodosVencidos()
        {
            DateTime fecha = DateTime.Now;
            IList<Contrato> res = new List<Contrato>();
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = $"SELECT contrato.Id, FechaInicio, FechaFin, Importe, DniGarante, " +
                    $"NombreCompletoGarante, TelefonoGarante, EmailGarante, InquilinoId, InmuebleId, " +
                    $"inquilinos.Nombre, inquilinos.Apellido, inmuebles.Direccion " +
                    $"FROM contrato JOIN inquilinos ON(inquilinos.Id = contrato.InquilinoId) " +
                    $"JOIN inmuebles ON(inmuebles.Id = contrato.InmuebleId) WHERE FechaFin < @fecha";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.Add("@fecha", MySqlDbType.DateTime).Value = fecha;
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Contrato a = new Contrato
                        {
                            Id = reader.GetInt32(0),
                            FechaInicio = reader.GetDateTime(1),
                            FechaFin = reader.GetDateTime(2),
                            Importe = reader.GetDecimal(3),
                            DniGarante = reader.GetString(4),
                            NombreCompletoGarante = reader.GetString(5),
                            TelefonoGarante = reader.GetString(6),
                            EmailGarante = reader.GetString(7),
                            Inquilino = new Inquilino
                            {
                                Id = reader.GetInt32(8),
                                Nombre = reader.GetString(10),
                                Apellido = reader.GetString(11),
                            },
                            Inmueble = new Inmueble
                            {
                                Id = reader.GetInt32(9),
                                Direccion = reader.GetString(12)
                            }
                        };
                        res.Add(a);
                    }
                    connection.Close();
                }
            }
            return res;
        }

        public IList<Contrato> ObtenerTodosPorInmuebleId(int id)
        {
            IList<Contrato> res = new List<Contrato>();
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = $"SELECT contrato.Id, FechaInicio, FechaFin, Importe, DniGarante," +
                    $" NombreCompletoGarante, TelefonoGarante, EmailGarante, InquilinoId, InmuebleId," +
                    $" p.Nombre, p.Apellido, inmuebles.Direccion, p.Id" +
                    $" FROM contrato JOIN inmuebles ON(inmuebles.Id = contrato.InmuebleId)" +
                    $" JOIN propietarios p ON(p.Id = inmuebles.PropietarioId)" +
                    $" WHERE contrato.InmuebleId = @id AND FechaFin > CURDATE()";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.Add("@id", MySqlDbType.Int32).Value = id;
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Contrato a = new Contrato
                        {
                            Id = reader.GetInt32(0),
                            FechaInicio = reader.GetDateTime(1),
                            FechaFin = reader.GetDateTime(2),
                            Importe = reader.GetDecimal(3),
                            DniGarante = reader.GetString(4),
                            NombreCompletoGarante = reader.GetString(5),
                            TelefonoGarante = reader.GetString(6),
                            EmailGarante = reader.GetString(7),
                            Inmueble = new Inmueble
                            {
                                Id = reader.GetInt32(9),
                                Direccion = reader.GetString(12),
                                Duenio = new Propietario 
                                {
                                    Id = reader.GetInt32(13),
                                    Nombre = reader.GetString(10),
                                    Apellido = reader.GetString(11),
                                }
                            }
                        };
                        res.Add(a);
                    }
                    connection.Close();
                }
            }
            return res;
        }

        public IList<Contrato> ObtenerTodosPorInmueble(int id, DateTime fi, DateTime ff)
        {
            IList<Contrato> res = new List<Contrato>();
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = $"SELECT contrato.Id, FechaInicio, FechaFin, Importe, DniGarante, " +
                    $" NombreCompletoGarante, TelefonoGarante, EmailGarante, InquilinoId, InmuebleId, " +
                    $" inquilinos.Nombre, inquilinos.Apellido, inmuebles.Direccion " +
                    $" FROM contrato JOIN inquilinos ON(inquilinos.Id = contrato.InquilinoId) " +
                    $" JOIN inmuebles ON(inmuebles.Id = contrato.InmuebleId) " +
                    $" WHERE contrato.InmuebleId = @id AND ((FechaInicio < @fi AND " +
                    $" FechaFin > @fi AND FechaFin < @ff ) " +
                    $" OR (FechaInicio > @fi AND FechaInicio < @ff " +
                    $" AND FechaFin > @ff ) OR (FechaInicio < @fi " +
                    $" AND FechaInicio < @ff AND FechaFin > @fi " +
                    $" AND FechaFin > @ff))";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.Add("@id", MySqlDbType.Int32).Value = id;
                    command.Parameters.Add("@fi", MySqlDbType.DateTime).Value = fi;
                    command.Parameters.Add("@ff", MySqlDbType.DateTime).Value = ff;
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Contrato a = new Contrato
                        {
                            Id = reader.GetInt32(0),
                            FechaInicio = reader.GetDateTime(1),
                            FechaFin = reader.GetDateTime(2),
                            Importe = reader.GetDecimal(3),
                            DniGarante = reader.GetString(4),
                            NombreCompletoGarante = reader.GetString(5),
                            TelefonoGarante = reader.GetString(6),
                            EmailGarante = reader.GetString(7),
                            Inquilino = new Inquilino
                            {
                                Id = reader.GetInt32(8),
                                Nombre = reader.GetString(10),
                                Apellido = reader.GetString(11),
                            },
                            Inmueble = new Inmueble
                            {
                                Id = reader.GetInt32(9),
                                Direccion = reader.GetString(12)
                            }
                        };
                        res.Add(a);
                    }
                    connection.Close();
                }
            }
            return res;
        }

        public Contrato ObtenerPorId(int id)
        {
            Contrato p = null;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = $"SELECT contrato.Id, FechaInicio, FechaFin, Importe, DniGarante, NombreCompletoGarante, TelefonoGarante, EmailGarante, InquilinoId, InmuebleId, inquilinos.Nombre, inquilinos.Apellido, inmuebles.Direccion FROM contrato JOIN inquilinos ON(inquilinos.Id = contrato.InquilinoId) JOIN inmuebles ON(inmuebles.Id = contrato.InmuebleId)" +
                    $" WHERE contrato.Id=@id";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.Add("@id", MySqlDbType.Int32).Value = id;
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        p = new Contrato
                        {
                            Id = reader.GetInt32(0),
                            FechaInicio = reader.GetDateTime(1),
                            FechaFin = reader.GetDateTime(2),
                            Importe = reader.GetDecimal(3),
                            DniGarante = reader.GetString(4),
                            NombreCompletoGarante = reader.GetString(5),
                            TelefonoGarante = reader.GetString(6),
                            EmailGarante = reader.GetString(7),
                            Inquilino = new Inquilino
                            {
                                Id = reader.GetInt32(8),
                                Nombre = reader.GetString(10),
                                Apellido = reader.GetString(11),
                            },
                            Inmueble = new Inmueble
                            {
                                Id = reader.GetInt32(9),
                                Direccion = reader.GetString(12)
                            },
                            InmuebleId = reader.GetInt32(9),
                            InquilinoId = reader.GetInt32(8)
                        };
                        return p;
                    }
                    connection.Close();
                }
            }
            return p;
        }
    }
}
