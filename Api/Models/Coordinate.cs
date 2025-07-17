﻿namespace Api.Models
{
    public class Coordinate
    {
        public int Id { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public int LocationId { get; set; }
        public Location Location { get; set; }
    }
}
