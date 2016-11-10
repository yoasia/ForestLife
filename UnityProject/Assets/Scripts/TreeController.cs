using UnityEngine;
using System.Collections;

public class TreeController : MonoBehaviour
{

    public bool selected = false;
    public Material newMaterialRef;
    Renderer rend;

    Ray ray;
    RaycastHit hit;
    // Use this for initialization
    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {


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
    }

    


}
