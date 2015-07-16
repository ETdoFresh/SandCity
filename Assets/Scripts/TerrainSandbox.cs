using UnityEngine;
using System.Collections;

public class TerrainSandbox : MonoBehaviour {
    private Terrain _terrain;

    void Start()
    {
        _terrain = GameObject.Find("Terrain").GetComponent<Terrain>();
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(30, 30, 200, 30), "Change Terrain Height"))
        {
            //get terain heightmap width and height
            int xRes = _terrain.terrainData.heightmapWidth;
            int yRes = _terrain.terrainData.heightmapHeight;
            Debug.Log(xRes + ", " + yRes);

            int xBase = 0;
            int yBase = 0;

            //GetHeights - gets the heightmap points of the terrain.
            float[,] heights = _terrain.terrainData.GetHeights(xBase, yBase, xRes, yRes);
            heights[10, 10] = 0f;

            _terrain.terrainData.SetHeights(0, 0, heights);

        }
    }
}
