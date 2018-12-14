using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class clearblanktiles : MonoBehaviour {
    public int x;
	// Use this for initialization
	void Start () {
        x = 0;
	}
	
	// Update is called once per frame
	void Update () {
        Destroy(gameObject.GetComponent<BoxCollider>());
	}

        void OnCollisionEnter(Collision col)
    {
            if (col.gameObject.tag == "walkable")
            {
                Destroy(col.gameObject);
            }
            {
            //Destroy(col.gameObject);
        }
    }
}
