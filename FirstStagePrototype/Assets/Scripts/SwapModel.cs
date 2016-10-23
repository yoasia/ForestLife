using UnityEngine;
using System.Collections;

public class SwapModel : MonoBehaviour
{

    public GameObject tree;
    public GameObject newModel;



    private bool mSwapModel = false;


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (mSwapModel && tree != null)
        {
            SwapingModel();
            mSwapModel = false;
        }
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(50, 50, 120, 40), "Swap Model"))
        {
            mSwapModel = true;
            
        }
    }

    private void SwapingModel()
    {

        GameObject trackableGameObject = tree;
        Vector3 pos = new Vector3(0,0,0);
        //disable any pre-existing augmentation
        for (int i = 0; i < trackableGameObject.transform.childCount; i++)
        {
            Transform child = trackableGameObject.transform.GetChild(i);
            pos = child.transform.position;
            child.gameObject.SetActive(false);
            
        }

       
        GameObject bigTree = (GameObject)Instantiate(newModel, pos, Quaternion.Euler(0, 0, 0));

        // Re-parent the cube as child of the trackable gameObject
        bigTree.transform.parent = tree.transform;

        
        // Make sure it is active
        bigTree.SetActive(true);
    }
}
