namespace Api.Topology.Models
{
    public class KmlField
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public float Size { get; set; }
        public List<(double Latitude, double Longitude)> Polygon { get; set; }
    }
}
