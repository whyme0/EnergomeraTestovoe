using Api.Topology.Models;
using System.Drawing;
using System.Globalization;
using System.Xml.Linq;

namespace Api.Topology
{
    public class TopologyWorker
    {
        public double MeasureDistance(double[] centerCoords, double[] coords)
        {
            // Расчет производится по формуле гаверсинуса
            double R = 6371000;

            double lat1 = centerCoords[0] * (Math.PI / 180);
            double lat2 = coords[0] * (Math.PI / 180);

            double lng1 = centerCoords[1] * (Math.PI / 180);
            double lng2 = coords[1] * (Math.PI / 180);

            double deltaLat = lat2 - lat1;
            double deltaLng = lng2 - lng1;

            double a = Math.Pow(Math.Sin(deltaLat / 2), 2)
                + Math.Cos(lat1)
                * Math.Cos(lat2)
                * Math.Pow(Math.Sin(deltaLng/2), 2);
            double d = 2 * R * Math.Asin(Math.Sqrt(a));

            return d;
        }

        public bool IsInside(List<double[]> coords, double[] innerCoord)
        {
            // Расчет производится по https://rosettacode.org/wiki/Ray-casting_algorithm
            int count = coords.Count;
            bool flag = false;
            for (int i = 0, j = count - 1; i < count; j = i++)
            {
                if ((coords[i][1] > innerCoord[1]) != (coords[j][1] > innerCoord[1]) &&
                    (innerCoord[0] < (coords[j][0] - coords[i][0]) *
                     (innerCoord[1] - coords[j][1]) /
                     (coords[j][1] - coords[i][1]) + coords[i][0]))
                {
                    flag = !flag;
                }
            }
            return flag;
        }

        public List<Centroid> ParseCentroids(string centroidsPath)
        {
            XDocument kml = XDocument.Load(centroidsPath);
            XNamespace ns = kml.Root.GetDefaultNamespace();
            return kml.Descendants(ns + "Placemark")
                .Select(p => new Centroid()
                {
                    Id = int.Parse(
                        p.Descendants(ns + "SimpleData").First(e => e.Attribute("name").Value == "fid").Value
                    ),
                    Latitude = double.Parse(
                        p.Descendants(ns + "coordinates").First().Value.Split(',')[0], CultureInfo.InvariantCulture
                    ),
                    Longitude = double.Parse(
                        p.Descendants(ns + "coordinates").First().Value.Split(',')[1], CultureInfo.InvariantCulture
                    )
                }).ToList();
        }

        public List<KmlField> ParseKmlFields(string kmlFieldPath)
        {
            XDocument kml = XDocument.Load(kmlFieldPath);
            XNamespace ns = kml.Root.GetDefaultNamespace();

            return kml.Descendants(ns + "Placemark")
                .Select(p => new KmlField()
                {
                    Id = int.Parse(
                        p.Descendants(ns + "SimpleData").First(e => e.Attribute("name").Value == "fid").Value
                    ),
                    Name = p.Descendants(ns + "name").First().Value,
                    Size = float.Parse(
                        p.Descendants(ns + "SimpleData").First(e => e.Attribute("name").Value == "size").Value
                    ),
                    Polygon = p.Descendants(ns + "coordinates").First().Value.Trim().Split(' ').Select(c => {
                        var cp = c.Split(',');
                        return (double.Parse(cp[0], CultureInfo.InvariantCulture), double.Parse(cp[1], CultureInfo.InvariantCulture));
                    }).ToList()
                }
            ).ToList();
        }
    }
}
