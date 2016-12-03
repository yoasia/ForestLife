using UnityEngine;
using System.Collections;
using System;

public class TreeController : MonoBehaviour
{
    public string species;

    public float rootsStrength = 1F;
    public float barkStrength = 1F;
    public float leavesStrength = 1F;

    public float upgradePoints = 0F;
    public float maxUpgradePoints = 100F;
    public float healthPoints = 10F;
    public float baseMaxHealthPoints = 100F;
    public float baseGrowthDemand = 1F;

    public float size;

    public float growthRate = 1F;
    public float maxGrowth = 1F;

    public float badTerrainFactor = 1F;
    public float goodTerrainFactor = 1F;

    public float minWaterLevel = 6F;
    public float maxWaterLevel = 8F;
    public float sunFactor = 1F;

    public int minSeeds = 2;
    public int maxSeeds = 6;
    public float maxSowDistance = 10f;
    public float minTreeDistance = 1f;

    public float timeBetweenSow = 120F;
    public int sowsLeft = 4;

    public bool selected = false;

    public float growthThreshold = 2F;

    public Mesh grownTreeMesh;
    public Mesh deadSmallMesh;
    public Mesh deadMesh;

    public float sun;
    public float soil;
    public float water;
    public float growth;

    Renderer rend;

    Ray ray;
    RaycastHit hit;

    private float startTime;
    private float lastGrowth;
    private float lastSow;
    private float timeToSow;
    private float timeBetweenGrowth = 1;
    private float growthRatePerSecond = 1F / 100;

    private float soilMid = 5;

    public bool isAlive = true;

    public float Age { get { return Time.time - startTime; } }

    private Color defaultColour;

    // Use this for initialization
    void Start()
    {
        startTime = Time.time;
        lastGrowth = startTime;
        lastSow = Age;
        timeToSow = timeBetweenSow * UnityEngine.Random.Range(0.75F, 1.25F);
        rend = GetComponent<Renderer>();
        rend.material.shader = Shader.Find("Standard");
        defaultColour = rend.material.color;
    }

    // Update is called once per frame
    void Update()
    {
        if (isAlive)
        {
            if (Time.time - lastGrowth > timeBetweenGrowth)
                Grow();

            if (size > growthThreshold)
                ChangeModel(grownTreeMesh);

            transform.localScale = new Vector3(size, size, size);

            if (selected)
            {
                if (rend != null)
                {
                    rend.material.shader = Shader.Find("Self-Illumin/Outlined Diffuse");
                }

            }

            if(sowsLeft > 1 && Age - lastSow > timeToSow)
                Sow();

            if (healthPoints <= 0)
                Kill();
        }
    }

    void ChangeModel(Mesh newMesh)
    {
        GetComponent<MeshFilter>().mesh = newMesh;
    }

    public void SelectTree()
    {
        selected = true;

        if (rend != null)
        {
            rend.material.shader = Shader.Find("Self-Illumin/Outlined Diffuse");
        }
    }

    public void DeselectTree()
    {
        selected = false;

        if (rend != null)
        {
            rend.material.shader = Shader.Find("Standard");
        }
    }

    //funkcja przyjmuje wartość od 0 do 1
    //0 - chore drzewo
    //1 - zdrowe drzewo
    public bool ChangeColor(float value)
    {

        if (value < 0 || value > 1)
            return false;

        Renderer rend = GetComponent<Renderer>();
        rend.material.shader = Shader.Find("Standard");
        Color newColor = defaultColour;

        if (species == "Magnolia")
        {
            newColor.b = 1 * (1f-value);
            rend.material.color = newColor;
        }
        else if (species == "Birch")
        {
            newColor.r = 1 * value;
            rend.material.color = newColor;
        }
        else if (species == "Spruce")
        {
            newColor.r = 1 * value;
            rend.material.color = newColor;
        }
        return true;
    }

    public void ReturnDefaultColour()
    {
        rend.material.color = defaultColour;
    }

    public bool CanBeUpgraded(int rootsUpgrade, int leavesUpgrade, int barkUpgrade)
    {
        if (GetUpgradesCost(rootsUpgrade, leavesUpgrade, barkUpgrade) <= upgradePoints)
            return true;

        return false;
    }

    public bool Upgrade(int rootsUpgrade, int leavesUpgrade, int barkUpgrade)
    {
        float cost = GetUpgradesCost(rootsUpgrade, leavesUpgrade, barkUpgrade);

        if (cost > upgradePoints)
            return false;

        rootsStrength += rootsUpgrade;
        leavesStrength += leavesUpgrade;
        barkStrength += barkUpgrade;
        upgradePoints -= cost;

        return true;
    }

    public void Kill()
    {
        if (size < growthThreshold)
            ChangeModel(deadSmallMesh);
        else
            ChangeModel(deadMesh);
        isAlive = false;
    }

    private float GetUpgradesCost(int rootsUpgrade, int leavesUpgrade, int barkUpgrade)
    {
        return rootsUpgrade + leavesUpgrade + barkUpgrade;
    }

    private void Grow()
    {
        float x = transform.position.x;
        float z = transform.position.z;
        //TO DO pobieranie jakości gleby, wody i nasłonecznienia z terenu
        //float soil = 5; // 0 - 10
        //float sun = 1;  // 0 - 1
        //float water = 5;    // 0 - 10
        soil = GameManager.instance.terrainManager.GetFertility(x, z); // 0 - 10
        sun = 1F;
        //sun = GameManager.instance.terrainManager.GetLight(x, z) / 10;  // 0 - 1
        water = 10 - GameManager.instance.terrainManager.GetIrrigation(x, z);    // 0 - 10
        
        //Debug.LogFormat("Soil: {0}; Sun: {1}; Water: {2}", soil, sun, water);

        //growth;// = 1F;
        if (soil < soilMid)
            growth = soilMid - badTerrainFactor * (soilMid - soil);
        else
            growth = soilMid + goodTerrainFactor * (soil - soilMid);

        growth *= 1 + rootsStrength / 10;

        sun *= sunFactor;
        if (sun > 1)
            sun = 1;
        sun *= 1 + leavesStrength / 10;
        growth *= sun;
        growth *= GetShadeFactor();

        if (water < minWaterLevel)
            growth *= water / minWaterLevel;
        if (water > maxWaterLevel)
            growth /= water / maxWaterLevel;

        growth *= (Time.time - lastGrowth);

        float growthDemand = baseGrowthDemand + baseGrowthDemand * Mathf.Pow((size - 1),2) / 3;

        growth = growth - growthDemand;

        //Debug.LogFormat("Growth: {0}, Demand: {1}", growth, growthDemand);

        if (growth > maxGrowth)
            growth = maxGrowth;

        growth *= growthRatePerSecond * growthRate;

        if (growth > 0)
        {
            upgradePoints += growth*20;
            if (upgradePoints > maxUpgradePoints)
                upgradePoints = maxUpgradePoints;

            size += growth;
        }

        //if(growth > 0 && growth < - 0.3F)
        healthPoints += (growth)*10;
        if (healthPoints > baseMaxHealthPoints + barkStrength * 10)
            healthPoints = baseMaxHealthPoints + barkStrength * 10;

        lastGrowth = Time.time;

        //Debug.LogFormat("Size: {0}; Health: {1}; Upgrade Points: {2}", size, healthPoints, upgradePoints);
    }

    private float GetShadeFactor()
    {
        float shadeFactor = 1;
        for(int i = 0; i < GameManager.instance.treesOnIsland.Count; i++)
        {
            var other = GameManager.instance.treesOnIsland[i];
            var otherTree = other.GetComponent<TreeController>();
            if (otherTree.isAlive && otherTree.size > size)
            {
                var tempFactor = DistanceTo(other) / (otherTree.size*4);
                if (tempFactor < 1)
                    shadeFactor *= tempFactor;
            }
        }
        return shadeFactor;
    }

    void Sow()
    {
        System.Random random = new System.Random();
        int seeds = random.Next(minSeeds, maxSeeds + 1);

        for (int i = 0; i < seeds; i++)
        {
            float distance = minTreeDistance + (maxSowDistance - minTreeDistance) * (float)random.NextDouble();
            double rad_angle = 2 * Math.PI * random.NextDouble();

            float new_x;
            float new_z;

            new_x = distance * (float)Math.Cos(rad_angle) + gameObject.transform.position.x + GameManager.Wind.x;
            new_z = distance * (float)Math.Sin(rad_angle) + gameObject.transform.position.z + GameManager.Wind.y;

            if (GameManager.instance.TreeDistance(new_x, new_z) >= minTreeDistance)
            {
                GameManager.instance.seedLanding(new_x, new_z, species, true);
            }
        }
        lastSow = Age;
        sowsLeft--;
        timeToSow = timeBetweenSow * UnityEngine.Random.Range(0.75F, 1.25F);
    }

    public float DistanceTo(GameObject other)
    {
        return Mathf.Sqrt(Mathf.Pow(transform.position.x - other.transform.position.x, 2) + Mathf.Pow(transform.position.z - other.transform.position.z, 2));
    }
}