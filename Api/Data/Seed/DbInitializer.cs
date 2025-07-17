using Api.Models;
using Api.Topology;

namespace Api.Data.Seed
{
    public class DbInitializer
    {
        public static void Seed(AppDbContext context, TopologyWorker topologyWorker)
        {
            var kmlFields = topologyWorker.ParseKmlFields(Path.Combine("..", "SourceData", "fields.kml"));
            var centroids = topologyWorker.ParseCentroids(Path.Combine("..", "SourceData", "centroids.kml"));
            
            foreach (var field in kmlFields)
            {
                var centroid = centroids.FirstOrDefault(c => c.Id == field.Id);
                if (centroid == null) continue;

                var center = new Coordinate
                {
                    Latitude = centroid.Latitude,
                    Longitude = centroid.Longitude
                };

                var polygon = field.Polygon.Select(p => new Coordinate
                {
                    Latitude = p.Latitude,
                    Longitude = p.Longitude
                }).ToList();

                var location = new Location
                {
                    Center = center,
                    Polygon = polygon
                };

                var fieldEntity = new Field
                {
                    Name = field.Name,
                    Size = field.Size,
                    Locations = location
                };

                context.Coordinates.Add(center);
                context.Locations.Add(location);
                context.Fields.Add(fieldEntity);
            }

            context.SaveChanges();
        }
    }
}
