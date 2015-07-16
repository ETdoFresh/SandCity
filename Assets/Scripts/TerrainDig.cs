using UnityEngine;
using System.Collections;

public class TerrainDig : MonoBehaviour {
    Terrain _terrain;
    int _xRes;
    int _yRes;

    void Start()
    {
        _terrain = GameObject.Find("MainTerrain").GetComponent<Terrain>();

        //get terain heightmap width and height
        _xRes = _terrain.terrainData.heightmapWidth;
        _yRes = _terrain.terrainData.heightmapHeight;
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {

            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                int x = Mathf.FloorToInt((hit.point.x + 40) / 80 * _xRes);
                int y = Mathf.FloorToInt((hit.point.z + 40) / 80 * _yRes);
                DigTenPixelSquare(x, y);
            }
        }
    }

    void DigTenPixelSquare(int x, int y)
    {
        int xLeft = Mathf.Max(0, x - 5);
        int yTop = Mathf.Max(0, y - 5);
        int xWidth = Mathf.Min(_xRes - xLeft, 10);
        int yHeight = Mathf.Min(_yRes - yTop, 10);

        //GetHeights - gets the heightmap points of the terrain.
        float[,] heightMap = _terrain.terrainData.GetHeights(xLeft, yTop, xWidth, yHeight);
        float digDepth = .02f;

        string debugStr = "";
        for (int i = 0; i < xWidth; i++)
            for (int j = 0; j < yHeight; j++)
            {
                debugStr += "(" + i + "," + j + ") " + heightMap[i, j] + "|";
                heightMap[i, j] = Mathf.Max(0, heightMap[i, j] - digDepth);
            }

                Debug.Log(debugStr);

        _terrain.terrainData.SetHeights(xLeft, yTop, heightMap);
    }
}
