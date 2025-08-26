using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using MovieAPI.AppDataContext;
using MovieAPI.Contracts;
using MovieAPI.Interface;
using MovieAPI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace MovieAPI.Services
{
    public class UserService : IUserService
    {
        private readonly MovieDbContext _context;
        private readonly ILogger<UserService> _logger;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;


        public UserService(MovieDbContext context, ILogger<UserService> logger, IMapper mapper, IConfiguration configuration)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<UserResponseDto?> FindAsync(Guid id)
        {
            var user = await _context.Users
                .Include(u => u.Reviews)
                .ThenInclude(r => r.Movie) // optional: include movie info
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null) return null;

            return _mapper.Map<UserResponseDto>(user);
        }


        public async Task<User> RegisterAsync(RegisterUserRequest request, bool isAdmin = false)
        {

            // 1. Check for duplicate Name
            if (await _context.Users.AnyAsync(u => u.Name == request.Name))
                throw new Exception("Name already exists");

            // 2. Check for duplicate Email
            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
                throw new Exception("Email already exists");

            try
            {            // Map DTO to entity
                var user = _mapper.Map<User>(request);

                // Hash password before saving
                user.Password = HashPassword(request.Password);
                user.Role = isAdmin ? UserRole.Admin : UserRole.User;
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a new User.");
                throw new Exception("An error occurred while creating a new User.");
            }
        }

        public async Task<LoginUserResponse> LoginAsync(LoginUserRequest request)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Name == request.Name);

            if (user == null || !VerifyPassword(request.Password, user.Password))
                throw new Exception("Invalid email or password");
            //Console.WriteLine("user found");
            // Generate JWT
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["JwtSettings:SecretKey"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim(ClaimTypes.Role, user.Role.ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return new LoginUserResponse
            {
                Token = tokenHandler.WriteToken(token),
                Name = user.Name,
                Role = user.Role.ToString(),
                id = user.Id.ToString()
            };
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User?> GetUserByIdAsync(Guid id)
        {
            return await _context.Users.FindAsync(id);
        }

        // Simple SHA256 hash (demo purposes)
        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }

        private bool VerifyPassword(string input, string hashed)
        {
            return HashPassword(input) == hashed;
        }
    }
}
