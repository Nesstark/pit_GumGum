using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class WaterShade: MonoBehaviour
{
    public Color shallowColor = new Color(0.1f, 0.6f, 1f);
    public Color deepColor = new Color(0f, 0.1f, 0.3f);

    void Update()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        Vector3[] verts = mesh.vertices;
        Color[] colors = new Color[verts.Length];

        float minY = float.MaxValue;
        float maxY = float.MinValue;

        // Find wave height range
        foreach (var v in verts)
        {
            float y = transform.TransformPoint(v).y;
            if (y < minY) minY = y;
            if (y > maxY) maxY = y;
        }

        // Color by normalized height
        for (int i = 0; i < verts.Length; i++)
        {
            float y = transform.TransformPoint(verts[i]).y;
            float t = Mathf.InverseLerp(minY, maxY, y);
            colors[i] = Color.Lerp(deepColor, shallowColor, t);
        }

        mesh.colors = colors;
    }
}
