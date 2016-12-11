using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PopupController : MonoBehaviour {


    public GameObject goodLandingPopup, badLandingPopup;
    private Animator animatorGoodLanding;
    private Animator animatorBadLanding;
    public Text soilInfo;

	
	void Start () {

        animatorGoodLanding = goodLandingPopup.GetComponent<Animator>();
        animatorGoodLanding.enabled = false;
        animatorBadLanding = badLandingPopup.GetComponent<Animator>();
        animatorBadLanding.enabled = false;
	}
	
	
	void Update () {

	}

    public void GoodLandingPopupOn(string soil)
    {
        badLandingPopup.SetActive(false);
        goodLandingPopup.SetActive(true);
        animatorGoodLanding.enabled = true;
        soilInfo.text = soil;
        animatorGoodLanding.Play("GoodLandPopupIn");

        
    }
    public void GoodLandingPopupOff()
    {
        
        if (goodLandingPopup.activeSelf)
        {
            animatorGoodLanding.Play("GoodLandPopupOut");
            goodLandingPopup.SetActive(false);
        }
    }

    public void BadLandingPopupOn()
    {
        goodLandingPopup.SetActive(false);
        badLandingPopup.SetActive(true);
        animatorBadLanding.enabled = true;
        animatorBadLanding.Play("BadLandPopupIn");
        
    }
    public void BadLandingPopupOff()
    {
        if (badLandingPopup.activeSelf)
        {
            animatorBadLanding.Play("BadLandPopupOut");
            badLandingPopup.SetActive(false);
        }
    }

    private IEnumerator Wait(float seconds)
    {
       
        yield return new WaitForSeconds(seconds);
        
    }
}
