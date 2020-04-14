﻿using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    public class RepositorioPago
    {
        private readonly string connectionString;
        private readonly IConfiguration configuration;

        public RepositorioPago(IConfiguration configuration)
        {
            this.configuration = configuration;
            connectionString = configuration["ConnectionStrings:DefaultConnection"];
        }

        public int Alta(Pago p)
        {
            int res = -1;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = $"INSERT INTO Pago (NroPago, Fecha, Importe, ContratoId) " +
                    $"VALUES (@nroPago, @fecha, @importe, @contratoId)" +
                    $"SELECT LAST_INSERT_ID();"; ;

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@nroPago", p.NroPago);
                    command.Parameters.AddWithValue("@fecha", p.Fecha);
                    command.Parameters.AddWithValue("@importe", p.Importe);
                    command.Parameters.AddWithValue("@contratoId", p.ContratoId);
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
                string sql = $"DELETE FROM Pago WHERE Id = {id}";
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

        public int Modificacion(Pago pago)
        {
            int res = -1;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = $"UPDATE Pago SET Importe=@importe " +
                    $"WHERE Id = {pago.Id}";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.Add("@importe", MySqlDbType.Decimal).Value = pago.Importe;
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    res = command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            return res;
        }

        public IList<Pago> ObtenerTodos()
        {
            IList<Pago> res = new List<Pago>();
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = $"SELECT Pago.Id, NroPago, Pago.ContratoId, Contrato.InmuebleId, Inmueble.Direccion, Contrato.InquilinoId, Inquilino.Nombre, Inquilino.Apellido, Fecha, Pago.Importe FROM Pago INNER JOIN Contrato ON (Contrato.Id=Pago.Id) INNER JOIN Inmueble ON (Inmueble.Id = Contrato.Id) INNER JOIN Inquilino ON (Inquilino.Id = Contrato.Id)";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Pago i = new Pago
                        {
                            Id = reader.GetInt32(0),
                            NroPago = reader.GetInt32(1),
                            Contrato = new Contrato
                            {
                                Id = reader.GetInt32(2),
                                Inmueble = new Inmueble
                                {
                                    Id = reader.GetInt32(3),
                                    Direccion = reader.GetString(4),
                                },
                                Inquilino = new Inquilino
                                {
                                    Id = reader.GetInt32(5),
                                    Nombre = reader.GetString(6),
                                    Apellido = reader.GetString(7)
                                }
                            },
                            Fecha = reader.GetString(8),
                            Importe = reader.GetDecimal(9)
                        };
                        res.Add(i);
                    }
                    connection.Close();
                }
            }
            return res;
        }

        public Pago ObtenerPorId(int id)
        {
            Pago p = null;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = $"SELECT IdPago, NroPago, Pago.ContratoId, Contrato.InmuebleId, Inmueble.Direccion, Contrato.InquilinoId, Inquilino.Nombre, Inquilino.Apellido, Fecha, Pago.Importe FROM Pago INNER JOIN Contrato ON (Contrato.Id=Pago.ContratoId) INNER JOIN Inmueble ON (Inmueble.Id = Contrato.InmuebleId) INNER JOIN Inquilino ON (Inquilino.Id = Contrato.InquilinoId) WHERE Pago.Id=@id";
                
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.Add("@id", MySqlDbType.Int32).Value = id;
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        p = new Pago
                        {
                            Id = reader.GetInt32(0),
                            NroPago = reader.GetInt32(1),
                            Contrato = new Contrato
                            {
                                Id = reader.GetInt32(2),
                                Inmueble = new Inmueble
                                {
                                    Id = reader.GetInt32(3),
                                    Direccion = reader.GetString(4),
                                },
                                Inquilino = new Inquilino
                                {
                                    Id = reader.GetInt32(5),
                                    Nombre = reader.GetString(6),
                                    Apellido = reader.GetString(7)
                                }
                            },
                            Fecha = reader.GetString(8),
                            Importe = reader.GetDecimal(9)
                        };
                    }
                    connection.Close();
                }
            }
            return p;
        }
    }
}