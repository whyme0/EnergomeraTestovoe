namespace Api.Models.DTOs
{
    public class LocationDto
    {
        public double[] Center { get; set; }
        public List<double[]> Polygon { get; set; }
    }
}
