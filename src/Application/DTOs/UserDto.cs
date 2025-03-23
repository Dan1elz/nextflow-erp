namespace dotnet_api_erp.src.Application.DTOs
{
    public class UserDto
    {
        public class LoginResponseDto 
        {
            public Guid UserId { get; set; }
            public string Token { get; set; } = string.Empty;
        }
        public class UserPasswordDto
        {
            public required string Password { get; set; }
        }
        public class LoginUserDto
        {
            public string Email { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }
    }
}