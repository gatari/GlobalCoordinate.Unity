using System;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Spatial.Euclidean;
using UnityEngine;
using Quaternion = MathNet.Spatial.Euclidean.Quaternion;

namespace GlobalCoordinate.Runtime.Foundation
{
    public static class Helper
    {
        public static Vector3D ToEcrVector(this Vector3 unityVector)
        {
            return new Vector3D(unityVector.x, unityVector.z, unityVector.y);
        }

        public static Vector3 ToUnityVector(this Vector3D ecrVector)
        {
            return new Vector3((float)ecrVector.X, (float)ecrVector.Z, (float)ecrVector.Y);
        }

        public static Vector3D ToVector3D(this Vector<double> vector)
        {
            if (vector.Count != 3)
                throw new Exception("Vector size must be 3");

            return new Vector3D(vector[0], vector[1], vector[2]);
        }

        public static UnityEngine.Quaternion ToUnityQuaternion(this Quaternion quaternion)
        {
            return new UnityEngine.Quaternion((float)quaternion.ImagX, (float)quaternion.ImagZ, (float)quaternion.ImagY,
                -(float)quaternion.Real);
        }

        public static Quaternion ToEnuQuaternion(this UnityEngine.Quaternion quaternion)
        {
            return new Quaternion(-quaternion.w, quaternion.x, quaternion.z, quaternion.y);
        }
    }
}