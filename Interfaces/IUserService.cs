using MovieAPI.Contracts;
using MovieAPI.Models;

namespace MovieAPI.Interface
{
    public interface IUserService
    {
        // Register a new user
        Task<User> RegisterAsync(RegisterUserRequest request, bool isAdmin = false);

        // Login user and return JWT token
        Task<LoginUserResponse> LoginAsync(LoginUserRequest request);

        // Get a user by ID
        Task<UserResponseDto?> FindAsync(Guid id);

        // // Optional: Get all users
        // Task<List<User>> GetAllUsersAsync();

        // // Optional: Get user by Id
        // Task<User?> GetUserByIdAsync(Guid id);
    }
}
