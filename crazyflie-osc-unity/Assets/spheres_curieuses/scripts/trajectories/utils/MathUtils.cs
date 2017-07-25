using UnityEngine;
using UnityEditor;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;

public class MathUtils
{
    static public float[] Vec3ToArray(Vector3 v)
    {
        return new float[3] { v.x, v.y, v.z };
    }

    static public Vector<float> UnityVectorsToMathNet(Vector3 v)
    {
        return Vector<float>.Build.Dense(Vec3ToArray(v));
    }

    static public Matrix<float> MatrixFromVectors(params Vector3[] vectors)
    {
        float[][] arr = new float[vectors.Length][];
        for (int i = 0; i < vectors.Length; ++i)
        {
            arr[i] = Vec3ToArray(vectors[i]);
        }
        return Matrix<float>.Build.DenseOfRowArrays(arr);
    }

    internal static Vector3 MathNetVectorsToUnity(object p)
    {
        throw new NotImplementedException();
    }

    static public Vector3 MathNetVectorsToUnity(Vector<float> v)
    {
        return new Vector3(v[0], v[1], v[2]);
    }

    static public Vector3 Cross(Vector3 left, Vector3 right)
    {
        Vector3 result = new Vector3();
        result.x = left.y * right.z - left.z * right.y;
        result.y = -left.x * right.z + left.z * right.x;
        result.z = left.x * right.y - left.y * right.x;

        return result;
    }

    // Used for storing Vectors with a timestamp information
    public struct Vec3Stamped
    {
        public Vector3 vec;
        public float tms;
        public Vec3Stamped(Vector3 v) { vec = v; tms = Time.time; }
        public static implicit operator Vec3Stamped(Vector3 v) { return new Vec3Stamped(v); }
        static public Vector3 average(IEnumerable<Vec3Stamped> vList)
        {
            Vec3Stamped sum = new Vec3Stamped();
            Vec3Stamped last = new Vec3Stamped();
            bool first = true;
            foreach (Vec3Stamped v in vList)
            {
                if (first)
                {
                    last = v;
                    first = false;
                    continue;
                }
                sum.vec += v.vec - last.vec;
                sum.tms += v.tms - last.tms;
            }
            return sum.vec / sum.tms;
        }
        
        static public Predicate<Vec3Stamped> tmsIsBefore(float t)
        {
            float now = Time.time;
            return (v) => v.tms < now - t;
        }
    }
}
