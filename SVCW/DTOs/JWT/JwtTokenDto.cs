namespace SVCW.DTOs.JWT
{
    public class JwtTokenDto
    {
        public string? accountid { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }

        public string jwtToken { get; set; }
        public bool isNewUser { get; set; } = false;
    }
}
