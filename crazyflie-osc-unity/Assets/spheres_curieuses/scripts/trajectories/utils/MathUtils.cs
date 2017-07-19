using UnityEngine;
using UnityEditor;
using MathNet.Numerics.LinearAlgebra;
using System;

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
        var mLeft = MathUtils.UnityVectorsToMathNet(left);
        var mRight = MathUtils.UnityVectorsToMathNet(right);

        Vector3 result = new Vector3();
        result.x = left.y * right.z - left.z * right.y;
        result.y = -left.x * right.z + left.z * right.x;
        result.z = left.x * right.y - left.y * right.x;

        return result;
    }
}