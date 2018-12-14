using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class muzzleflash1 : MonoBehaviour {
    int x = 0;
    public bool iscasing;
    public bool ismagdrop;
    public GameObject casing;
    public GameObject magdrop;
	// Use this for initialization
	void Start () {
        this.gameObject.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
		if (this.isActiveAndEnabled == true)
        {
            if (iscasing == true)
            {
                Instantiate(casing,transform.position,transform.rotation);
                this.gameObject.SetActive(false);

                x++;
            }
            if (ismagdrop == true)
            {
                Instantiate(magdrop,transform.position, transform.rotation);
                this.gameObject.SetActive(false);
                x +=1;
            }
            if (x > 1)
            {
                this.gameObject.SetActive(false);
                x = 0;
            }
            x++;
        }
	}
}
