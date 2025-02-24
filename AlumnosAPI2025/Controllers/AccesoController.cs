using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AlumnosAPI2025.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Data.SqlClient;
using AlumnosAPI2025.Custom;

namespace AlumnosAPI2025.Controllers
{
    [Route("api/[controller]")]
    
    [ApiController]
    public class AccesoController : ControllerBase
    {
        private readonly Utilidades _utilidades;
        private readonly AutenticacionController _autenticacionController;
        private readonly string cadenaSQL;
        public AccesoController(IConfiguration config, Utilidades utilidades)
        {
           
            _utilidades = utilidades;
            cadenaSQL = config.GetConnectionString("Connection");

        }


        [HttpPost]
        [Route("GuardarUsuario")]
        public IActionResult GuardarUsuario([FromBody] Alumno objeto)
        {
            try
            {
                using (var conexion = new SqlConnection(cadenaSQL))
                {
                    conexion.Open();
                    var cmd = new SqlCommand("InsertarAlumno2", conexion);
                    cmd.Parameters.AddWithValue("Nombre", objeto.Nombre);
                    cmd.Parameters.AddWithValue("Apellidos", objeto.Apellidos);
                    cmd.Parameters.AddWithValue("DNI", objeto.DNI);
                    cmd.Parameters.AddWithValue("Telefono", objeto.Telefono);
                    cmd.Parameters.AddWithValue("Correo", objeto.Correo);
                    cmd.Parameters.AddWithValue("Login", objeto.Login);
                    cmd.Parameters.AddWithValue("Genero", objeto.Genero);
                    cmd.Parameters.AddWithValue("Clave", _utilidades.encriptarSHA256(objeto.Clave));
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.ExecuteNonQuery();


                }

                return StatusCode(StatusCodes.Status200OK, new { mensaje = "ok" });
            }
            catch (Exception Error)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = Error.Message });
            }
        }

        [HttpPost]
        [Route("ValidarLogin")]
        public IActionResult ValidarLogin([FromBody] Login request)
        {
            try
            {
                using (var conexion = new SqlConnection(cadenaSQL))

                //if (request.correo == "c@gmail.com" && request.DNI == 123)
                {
                    conexion.Open();

                    // Llamamos al procedimiento almacenado para validar el correo y el DNI
                    var cmd = new SqlCommand("ValidarLogin", conexion);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Login", request.LoginUsuario);
                    cmd.Parameters.AddWithValue("@Clave", _utilidades.encriptarSHA256(request.Clave));

                    // Ejecutamos el procedimiento y obtenemos el resultado
                    var resultado = cmd.ExecuteScalar();

                    if (Convert.ToInt32(resultado) == 1)
                    {
                       


                        return StatusCode(StatusCodes.Status200OK, new { isSuccess = true, token = _utilidades.generarJWT(request) });

                    }
                    else
                    {

                        return StatusCode(StatusCodes.Status401Unauthorized, new { isSuccess = false, token = "" });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = ex.Message });
            }
        }

    }
}
