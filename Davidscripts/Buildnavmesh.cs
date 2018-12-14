using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Buildnavmesh : MonoBehaviour
{

    public List<NavMeshSurface> navsurf;



    public int x;
    // Use this for initialization
    void Start()
    {
        x = 0;
    }



    // Update is called once per frame
    void Update()
    {
        if (x < 1)
        {
            StartCoroutine(waitFor(1));
            
        }
        x++;
    }

    IEnumerator waitFor(float x)
    {
        GameObject[] gos2 = GameObject.FindGameObjectsWithTag("Nav");
        foreach (GameObject go in gos2)
        {
            if (go.GetComponent<NavMeshSurface>())
            {
                navsurf.Add(go.GetComponent<NavMeshSurface>());
            }

        }
        for (int j = 0; j < navsurf.Count; j++)
        {
            navsurf[j].BuildNavMesh();
        }

        yield return new WaitForSeconds(x);

        GameObject[] gos3 = GameObject.FindGameObjectsWithTag("Nav");
        foreach (GameObject go in gos3)
        {
            if (go.GetComponent<NavMeshSurface>())
            {
                navsurf.Add(go.GetComponent<NavMeshSurface>());
            }

        }
        for (int j = 0; j < navsurf.Count; j++)
        {
            navsurf[j].BuildNavMesh();
        }

    }
}
