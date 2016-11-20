using UnityEngine;
using System.Collections;
using System;

public class TerrainManager : MonoBehaviour
{
    public static TerrainManager instance;

    public GameObject water;
    public GameObject sun;
    public int map_size_factor = 5;
    public float max_light = -1;

    Terrain terrain;

    float[,] heights;
    float[,] heights_dif;

    int[,] textures;

    bool[,] is_water;
    float[,] water_dist;

    float[,] lightmap;

    int size_x;
    int size_z;

    public float GetHeight(int x, int y)
    {
        if (x >= 0 && x < size_x && y >= 0 && y < size_z)
        {
            return heights[x, y];
        }
        else
        {
            return 1.0f/0.0f;
        }
    }
    
    public float GetHeightDif(int x, int y)
    {
        if (x >= 0 && x < size_x && y >= 0 && y < size_z)
        {
            return heights_dif[x, y];
        }
        else
        {
            return 1.0f / 0.0f;
        }
    }

    public int GetTexture(int x, int y)
    {
        if (x >= 0 && x < size_x && y >= 0 && y < size_z)
        {
            return textures[x, y];
        }
        else
        {
            return -1;
        }
    }

    public float GetWaterDistance(int x, int y)
    {
        if (x >= 0 && x < size_x && y >= 0 && y < size_z)
        {
            return water_dist[x, y];
        }
        else
        {
            return 1.0f / 0.0f;
        }
    }

    public bool IsWater(int x, int y)
    {
        if (x >= 0 && x < size_x && y >= 0 && y < size_z)
        {
            return is_water[x, y];
        }
        else
        {
            return true;
        }
    }

    public float GetLight(int x, int y)
    {
        if (x >= 0 && x < size_x && y >= 0 && y < size_z)
        {
            return lightmap[x, y];
        }
        else
        {
            return 1.0f / 0.0f;
        }
    }


    float GetAngle(int x, int y)
    {
        if (is_water[x, y] == false)
        {
            float[,] dif = new float[2, 2];
            float[] angle = new float[2];

            for (int n = 0; n < 2; n++)
            {
                for (int m = 0; m < 2; m++)
                {
                    if (x + n >= size_x || y + m >= size_z)
                    {
                        dif[n, m] = heights[x, y];
                    }
                    else
                    {
                        dif[n, m] = heights[x + n, y + m];
                    }
                }
            }

            float a = Math.Abs(dif[0, 1] - dif[0, 0]);
            float b = (float)map_size_factor;
            float c = (float)Math.Sqrt(a * a + b * b);

            angle[0] = (float)(Math.Acos((double)((b * b + c * c - a * a) / (2 * b * c))) * 180.0f / Math.PI);

            if (dif[0, 1] < dif[0, 0])
            {
                angle[0] = -1 * angle[0];
            }

            a = Math.Abs(dif[1, 0] - dif[0, 0]);
            b = (float)map_size_factor;
            c = (float)Math.Sqrt(a * a + b * b);

            angle[1] = (float)(Math.Acos((double)((b * b + c * c - a * a) / (2 * b * c))) * 180.0f / Math.PI);

            if (dif[1, 0] < dif[0, 0])
            {
                angle[1] = -1 * angle[1];
            }

            if (angle[0] > angle[1])
            {
                return angle[0];
            }
            else
            {
                return angle[1];
            }
        }
        else
        {
            return -1.0f;
        }
    }

    float WaterDistance(int x, int y)
    {
        float dist;

        if (is_water[x, y] == false)
        {
            int r = 1;
            bool is_dist = false;

            if (size_x > size_z)
            {
                dist = size_x + 1;
            }
            else
            {
                dist = size_z + 1;
            }

            while (is_dist == false)
            {
                for (int n = -1 * r; n < r; n++)
                {
                    for (int m = -1 * r; m < r; m++)
                    {
                        if ((n != 0 && m != 0) && (x + n >= 0 && y + m >= 0 && x + n < size_x && y + m < size_z))
                        {
                            if (is_water[x + n, y + m] == true && Math.Sqrt(n * n + m * m) < dist)
                            {
                                dist = (float)Math.Sqrt(n * n + m * m);
                                is_dist = true;
                            }
                        }
                    }
                }

                r++;
            }
        }
        else
        {
            dist = 0.0f;
        }

        if (dist == size_x + 1 || dist == size_z + 1)
        {
            dist = -1.0f;
        }

        return dist;
    }

    // Use this for initialization
    void Start()
    {
        instance = this;
        terrain = gameObject.GetComponent<Terrain>();
        Vector3 size = terrain.terrainData.size;

        size_x = (int)size.x / map_size_factor;
        size_z = (int)size.z / map_size_factor;
        float water_height = water.transform.position.y;

        heights = new float[size_x, size_z];
        heights_dif = new float[size_x, size_z];
        textures = new int[size_x, size_z];
        is_water = new bool[size_x, size_z];
        water_dist = new float[size_x, size_z];
        lightmap = new float[size_x, size_z];

        String h = "";
        String g = "";

        for (int i = 0; i < size_x; i++)
        {
            for (int j = 0; j < size_z; j++)
            {
                Vector3 point = new Vector3(i * map_size_factor, size.y + 10, j * map_size_factor);

                heights[i, j] = terrain.SampleHeight(point);

                if (heights[i, j] < water_height)
                {
                    is_water[i, j] = true;
                    textures[i, j] = -1;
                }
                else
                {
                    is_water[i, j] = false;
                    textures[i, j] = GetMainTexture(point);
                }
            }
        }

        for (int i = 0; i < size_x; i++)
        {
            for (int j = 0; j < size_z; j++)
            {
                heights_dif[i, j] = GetAngle(i, j);
                water_dist[i, j] = WaterDistance(i, j);

                Vector3 point = new Vector3(i * map_size_factor, heights[i, j], j * map_size_factor);

                RaycastHit hit;
                if (is_water[i,j]==true || Physics.Raycast(sun.transform.position, point - sun.transform.position, out hit, Vector3.Distance(point, sun.transform.position)))
                {
                    lightmap[i, j] = 0;
                }
                else
                {
                    lightmap[i, j] = Vector3.Distance(point, sun.transform.position) * sun.GetComponent<Light>().intensity;
                }

                if (lightmap[i, j] > max_light)
                {
                    max_light = lightmap[i, j];
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private float[] GetTextureMix(Vector3 point)
    {
        Vector3 position = terrain.transform.position;
        // returns an array containing the relative mix of textures
        // on the main terrain at this world position.

        // The number of values in the array will equal the number
        // of textures added to the terrain.

        int mapX = (int)(((point.x - position.x) / terrain.terrainData.size.x) * terrain.terrainData.alphamapWidth);
        int mapZ = (int)(((point.z - position.z) / terrain.terrainData.size.z) * terrain.terrainData.alphamapHeight);

        float[, ,] splatmapData = terrain.terrainData.GetAlphamaps(mapX, mapZ, 1, 1);

        float[] cellMix = new float[splatmapData.GetUpperBound(2) + 1];

        for (int n = 0; n < cellMix.Length; n++)
        {
            cellMix[n] = splatmapData[0, 0, n];
        }
        return cellMix;
    }

    private int GetMainTexture(Vector3 point)
    {
        // returns the zero-based index of the most dominant texture
        // on the main terrain at this world position.
        float[] mix = GetTextureMix(point);

        float maxMix = 0;
        int maxIndex = 0;

        for (int n = 0; n < mix.Length; n++)
        {
            if (mix[n] > maxMix)
            {
                maxIndex = n;
                maxMix = mix[n];
            }
        }
        return maxIndex;
    }
}
