using UnityEngine;

public class MagneticFieldVisualization : MonoBehaviour
{
    public int resolution = 64;
    public float XMAX = 40f;
    public float YMAX = 40f;
    public float RE = 6.37f;

    private void Start()
    {
        DrawMagneticField();
        DrawEarth();
    }

    void DrawMagneticField()
    {
        LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.positionCount = resolution * resolution;
        lineRenderer.widthCurve = new AnimationCurve(new Keyframe(0, 0.2f), new Keyframe(1, 0.2f));
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));

        float[] x = new float[resolution];
        float[] y = new float[resolution];

        for (int i = 0; i < resolution; i++)
        {
            float t = i / (float)(resolution - 1);
            float angle = Mathf.Lerp(-Mathf.PI, Mathf.PI, t);
            x[i] = XMAX * Mathf.Cos(angle);
            y[i] = YMAX * Mathf.Sin(angle);
        }

        for (int i = 0; i < resolution; i++)
        {
            float angle = Mathf.Atan2(y[i], x[i]);
            float r = Mathf.Sqrt(x[i] * x[i] + y[i] * y[i]);

            Vector2 magneticField = CalculateMagneticField(r, angle);
            lineRenderer.SetPosition(i, new Vector3(x[i], y[i], 0f));
            lineRenderer.SetPosition(i + resolution, new Vector3(x[i] + magneticField.x, y[i] + magneticField.y, 0f));
        }
    }

    void DrawEarth()
    {
        GameObject earth = new GameObject("Earth");
        earth.transform.position = Vector3.zero;

        SpriteRenderer spriteRenderer = earth.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = Sprite.Create(new Texture2D(1, 1), new Rect(0, 0, 1, 1), Vector2.one * 0.5f);

        spriteRenderer.color = Color.blue;
        earth.transform.localScale = new Vector3(RE * 2, RE * 2, 1f);
    }

    Vector2 CalculateMagneticField(float r, float theta)
    {
        // Mean magnitude of the Earth's magnetic field at the equator in T
        float B0 = 3.12e-5f;
        // Deviation of magnetic pole from axis
        float alpha = Mathf.Deg2Rad * 9.6f;

        float fac = B0 * Mathf.Pow(RE / r, 3);
        float Bx = -2 * fac * Mathf.Cos(theta + alpha);
        float By = -fac * Mathf.Sin(theta + alpha);

        return new Vector2(Bx, By);
    }
}
