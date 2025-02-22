using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AlumnosAPI2025.Model;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Authorization;

namespace AlumnosAPI2025.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class AlumnoController : ControllerBase
    {
        private readonly  string cadenaSQL;


        public AlumnoController(IConfiguration config) {
            cadenaSQL = config.GetConnectionString("Connection");           
        }

        [HttpGet]
        [Route("Lista")]
        public IActionResult Lista()
        {
            List<Alumno> lista = new List<Alumno>();
            try 
            {
                using (var conexion = new SqlConnection(cadenaSQL)) {
                    conexion.Open();
                    var cmd = new SqlCommand("ListarAlumnos", conexion);
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (var rd = cmd.ExecuteReader()) 
                    {
                        while (rd.Read()) 
                        {
                            lista.Add(new Alumno
                            {
                                Id = Convert.ToInt32(rd["Id"]),
                                Nombre = rd["Nombre"].ToString(),
                                Apellidos = rd["Apellidos"].ToString(),
                                DNI = Convert.ToInt64(rd["DNI"]),
                                Login = rd["Login"].ToString(),
                                Genero = rd["Genero"].ToString()
                            });
                        }
                    }
                }
                return StatusCode(StatusCodes.Status200OK, new { mensaje = "ok", Response = lista });
            } 
            catch (Exception Error)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = Error.Message, Response = lista });
            }
        }


        [HttpGet]
        [Route("Obtener/{DNI:long}")]
        public IActionResult Obtener(int DNI)
        {
            List<Alumno> lista = new List<Alumno>();
            Alumno alumno = new Alumno();
            try
            {
                using (var conexion = new SqlConnection(cadenaSQL))
                {
                    conexion.Open();
                    var cmd = new SqlCommand("ListarAlumnos", conexion);
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (var rd = cmd.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            lista.Add(new Alumno
                            {
                                Id = Convert.ToInt32(rd["Id"]),
                                Nombre = rd["Nombre"].ToString(),
                                Apellidos = rd["Apellidos"].ToString(),
                                DNI = Convert.ToInt64(rd["DNI"]),
                                Login = rd["Login"].ToString(),
                                Genero = rd["Genero"].ToString()
                            });
                        }
                    }
                }
                alumno = lista.Where(item => item.DNI == DNI).FirstOrDefault();
                return StatusCode(StatusCodes.Status200OK, new { mensaje = "ok", Response = alumno });
            }
            catch (Exception Error)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = Error.Message, Response = alumno });
            }
        }


        [HttpPost]
        [Route("Guardar")]
        public IActionResult Guardar([FromBody] Alumno objeto)
        {
            try
            {
                using (var conexion = new SqlConnection(cadenaSQL))
                {
                    conexion.Open();
                    var cmd = new SqlCommand("InsertarAlumno", conexion);
                    cmd.Parameters.AddWithValue("Nombre", objeto.Nombre);
                    cmd.Parameters.AddWithValue("Apellidos", objeto.Apellidos);
                    cmd.Parameters.AddWithValue("DNI", objeto.DNI);
                    cmd.Parameters.AddWithValue("Telefono", objeto.Telefono);
                    cmd.Parameters.AddWithValue("Correo", objeto.Correo);
                    cmd.Parameters.AddWithValue("Login", objeto.Login);
                    cmd.Parameters.AddWithValue("Genero", objeto.Genero);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.ExecuteNonQuery();

                    
                }
            
                return StatusCode(StatusCodes.Status200OK, new { mensaje = "ok"});
            }
            catch (Exception Error)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = Error.Message });
            }
        }

        [HttpPut]
        [Route("Editar")]
        public IActionResult Editar([FromBody] Alumno objeto)
        {
            try
            {
                using (var conexion = new SqlConnection(cadenaSQL))
                {
                    conexion.Open();
                    var cmd = new SqlCommand("ActualizarAlumno", conexion);
                    cmd.Parameters.AddWithValue("ID", objeto.Id == 0 ? DBNull.Value : objeto.Id);
                    cmd.Parameters.AddWithValue("Nombre", objeto.Nombre is null ? DBNull.Value : objeto.Nombre);
                    cmd.Parameters.AddWithValue("Apellidos", objeto.Apellidos is null ? DBNull.Value : objeto.Apellidos);
                    cmd.Parameters.AddWithValue("DNI", objeto.DNI == 0 ? DBNull.Value : objeto.DNI);
                    cmd.Parameters.AddWithValue("Telefono", objeto.Telefono == 0 ? DBNull.Value : objeto.Telefono);
                    cmd.Parameters.AddWithValue("Correo", objeto.Correo is null ? DBNull.Value : objeto.Correo);
                    cmd.Parameters.AddWithValue("Login", objeto.Login is null ? DBNull.Value : objeto.Login);
                    cmd.Parameters.AddWithValue("Genero", objeto.Genero is null ? DBNull.Value : objeto.Genero);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.ExecuteNonQuery();


                }

                return StatusCode(StatusCodes.Status200OK, new { mensaje = "editado" });
            }
            catch (Exception Error)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = Error.Message });
            }
        }

        [HttpDelete]
        [Route("Eliminar/{id:int}")]
        public IActionResult Eliminar(int id)
        {
            try
            {
                using (var conexion = new SqlConnection(cadenaSQL))
                {
                    conexion.Open();
                    var cmd = new SqlCommand("EliminarAlumno", conexion);
                    cmd.Parameters.AddWithValue("ID", id);

                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.ExecuteNonQuery();


                }

                return StatusCode(StatusCodes.Status200OK, new { mensaje = "Eliminado" });
            }
            catch (Exception Error)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = Error.Message });
            }
        }

    }
}
