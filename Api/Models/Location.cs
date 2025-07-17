namespace Api.Models
{
    public class Location
    {
        public int Id { get; set; }

        public int FieldId { get; set; }
        public Field Field { get; set; }

        public int CenterId { get; set; }
        public Coordinate Center { get; set; }
        public List<Coordinate> Polygon { get; set; }
    }
}
