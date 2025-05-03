

namespace FacturaSii.src.Auth.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(string username);
    }
}
