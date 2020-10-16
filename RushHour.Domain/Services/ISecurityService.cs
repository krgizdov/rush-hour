namespace RushHour.Domain.Services
{
    using System.Collections.Generic;

    public interface ISecurityService
    {
        string GenerateJwtToken(string username, IList<string> roles);
    }
}
