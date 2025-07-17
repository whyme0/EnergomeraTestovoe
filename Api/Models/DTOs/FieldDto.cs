namespace Api.Models.DTOs
{
    public class FieldDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public float Size { get; set; }
        public LocationDto Locations { get; set; }
    }
}
