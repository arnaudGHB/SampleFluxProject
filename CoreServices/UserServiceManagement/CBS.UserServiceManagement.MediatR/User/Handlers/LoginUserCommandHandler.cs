using CBS.UserServiceManagement.Data;
using CBS.UserServiceManagement.Helper;
using CBS.UserServiceManagement.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

// --- CORRECTION : Le namespace est conservé tel quel, conformément au modèle ---
namespace CBS.UserServiceManagement.MediatR
{
    public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, ServiceResponse<LoginResponse>>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<LoginUserCommandHandler> _logger;
        private readonly IConfiguration _configuration;

        public LoginUserCommandHandler(
            IUserRepository userRepository,
            ILogger<LoginUserCommandHandler> logger,
            IConfiguration configuration)
        {
            _userRepository = userRepository;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<ServiceResponse<LoginResponse>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userRepository.FindBy(u => u.Email == request.Email)
                    .FirstOrDefaultAsync(cancellationToken);

                if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                {
                    // --- CORRECTION : Utilisation du template de message pour le logging ---
                    _logger.LogWarning("Login validation failed for email: {Email}", request.Email);
                    return ServiceResponse<LoginResponse>.Return401("Invalid credentials provided to UserServiceManagement.");
                }

                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_configuration["JwtSettings:Key"]);

                var claims = new System.Collections.Generic.List<Claim>
                {
                    new Claim("Id", user.Id),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role)
                };

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.UtcNow.AddMinutes(Convert.ToInt32(_configuration["JwtSettings:minutesToExpiration"])),
                    Issuer = _configuration["JwtSettings:issuer"],
                    Audience = _configuration["JwtSettings:audience"],
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);

                var response = new LoginResponse
                {
                    Token = tokenString,
                    RefreshToken = string.Empty
                };
                
                // --- CORRECTION : Utilisation du template de message pour le logging ---
                _logger.LogInformation("User {Email} logged in successfully.", user.Email);

                return ServiceResponse<LoginResponse>.ReturnResultWith200(response, "Credentials are valid. Token obtained from UserServiceManagement.");
            }
            catch (Exception ex)
            {
                // --- CORRECTION : Utilisation du template de message pour le logging ---
                _logger.LogError(ex, "An unexpected error occurred during login for email: {Email}", request.Email);
                return ServiceResponse<LoginResponse>.Return500(ex);
            }
        }
    }
}
