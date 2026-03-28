namespace AniRay.Model.Requests.UserCartRequests
{
    public class UserCartURU
    {
        public string CartNotes { get; set; } = string.Empty;
        public virtual ICollection<BluRayCartUR>? BluRay { get; set; } = new List<BluRayCartUR>();
    }
}
