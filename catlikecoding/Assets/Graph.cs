using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using System.Security.AccessControl;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public enum graphFunctionName
{
    SinFunc,
    MultiSinFunc,
    Sin2DFunc,
    MultiSin2DFunc,
    Ripple,
}

public delegate float graphFunction(float x, float z, float t);

public class Graph : MonoBehaviour
{

    public Transform pointPrefab;
    [Range(10, 100)] public int resolution = 10;
    public graphFunctionName function;
    private Transform[] points;
    static graphFunction[] functions =
    {
        SinFunc, MultiSinFunc, Sin2DFunc, MultiSin2DFunc,Ripple,
    };

    private const float pi = Mathf.PI;

    private void Awake ()
    {
        points = new Transform[resolution * resolution];
        float step = 2f / resolution;
        Vector3 scale = Vector3.one * step;
        Vector3 position;
        position.y = 0f;
        for (int z = 0, i = 0; z < resolution; ++z)
        {
            position.z = (z + 0.5f) * step - 1f;
            for (int x = 0; x < resolution; ++i, ++x)
            {
                points[i] = Instantiate(pointPrefab);
                position.x = (x + 0.5f) * step - 1f;
                points[i].localPosition = position;
                points[i].localScale = scale;
                points[i].SetParent(transform, false);
            }
        }
    }

    private void Update ()
    {
        float t = Time.time;
        graphFunction f = functions[(int)function];
        for (int i = 0; i < points.Length; ++i)
        {
            var point = points[i];
            var pos = points[i].localPosition;
            pos.y = f(pos.x, pos.z, t);
            point.localPosition = pos;
        }
    }

    static float MultiSinFunc(float x, float z, float t)
    {
        float y = Mathf.Sin(pi * (x + t)) + Mathf.Sin(2.0f * pi * (x + t)) / 2.0f;
        y *= 2.0f / 3.0f;
        return y;
    }

    static float SinFunc(float x, float z, float t)
    {
        return Mathf.Sin(pi * (x + t));
    }

    static float Sin2DFunc(float x, float z, float t)
    {
        float a = (Mathf.Sin(pi * (x + t)) + Mathf.Sin(pi * (z + t)));
        a *= 0.5f;
        return a;
    }

    static float MultiSin2DFunc(float x, float z, float t)
    {
        float a = 4 * Mathf.Sin(pi * (x + z + t * 0.5f));
        float b = Mathf.Sin(pi * (x + t));
        float c = Mathf.Sin(pi * (z + 2.0f * t)) * 0.5f;
        return (a + b + c) * 1f / 5.5f;
    }

    static float Ripple(float x, float z, float t)
    {
        float d = Mathf.Sqrt(x * x + z * z);
        float y = Mathf.Sin(pi * (4f * d - t));
        y /= 1f + 10f * d;
        return y;
    }

}
