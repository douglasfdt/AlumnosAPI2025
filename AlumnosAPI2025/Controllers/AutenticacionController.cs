using AlumnosAPI2025.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AlumnosAPI2025.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AutenticacionController : ControllerBase
    {

        private readonly string secretKey;
        private readonly string cadenaSQL;

        public AutenticacionController(IConfiguration config)
        {
            secretKey = config.GetSection("settings").GetSection("secretKey").ToString();
            cadenaSQL = config.GetConnectionString("Connection");
        }

        [HttpPost]
        [Route("Validar")]
        public IActionResult Validar([FromBody] Usuario request)
        {
            try
            {
                using (var conexion = new SqlConnection(cadenaSQL))

                //if (request.correo == "c@gmail.com" && request.DNI == 123)
                {
                    conexion.Open();

                    // Llamamos al procedimiento almacenado para validar el correo y el DNI
                    var cmd = new SqlCommand("ValidarUsuario", conexion);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Correo", request.correo);
                    cmd.Parameters.AddWithValue("@DNI", request.DNI);

                    // Ejecutamos el procedimiento y obtenemos el resultado
                    var resultado = cmd.ExecuteScalar();

                    if (Convert.ToInt32(resultado) == 1)
                    {
                        var keyBytes = Encoding.ASCII.GetBytes(secretKey);
                        var claims = new ClaimsIdentity();
                        claims.AddClaim(new Claim(ClaimTypes.NameIdentifier, request.correo));

                        var tokenDescriptor = new SecurityTokenDescriptor
                        {
                            Subject = claims,
                            Expires = DateTime.UtcNow.AddMinutes(10),
                            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256Signature)
                        };

                        var tokenHandler = new JwtSecurityTokenHandler();
                        var tokenConfig = tokenHandler.CreateToken(tokenDescriptor);

                        string tokencreado = tokenHandler.WriteToken(tokenConfig);


                        return StatusCode(StatusCodes.Status200OK, new { token = tokencreado });

                    }
                    else
                    {

                        return StatusCode(StatusCodes.Status401Unauthorized, new { token = "" });
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
    
