using Api.Models;
using Api.Models.DTOs;

namespace Api.Mappers
{
    public class FieldMapper
    {
        public static FieldDto MapField(Field field)
        {
            return new FieldDto()
            {
                Id = field.Id,
                Name = field.Name,
                Size = field.Size,
                Locations = MapLocation(field.Locations)
            };
        }

        public static LocationDto MapLocation(Location location)
        {
            return new LocationDto()
            {
                Center = [location.Center.Latitude, location.Center.Longitude],
                Polygon = location.Polygon.Select(p => new[] { p.Latitude, p.Longitude }).ToList()
            };
        }
    }
}
