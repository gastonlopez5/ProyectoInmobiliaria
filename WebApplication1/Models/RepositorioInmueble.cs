using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    public class RepositorioInmueble
    {
		private readonly string connectionString;
		private readonly IConfiguration configuration;

		public RepositorioInmueble(IConfiguration configuration) 
		{
			this.configuration = configuration;
			connectionString = configuration["ConnectionStrings:DefaultConnection"];
		}

		public int Alta(Inmueble p)
		{
			int res = -1;
			using (var connection = new MySqlConnection(connectionString))
			{
				string sql = "INSERT INTO Inmuebles (Direccion, Tipo, Uso, Ambientes, Costo, Disponible, PropietarioId) " +
					"VALUES (@direccion, @tipo, @uso, @ambientes, @costo, @disponible, @propietarioId);" +
					"SELECT LAST_INSERT_ID();";//devuelve el id insertado (LAST_INSERT_ID para mysql)
				using (var command = new MySqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@direccion", p.Direccion);
					command.Parameters.AddWithValue("@tipo", p.Tipo);
					command.Parameters.AddWithValue("@uso", p.Uso);
					command.Parameters.AddWithValue("@ambientes", p.Ambientes);
					command.Parameters.AddWithValue("@costo", p.Costo);
					command.Parameters.AddWithValue("@disponible", p.Disponible);
					command.Parameters.AddWithValue("@propietarioId", p.PropietarioId);
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
				string sql = $"DELETE FROM inmuebles WHERE Id = {id}";
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
		public int Modificacion(Inmueble p)
		{
			int res = -1;
			using (var connection = new MySqlConnection(connectionString))
			{
				string sql = $"UPDATE inmuebles SET Direccion=@direccion,  Tipo=@tipo, Uso=@uso, Ambientes=@ambientes, Costo=@costo, Disponible=@disponible, PropietarioId=@propietarioId " +
					$"WHERE Id = {p.Id}";
				using (var command = new MySqlCommand(sql, connection))
				{
					command.Parameters.Add("@direccion", MySqlDbType.VarChar).Value = p.Direccion;
					command.Parameters.Add("@tipo", MySqlDbType.Int32).Value = p.Tipo;
					command.Parameters.Add("@uso", MySqlDbType.VarChar).Value = p.Uso;
					command.Parameters.Add("@ambientes", MySqlDbType.Int32).Value = p.Ambientes;
					command.Parameters.Add("@costo", MySqlDbType.Decimal).Value = p.Costo;
					command.Parameters.Add("@disponible", MySqlDbType.Int32).Value = p.Disponible;
					command.Parameters.Add("@propietarioId", MySqlDbType.Int32).Value = p.PropietarioId;
					command.CommandType = CommandType.Text;
					connection.Open();
					res = command.ExecuteNonQuery();
					connection.Close();
				}
			}
			return res;
		}

		public int CambioDisponible(int p, string resp)
		{
			int res = -1;
			using (var connection = new MySqlConnection(connectionString))
			{
				string sql = $"UPDATE inmuebles SET Disponible=@disponible WHERE Id =@id";

				using (var command = new MySqlCommand(sql, connection))
				{
					command.Parameters.Add("@disponible", MySqlDbType.VarChar).Value = resp;
					command.Parameters.Add("@id", MySqlDbType.Int32).Value = p;
					command.CommandType = CommandType.Text;
					connection.Open();
					res = command.ExecuteNonQuery();
					connection.Close();
				}
			}
			return res;
		}

		public IList<Inmueble> ObtenerTodos()
		{
			IList<Inmueble> res = new List<Inmueble>();
			using (var connection = new MySqlConnection(connectionString))
			{
				string sql = $"SELECT i.Id, Direccion, i.Tipo, Uso, Ambientes, Costo, Disponible, PropietarioId, p.Nombre, p.apellido, t.Id, t.Tipo " +
					$" FROM inmuebles i JOIN propietarios p ON(i.PropietarioId = p.Id)" +
					$" JOIN tipoinmueble t ON(i.Tipo = t.Id)";
				using (var command = new MySqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					while (reader.Read())
					{
						Inmueble p = new Inmueble
						{
							Id = reader.GetInt32(0),
							Direccion = reader.GetString(1),
							Tipo = reader.GetInt32(2),
							Uso = reader.GetString(3),
							Ambientes = reader.GetInt32(4),
							Costo = reader.GetDecimal(5),
							Disponible = reader.GetBoolean(6),
							PropietarioId = reader.GetInt32(7),
							Duenio = new Propietario
							{
								Id = reader.GetInt32(7),
								Nombre = reader.GetString(8),
								Apellido = reader.GetString(9),
							},
							TipoInmueble = new TipoInmueble
							{
								Id = reader.GetInt32(10),
								Tipo = reader.GetString(11),
							}
						};
						res.Add(p);
					}
					connection.Close();
				}
			}
			return res;
		}

		public IList<Inmueble> ObtenerTodosDisponibles()
		{
			IList<Inmueble> res = new List<Inmueble>();
			using (var connection = new MySqlConnection(connectionString))
			{
				string sql = $"SELECT i.Id, Direccion, i.Tipo, Uso, Ambientes, Costo, Disponible, PropietarioId, p.Nombre, p.apellido, t.Id, t.Tipo " +
					$" FROM inmuebles i JOIN propietarios p ON(i.PropietarioId = p.Id)" +
					$" JOIN tipoinmueble t ON(i.Tipo = t.Id)" +
					$" WHERE Disponible = true";

				using (var command = new MySqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					while (reader.Read())
					{
						Inmueble p = new Inmueble
						{
							Id = reader.GetInt32(0),
							Direccion = reader.GetString(1),
							Tipo = reader.GetInt32(2),
							Uso = reader.GetString(3),
							Ambientes = reader.GetInt32(4),
							Costo = reader.GetDecimal(5),
							Disponible = reader.GetBoolean(6),
							PropietarioId = reader.GetInt32(7),
							Duenio = new Propietario
							{
								Id = reader.GetInt32(7),
								Nombre = reader.GetString(8),
								Apellido = reader.GetString(9),
							},
							TipoInmueble = new TipoInmueble
							{
								Id = reader.GetInt32(10),
								Tipo = reader.GetString(11),
							}
						};
						res.Add(p);
					}
					connection.Close();
				}
			}
			return res;
		}

		public IList<Inmueble> ObtenerTodosNoDisponibles()
		{
			IList<Inmueble> res = new List<Inmueble>();
			using (var connection = new MySqlConnection(connectionString))
			{
				string sql = $"SELECT i.Id, Direccion, i.Tipo, Uso, Ambientes, Costo, Disponible, PropietarioId, p.Nombre, p.apellido, t.Id, t.Tipo " +
					$" FROM inmuebles i JOIN propietarios p ON(i.PropietarioId = p.Id)" +
					$" JOIN tipoinmueble t ON(i.Tipo = t.Id)" +
					$" WHERE Disponible = false";

				using (var command = new MySqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					while (reader.Read())
					{
						Inmueble p = new Inmueble
						{
							Id = reader.GetInt32(0),
							Direccion = reader.GetString(1),
							Tipo = reader.GetInt32(2),
							Uso = reader.GetString(3),
							Ambientes = reader.GetInt32(4),
							Costo = reader.GetDecimal(5),
							Disponible = reader.GetBoolean(6),
							PropietarioId = reader.GetInt32(7),
							Duenio = new Propietario
							{
								Id = reader.GetInt32(7),
								Nombre = reader.GetString(8),
								Apellido = reader.GetString(9),
							},
							TipoInmueble = new TipoInmueble
							{
								Id = reader.GetInt32(10),
								Tipo = reader.GetString(11),
							}
						};
						res.Add(p);
					}
					connection.Close();
				}
			}
			return res;
		}

		public IList<TipoInmueble> ObtenerTodosTipos()
		{
			IList<TipoInmueble> res = new List<TipoInmueble>();
			using (var connection = new MySqlConnection(connectionString))
			{
				string sql = $"SELECT Id, Tipo FROM tipoinmueble";
				using (var command = new MySqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					while (reader.Read())
					{
						TipoInmueble p = new TipoInmueble
						{
							Id = reader.GetInt32(0),
							Tipo = reader.GetString(1),
						};
						res.Add(p);
					}
					connection.Close();
				}
			}
			return res;
		}

		public Inmueble ObtenerPorId(int id)
		{
			Inmueble entidad = null;
			using (var connection = new MySqlConnection(connectionString))
			{
				string sql = $"SELECT i.Id, Direccion, i.Tipo, Uso, Ambientes, Costo, Disponible, PropietarioId, p.Nombre, p.Apellido, t.Id, t.Tipo " +
					$" FROM inmuebles i JOIN propietarios p ON(i.PropietarioId = p.Id)" +
					$" JOIN tipoinmueble t ON(i.Tipo = t.Id)" +
					$" AND i.Id=@id";
				using (var command = new MySqlCommand(sql, connection))
				{
					command.Parameters.Add("@id", MySqlDbType.Int32).Value = id;
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					if (reader.Read())
					{
						entidad = new Inmueble
						{
							Id = reader.GetInt32(0),
							Direccion = reader.GetString(1),
							Tipo = reader.GetInt32(2),
							Uso = reader.GetString(3),
							Ambientes = reader.GetInt32(4),
							Costo = reader.GetDecimal(5),
							Disponible = reader.GetBoolean(6),
							PropietarioId = reader.GetInt32(7),
							Duenio = new Propietario
							{
								Id = reader.GetInt32(7),
								Nombre = reader.GetString(8),
								Apellido = reader.GetString(9),
							},
							TipoInmueble = new TipoInmueble
							{
								Id = reader.GetInt32(10),
								Tipo = reader.GetString(11),
							}
						};
					}
					connection.Close();
				}
			}
			return entidad;
		}

		public IList<Inmueble> BuscarPorPropietario(int idPropietario)
		{
			List<Inmueble> res = new List<Inmueble>();
			Inmueble p = null;
			using (var connection = new MySqlConnection(connectionString))
			{
				string sql = $"SELECT i.Id, Direccion, i.Tipo, Uso, Ambientes, Costo, Disponible, PropietarioId, p.Nombre, p.Apellido, t.Id, t.Tipo " +
					$" FROM Inmuebles i INNER JOIN Propietarios p ON i.PropietarioId = p.Id JOIN tipoinmueble t ON(i.Tipo = t.Id) " +
					$" WHERE PropietarioId=@idPropietario";
				using (var command = new MySqlCommand(sql, connection))
				{
					command.Parameters.Add("@idPropietario", MySqlDbType.Int32).Value = idPropietario;
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					while (reader.Read())
					{
						p = new Inmueble
						{
							Id = reader.GetInt32(0),
							Direccion = reader.GetString(1),
							Tipo = reader.GetInt32(2),
							Uso = reader.GetString(3),
							Ambientes = reader.GetInt32(4),
							Costo = reader.GetDecimal(5),
							Disponible = reader.GetBoolean(6),
							PropietarioId = reader.GetInt32(7),
							TipoInmueble = new TipoInmueble
							{
								Id = reader.GetInt32(10),
								Tipo = reader.GetString(11),
							},
							Duenio = new Propietario
							{
								Id = reader.GetInt32(7),
								Nombre = reader.GetString(8),
								Apellido = reader.GetString(9),
							}
						};
						res.Add(p);
					}
					connection.Close();
				}
			}
			return res;
		}

		public IList<Inmueble> BuscarDisponibles(string uso, decimal costo)
		{
			List<Inmueble> res = new List<Inmueble>();
			Inmueble p = null;
			using (var connection = new MySqlConnection(connectionString))
			{
				string sql = $"SELECT i.Id, Direccion, Tipo, Uso, Ambientes, Costo, Disponible, PropietarioId, p.Nombre, p.Apellido " +
					$" FROM Inmuebles i INNER JOIN Propietarios p ON i.PropietarioId = p.Id" +
					$" WHERE Uso=@uso AND Costo<=@costo AND Disponible=true";
				using (var command = new MySqlCommand(sql, connection))
				{
					command.Parameters.Add("@uso", MySqlDbType.String).Value = uso;
					command.Parameters.Add("@costo", MySqlDbType.Decimal).Value = costo;
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					while (reader.Read())
					{
						p = new Inmueble
						{
							Id = reader.GetInt32(0),
							Direccion = reader.GetString(1),
							Tipo = reader.GetInt32(2),
							Uso = reader.GetString(3),
							Ambientes = reader.GetInt32(4),
							Costo = reader.GetDecimal(5),
							Disponible = reader.GetBoolean(6),
							PropietarioId = reader.GetInt32(7),
							Duenio = new Propietario
							{
								Id = reader.GetInt32(7),
								Nombre = reader.GetString(8),
								Apellido = reader.GetString(9),
							}
						};
						res.Add(p);
					}
					connection.Close();
				}
			}
			return res;
		}

		public Contrato ObtenerTodosPorInmuebleId(int id)
		{
			Contrato p = null;
			using (var connection = new MySqlConnection(connectionString))
			{
				string sql = $"SELECT contrato.Id, FechaInicio, FechaFin, Importe, DniGarante, NombreCompletoGarante, TelefonoGarante, EmailGarante, InquilinoId, InmuebleId, p.Nombre, p.Apellido, inmuebles.Direccion " +
					$"FROM contrato JOIN inmuebles ON(inmuebles.Id = contrato.InmuebleId) JOIN propietarios p ON(propietarios.Id = inmuebles.PropietarioId)" +
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

