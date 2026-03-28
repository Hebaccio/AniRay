using AniRay.Model.Requests.HelperRequests;

namespace AniRay.Model.Requests.UserRequests
{
    public class UserMU
    {
        public int Id { get; set; }
        public string Pfp { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateOnly Birthday { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool TwoFA { get; set; }
        public BaseClassShortMU UserRole { get; set; }
        public BaseClassShortMU UserStatus { get; set; }
        public BaseClassShortMU Gender { get; set; }
    }
}
