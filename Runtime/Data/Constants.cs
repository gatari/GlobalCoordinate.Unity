using System;

namespace GlobalCoordinate.Runtime.Data
{
    public class Constants
    {
        public const double Wgs84A = 6378137.0; // semi-major axis
        public const double F = 1.0 / 298.257223563; // inverse flattening
        public const double E2 = 2 * F - F * F;
        public const double Wgs84B = Wgs84A * (1.0d - F); // semi-minor axis

        public static readonly double Ea = (Math.Pow(Wgs84A, 2) - Math.Pow(Wgs84B, 2)) / Math.Pow(Wgs84A, 2);
        public static readonly double Eb = (Math.Pow(Wgs84A, 2) - Math.Pow(Wgs84B, 2)) / Math.Pow(Wgs84B, 2);
    }
}