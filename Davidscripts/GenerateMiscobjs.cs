using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateMiscobjs : MonoBehaviour {

    public int environmentType; //0 = office, 1 = desert..
    public GameObject[] miscObjects;
    public int rangex;
    public int rangey;
    public float curx = 0;
    public float curz = 0;
    public float cury = 0;
    public int x;



    // Use this for initialization
    void Start () {
        GetEnvironment();
       GameObject misc = Instantiate(miscObjects[Random.Range(rangex, rangey)],new Vector3 (transform.position.x,0.0f,transform.position.z), transform.rotation);
       // misc.transform.tag = "furniture";
      
          
	}
	
	// Update is called once per frame
	void Update () {

        if (x >= 20)
        {
            Destroy(gameObject);
        }
        if (x <= 20)
        {
            x++;
        }
    }

    public void GetEnvironment()
    {
        switch (environmentType)
        {
            case 0:
                rangex = 0;
                rangey = 40;
                break;
        }
    }
}
