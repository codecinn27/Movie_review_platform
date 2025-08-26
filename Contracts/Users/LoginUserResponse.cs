namespace MovieAPI.Contracts
{
    public class LoginUserResponse
    {
        public string Token { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;

        public string id { get; set; } = string.Empty;
    }
}
