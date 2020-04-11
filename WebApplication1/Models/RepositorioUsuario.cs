using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    public class RepositorioUsuario
    {
		private readonly string connectionString;
		private readonly IConfiguration configuration;

		public RepositorioUsuario(IConfiguration configuration)
		{
			this.configuration = configuration;
			connectionString = configuration["ConnectionStrings:DefaultConnection"];
		}

		public int Alta(Usuario p)
		{
			int res = -1;
			using (var connection = new MySqlConnection(connectionString))
			{
				string sql = $"INSERT INTO usuarios (Email, Clave, RolId) " +
					$"VALUES (@email, @clave, @rol);" +
					$"SELECT LAST_INSERT_ID();";//devuelve el id insertado
				using (MySqlCommand command = new MySqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@email", p.Email);
					command.Parameters.AddWithValue("@clave", p.Clave);
					command.Parameters.AddWithValue("@rol", p.RolId);
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
				string sql = $"DELETE FROM usuarios WHERE Id = @id";
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
		public int Modificacion(Usuario p)
		{
			int res = -1;
			using (var connection = new MySqlConnection(connectionString))
			{
				string sql = $"UPDATE usuarios SET Email=@email, Clave=@clave, RolId=@rol " +
					$"WHERE Id = @id";
				using (var command = new MySqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@email", p.Email);
					command.Parameters.AddWithValue("@clave", p.Clave);
					command.Parameters.AddWithValue("@rol", p.RolId);
					connection.Open();
					res = Convert.ToInt32(command.ExecuteScalar());
					p.Id = res;
					connection.Close();
				}
			}
			return res;
		}

		public IList<Usuario> ObtenerTodos()
		{
			IList<Usuario> res = new List<Usuario>();
			using (var connection = new MySqlConnection(connectionString))
			{
				string sql = $"SELECT u.Id, Email, Clave, RolId, r.Id, r.Rol" +
					$" FROM usuarios u JOIN roles r ON(u.RolId = r.Id)" +
					$" ORDER BY u.Id";
				using (var command = new MySqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					while (reader.Read())
					{
						Usuario p = new Usuario
						{
							Id = reader.GetInt32(0),
							Email = reader.GetString(1),
							Clave = reader.GetString(2),
							RolId = reader.GetInt32(3),
							TipoUsuario = new TipoUsuario
							{
								Id = reader.GetInt32(4),
								Rol = reader.GetString(5),
							}
						};
						res.Add(p);
					}
					connection.Close();
				}
			}
			return res;
		}

		public Usuario ObtenerPorId(int id)
		{
			Usuario p = null;
			using (var connection = new MySqlConnection(connectionString))
			{
				string sql = $"SELECT u.Id, Email, Clave, RolId, r.Id, r.Rol FROM usuarios u JOIN roles r ON(u.RolId = r.Id)" +
					$" WHERE u.Id=@id";
				using (var command = new MySqlCommand(sql, connection))
				{
					command.Parameters.Add("@id", MySqlDbType.Int32).Value = id;
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					if (reader.Read())
					{
						p = new Usuario
						{
							Id = reader.GetInt32(0),
							Email = reader.GetString(1),
							Clave = reader.GetString(2),
							RolId = reader.GetInt32(3),
							TipoUsuario = new TipoUsuario
							{
								Id = reader.GetInt32(4),
								Rol = reader.GetString(5),
							}
						};
					}
					connection.Close();
				}
			}
			return p;
		}

		public Usuario ObtenerPorEmail(string email)
		{
			Usuario p = null;
			using (var connection = new MySqlConnection(connectionString))
			{
				string sql = $"SELECT u.Id, Email, Clave, RolId, r.Id, r.Rol FROM usuarios u JOIN roles r ON(u.RolId = r.Id)" +
					$" WHERE Email=@email";
				using (var command = new MySqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					command.Parameters.Add("@email", MySqlDbType.VarChar).Value = email;
					connection.Open();
					var reader = command.ExecuteReader();
					if (reader.Read())
					{
						p = new Usuario
						{
							Id = reader.GetInt32(0),
							Email = reader.GetString(1),
							Clave = reader.GetString(2),
							RolId = reader.GetInt32(3),
							TipoUsuario = new TipoUsuario
							{
								Id = reader.GetInt32(4),
								Rol = reader.GetString(5),
							}
						};
					}
					connection.Close();
				}
			}
			return p;
		}

		public IList<TipoUsuario> ObtenerTiposUsuario()
		{
			IList<TipoUsuario> res = new List<TipoUsuario>();
			using (var connection = new MySqlConnection(connectionString))
			{
				string sql = $"SELECT Id, Rol FROM roles";
				using (var command = new MySqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					while (reader.Read())
					{
						TipoUsuario p = new TipoUsuario
						{
							Id = reader.GetInt32(0),
							Rol = reader.GetString(1),
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
