using System;
using GlobalCoordinate.Runtime.Foundation;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Spatial.Euclidean;
using Quaternion = MathNet.Spatial.Euclidean.Quaternion;

namespace GlobalCoordinate.Runtime.Data
{
    public static class CoordinateConverter
    {
        private const double Wgs84A = 6378137.0; // semi-major axis
        private const double F = 1.0 / 298.257223563; // inverse flattening
        private const double E2 = 2 * F - F * F;
        private const double Wgs84B = Wgs84A * (1.0d - F); // semi-minor axis

        private static readonly double Ea = (Math.Pow(Wgs84A, 2) - Math.Pow(Wgs84B, 2)) / Math.Pow(Wgs84A, 2);
        private static readonly double Eb = (Math.Pow(Wgs84A, 2) - Math.Pow(Wgs84B, 2)) / Math.Pow(Wgs84B, 2);

        public static Vector3D ConvertToEcef(double longitude, double latitude, double altitude)
        {
            longitude *= Math.PI / 180.0d;
            latitude *= Math.PI / 180.0d;

            var n = Wgs84A / Math.Sqrt(1.0d - E2 * Math.Pow(Math.Sin(latitude), 2));

            var x = (n + altitude) * Math.Cos(latitude) * Math.Cos(longitude);
            var y = (n + altitude) * Math.Cos(latitude) * Math.Sin(longitude);
            var z = (n * (1.0d - E2) + altitude) * Math.Sin(latitude);

            return new Vector3D(x, y, z);
        }

        public static Vector3D ConvertGeodeticToEcef(GeodeticCoordinate geodetic)
        {
            return ConvertToEcef(geodetic.Longitude, geodetic.Latitude, geodetic.Altitude);
        }

        private static double N(double psy)
        {
            return Wgs84A / Math.Sqrt(1 - E2 * Math.Pow(Math.Sin(psy), 2));
        }

        public static GeodeticCoordinate ConvertEcefToGeodetic(Vector3D ecefPosition)
        {
            var longitude = Math.Atan2(ecefPosition.Y, ecefPosition.X);

            var p = Math.Sqrt(ecefPosition.X * ecefPosition.X + ecefPosition.Y * ecefPosition.Y);
            var theta = Math.Atan2(ecefPosition.Z * Wgs84A, p * Wgs84B);
            var sinPhi = (ecefPosition.Z + E2 * Wgs84B * Math.Pow(Math.Sin(theta), 3)) /
                         (p - E2 * Wgs84A * Math.Pow(Math.Cos(theta), 3));
            var phi = Math.Asin(sinPhi);

            Iterate();
            Iterate();
            Iterate();
            Iterate();

            var altitude = p / Math.Cos(phi) - N(phi);
            var latitude = phi;

            return new GeodeticCoordinate()
            {
                Altitude = altitude,
                Longitude = 180 * longitude / Math.PI,
                Latitude = 180 * phi / Math.PI,
            };

            void Iterate()
            {
                var n = N(phi);
                phi = Math.Atan2(ecefPosition.Z + n * E2 * Math.Sin(phi), p);
            }
        }

        /// sourceに対するrelativePositionに相当する点のEcef座標を計算する
        public static Vector3D ConvertEnuToEcef(GeodeticCoordinate sourceGeodeticCoordinate,
            Vector3D enuPosition)
        {
            var sourceEcefPose = ConvertGeodeticToEcef(sourceGeodeticCoordinate);

            // ENU座標系上のベクトルをECEF座標系に変換するための行列を計算
            var enu2EcefMatrix = EnuToEcefMatrix(sourceGeodeticCoordinate.Longitude, sourceGeodeticCoordinate.Latitude);

            // ターゲットのECEF座標を計算
            var position = sourceEcefPose.ToVector() + enu2EcefMatrix * enuPosition.ToVector();

            return position.ToVector3D();
        }

        public static GeodeticCoordinate ConvertEnuToGeodetic(
            GeodeticCoordinate sourceGeodeticPose,
            Vector3D enuPosition)
        {
            // ENU座標からECEF座標を計算
            var pointEcefPose = ConvertEnuToEcef(sourceGeodeticPose, enuPosition);

            // ECEF座標から緯度経度高度を計算
            var pointLlaPose = ConvertEcefToGeodetic(pointEcefPose);

            return new GeodeticCoordinate()
            {
                Latitude = pointLlaPose.Latitude,
                Longitude = pointLlaPose.Longitude,
                Altitude = pointLlaPose.Altitude,
                Rotation = Quaternion.One
            };
        }

        /// sourceLlaPoseから見たtargetLlaPoseのENU座標を計算する
        public static Vector3D GetEnuPosition(GeodeticCoordinate originGeodeticCoordinate,
            GeodeticCoordinate targetGeodeticPose)
        {
            var sourceEcefPose = ConvertGeodeticToEcef(originGeodeticCoordinate);
            var targetEcefPose = ConvertGeodeticToEcef(targetGeodeticPose);

            var result = EcefToEnuMatrix(originGeodeticCoordinate.Longitude, originGeodeticCoordinate.Latitude) *
                         (targetEcefPose - sourceEcefPose).ToVector();

            return new Vector3D(result[0], result[1], result[2]);
        }

        public static Matrix<double> EcefToEnuMatrix(double longitude, double latitude)
        {
            longitude *= Math.PI / 180.0d;
            latitude *= Math.PI / 180.0d;

            return Matrix.Build.DenseOfArray(new[,]
            {
                { -Math.Sin(longitude), Math.Cos(longitude), 0 },
                {
                    -Math.Sin(latitude) * Math.Cos(longitude), -Math.Sin(latitude) * Math.Sin(longitude),
                    Math.Cos(latitude)
                },
                {
                    Math.Cos(latitude) * Math.Cos(longitude), Math.Cos(latitude) * Math.Sin(longitude),
                    Math.Sin(latitude)
                },
            });
        }

        public static Matrix<double> EnuToEcefMatrix(double longitude, double latitude)
        {
            longitude *= Math.PI / 180.0d;
            latitude *= Math.PI / 180.0d;

            return Matrix.Build.DenseOfArray(new double[,]
            {
                {
                    -Math.Sin(longitude), -Math.Sin(latitude) * Math.Cos(longitude),
                    Math.Cos(latitude) * Math.Cos(longitude)
                },
                {
                    Math.Cos(longitude), -Math.Sin(latitude) * Math.Sin(longitude),
                    Math.Cos(latitude) * Math.Sin(longitude)
                },
                { 0, Math.Cos(latitude), Math.Sin(latitude) },
            });
        }
    }
}