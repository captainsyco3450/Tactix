using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class destroyobj : MonoBehaviour {
    public GameObject smoke;
    public int x = 0;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (x > 1)
        {
            Destroy(this.gameObject);
        }
        x++;
	}
}
