using MathNet.Spatial.Euclidean;

namespace GlobalCoordinate.Runtime.Data
{
    public class GeodeticCoordinate
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Altitude { get; set; }

        public Quaternion Rotation { get; set; }

        public override string ToString()
        {
            return $"Lat: {Latitude}, Lon: {Longitude}, Alt: {Altitude}, Rot: {Rotation}";
        }
    }
}