using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GenerateEnvironment : MonoBehaviour
{

    // Use this for initialization
    //the tiles
    public int tier;
    public GameObject[] tilesx;
    public GameObject[] tilesz;
    public GameObject[] buildings;
    public GameObject currentTile;
    public GameObject currentTile2;
    public GameObject nextTile;
    public float curx;
    public float curz;
    public float cury;
    public float curx2;
    public float curz2;
    public float cury2;
    public int rangex;
    public int rangey;
    public int maxTiles;
    public int countx;
    public int countz;
    public int environmentType; //0 = office, 1 = desert..
    public int maxx;
    public int maxz;
    public int numBuildings;
    public int maxBuildings;

    public Vector3 currentpos;
    public Vector3 currentpos2;
    public Vector3 buildingpos;
    public Buildnavmesh bnm;


    // Use this for initialization
    void Start()
    {
        bnm = GetComponent<Buildnavmesh>();
        countx = 0;
        countz = 0;
        currentTile = gameObject;
        currentTile2 = gameObject;


    }

    // Update is called once per frame
    void Update()
    {
        if (countx < maxx)
        {
            
            curx = currentTile.transform.position.x + 10;
            cury = currentTile.transform.position.y;
            curz = currentTile.transform.position.z;
            currentpos = new Vector3(curx, cury, curz);
            countx++;
            placeTile();
        }
           if (countz < maxz)
        {
            
            curx2 = currentTile2.transform.position.x;
            cury2 = currentTile2.transform.position.y;
            curz2 = currentTile2.transform.position.z + 10;
            currentpos2 = new Vector3(curx2, cury2, curz2);
            countz++;

            placeTile2();
        }

        if (numBuildings < maxBuildings && countx >= maxx && countz >= maxz)
        {
            buildingpos = new Vector3(maxx * 10, 0.2f, maxz * 10);
            GetEnvironment();
            Instantiate(buildings[Random.Range(0,2)], (buildingpos), transform.rotation);
            numBuildings++;
            //bnm.bake = true;
        }

    }

    public void placeTile()
    {
       currentTile = Instantiate(tilesx[environmentType],(currentpos),transform.rotation);
    }
    public void placeTile2()
    {
        currentTile2 = Instantiate(tilesz[environmentType], (currentpos2), transform.rotation);
        currentTile2.GetComponent<GenerateEnvironment>().maxx = maxx;
    }

    public void GetEnvironment()
    {
        switch (environmentType)
        {
            case 0:
                rangex = 0;
                rangey = 2;
                break;
        }
    }

}
