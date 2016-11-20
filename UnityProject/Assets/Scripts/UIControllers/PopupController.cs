using UnityEngine;
using System.Collections;

public class PopupController : MonoBehaviour {


    public GameObject goodLandingPopup, badLandingPopup;
    bool goodPopVisable = false, badPopVisable = false;
    private Animator animatorGoodLanding;
    private Animator animatorBadLanding;

	// Use this for initialization
	void Start () {

        animatorGoodLanding = goodLandingPopup.GetComponent<Animator>();
        animatorGoodLanding.enabled = false;
        animatorBadLanding = badLandingPopup.GetComponent<Animator>();
        animatorBadLanding.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {

        //if (!badPopVisable)
        //{
        //    badLandingPopup.SetActive(false);
        //}
        //if (!goodPopVisable)
        //{
        //    goodLandingPopup.SetActive(false);
        //}
	}

    public void GoodLandingPopupOn()
    {
        //goodPopVisable = true;
        goodLandingPopup.SetActive(true);
        animatorGoodLanding.enabled = true;
        animatorGoodLanding.Play("GoodLandPopupIn");
        
    }
    public void GoodLandingPopupOff()
    {
        animatorGoodLanding.Play("GoodLandPopupOut");
        //StartCoroutine(Wait(300.0f));
        //goodPopVisable = false;
        goodLandingPopup.SetActive(false);
    }

    public void BadLandingPopupOn()
    {
        //badPopVisable = true;
        badLandingPopup.SetActive(true);
        animatorBadLanding.enabled = true;
        animatorBadLanding.Play("BadLandPopupIn");
        
    }
    public void BadLandingPopupOff()
    {
        animatorBadLanding.Play("BadLandPopupOut");
        //badPopVisable = false;
        badLandingPopup.SetActive(false);
    }

    private IEnumerator Wait(float seconds)
    {
       
        yield return new WaitForSeconds(seconds);
        
    }
}
