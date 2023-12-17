using System;

namespace jp.co.gatari.GlobalCoordinate.Runtime.Data
{
    [Serializable]
    public class HelmertTransform2DData
    {
        public double x;
        public double y;
        public double projectionX;
        public double projectionY;

        public HelmertTransform2DData(double x, double y, double projectionX, double projectionY)
        {
            this.x = x;
            this.y = y;
            this.projectionX = projectionX;
            this.projectionY = projectionY;
        }
    }
}