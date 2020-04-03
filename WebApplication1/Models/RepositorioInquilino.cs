using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    public class RepositorioInquilino
    {
		private readonly string connectionString;
		private readonly IConfiguration configuration;

		public RepositorioInquilino(IConfiguration configuration)
		{
			this.configuration = configuration;
			connectionString = configuration["ConnectionStrings:DefaultConnection"];
		}

		public int Alta(Inquilino p)
		{
			int res = -1;
			using (var connection = new MySqlConnection(connectionString))
			{
				string sql = $"INSERT INTO inquilinos (Nombre, Apellido, Dni, Telefono, Email, DireccionTrabajo, DniGarante, NombreCompletoGarante, TelefonoGarante, EmailGarante) " +
					$"VALUES ('{p.Nombre}', '{p.Apellido}','{p.Dni}','{p.Telefono}','{p.Email}', '{p.DireccionTrabajo}', '{p.DniGarante}','{p.NombreCompletoGarante}','{p.TelefonoGarante}','{p.EmailGarante}')";
				using (var command = new MySqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					connection.Open();
					res = command.ExecuteNonQuery();
					command.CommandText = "SELECT LAST_INSERT_ID()";
					p.Id = Convert.ToInt32(command.ExecuteScalar());
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
				string sql = $"DELETE FROM inquilinos WHERE Id = {id}";
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
		public int Modificacion(Inquilino p)
		{
			int res = -1;
			using (var connection = new MySqlConnection(connectionString))
			{
				string sql = $"UPDATE inquilinos SET Nombre='{p.Nombre}', Apellido='{p.Apellido}', Dni='{p.Dni}', Telefono='{p.Telefono}', Email='{p.Email}', DireccionTrabajo='{p.DireccionTrabajo}', DniGarante='{p.DniGarante}', NombreCompletoGarante='{p.NombreCompletoGarante}', TelefonoGarante='{p.TelefonoGarante}', EmailGarante='{p.EmailGarante}' " +
					$"WHERE Id = {p.Id}";
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

		public IList<Inquilino> ObtenerTodos()
		{
			IList<Inquilino> res = new List<Inquilino>();
			using (var connection = new MySqlConnection(connectionString))
			{
				string sql = $"SELECT Id, Nombre, Apellido, Dni, Telefono, Email, DireccionTrabajo, DniGarante, NombreCompletoGarante, TelefonoGarante, EmailGarante" +
					$" FROM inquilinos";
				using (var command = new MySqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					while (reader.Read())
					{
						Inquilino p = new Inquilino
						{
							Id = reader.GetInt32(0),
							Nombre = reader.GetString(1),
							Apellido = reader.GetString(2),
							Dni = reader.GetString(3),
							Telefono = reader.GetString(4),
							Email = reader.GetString(5),
							DireccionTrabajo = reader.GetString(6),
							DniGarante = reader.GetString(7),
							NombreCompletoGarante = reader.GetString(8),
							TelefonoGarante = reader.GetString(9),
							EmailGarante = reader.GetString(10),
						};
						res.Add(p);
					}
					connection.Close();
				}
			}
			return res;
		}

		public Inquilino ObtenerPorId(int id)
		{
			Inquilino p = null;
			using (var connection = new MySqlConnection(connectionString))
			{
				string sql = $"SELECT Id, Nombre, Apellido, Dni, Telefono, Email, DireccionTrabajo, DniGarante, NombreCompletoGarante, TelefonoGarante, EmailGarante FROM inquilinos" +
					$" WHERE Id=@id";
				using (var command = new MySqlCommand(sql, connection))
				{
					command.Parameters.Add("@id", MySqlDbType.Int32).Value = id;
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					while (reader.Read())
					{
						p = new Inquilino
						{
							Id = reader.GetInt32(0),
							Nombre = reader.GetString(1),
							Apellido = reader.GetString(2),
							Dni = reader.GetString(3),
							Telefono = reader.GetString(4),
							Email = reader.GetString(5),
							DireccionTrabajo = reader.GetString(6),
							DniGarante = reader.GetString(7),
							NombreCompletoGarante = reader.GetString(8),
							TelefonoGarante = reader.GetString(9),
							EmailGarante = reader.GetString(10),
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

