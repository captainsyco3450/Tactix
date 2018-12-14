using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class navmeshtesterguy : MonoBehaviour
{

    // Use this for initialization
    NavMeshAgent agent;
    public GameObject enemy;
    public GameObject[] enemies;
    public List<GameObject> visibleenemies;
    public bool findcover;
    public string enemytag;
    public bool isWaiting;
    public bool isWaiting2;
    public float enemydistance;
    public float coverdistance;
    public gunscriptv1 gunscript;
    public GameObject gun;
    public bool trigger;
    public bool reload;
    int firemode;
    public float health;
    public Animator anim;
    public GameObject rag;
    public navmeshtesterguy enemyscript;
    public float aitoWaypointdistance;
    public float aitoWaypointdistance2;
    Vector3 newpos;
    Vector3 newpos2;
    public GameObject blood;
    public GameObject wallhit;
    public int x;

    void Start()
    {
        x = 0;
        agent = GetComponent<NavMeshAgent>();
        gunscript = gun.GetComponent<gunscriptv1>();
        
        //trigger = false;
        anim = rag.GetComponent<Animator>();
        newpos = transform.position;
        newpos2 = transform.position;
    }

    void Update()
    {
        //visibleenemies.Clear();
        if (x < 10)
        {
            StartCoroutine("waitFor2");
            newpos = transform.position;
            newpos2 = transform.position;
            GetComponent<BoxCollider>().enabled = false;
        }
        if (x > 100)
        {
            GetComponent<BoxCollider>().enabled = true;
        }
        aitoWaypointdistance = Vector3.Distance(newpos, transform.position);
        aitoWaypointdistance2 = Vector3.Distance(newpos2, transform.position);

        //enemy = null;
        gunscript = gun.GetComponent<gunscriptv1>();
        //visibleenemies.Clear();
        if (health <= 0)
        {
            anim.enabled = false;
            gameObject.tag = "DEAD";
            //Destroy(this);
            agent.enabled = false;
        }
        if (gameObject.tag != "DEAD")
        {

            enemies = GameObject.FindGameObjectsWithTag(enemytag);
            foreach (GameObject e in enemies)
            {
                RaycastHit eHit;
                Physics.Linecast(transform.position, e.transform.position, out eHit, -1);
                {
                    if (eHit.collider != null)
                    {
                        if (eHit.collider.tag == enemytag)
                        {
                            visibleenemies.Add(e);
                        }
                    }

                }
            }
            if (visibleenemies.Count > 0)
            {
                enemy = findClosestTarget(enemytag, visibleenemies);
                enemyscript = enemy.GetComponent<navmeshtesterguy>();
            }
            else if (gunscript.ammo < gunscript.magCapacity && visibleenemies.Count == 0)
            {
                reload = true;
            }



            /*if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;

                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 1000))
                {
                    agent.destination = hit.point;
                }
            }*/
            attackReload();
            //or
            goTowardsEnemy();


            Debug.DrawLine(transform.position, enemy.transform.position);
            x++;
        }
    }

    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;

        randDirection += origin;

        NavMeshHit navHit;

        NavMesh.SamplePosition(randDirection, out navHit, dist, 1);

        return navHit.position;
    }

    IEnumerator waitFor(float x)
    {
        //Debug.Log(gameObject.tag + "is waiting");
        isWaiting = true;
        yield return new WaitForSeconds(x);
        isWaiting = false;
    }
    IEnumerator waitFor2(float x)
    {
        //Debug.Log(gameObject.tag + "is waiting");
        isWaiting2 = true;
        yield return new WaitForSeconds(x);
        isWaiting2 = false;
    }

    GameObject findClosestTarget(string trgt, List<GameObject> gl)
    {
        List<GameObject> gos = gl;

        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;
        foreach (GameObject go in gos)
        {
            Vector3 diff = go.transform.position - position;
            float curDistance = diff.sqrMagnitude;

            if (curDistance < distance && curDistance != 0)
            {
                closest = go;
                distance = curDistance;
            }

        }

        return closest;
    }

    public void shoot()
    {
        reload = false;

        trigger = true;
        RaycastHit bullet;
        Physics.Linecast(transform.position, enemy.transform.position, out bullet, -1);
        if (bullet.collider.gameObject == enemy && gunscript.isShooting == true)
        {
            enemyscript = enemy.GetComponent<navmeshtesterguy>();
            enemyscript.health -= gunscript.damage;
            Instantiate(blood, bullet.collider.transform.position, transform.rotation);
        }
        if (bullet.collider.gameObject.tag != enemy.tag && gunscript.isShooting == true)
        {
            Instantiate(wallhit, bullet.collider.transform.position, transform.rotation);
        }
    }

    public void attackReload()
    {
        RaycastHit hit2;

        if (Physics.Linecast(transform.position, enemy.transform.position, out hit2, -1) && visibleenemies.Count > 0)
        {
            if (hit2.collider.tag == enemytag && x >= 1)
            {
                //Debug.Log("collider is " + hit2.collider.tag);

                if (gunscript.ammo > 0 && enemy.tag != "DEAD")
                {
                    shoot();
                    Debug.Log(transform.tag + " sees " + enemytag);
                    Vector3 targetDir = enemy.transform.position - transform.position;
                    Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, .7f, 0.0f);
                    transform.rotation = Quaternion.LookRotation(newDir);

                    if (isWaiting == false)
                    {
                        findcover = true;
                        newpos = RandomNavSphere(transform.position, 5, -1);
                        RaycastHit hit3;

                        Physics.Linecast(newpos, enemy.transform.position, out hit3, -1);

                        if (hit3.collider != null)
                        {
                            if (hit3.collider.tag != enemytag)
                            {
                                findcover = false;
                                agent.speed = 9;
                                agent.SetDestination(newpos);
                                //StartCoroutine(waitFor2(Random.Range(1, 2)));
                            }
                        }

                    }
                }

                if (gunscript.ammo < 5)
                {


                    trigger = false;
                    Debug.Log(transform.tag + " sees " + enemytag);
                    if (isWaiting == false)
                    {
                        findcover = true;
                        newpos = RandomNavSphere(transform.position, 15, -1);
                        RaycastHit hit3;
                        if (Physics.Linecast(newpos, enemy.transform.position, out hit3, -1))
                        {
                            if (hit3.collider.tag != enemytag)
                            {
                                findcover = true;
                                agent.speed = 10;
                                agent.SetDestination(newpos);

                                trigger = false;
                                //StartCoroutine(waitFor2(Random.Range(2, 3)));
                                reload = true;
                            }
                        }
                    }
                }
            }
            else
            {
                trigger = false;
            }

        }
    }

    public void goTowardsEnemy()
    {
            if (enemy.gameObject != null)
        {
            agent.SetDestination(newpos2);
            if (aitoWaypointdistance2 < 3 && gunscript.ammo >= 1 && x >= 1)
            {
                agent.speed = 5;
                newpos2 = RandomNavSphere(transform.position, 15, -1);
                findcover = false;
                if (aitoWaypointdistance2 < 3)
                {
                    agent.SetDestination(newpos2);
                    StartCoroutine(waitFor(Random.Range(2, 3)));
                }
                //else newpos2 = RandomNavSphere(transform.position, 15, -1);

            }
        }
       else
        {
            if (isWaiting2 == false)
            {
                agent.speed = 5;
                //Vector3 newpos2 = RandomNavSphere(transform.position, 15, -1);
                findcover = false;
                if (Vector3.Distance(transform.position, newpos2) < 3)
                {
                    agent.SetDestination(newpos2);
                    StartCoroutine(waitFor(Random.Range(2, 3)));
                }
                //else newpos2 = RandomNavSphere(transform.position, 15, -1);

            }
        }
    }

 }
