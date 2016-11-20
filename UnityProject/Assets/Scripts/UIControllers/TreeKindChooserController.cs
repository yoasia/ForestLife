using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class TreeKindChooserController : MonoBehaviour {

    public Image TreeImage;
    public Text TreeNameText;
    public List<TreeKind> treeKindList = new List<TreeKind>();
    public Sprite s1Brzoza, s2Magnolia, s3Swierk;
    public struct TreeKind
    {
        public Sprite image;
        public string name;
    }

    

    TreeKind chosenTree;

	// Use this for initialization
	void Start () {

        TreeKind t1, t2, t3;
        t1.name = "Brzoza brodawkowata";
        t1.image = s1Brzoza;
        t2.name = "Magnolia pośrednia";
        t2.image = s2Magnolia;
        t3.name = "Świerk pospolity";
        t3.image = s3Swierk;

        treeKindList.Add(t1);
        treeKindList.Add(t2);
        treeKindList.Add(t3);

        if(treeKindList.Count != 0) 
        {
            chosenTree = treeKindList[0];
            TreeImage.sprite = chosenTree.image;
            TreeNameText.text = chosenTree.name;

        }
        
	}
	
	// Update is called once per frame
	void Update () {
	
	}


    public void MoveNextTreeKind(Button b)
    {
        int index = treeKindList.FindIndex(m => m.name == chosenTree.name);
        if (b.name == "NextButton")
        {
            if (index < treeKindList.Count - 1)
            {
                chosenTree = treeKindList[index + 1];
            }
            else
            {
                chosenTree = treeKindList[0];
            }
        }
        else
        {
            if (index > 0)
            {
                chosenTree = treeKindList[index - 1];

            }
            else
            {
                chosenTree = treeKindList[treeKindList.Count - 1];
            }
        }
        DisplayCurrentTree();
    }

    public void DisplayCurrentTree()
    {
        TreeImage.sprite = chosenTree.image;
        TreeNameText.text = chosenTree.name;
    }

    public string ReturnChosenKind()
    {
        return chosenTree.name;
    }

}
