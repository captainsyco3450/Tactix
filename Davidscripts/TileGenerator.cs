using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TileGenerator : MonoBehaviour
{

    //the tiles
    public GameObject WallNS;
    public GameObject WallEW;
    public GameObject WallSWE;
    public GameObject WallSE;
    public GameObject WallSW;
    public GameObject WindowsEW;
    public GameObject WindowsNS;
    public GameObject WallNE;
    public GameObject WallWN;
    public GameObject Blank;
    public GameObject Generator;
    public GameObject[] tiles;
    public List<GameObject> compatibleTiles;
    public GameObject currentTile;
    public GameObject nextTile;
    public Vector3 currentposition;
    public Vector3 originPosition;
    public float curx = 0;
    public float curz = 0;
    public float cury = 0;
    public ConnectorCheck ccheck;
    public ConnectorCheck currentcheck;
    public TileGenerator tilegen;
    public int maxTiles;
    public int count;
    public int countx;
    public int countz;
    public int maxX;
    public int maxZ;
    public string currentGrid;
    public int length;
    public bool genSpawned;

    // Use this for initialization
    void Start()
    {
        countx = 0;
        //countz = 0;
        currentGrid = "x";
        currentTile = gameObject;
        currentTile.transform.position = transform.position;
        currentcheck = GetComponent<ConnectorCheck>();
        currentposition = transform.position;
        originPosition = transform.position;
        length = compatibleTiles.Count;
        
    }

    // Update is called once per frame
    void Update()
    {
        originPosition = transform.position;
        {

            if (countx < maxX)
            {
                getCompatability();

                nextTile = Instantiate(compatibleTiles[Random.Range(0, length)]);
                nextTile.transform.position = currentposition;
                curx = currentTile.transform.position.x + 10;
                curz = currentTile.transform.position.z;
                cury = currentTile.transform.position.y;
                nextTile.transform.position = new Vector3(curx, cury, curz);
                countx += 1;
                currentTile = nextTile;
                count++;
            }
        }
       
    }


    public void getCompatability()
    {
        foreach (GameObject t in tiles)
        {
            ccheck = t.transform.GetComponent<ConnectorCheck>();
            if (ccheck.north == currentcheck.south || ccheck.south == currentcheck.north || ccheck.east == currentcheck.west || ccheck.west == currentcheck.east)
            {
                ccheck.isCompatible = true;
            }
        }
        length = 0;
        foreach (GameObject t in tiles)
        {

            ccheck = t.GetComponent<ConnectorCheck>();
            if (ccheck.isCompatible == true)
            {
                length++;
                compatibleTiles.Add(t);
            }
        }
    }
}

           