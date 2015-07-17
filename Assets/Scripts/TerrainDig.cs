using UnityEngine;
using System.Collections;

public class TerrainDig : MonoBehaviour
{
    public int brushSize = 10;
    public float brushOpacity = 0.0005f;
    public float minDistanceUntilNextUpdate = 0.5f;

    Terrain _terrain;
    Vector3 _lastUpdatePosition;

    void Start()
    {
        _terrain = GetComponent<Terrain>();
        Debug.Log(arrayToString(RectangleArray2D(10, 10)));
        Debug.Log(arrayToString(EllipseArray2D(10, 10)));
        Debug.Log(arrayToString(Gaussian2D(10, 10)));
    }

    public void ResetTerrain()
    {
        int width = _terrain.terrainData.heightmapWidth;
        int height = _terrain.terrainData.heightmapHeight;
        float[,] heightMap = _terrain.terrainData.GetHeights(0, 0, width, height);

        for (int i = 0; i < width; i++)
            for (int j = 0; j < height; j++)
                heightMap[i, j] = 0.5f;

        _terrain.terrainData.SetHeights(0, 0, heightMap);
    }

    void Update()
    {
        if (Input.GetButton("Fire1"))
        {
            if (Input.GetButtonDown("Fire1"))
                _lastUpdatePosition = new Vector3(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity);

            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                if (Vector3.Distance(hit.point, _lastUpdatePosition) >= minDistanceUntilNextUpdate)
                {
                    //TODO: Cleanup Edge cases

                    _lastUpdatePosition = hit.point;

                    float x = (hit.point.x - transform.position.x) / _terrain.terrainData.size.x;
                    float y = (hit.point.z - transform.position.z) / _terrain.terrainData.size.z;

                    // Digging...
                    int leftBrush = (int)(x * _terrain.terrainData.heightmapWidth - brushSize / 2);
                    int topBrush = (int)(y * _terrain.terrainData.heightmapHeight - brushSize / 2);

                    float[,] heightMap = _terrain.terrainData.GetHeights(leftBrush, topBrush, brushSize, brushSize);
                    float[,] brush = EllipseArray2D(brushSize, brushSize);

                    for (int i = 0; i < brushSize; i++)
                        for (int j = 0; j < brushSize; j++)
                            heightMap[i, j] -= brush[i, j] * brushOpacity;

                    _terrain.terrainData.SetHeights(leftBrush, topBrush, heightMap);
                }
            }
        }
    }

    float[,] RectangleArray2D(int width, int height)
    {
        float[,] rectangleArray = new float[width, height];
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                rectangleArray[x, y] = 1;
        return rectangleArray;
    }

    float[,] EllipseArray2D(int width, int height)
    {
        float[,] circleArray = new float[width, height];
        float a = width / 2;
        float b = height / 2;
        float x0 = width / 2;
        float y0 = height / 2;
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                if (Mathf.Pow(x - x0, 2) / Mathf.Pow(a, 2) + Mathf.Pow(y - y0, 2) / Mathf.Pow(b, 2) <= 1)
                    circleArray[x, y] = 1;
        return circleArray;
    }

    float[,] Gaussian2D(int width, int height, float hardness = 0)
    {
        float[,] gaussian2DArray = new float[width, height];

        float x0 = width / 2;
        float y0 = height / 2;
        float stdx = width;
        float stdy = height;

        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
            {
                gaussian2DArray[x, y] = 1 * Mathf.Exp(-(Mathf.Pow(x - x0, 2) / (2 * Mathf.Pow(stdx, 2)) + Mathf.Pow(y - y0, 2) / (2 * Mathf.Pow(stdy, 2))));
            }

        return gaussian2DArray;
    }

    string arrayToString(float[,] array2D)
    {
        string str = "[";
        for (int j = 0; j < array2D.GetLength(1); j++)
        {
            for (int i = 0; i < array2D.GetLength(0); i++)
            {
                str += "{" + array2D[i, j] + "}";
                if (i != array2D.GetLength(0) - 1) str += ", ";
            }
            if (j != array2D.GetLength(1) - 1) str += " \n ";
        }
        str += "]";
        return str;
    }
}
