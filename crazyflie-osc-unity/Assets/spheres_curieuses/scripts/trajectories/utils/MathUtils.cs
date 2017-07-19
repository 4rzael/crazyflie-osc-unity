using UnityEngine;
using UnityEditor;
using MathNet.Numerics.LinearAlgebra;

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

    static public Vector3 MathNetVectorsToUnity(Vector<float> v)
    {
        return new Vector3(v[0], v[1], v[2]);
    }
}