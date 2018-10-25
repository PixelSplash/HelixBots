using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupController : MonoBehaviour {
    public GameObject PopupTextParent;
    private GameObject text;
	// Use this for initialization
	void Start () {

    }
	
	// Update is called once per frame
	void Update () {
		
	}
    public void showText(float x, float y, string ptext)
    {
        text = Instantiate(PopupTextParent);
        Vector3 newPosition = Camera.main.WorldToScreenPoint(new Vector3(x, y, 0f));
        //Debug.Log(x.ToString() + " " + y.ToString());
        text.transform.SetParent(transform, false);
        text.transform.position = newPosition;
        //text.transform.localPosition = new Vector3(x,y,0f);
        text.transform.Find("PopupText").gameObject.GetComponent<Text>().text = ptext;
        //Debug.Log(PopupTextParent.transform.localPosition.x + " " + PopupTextParent.transform.localPosition.y.ToString());
        StartCoroutine(hideText());
    }
    private IEnumerator hideText()
    {
        yield return new WaitForSeconds(1f);
        Destroy(text);
    }
}
