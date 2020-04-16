using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    public class RepositorioEmpleado
    {
		private readonly string connectionString;
		private readonly IConfiguration configuration;

		public RepositorioEmpleado(IConfiguration configuration)
		{
			this.configuration = configuration;
			connectionString = configuration["ConnectionStrings:DefaultConnection"];
		}

		public int Alta(Empleado p)
		{
			int res = -1;
			using (var connection = new MySqlConnection(connectionString))
			{
				string sql = $"INSERT INTO empleados (Nombre, Apellido, Telefono, Email, Dni) " +
					$"VALUES (@nombre, @apellido, @telefono, @email, @dni);" +
					$"SELECT LAST_INSERT_ID();";//devuelve el id insertado

				using (MySqlCommand command = new MySqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@nombre", p.Nombre);
					command.Parameters.AddWithValue("@apellido", p.Apellido);
					command.Parameters.AddWithValue("@telefono", p.Telefono);
					command.Parameters.AddWithValue("@email", p.Email);
					command.Parameters.AddWithValue("@dni", p.Dni);
					connection.Open();
					res = Convert.ToInt32(command.ExecuteScalar());
					p.Id = res;
					connection.Close();
				}

				
			}
			return res;
		}

		public IList<Empleado> ObtenerTodos()
		{
			IList<Empleado> res = new List<Empleado>();
			using (var connection = new MySqlConnection(connectionString))
			{
				string sql = $"SELECT Id, Nombre, Apellido, Telefono, Email, Dni" +
					$" FROM empleados" +
					$" ORDER BY Nombre";
				using (var command = new MySqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					while (reader.Read())
					{
						Empleado p = new Empleado
						{
							Id = reader.GetInt32(0),
							Nombre = reader.GetString(1),
							Apellido = reader.GetString(2),
							Telefono = reader.GetString(3),
							Email = reader.GetString(4),
							Dni = reader.GetString(5),
						};
						res.Add(p);
					}
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
				string sql = $"DELETE FROM empleados WHERE Id = @id";
				using (var command = new MySqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@id", id);
					connection.Open();
					res = command.ExecuteNonQuery();
					connection.Close();
				}
			}
			return res;
		}

		public int Modificacion(Empleado p)
		{
			int res = -1;
			using (var connection = new MySqlConnection(connectionString))
			{
				string sql = $"UPDATE empleados SET Nombre=@nombre, Apellido=@apellido, Telefono=@telefono, Email=@email, Dni=@dni " +
					$"WHERE Id = @id";
				using (var command = new MySqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@apellido", p.Apellido);
					command.Parameters.AddWithValue("@nombre", p.Nombre);
					command.Parameters.AddWithValue("@telefono", p.Telefono);
					command.Parameters.AddWithValue("@email", p.Email);
					command.Parameters.AddWithValue("@id", p.Id);
					command.Parameters.AddWithValue("@dni", p.Dni);
					connection.Open();
					res = command.ExecuteNonQuery();
					connection.Close();
				}

			}
			return res;
		}

		public Empleado ObtenerPorEmail(string email)
		{
			Empleado p = null;
			using (var connection = new MySqlConnection(connectionString))
			{
				string sql = $"SELECT Id, Nombre, Apellido, Telefono, Email, Dni FROM empleados" +
					$" WHERE Email=@email";
				using (var command = new MySqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					command.Parameters.Add("@email", MySqlDbType.VarChar).Value = email;
					connection.Open();
					var reader = command.ExecuteReader();
					if (reader.Read())
					{
						p = new Empleado
						{
							Id = reader.GetInt32(0),
							Nombre = reader.GetString(1),
							Apellido = reader.GetString(2),
							Telefono = reader.GetString(3),
							Email = reader.GetString(4),
							Dni = reader.GetString(5),
						};
					}
					connection.Close();
				}
			}
			return p;
		}

		public Empleado ObtenerPorId(int id)
		{
			Empleado p = null;
			using (var connection = new MySqlConnection(connectionString))
			{
				string sql = $"SELECT Id, Nombre, Apellido, Telefono, Email, Dni FROM empleados" +
					$" WHERE Id=@id";
				using (var command = new MySqlCommand(sql, connection))
				{
					command.Parameters.Add("@id", MySqlDbType.Int32).Value = id;
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					if (reader.Read())
					{
						p = new Empleado
						{
							Id = reader.GetInt32(0),
							Nombre = reader.GetString(1),
							Apellido = reader.GetString(2),
							Telefono = reader.GetString(3),
							Email = reader.GetString(4),
							Dni = reader.GetString(5),
						};
					}
					connection.Close();
				}
			}
			return p;
		}

	}
}
