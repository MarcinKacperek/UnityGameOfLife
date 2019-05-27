using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoardDrawer : MonoBehaviour {

    private const float WHITE_UV_X_START = 0.0f;
    private const float WHITE_UV_X_END = 0.49f;
    private const float BLACK_UV_X_START = 0.5f;
    private const float BLACK_UV_X_END = 1.0f;

    [SerializeField] private Material material = null;

    private GameObject board;
    private Mesh boardMesh;
    private int size;

    public void Draw(GameBoard gameBoard) {
        if (board != null) {
            Destroy(board);
        }
        this.size = gameBoard.Size;

        Vector3[] vertices = new Vector3[4 * gameBoard.Size * gameBoard.Size];
        Vector2[] uvs = new Vector2[4 * gameBoard.Size * gameBoard.Size];
        int[] triangles = new int[6 * gameBoard.Size * gameBoard.Size];

        int index = 0;
        int triangleIndex = 0;
        for (int x = 0; x < gameBoard.Size; x++) {
            for (int y = 0; y < gameBoard.Size; y++) {
                AddVertices(vertices, x, y, index);
                AddUvs(uvs, gameBoard[x, y], index);
                AddTriangles(triangles, index, triangleIndex);

                index += 4;
                triangleIndex += 6;
            }
        }

        boardMesh = new Mesh();
        boardMesh.vertices = vertices;
        boardMesh.uv = uvs;
        boardMesh.triangles = triangles;
        
        board = new GameObject("Board", typeof(MeshFilter), typeof(MeshRenderer));
        board.GetComponent<MeshFilter>().mesh = boardMesh;
        board.GetComponent<MeshRenderer>().material = material;
    }

    void AddVertices(Vector3[] vertices, int x, int y, int index) {
        vertices[index]     = new Vector3(x, y);
        vertices[index + 1] = new Vector3(x + 1, y);
        vertices[index + 2] = new Vector3(x + 1, y + 1);
        vertices[index + 3] = new Vector3(x, y + 1);
    }

    void AddUvs(Vector2[] uvs, bool alive, int index) {
        float start = alive ? WHITE_UV_X_START : BLACK_UV_X_START;
        float end = alive ? WHITE_UV_X_END : BLACK_UV_X_END;

        uvs[index]     = new Vector2(start, 0.0f);
        uvs[index + 1] = new Vector2(end, 0.0f);
        uvs[index + 2] = new Vector2(end, 1.0f);
        uvs[index + 3] = new Vector2(start, 1.0f);
    }

    void AddTriangles(int[] triangles, int index, int triangleIndex) {
        triangles[triangleIndex]     = index;
        triangles[triangleIndex + 1] = index + 2;
        triangles[triangleIndex + 2] = index + 1;
        triangles[triangleIndex + 3] = index;
        triangles[triangleIndex + 4] = index + 3;
        triangles[triangleIndex + 5] = index + 2;
    }

    public void SetAlive(int x, int y, bool alive) {
        Vector2[] uvs = boardMesh.uv;
        int index = x * 4 * size + y * 4;
        AddUvs(uvs, alive, index);
        boardMesh.uv = uvs;
    }

}