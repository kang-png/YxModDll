using UnityEngine;

public class ModWireframeRenderer : MonoBehaviour
{
    private static Material _lineMaterial;

    public static void DrawWireframe(GameObject obj)
    {
        if (_lineMaterial == null)
        {
            //Shader shader = Shader.Find("Unlit/Color");
            // ✅ 替代 Unlit/Color，选择一个更保险的 Shader
            //Shader shader = Shader.Find("Legacy Shaders/Particles/Alpha Blended");
            // 或者用这个：
            Shader shader = Shader.Find("GUI/Text Shader");

            if (shader == null)
            {
                Debug.LogError("找不到合适的 Shader！");
                return;
            }
            _lineMaterial = new Material(shader);
            _lineMaterial.color = Color.green;
        }

        MeshFilter meshFilter = obj.GetComponent<MeshFilter>();
        if (meshFilter == null || meshFilter.sharedMesh == null) return;

        Mesh mesh = meshFilter.sharedMesh;
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;

        GL.PushMatrix();
        GL.MultMatrix(obj.transform.localToWorldMatrix);
        _lineMaterial.SetPass(0);
        GL.Begin(GL.LINES);

        for (int i = 0; i < triangles.Length; i += 3)
        {
            GL.Vertex(vertices[triangles[i]]);
            GL.Vertex(vertices[triangles[i + 1]]);
            GL.Vertex(vertices[triangles[i + 1]]);
            GL.Vertex(vertices[triangles[i + 2]]);
            GL.Vertex(vertices[triangles[i + 2]]);
            GL.Vertex(vertices[triangles[i]]);
        }

        GL.End();
        GL.PopMatrix();
    }
}
