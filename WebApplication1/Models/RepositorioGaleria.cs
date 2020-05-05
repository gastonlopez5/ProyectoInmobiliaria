using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    public class RepositorioGaleria
    {
		private readonly string connectionString;
		private readonly IConfiguration configuration;

		public RepositorioGaleria(IConfiguration configuration)
		{
			this.configuration = configuration;
			connectionString = configuration["ConnectionStrings:DefaultConnection"];
		}

		public int Alta(Galeria p)
		{
			int res = -1;
			using (var connection = new MySqlConnection(connectionString))
			{
				string sql = "INSERT INTO galeria (Ruta, InmuebleId) " +
					"VALUES (@ruta, @inmuebleId);" +
					"SELECT LAST_INSERT_ID();";//devuelve el id insertado (LAST_INSERT_ID para mysql)
				using (var command = new MySqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@ruta", p.Ruta);
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
				string sql = $"DELETE FROM galeria WHERE Id = {id}";
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
		public int Modificacion(Galeria p)
		{
			int res = -1;
			using (var connection = new MySqlConnection(connectionString))
			{
				string sql = $"UPDATE galeria SET Ruta=@ruta, InmuebleId=@inmuebleId " +
					$"WHERE Id = {p.Id}";
				using (var command = new MySqlCommand(sql, connection))
				{
					command.Parameters.Add("@ruta", MySqlDbType.VarChar).Value = p.Ruta;
					command.Parameters.Add("@inmuebleId", MySqlDbType.Int32).Value = p.InmuebleId;
					command.CommandType = CommandType.Text;
					connection.Open();
					res = command.ExecuteNonQuery();
					connection.Close();
				}
			}
			return res;
		}

		public IList<Galeria> ObtenerTodosPorInmuebleId(int id)
		{
			IList<Galeria> res = new List<Galeria>();
			using (var connection = new MySqlConnection(connectionString))
			{
				string sql = $"SELECT g.Id, g.Ruta, g.InmuebleId, i.Direccion, p.Id, p.Apellido, p.Nombre " +
					$" FROM galeria g JOIN inmuebles i ON(g.InmuebleId=i.Id) " +
					$" JOIN propietarios p ON(i.PropietarioId = p.Id) " +
					$" WHERE g.InmuebleId=@id";

				using (var command = new MySqlCommand(sql, connection))
				{
					command.Parameters.Add("@id", MySqlDbType.Int32).Value = id;
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					while (reader.Read())
					{
						Galeria g = new Galeria
						{
							Id = reader.GetInt32(0),
							Ruta = reader.GetString(1),
							InmuebleId = reader.GetInt32(2),
							Propiedad = new Inmueble
							{
								Id = reader.GetInt32(2),
								Direccion = reader.GetString(3),
								Duenio = new Propietario
								{
									Id = reader.GetInt32(4),
									Apellido = reader.GetString(5),
									Nombre = reader.GetString(6),
								}
							}
						};
						res.Add(g);
					}
					connection.Close();
				}
			}
			return res;
		}
	}
}
