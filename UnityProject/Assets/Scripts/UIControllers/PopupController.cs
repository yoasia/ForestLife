using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PopupController : MonoBehaviour {


    public GameObject goodLandingPopup, badLandingPopup;
    bool goodPopVisable = false, badPopVisable = false;
    private Animator animatorGoodLanding;
    private Animator animatorBadLanding;
    public Text tt;

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
        badLandingPopup.SetActive(false);
        //goodPopVisable = true;
        goodLandingPopup.SetActive(true);
        animatorGoodLanding.enabled = true;
        animatorGoodLanding.Play("GoodLandPopupIn");
        
    }
    public void GoodLandingPopupOff()
    {
        
        if (goodLandingPopup.activeSelf)
        {
            animatorGoodLanding.Play("GoodLandPopupOut");
            //StartCoroutine(Wait(300.0f));
            //goodPopVisable = false;
            goodLandingPopup.SetActive(false);
        }
    }

    public void BadLandingPopupOn(string s)
    {
        goodLandingPopup.SetActive(false);
        //badPopVisable = true;
        badLandingPopup.SetActive(true);
        animatorBadLanding.enabled = true;
        tt.text = s;
        animatorBadLanding.Play("BadLandPopupIn");
        
    }
    public void BadLandingPopupOff()
    {
        if (badLandingPopup.activeSelf)
        {
            animatorBadLanding.Play("BadLandPopupOut");
            //badPopVisable = false;
            badLandingPopup.SetActive(false);
        }
    }

    private IEnumerator Wait(float seconds)
    {
       
        yield return new WaitForSeconds(seconds);
        
    }
}
