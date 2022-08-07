using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))/*, ExecuteInEditMode*/]
public class DirectionalVisualizer : MonoBehaviour
{
    public PlayerMovement target = null;
    public float lineLength = 5f;

    static Material lineMaterial = null;
    static void CreateLineMaterial()
    {
        if (!lineMaterial)
        {
            // Unity has a built-in shader that is useful for drawing
            // simple colored things.
            Shader shader = Shader.Find("Hidden/Internal-Colored");
            lineMaterial = new Material(shader);
            lineMaterial.hideFlags = HideFlags.HideAndDontSave;
            // Turn on alpha blending
            lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            // Turn backface culling off
            lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            // Turn off depth writes
            lineMaterial.SetInt("_ZWrite", 0);
        }
    }

    private void OnPostRender()
    {
        if (ReferenceEquals(null, target))
        {
            return;
        }

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        CreateLineMaterial();
        GL.PushMatrix();
        lineMaterial.SetPass(0);

        Vector3 l = target.transform.position + target.GetMoveDirection(-1f, 0f) * lineLength;
        Vector3 r = target.transform.position + target.GetMoveDirection(1f, 0f) * lineLength;
        Vector3 t = target.transform.position + target.GetMoveDirection(0f, 1f) * lineLength;
        Vector3 b = target.transform.position + target.GetMoveDirection(0f, -1f) * lineLength;

        Vector3 notchX = Vector3.Lerp(l, r, (horizontal + 1f) / 2f);
        Vector3 notchY = Vector3.Lerp(b, t, (vertical + 1f) / 2f);

        GL.Begin(GL.LINES);
        {
            GL.Color(Color.red);
            GL.Vertex(l);
            GL.Vertex(r);
            GL.Vertex(notchX + Vector3.up * 0.25f);
            GL.Vertex(notchX - Vector3.up * 0.25f);

            GL.Color(Color.green);
            GL.Vertex(t);
            GL.Vertex(b);
            GL.Vertex(notchY + Vector3.left * 0.25f);
            GL.Vertex(notchY + Vector3.right * 0.25f);

            GL.Color(Color.blue);
            GL.Vertex(target.transform.position + target.GetMoveDirection(1f, -1f) * lineLength);
            GL.Vertex(target.transform.position + target.GetMoveDirection(-1f, 1f) * lineLength);

            GL.Vertex(target.transform.position + target.GetMoveDirection(1f, 1f) * lineLength);
            GL.Vertex(target.transform.position + target.GetMoveDirection(-1f, -1f) * lineLength);
        }
        GL.End();
        GL.PopMatrix();
    }

}
