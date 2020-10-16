namespace RushHour.ViewModels.Account
{
    using System.Collections.Generic;

    public class UserViewModel
    {
        public string Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public IList<string> Roles { get; set; } = new List<string>();
    }
}
