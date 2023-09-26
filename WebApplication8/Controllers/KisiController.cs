using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using WebApplication8.Models;

namespace WebApplication8.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KisiController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public KisiController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult GetKisi()
        {
            List<kisi> kisiler = new List<kisi>();

            try
            {
                string dbConnectionString = _configuration.GetConnectionString("kisiDbConnection");
                using (SqlConnection connection = new SqlConnection(dbConnectionString))
                {
                    connection.Open();

                    using (SqlCommand cmd = new SqlCommand("GetKisi", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                kisi kisi = new kisi
                                {
                                    PersonalId = Convert.ToInt32(reader["PersonalId"]),
                                    Name = reader["Name"].ToString(),
                                    SurName = reader["Surname"].ToString(),
                                    Email = reader["Email"].ToString(),
                                    BirthDate = reader.GetDateTime(reader.GetOrdinal("Birthdate")),
                                    Gender = reader.GetBoolean(reader.GetOrdinal("Gender"))
                                };

                                kisiler.Add(kisi);
                            }
                        }
                    }
                }
                return Ok(kisiler);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("KisiEkle")]
        public IActionResult Kayıt(kisi kisi)
        {
            try
            {
                string dbConnectionString = _configuration.GetConnectionString("kisiDbConnection");
                using (SqlConnection con = new SqlConnection(dbConnectionString))
                {
                    con.Open();

                    using (SqlCommand cmd = new SqlCommand("InsertKisibilgi", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Name", kisi.Name);
                        cmd.Parameters.AddWithValue("@Surname", kisi.SurName);
                        cmd.Parameters.AddWithValue("@Gender", kisi.Gender);
                        cmd.Parameters.AddWithValue("@Email", kisi.Email);
                        cmd.Parameters.AddWithValue("@Birthdate", kisi.BirthDate);

                        int i = cmd.ExecuteNonQuery();

                        if (i > 0)
                        {
                            return Ok("Data inserted");
                        }
                        else
                        {
                            return BadRequest("Error");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{PersonalId:int}")]
        public IActionResult DeleteKisi(int PersonalId)
        {
            try
            {
                if (PersonalId < 0)
                    throw new Exception("Personal ID 0'dan küçük olamaz");

                string dbConnectionString = _configuration.GetConnectionString("kisiDbConnection");
                using (SqlConnection connection = new SqlConnection(dbConnectionString))
                {
                    connection.Open();

                    using (SqlCommand cmd = new SqlCommand("DeleteKisi", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@PersonalId", PersonalId);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return Ok();
                        }
                        else
                        {
                            return NotFound();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

