using UnityEngine;
using System.Collections;

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

    public float maxGrowth = 1F;

    public float badTerrainFactor = 1F;
    public float goodTerrainFactor = 1F;

    public float minWaterLevel = 6F;
    public float maxWaterLevel = 8F;
    public float sunFactor = 1F;

    public bool selected = false;
    public Material newMaterialRef;
    Renderer rend;

    Ray ray;
    RaycastHit hit;

    private float startTime;
    private float lastGrowth;
    private float timeBetweenGrowth = 1;
    private float growthRatePerSecond = 1F / 100;

    private float soilMid = 5;
        
    public float Age { get { return Time.time - startTime; } }
    
    // Use this for initialization
    void Start()
    {
        startTime = Time.time;
        lastGrowth = startTime;
        rend = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - lastGrowth > timeBetweenGrowth)
            Grow();

        if (selected)
        {
			Debug.Log ("dzrewozaznaczone");
            if (rend != null)
            {
                rend.material.shader = Shader.Find("Self-Illumin/Outlined Diffuse");
            }

        }
        else
        {
            if (rend != null)
            {
                rend.material.shader = Shader.Find("Diffuse");
            }
        }

        if (healthPoints <= 0)
            Kill();
    }

    public bool CanBeUpgraded(int rootsUpgrade, int leavesUpgrade, int barkUpgrade)
    {
        if(GetUpgradesCost(rootsUpgrade, leavesUpgrade, barkUpgrade) <= upgradePoints)
            return false;
        return true;
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
        Destroy(gameObject);
        //TO DO: może dodać lepszą śmierć drzewa
    }

    private float GetUpgradesCost(int rootsUpgrade, int leavesUpgrade, int barkUpgrade)
    {
        return rootsUpgrade + leavesUpgrade + barkUpgrade;
    }

    private void Grow()
    {
        //TO DO pobieranie jakości gleby, wody i nasłonecznienia z terenu
        float soil = 5; // 0 - 10
        float sun = 1;  // 0 - 1
        float water = 5;    // 0 - 10
        //

        float growth;// = 1F;
        if (soil < soilMid)
            growth = soilMid - badTerrainFactor * (soilMid - soil);
        else
            growth = soilMid + goodTerrainFactor * (soil - soilMid);

        sun *= sunFactor;
        if (sun > 1)
            sun = 1;
        sun *= 1 + leavesStrength / 10;
        growth *= sun;

        if (water < minWaterLevel)
            growth *= water / minWaterLevel;
        if (water > maxWaterLevel)
            growth /= water / maxWaterLevel;

        growth *= (Time.time - lastGrowth);

        float growthDemand = baseGrowthDemand + baseGrowthDemand * (size - 1)/5;

        growth = growth - growthDemand;

        Debug.LogFormat("Growth: {0}, Demand: {1}", growth, growthDemand);

        if (growth > maxGrowth)
            growth = maxGrowth;

        growth *= growthRatePerSecond;

        if (growth > 0)
        {
            upgradePoints += growth;
            if (upgradePoints > maxUpgradePoints)
                upgradePoints = maxUpgradePoints;

            size += growth;
        }

        healthPoints += growth;

        lastGrowth = Time.time;

        Debug.LogFormat("Size: {0}; Health: {1}; Upgrade Points: {2}", size, healthPoints, upgradePoints);
    }
}
