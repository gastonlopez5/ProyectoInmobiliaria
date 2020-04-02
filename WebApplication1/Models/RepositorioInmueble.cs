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
				string sql = $"INSERT INTO inmuebles (Direccion, Tipo, Uso, Ambientes, Costo, Disponible, PropietarioId) " +
					$"VALUES ('{p.Direccion}', '{p.Tipo}','{p.Uso}','{p.Ambientes}','{p.Costo}', '{p.Disponible}','{p.PropietarioId}')";
				using (var command = new MySqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					connection.Open();
					res = command.ExecuteNonQuery();
					command.CommandText = "SELECT LAST_INSERT_ID()";
					var id = command.ExecuteScalar();
					p.Id = Convert.ToInt32(id);
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
				string sql = $"UPDATE Inmueble SET Disponible='{resp}' WHERE Id = {p}";
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

		public IList<Inmueble> ObtenerTodos()
		{
			IList<Inmueble> res = new List<Inmueble>();
			using (var connection = new MySqlConnection(connectionString))
			{
				string sql = $"SELECT i.Id, Direccion, Tipo, Uso, Ambientes, Costo, Disponible, PropietarioId, p.Nombre, p.apellido " +
					$" FROM inmuebles i JOIN propietarios p ON(i.PropietarioId = p.Id)";
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
							}
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
				string sql = $"SELECT i.Id, Direccion, Tipo, Uso, Ambientes, Costo, Disponible, PropietarioId, p.Nombre, p.Apellido " +
					$" FROM inmuebles i JOIN propietarios p ON(i.PropietarioId = p.Id)" +
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
				string sql = $"SELECT i.Id, Direccion, Tipo, Uso, Ambientes, Costo, Disponible, PropietarioId, p.Nombre, p.Apellido " +
					$" FROM Inmuebles i INNER JOIN Propietarios p ON i.PropietarioId = p.Id" +
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
	}
}

