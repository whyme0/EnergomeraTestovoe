using Api.Data;
using Api.Mappers;
using Api.Models.DTOs;
using Api.Topology;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FieldsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly TopologyWorker _topologyWorker;

        public FieldsController(AppDbContext context, TopologyWorker topologyWorker)
        {
            _context = context;
            _topologyWorker = topologyWorker;
        }

        /// <summary>
        /// Получение всех элементов fields с полями (id, name, size, locations).<br/>
        /// Locations должен содержать структуру: {"Center":[lat,lng],"Polygon":[[lat,lng],.. ..,[lat,lng]]}, где lat – широта, lng – долгота
        /// </summary>
        [HttpGet("all", Name = "GetAllFields")]
        public IEnumerable<FieldDto> GetAllFields()
        {
            return _context.Fields
                .Include(f => f.Locations)
                .ThenInclude(l => l.Center)
                .Include(f => f.Locations)
                .ThenInclude(l => l.Polygon)
                .Select(f => FieldMapper.MapField(f)).ToList();
        }

        /// <summary>
        /// Получение площади поля (size) по идентификатору (id)
        /// </summary>
        [HttpGet("{id}/size", Name = "GetFieldSize")]
        public IActionResult GetFieldSize(int id)
        {
            var field = _context.Fields.FirstOrDefault(f => f.Id == id);
            
            if (field == null) return NotFound();
            
            return Ok(new { field.Size });
        }

        /// <summary>
        /// Получение расстояния в метрах от центра поля до точки, переданной во входном параметре
        /// (т.е. в запросе должны содержаться координаты точки и идентификатор поля, на выходе получаем расстояние в метрах)
        /// </summary>
        [HttpGet("{id}/{lat}/{lon}/distance", Name = "GetDistanceFromCenter")]
        public IActionResult GetDistanceFromCenter(int id, double lat, double lon)
        {
            var field = _context.Fields
                .Include(f => f.Locations)
                .ThenInclude(l => l.Center)
                .FirstOrDefault(f => f.Id == id);

            if (field == null) return NotFound();

            var distance = _topologyWorker.MeasureDistance(
                [field.Locations.Center.Longitude, field.Locations.Center.Latitude],
                [lon, lat]
            );

            return Ok(new { distance });
        }

        /// <summary>
        /// Получение принадлежности точки к полям (т.е. лежит ли точка в контуре одного из полей).
        /// В запросе - координаты точки, на выходе идентификатор и название поля (id, name), в случае если точка находится в одном из контуров полей.
        /// В случае, если точка не принадлежит ни одному из контуров полей, возвращаем false
        /// </summary>
        [HttpGet("{id}/{lat}/{lon}/affiliation", Name = "GetAffiliation")]
        public IActionResult GetAffiliation(int id, double lat, double lon)
        {
            var field = _context.Fields
                .Include(f => f.Locations)
                .ThenInclude(l => l.Center)
                .Include(f => f.Locations)
                .ThenInclude(l => l.Polygon)
                .FirstOrDefault(f => f.Id == id);

            if (field == null) return NotFound();

            var isInside = _topologyWorker.IsInside(
                field.Locations.Polygon.Select(p => new[] { p.Longitude, p.Latitude }).ToList(),
                [lon, lat]
            );

            if (isInside)
                return Ok(new
                { 
                    IsInside = isInside,
                    Name = field.Name
                });
            
            return Ok(isInside);
        }
    }
}
