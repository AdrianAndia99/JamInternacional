using UnityEngine;

public static class RandomPositionUtility
{
    // Devuelve un punto aleatorio dentro del triángulo a-b-c (distribución uniforme)
    private static Vector3 SamplePointInTriangle(Vector3 a, Vector3 b, Vector3 c)
    {
        float r1 = Random.value;
        float r2 = Random.value;

        float sqrtR1 = Mathf.Sqrt(r1);
        float u = 1f - sqrtR1;
        float v = sqrtR1 * (1f - r2);
        float w = sqrtR1 * r2; // u + v + w = 1

        return a * u + b * v + c * w;
    }

    // Área de triángulo (abs(cross) / 2)
    private static float TriangleArea(Vector3 a, Vector3 b, Vector3 c)
    {
        return Vector3.Cross(b - a, c - a).magnitude * 0.5f;
    }

    // Parámetros: esquinas del quad en orden (horario o antihorario)
    public static Vector3 GetRandomPointInQuad(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
    {
        // Triángulos: (a,b,c) y (a,c,d) — diagonal AC
        float area1 = TriangleArea(a, b, c);
        float area2 = TriangleArea(a, c, d);
        float total = area1 + area2;

        // Si el quad es degenerado (áreas 0), devolvemos el centro como fallback
        if (total <= Mathf.Epsilon)
            return (a + b + c + d) * 0.25f;

        // Elegir triángulo en función del área
        float pick = Random.value * total;
        if (pick <= area1)
            return SamplePointInTriangle(a, b, c);
        else
            return SamplePointInTriangle(a, c, d);
    }
}