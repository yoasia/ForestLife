using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class TerrainManager : MonoBehaviour
{
    public static TerrainManager instance;

    public GameObject water;
    public GameObject sun;
    public int map_size_factor = 5;
    public float max_light = -1;
    public float max_gradient = 60.0f;
    public float irrigation_dry = 0.1f;
    public int irrigation_dry_time = 10;

    Terrain terrain;

    float[,] heights;
    float[,] heights_dif;

    int[,] textures;

    bool[,] is_water;
    float[,] water_dist;
    float[,] irrigation;
    float[,] fertility;
    float max_water_dist = -1;
    float max_light_dist = -1;

    float[,] lightmap;

    int size_x;
    int size_z;

    bool get_dry = false;
    private IEnumerator coroutine;

    public float GetHeight(int x, int z)
    {
        if (x >= 0 && x < size_x && z >= 0 && z < size_z)
        {
            return heights[x, z];
        }
        else
        {
            return 1.0f / 0.0f;
        }
    }

    public float GetHeightDif(int x, int z) //<-90, 90>, error -> 180
    {
        if (x >= 0 && x < size_x && z >= 0 && z < size_z)
        {
            return heights_dif[x, z];
        }
        else
        {
            return 1.0f / 0.0f;
        }
    }

    public int GetTexture(int x, int z) //<-1, 10>
    {
        if (x >= 0 && x < size_x && z >= 0 && z < size_z)
        {
            return textures[x, z];
        }
        else
        {
            return -1;
        }
    }

    public float GetWaterDistance(int x, int z) //<0, map_size>, error -> -1, water -> 0
    {
        if (x >= 0 && x < size_x && z >= 0 && z < size_z)
        {
            return water_dist[x, z];
        }
        else
        {
            return 1.0f / 0.0f;
        }
    }

    public bool IsWater(int x, int z)
    {
        if (x >= 0 && x < size_x && z >= 0 && z < size_z)
        {
            return is_water[x, z];
        }
        else
        {
            return true;
        }
    }

    public float GetLight(int x, int z) //<0, 10>, error -> -1
    {
        if (x >= 0 && x < size_x && z >= 0 && z < size_z)
        {
            return lightmap[x, z];
        }
        else
        {
            return 1.0f / 0.0f;
        }
    }

    public float GetIrrigation(int x, int z) //<0, 10>, error -> -1
    {
        if (x >= 0 && x < size_x && z >= 0 && z < size_z)
        {
            return irrigation[x, z];
        }
        else
        {
            return 1.0f / 0.0f;
        }
    }

    public float GetFertility(int x, int z) //<-1, 10>
    {
        if (x >= 0 && x < size_x && z >= 0 && z < size_z)
        {
            return fertility[x, z];
        }
        else
        {
            return 1.0f / 0.0f;
        }
    }

    public bool WaterArea(int x1, int z1, int x2, int z2, float strenght)
    {
        if (x1 >= 0 && x1 < size_x + 1 && z1 >= 0 && z1 < size_z + 1 &&
            x2 >= 0 && x2 < size_x + 1 && z2 >= 0 && z2 < size_z + 1)
        {
            int x_start, y_start;

            if (x2 > x1)
            {
                x_start = x1;
            }
            else
            {
                x_start = x2;
            }

            if (z2 > z1)
            {
                y_start = z1;
            }
            else
            {
                y_start = z2;
            }

            int x_dist = Math.Abs(x2 - x1);
            int y_dist = Math.Abs(z2 - z1);

            for (int i = 0; i < x_dist; i++)
            {
                for (int j = 0; j < y_dist; j++)
                {
                    if (irrigation[x_start + i, y_start + j] + strenght <= 10.0f)
                    {
                        irrigation[x_start + i, y_start + j] += strenght;
                    }
                }
            }

            return true;
        }
        else
        {
            return false;
        }
    }

    public void DryArea(int x1, int z1, int x2, int z2, float strenght)
    {
        if (x1 >= 0 && x1 < size_x + 1 && z1 >= 0 && z1 < size_z + 1 &&
            x2 >= 0 && x2 < size_x + 1 && z2 >= 0 && z2 < size_z + 1)
        {
            int x_start, y_start;

            if (x2 > x1)
            {
                x_start = x1;
            }
            else
            {
                x_start = x2;
            }

            if (z2 > z1)
            {
                y_start = z1;
            }
            else
            {
                y_start = z2;
            }

            int x_dist = Math.Abs(x2 - x1);
            int y_dist = Math.Abs(z2 - z1);

            for (int i = 0; i < x_dist; i++)
            {
                for (int j = 0; j < y_dist; j++)
                {
                    float start_irr;

                    if (water_dist == null)
                    {
                        start_irr = 0;
                    }
                    else
                    {
                        start_irr = GetStartIrrigation(x_start + i, y_start + j);
                    }

                    if (irrigation[x_start + i, y_start + j] - strenght >= start_irr)
                    {
                        irrigation[x_start + i, y_start + j] -= strenght;
                    }
                }
            }

            get_dry = true;
        }
        else
        {
            get_dry = false;
        }
    }

    public bool CanGrow(int x, int z)
    {
        bool result = false;

        if (IsWater(x, z) == false && Math.Abs(GetHeightDif(x, z)) <= max_gradient)
        {
            result = true;
        }

        return result;
    }

    public float GetTerrainFactor(int x, int z)
    {
        float result = 0F;

        if (CanGrow(x, z) == false)
        {
            return -1F;
        }
        else
        {
            result = GetTexture(x, z);
            result = result + 90 - Math.Abs(GetHeightDif(x, z));

            float light = GetLight(x, z);

            if (light > 0)
            {
                result = result + 100 - 100 * (light / max_light);
            }

            result = result * 1 / GetWaterDistance(x, z);
        }

        return result;
    }


    public float GetHeight(float x, float z)
    {
        return GetHeight(ConvertCoordinate(x), ConvertCoordinate(z));
    }

    public float GetHeightDif(float x, float z) //<-90, 90>, error -> 180
    {
        return GetHeightDif(ConvertCoordinate(x), ConvertCoordinate(z));
    }

    public int GetTexture(float x, float z) //<-1, 10>
    {
        return GetTexture(ConvertCoordinate(x), ConvertCoordinate(z));
    }

    public float GetWaterDistance(float x, float z) //<0, map_size>, error -> -1, water -> 0
    {
        return GetWaterDistance(ConvertCoordinate(x), ConvertCoordinate(z));
    }

    public bool IsWater(float x, float z)
    {
        return IsWater(ConvertCoordinate(x), ConvertCoordinate(z));
    }

    public float GetLight(float x, float z) //<0, 10>, error -> -1
    {
        return GetLight(ConvertCoordinate(x), ConvertCoordinate(z));
    }

    public float GetIrrigation(float x, float z) //<0, 10>, error -> -1
    {
        return GetIrrigation(ConvertCoordinate(x), ConvertCoordinate(z));
    }

    public float GetFertility(float x, float z) //<-1, 10>
    {
        return GetFertility(ConvertCoordinate(x), ConvertCoordinate(z));
    }

    public bool WaterArea(float x1, float z1, float x2, float z2, float strenght)
    {
        return WaterArea(ConvertCoordinate(x1), ConvertCoordinate(z1), ConvertCoordinate(x2), ConvertCoordinate(z2), strenght);
    }

    public void DryArea(float x1, float z1, float x2, float z2, float strenght)
    {
        DryArea(ConvertCoordinate(x1), ConvertCoordinate(z1), ConvertCoordinate(x2), ConvertCoordinate(z2), strenght);
    }

    public bool CanGrow(float x, float z)
    {
        return CanGrow(ConvertCoordinate(x), ConvertCoordinate(z));
    }

    public float GetTerrainFactor(float x, float z)
    {
        return GetTerrainFactor(ConvertCoordinate(x), ConvertCoordinate(z));
    }


    public IEnumerator Dry(int time)
    {
        yield return new WaitForSeconds(time);
        get_dry = false;
    }


    int ConvertCoordinate(float x)
    {
        return (int)(x / map_size_factor);
    }

    float GetAngle(int x, int z)
    {
        if (is_water[x, z] == false)
        {
            float[,] dif = new float[2, 2];
            float[] angle = new float[2];

            for (int n = 0; n < 2; n++)
            {
                for (int m = 0; m < 2; m++)
                {
                    if (x + n >= size_x || z + m >= size_z)
                    {
                        dif[n, m] = heights[x, z];
                    }
                    else
                    {
                        dif[n, m] = heights[x + n, z + m];
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
            return 180.0f;
        }
    }

    float WaterDistance(int x, int z)
    {
        float dist;

        if (is_water[x, z] == false)
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
                        if ((n != 0 && m != 0) && (x + n >= 0 && z + m >= 0 && x + n < size_x && z + m < size_z))
                        {
                            if (is_water[x + n, z + m] == true && Math.Sqrt(n * n + m * m) < dist)
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

    float GetStartIrrigation(int x, int z) //<0, 10>, error -> -1
    {
        float irr;

        if (is_water[x, z] == false)
        {
            irr = 10.0f * water_dist[x, z] / max_water_dist;
        }
        else
        {
            irr = -1.0f;
        }

        return irr;
    }

    float GetTextureFertility(int x, int z)
    {
        try
        {
            SplatPrototype main_texture = terrain.terrainData.splatPrototypes[textures[x, z]];
            String main_texture_name = main_texture.texture.name;

            if (main_texture_name.Contains("rock"))
            {
                return 0.0f;
            }
            else if (main_texture_name.Contains("grass"))
            {
                return 10.0f;
            }
            else if (main_texture_name.Contains("sand"))
            {
                return 4.0f;
            }
            else
            {
                return 5.0f;
            }
        }
        catch (Exception e)
        {
            return -1.0f;
        }
    }

    void AdjustLight()
    {
        for (int i = 0; i < size_x; i++)
        {
            for (int j = 0; j < size_z; j++)
            {
                lightmap[i, j] = 10.0f * lightmap[i, j] / max_light_dist;
            }
        }
    }


    void Start()
    {
        instance = this;

        terrain = gameObject.GetComponent<Terrain>();
        Vector3 size = terrain.terrainData.size;

        size_x = ConvertCoordinate(size.x);
        size_z = ConvertCoordinate(size.z);
        float water_height;

        heights = new float[size_x, size_z];
        heights_dif = new float[size_x, size_z];
        textures = new int[size_x, size_z];
        is_water = new bool[size_x, size_z];
        water_dist = new float[size_x, size_z];
        lightmap = new float[size_x, size_z];
        irrigation = new float[size_x, size_z];
        fertility = new float[size_x, size_z];

        max_water_dist = -1;

        if (water != null)
        {
            water_height = water.transform.position.y;
        }
        else
        {
            water_height = 0;
            water_dist = null;
            Debug.LogError("ERROR: No water!", terrain);
        }

        if (water_height < terrain.transform.position.y)
        {
            water_dist = null;
            Debug.LogError("ERROR: Water below terrain!", terrain);
        }

        for (int i = 0; i < size_x; i++)
        {
            for (int j = 0; j < size_z; j++)
            {
                Vector3 point = new Vector3(i * map_size_factor, size.y + 10, j * map_size_factor);

                heights[i, j] = terrain.SampleHeight(point);

                if (heights[i, j] + terrain.transform.position.y < water_height)
                {
                    is_water[i, j] = true;
                    textures[i, j] = -1;
                    fertility[i, j] = -1;
                }
                else
                {
                    is_water[i, j] = false;
                    textures[i, j] = GetMainTexture(point);
                    fertility[i, j] = GetTextureFertility(i, j);
                }
            }
        }

        for (int i = 0; i < size_x; i++)
        {
            for (int j = 0; j < size_z; j++)
            {
                heights_dif[i, j] = GetAngle(i, j);

                if (water_dist != null)
                {
                    water_dist[i, j] = WaterDistance(i, j);

                    if (water_dist[i, j] > max_water_dist)
                    {
                        max_water_dist = water_dist[i, j];
                    }
                }

                Vector3 point = new Vector3(i * map_size_factor, heights[i, j], j * map_size_factor);

                RaycastHit hit;
                if (is_water[i, j] == true || Physics.Raycast(sun.transform.position, point - sun.transform.position, out hit, Vector3.Distance(point, sun.transform.position)))
                {
                    lightmap[i, j] = 0;
                }
                else
                {
                    lightmap[i, j] = Vector3.Distance(point, sun.transform.position) * sun.GetComponent<Light>().intensity;
                }

                if (lightmap[i, j] > max_light_dist)
                {
                    max_light_dist = lightmap[i, j];
                }

                if (lightmap[i, j] > max_light)
                {
                    max_light = lightmap[i, j];
                }
            }
        }

        AdjustLight();

        if (water_dist != null)
        {
            for (int i = 0; i < size_x; i++)
            {
                for (int j = 0; j < size_z; j++)
                {
                    irrigation[i, j] = GetStartIrrigation(i, j);
                }
            }
        }

        coroutine = Dry(irrigation_dry_time);
        StartCoroutine(coroutine);
    }

    void Update()
    {
        if (get_dry == false)
        {
            DryArea(0, 0, size_x * map_size_factor, size_z * map_size_factor, irrigation_dry);

            coroutine = Dry(irrigation_dry_time);
            StartCoroutine(coroutine);
        }
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

        float[, ,] splatmapData = terrain.terrainData.GetAlphamaps(mapX, mapZ, map_size_factor, map_size_factor);

        float[] cellMix = new float[splatmapData.GetUpperBound(2) + 1];

        for (int n = 0; n < cellMix.Length; n++)
        {
            cellMix[n] = splatmapData[0, 0, n];
        }

        return cellMix;
    }

    private int GetMainTexture(int x, int z)
    {
        return GetMainTexture(new Vector3(x, 0, z));
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
