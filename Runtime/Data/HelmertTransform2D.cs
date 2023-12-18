using System;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Spatial.Euclidean;

namespace GlobalCoordinate.Runtime.Data
{
    // 二次元のヘルマート変換
    public class HelmertTransform2D
    {
        private Matrix<double> A { get; set; }
        private Vector<double> L { get; set; }

        private double _kA;
        private double _kB;

        public double Rot { get; set; }
        public double TransX { get; set; }
        public double TransY { get; set; }
        public double Scale { get; set; }

        public void Initialize(HelmertTransform2DData[] data)
        {
            var n = data.Length;

            if (n < 2)
                throw new Exception("データ数が足りません");

            var a = DenseMatrix.Create(n * 2, 4, 0.0d);
            var l = DenseVector.Create(n * 2, 0.0d);

            for (var i = 0; i < n; i++)
            {
                var x = data[i].x;
                var y = data[i].y;
                var xDash = data[i].projectionX;
                var yDash = data[i].projectionY;

                a[i * 2, 0] = x;
                a[i * 2, 1] = y;
                a[i * 2, 2] = 1.0d;
                a[i * 2, 3] = 0.0d;
                l[i * 2] = xDash;

                a[i * 2 + 1, 0] = y;
                a[i * 2 + 1, 1] = -x;
                a[i * 2 + 1, 2] = 0.0d;
                a[i * 2 + 1, 3] = 1.0d;
                l[i * 2 + 1] = yDash;
            }

            A = a;
            L = l;
        }

        public void Resolve(bool fixScale = false)
        {
            var x = A.TransposeThisAndMultiply(A).Inverse() * A.TransposeThisAndMultiply(L);

            Rot = Math.Atan2(-x[1], x[0]) / Math.PI * 180.0d;
            TransX = x[2];
            TransY = x[3];
            var s = Math.Sqrt(x[0] * x[0] + x[1] * x[1]);
            Scale = fixScale ? 1.0d : s;
            _kA = fixScale ? x[0] / s : x[0];
            _kB = fixScale ? x[1] / s : x[1];
        }

        public Vector2D Transform(Vector2D input)
        {
            var projectionX = _kA * input.X + _kB * input.Y + TransX;
            var projectionY = -_kB * input.X + _kA * input.Y + TransY;

            return new Vector2D(projectionX, projectionY);
        }

        public Vector2D TransformInverse(Vector2D input)
        {
            var projectionX = (_kA * (input.X - TransX) - _kB * (input.Y - TransY)) / Scale;
            var projectionY = (_kB * (input.X - TransX) + _kA * (input.Y - TransY)) / Scale;

            return new Vector2D(projectionX, projectionY);
        }
    }
}