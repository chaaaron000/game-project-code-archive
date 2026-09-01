using UnityEngine;

public class DeformMesh : MonoBehaviour
{
    public MeshFilter meshFilter;

    void Start()
    {
        // 기존 메쉬를 복사하여 Deformed Mesh 생성
        Mesh deformedMesh = new Mesh();
        deformedMesh.vertices = meshFilter.mesh.vertices;
        deformedMesh.triangles = meshFilter.mesh.triangles;
        deformedMesh.uv = meshFilter.mesh.uv;
        deformedMesh.normals = meshFilter.mesh.normals;

        // Deformed Mesh의 정점들을 변경하여 구부리기
        Vector3[] vertices = deformedMesh.vertices;
        for (int i = 0; i < vertices.Length; i++)
        {
            // 각 정점의 y좌표를 sin 함수를 이용하여 변경
            vertices[i].y = Mathf.Sin(vertices[i].x * 3f + Time.time * 3f) * 0.2f;
        }
        deformedMesh.vertices = vertices;

        // 변경된 Deformed Mesh를 Mesh Filter에 할당
        meshFilter.mesh = deformedMesh;
    }
}
