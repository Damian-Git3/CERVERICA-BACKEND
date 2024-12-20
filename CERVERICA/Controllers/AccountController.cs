﻿using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using CERVERICA.Dtos;
using CERVERICA.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RestSharp;
using System.Transactions;
using CERVERICA.Data;
using Bogus;
using Microsoft.AspNetCore.Rewrite;

namespace CERVERICA.Controllers
{

    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;


        public AccountController(UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        ApplicationDbContext context,
        IConfiguration configuration
        )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
            _configuration = configuration;

        }

        [AllowAnonymous]
        [HttpGet("generate-fake-users/{count}")]
        public async Task<ActionResult> GenerateFakeUsers(int count)
        {
            if (count <= 0)
            {
                return BadRequest(new
                {
                    IsSuccess = false,
                    Message = "El número de usuarios debe ser mayor que cero."
                });
            }

            try
            {
                using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var fakeUsers = new Faker<ApplicationUser>()
                        .RuleFor(u => u.Email, f => f.Internet.Email())
                        .RuleFor(u => u.FullName, f => f.Name.FullName())
                        .RuleFor(u => u.UserName, f => f.Internet.UserName())
                        .RuleFor(u => u.Activo, f => true)
                        .RuleFor(u => u.FechaRegistro, f => f.Date.Past(1));

                    var users = fakeUsers.Generate(count);
                    var result = new List<string>();

                    foreach (var user in users)
                    {
                        var identityResult = await _userManager.CreateAsync(user, "Contra1234?");

                        if (identityResult.Succeeded)
                        {
                            var roleResult = await _userManager.AddToRoleAsync(user, "Cliente");

                            if (roleResult.Succeeded)
                            {
                                result.Add($"Usuario {user.Email} creado exitosamente.");
                            }
                            else
                            {
                                result.Add($"Error al asignar rol cliente {user.Email}: {string.Join(", ", identityResult.Errors.Select(e => e.Description))}");
                            }

                        }
                        else
                        {
                            result.Add($"Error al crear usuario {user.Email}: {string.Join(", ", identityResult.Errors.Select(e => e.Description))}");
                        }
                    }

                    transaction.Complete();

                    return Ok(new
                    {
                        IsSuccess = true,
                        Message = "Usuarios generados exitosamente.",
                        Results = result
                    });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    IsSuccess = false,
                    Message = "Error al generar usuarios.",
                    Error = ex.Message
                });
            }
        }


        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<string>> Register(RegisterDto registerDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var user = new ApplicationUser
                {
                    Email = registerDto.Email,
                    FullName = registerDto.FullName,
                    UserName = registerDto.Email,
                    PhoneNumber = registerDto.Telefono,
                    Activo = true
                };

                using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var result = await _userManager.CreateAsync(user, registerDto.Password);

                    if (!result.Succeeded)
                    {
                        var errors = result.Errors.Select(e => e.Description).ToList();
                        return BadRequest(new
                        {
                            IsSuccess = false,
                            Message = "Faltan datos del registro",
                            Errors = errors
                        });
                    }

                    IdentityResult roleResult;
                    if (registerDto.Role is null)
                    {
                        roleResult = await _userManager.AddToRoleAsync(user, "Cliente");
                    }
                    else
                    {
                        roleResult = await _userManager.AddToRoleAsync(user, registerDto.Role);
                    }

                    if (!roleResult.Succeeded)
                    {
                        // Rollback user creation
                        await _userManager.DeleteAsync(user);
                        return BadRequest(new
                        {
                            IsSuccess = false,
                            Message = "El registro no se pudo realizar."
                        });
                    }

                    Notificacion notificacion = new Notificacion
                    {
                        IdUsuario = user.Id,
                        Fecha = DateTime.Now,
                        Tipo = 1,
                        Mensaje = "Bienvenido a Cerverica",
                        Visto = false
                    };

                    _context.Notificaciones.Add(notificacion);
                    await _context.SaveChangesAsync();

                    transaction.Complete();
                }

                return Ok(new
                {
                    IsSuccess = true,
                    Message = "Cuenta creada"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    IsSuccess = false,
                    Message = "El registro no se pudo realizar."
                });
            }

        }


        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login(LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByEmailAsync(loginDto.Email);

            if (user is null)
            {
                return NotFound(new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "Usuario no encontrado con este email",
                });
            }

            if (!user.Activo)
            {
                return BadRequest(new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "Tu cuenta esta inactiva. Contacta a soporte al cliente"
                });
            }

            var result = await _userManager.CheckPasswordAsync(user, loginDto.Password);

            if (!result)
            {
                return BadRequest(new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "Contraseña incorrecta"
                });
            }

            var token = GenerateToken(user);
            var refreshToken = GenerateRefreshToken();
            _ = int.TryParse(_configuration.GetSection("JWTSetting").GetSection("RefreshTokenValidityIn").Value!, out int RefreshTokenValidityIn);
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(RefreshTokenValidityIn);
            await _userManager.UpdateAsync(user);

            return Ok(new AuthResponseDto
            {
                Token = token,
                IsSuccess = true,
                Message = "Login Success.",
                RefreshToken = refreshToken,
                IdUsuario = user.Id,
                Nombre = user.FullName,
                Email = user.Email,
                Rol = (await _userManager.GetRolesAsync(user)).FirstOrDefault()
            });
        }

        [HttpGet("validate-token")]
        public async Task<ActionResult<AuthResponseDto>> ValidateToken()
        {
            try
            {
                var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                var idUsuario = HttpContext.Items["idUsuario"] as string;

                if (string.IsNullOrEmpty(idUsuario))
                {
                    return Unauthorized(new AuthResponseDto
                    {
                        IsSuccess = false,
                        Message = "Token inválido o expirado"
                    });
                }

                // Buscar el usuario por ID
                var user = await _userManager.FindByIdAsync(idUsuario);

                if (user == null)
                {
                    return NotFound(new AuthResponseDto
                    {
                        IsSuccess = false,
                        Message = "Usuario no encontrado"
                    });
                }

                if (!user.Activo)
                {
                    return BadRequest(new AuthResponseDto
                    {
                        IsSuccess = false,
                        Message = "Tu cuenta está inactiva. Contacta a soporte al cliente"
                    });
                }

                // Devolver la respuesta exitosa con la información del usuario y los tokens existentes
                return Ok(new AuthResponseDto
                {
                    Token = token,                     // Retornamos el mismo token
                    RefreshToken = user.RefreshToken,  // Retornamos el refresh token existente
                    IsSuccess = true,
                    Message = "Token válido",
                    IdUsuario = user.Id,
                    Nombre = user.FullName,
                    Email = user.Email,
                    Rol = (await _userManager.GetRolesAsync(user)).FirstOrDefault()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "Error interno del servidor al validar el token"
                });
            }
        }

        [HttpPost("logout")]
        public async Task<ActionResult<AuthResponseDto>> Logout()
        {
            var idUsuario = HttpContext.Items["idUsuario"] as string;
            var user = await _userManager.FindByIdAsync(idUsuario!);

            if (user is null)
            {
                return NotFound(new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "Usuario no encontrado"
                });
            }

            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;
            await _userManager.UpdateAsync(user);

            return Ok(new AuthResponseDto
            {
                IsSuccess = true,
                Message = "Logout Success"
            });
        }

        //Activar usuario
        [HttpPost("activar/{id}")]
        public async Task<ActionResult<AuthResponseDto>> ActivateUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user is null)
            {
                return NotFound(new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "Usuario no encontrado"
                });
            }

            user.Activo = true;
            await _userManager.UpdateAsync(user);

            Notificacion notificacion = new Notificacion
            {
                IdUsuario = user.Id,
                Fecha = DateTime.Now,
                Tipo = 1,
                Mensaje = "Tu usuario ha sido activado otra vez",
                Visto = false
            };

            _context.Notificaciones.Add(notificacion);
            await _context.SaveChangesAsync();

            return Ok(new AuthResponseDto
            {
                IsSuccess = true,
                Message = "Usuario Activado"
            });
        }

        //Desactivar usuario
        [HttpPost("desactivar/{id}")]
        public async Task<ActionResult<AuthResponseDto>> DeactivateUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user is null)
            {
                return NotFound(new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "Usuario no encontrado"
                });
            }

            user.Activo = false;
            await _userManager.UpdateAsync(user);

            Notificacion notificacion = new Notificacion
            {
                IdUsuario = user.Id,
                Fecha = DateTime.Now,
                Tipo = 1,
                Mensaje = "Tu usuario ha sido desactivado",
                Visto = false
            };

            _context.Notificaciones.Add(notificacion);
            await _context.SaveChangesAsync();

            return Ok(new AuthResponseDto
            {
                IsSuccess = true,
                Message = "Usuario Desactivado"
            });
        }

        [AllowAnonymous]
        [HttpPost("refresh-token")]

        public async Task<ActionResult<AuthResponseDto>> RefreshToken(TokenDto tokenDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var principal = GetPrincipalFromExpiredToken(tokenDto.Token);
            var user = await _userManager.FindByEmailAsync(tokenDto.Email);

            if (principal is null || user is null || user.RefreshToken != tokenDto.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                return BadRequest(new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "Invalid client request"
                });

            var newJwtToken = GenerateToken(user);
            var newRefreshToken = GenerateRefreshToken();
            _ = int.TryParse(_configuration.GetSection("JWTSetting").GetSection("RefreshTokenValidityIn").Value!, out int RefreshTokenValidityIn);

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(RefreshTokenValidityIn);

            await _userManager.UpdateAsync(user);

            return Ok(new AuthResponseDto
            {
                IsSuccess = true,
                Token = newJwtToken,
                RefreshToken = newRefreshToken,
                Message = "Refreshed token successfully"
            });
        }

        private ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
        {
            var tokenParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("JwtSetting").GetSection("securityKey").Value!)),
                ValidateLifetime = false


            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenParameters, out SecurityToken securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;
        }

        [AllowAnonymous]
        [HttpPost("forgot-password")]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordDto forgotPasswordDto)
        {
            var user = await _userManager.FindByEmailAsync(forgotPasswordDto.Email);

            if (user is null)
            {
                return Ok(new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "User does not exist with this email"
                });
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetLink = $"http://localhost:4200/reset-password?email={user.Email}&token={WebUtility.UrlEncode(token)}";

            var client = new RestClient("https://send.api.mailtrap.io/api/send");

            var request = new RestRequest
            {
                Method = Method.Post,
                RequestFormat = DataFormat.Json
            };

            request.AddHeader("Authorization", "Bearer 62a57db5c125073400f28db0b418c6d0");
            request.AddJsonBody(new
            {
                from = new { email = "mailtrap@demomailtrap.com" },
                to = new[] { new { email = user.Email } },
                template_uuid = "b8bd784f-961a-4c6f-bdcd-fbddfc8ceabe",
                template_variables = new { user_email = user.Email, pass_reset_link = resetLink }
            });

            var response = client.Execute(request);

            if (response.IsSuccessful)
            {
                return Ok(new AuthResponseDto
                {
                    IsSuccess = true,
                    Message = "Email sent with password reset link. Please check your email."
                });
            }
            else
            {
                return BadRequest(new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = response.Content!.ToString()
                });
            }

        }



        [AllowAnonymous]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto resetPasswordDto)
        {
            var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);
            // resetPasswordDto.Token = WebUtility.UrlDecode(resetPasswordDto.Token);

            if (user is null)
            {
                return BadRequest(new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "User does not exist with this email"
                });
            }

            var result = await _userManager.ResetPasswordAsync(user, resetPasswordDto.Token, resetPasswordDto.NewPassword);

            if (result.Succeeded)
            {

                Notificacion notificacion = new Notificacion
                {
                    IdUsuario = user.Id,
                    Fecha = DateTime.Now,
                    Tipo = 1,
                    Mensaje = "Restauraste tu contraseña",
                    Visto = false
                };

                _context.Notificaciones.Add(notificacion);
                await _context.SaveChangesAsync();

                return Ok(new AuthResponseDto
                {
                    IsSuccess = true,
                    Message = "Password reset Successfully"
                });
            }

            return BadRequest(new AuthResponseDto
            {
                IsSuccess = false,
                Message = result.Errors.FirstOrDefault()!.Description
            });
        }


        [HttpPost("change-password")]
        public async Task<ActionResult> ChangePassword(ChangePasswordDto changePasswordDto)
        {
            var user = await _userManager.FindByEmailAsync(changePasswordDto.Email);
            if (user is null)
            {
                return BadRequest(new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "User does not exist with this email"
                });
            }

            var result = await _userManager.ChangePasswordAsync(user, changePasswordDto.CurrentPassword, changePasswordDto.NewPassword);

            if (result.Succeeded)
            {

                Notificacion notificacion = new Notificacion
                {
                    IdUsuario = user.Id,
                    Fecha = DateTime.Now,
                    Tipo = 1,
                    Mensaje = "Cambiaste tu contraseña",
                    Visto = false
                };

                _context.Notificaciones.Add(notificacion);
                await _context.SaveChangesAsync();
                return Ok(new AuthResponseDto
                {
                    IsSuccess = true,
                    Message = "Password changed successfully"
                });
            }

            return BadRequest(new AuthResponseDto
            {
                IsSuccess = false,
                Message = result.Errors.FirstOrDefault()!.Description
            });
        }



        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }


        private string GenerateToken(ApplicationUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII
            .GetBytes(_configuration.GetSection("JWTSetting").GetSection("securityKey").Value!);

            var roles = _userManager.GetRolesAsync(user).Result;

            List<Claim> claims =
            [
                new (JwtRegisteredClaimNames.Email,user.Email??""),
                new (JwtRegisteredClaimNames.Name,user.FullName??""),
                new (JwtRegisteredClaimNames.NameId,user.Id ??""),
                new (JwtRegisteredClaimNames.Aud,
                _configuration.GetSection("JWTSetting").GetSection("validAudience").Value!),
                new (JwtRegisteredClaimNames.Iss,_configuration.GetSection("JWTSetting").GetSection("validIssuer").Value!)
            ];


            foreach (var role in roles)

            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            _ = int.TryParse(_configuration.GetSection("JWTSetting").GetSection("expireInMinutes").Value!, out int tokenExpirationTime);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(tokenExpirationTime),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256
                )
            };


            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);


        }

        //api/account/detail
        [HttpGet("detail")]
        public async Task<ActionResult<UserDetailDto>> GetUserDetail()
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(currentUserId!);


            if (user is null)
            {
                return NotFound(new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "User not found"
                });
            }

            return Ok(new UserDetailDto
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                Roles = [.. await _userManager.GetRolesAsync(user)],
                PhoneNumber = user.PhoneNumber,
                PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                AccessFailedCount = user.AccessFailedCount,
                Activo = user.Activo
            });

        }

        [HttpGet("detail-mayorista")]
        public async Task<ActionResult<UserMayoristaDetailDto>> GetUserMayoristaDetail()
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(currentUserId!);

            if (user is null)
            {
                return NotFound(new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "User not found"
                });
            }

            // Obteniendo detalles del cliente mayorista relacionado, incluyendo el agente de ventas
            var clienteMayorista = await _context.ClientesMayoristas
                .Include(cm => cm.AgenteVenta)
                .FirstOrDefaultAsync(cm => cm.IdUsuario == currentUserId);

            if (clienteMayorista is null)
            {
                return NotFound(new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "Cliente mayorista not found"
                });
            }

            // Construyendo el objeto de respuesta
            var response = new UserMayoristaDetailDto
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                Roles = [.. await _userManager.GetRolesAsync(user)],
                PhoneNumber = user.PhoneNumber,
                PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                AccessFailedCount = user.AccessFailedCount,
                Activo = user.Activo,

                // Datos del cliente mayorista
                IdMayorista = clienteMayorista.Id,
                RFCEmpresa = clienteMayorista.RFCEmpresa,
                NombreEmpresa = clienteMayorista.NombreEmpresa,
                DireccionEmpresa = clienteMayorista.DireccionEmpresa,
                TelefonoEmpresa = clienteMayorista.TelefonoEmpresa,
                EmailEmpresa = clienteMayorista.EmailEmpresa,
                NombreContacto = clienteMayorista.NombreContacto,
                CargoContacto = clienteMayorista.CargoContacto,
                TelefonoContacto = clienteMayorista.TelefonoContacto,
                EmailContacto = clienteMayorista.EmailContacto,

                // Datos completos del agente de ventas
                AgenteVenta = clienteMayorista.AgenteVenta != null ? new AgenteVentaDto
                {
                    Id = clienteMayorista.AgenteVenta.Id,
                    FullName = clienteMayorista.AgenteVenta.FullName,
                    Email = clienteMayorista.AgenteVenta.Email,
                    PhoneNumber = clienteMayorista.AgenteVenta.PhoneNumber
                    // Otros datos relevantes del agente de ventas
                } : null
            };

            return Ok(response);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDetailDto>>> GetUsers()
        {
            var users = await _userManager.Users.Select(u => new UserDetailDto
            {
                Id = u.Id,
                Email = u.Email,
                FullName = u.FullName,
                Roles = _userManager.GetRolesAsync(u).Result.ToArray(),
                Activo = u.Activo
            }).ToListAsync();

            return Ok(users);
        }


    }
}