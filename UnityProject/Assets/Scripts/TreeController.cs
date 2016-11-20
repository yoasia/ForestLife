using UnityEngine;
using System.Collections;

public class TreeController : MonoBehaviour
{

    public bool selected = false;
    public int korzen, kora, liscie, lp;
    Renderer rend;

    // Use this for initialization
    void Start()
    {
        rend = GetComponent<Renderer>();
        rend.material.shader = Shader.Find("Standard");
    }

    // Update is called once per frame
    void Update()
    {


        
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

}
