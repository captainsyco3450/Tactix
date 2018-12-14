using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gunscriptv1 : MonoBehaviour
{
    public GameObject muzzleflashobj;
    public GameObject muzzleflashobjtarget;
    public string weaponName;
    public float damage;
    public int magCapacity;
    public double ammo;
    public double mags;
    public float rateOfFire;
    public float burstspeed;
    public int reloadSpeed;
    public int fireMode;
    public bool canReload = false;
    public bool canShoot = false;
    public bool hasAmmo;
    public bool isShooting;//for testing purposes
    public bool startedWaitFor = false;//to see if its waiting for seconds or not
    public bool startedWaitFor2 = false;
    public bool isReloading = false; //self explanatory, could be on a timer or by animation (as of now its timed by reload speed)
    public int shots = 0; //keeping track of ammo and number of rounds shot etc
    public float counter;
    public GameObject magdroptarget; //reload
    public GameObject casingtarget;//go with the shoot methodation
    public GameObject aiplayer;
    public navmeshtesterguy ai;
    public AudioSource audios;
    public AudioClip shoot1;

    //    also make the gunscript have traits that we can just modify and make prefabs for
    //like ammo capacity, fire rate, semi vs auto etc
    //bullet casings, mag drops,

    void Start()
    {
       
        ammo = magCapacity;
        //InvokeRepeating("shoot", 0, rateOfFire);
        ai = aiplayer.GetComponent<navmeshtesterguy>();
        audios = GetComponent<AudioSource>();
     

    }

    // Update is called once per frame
    void Update()
    {
        isShooting = false;
        if (ai.health <= 0)
        {
            canShoot = false;
            ai.trigger = false;
        }

        if (ai.trigger == true)
        {
            shoot();
        }
        if (Input.GetKeyUp(KeyCode.F))
        {
            fireMode++;
            if (fireMode > 3)
            {
                fireMode = 1;
            }
                
        }
        if (ai.reload == true && isReloading == false)
        {
            if (!startedWaitFor)
            {
                reload();
            }

        }

        if (startedWaitFor2 == false && startedWaitFor == false && canShoot == true) //had to do this to make the 3 round burst limited and continue after you let up on the mouse
        {
            if (shots < 3 && shots > 0 && ai.trigger == false)
            {
                //raycast
                ammo -= 1;
                flash();
                StartCoroutine(waitFor(rateOfFire));
                shots++;
            }
            else if (shots > 2 && ai.trigger == false)
            {
                StartCoroutine(waitFor(burstspeed));
                shots = 0;
            }
        }
    }

    public void checkAmmo()
    {

        if (ammo > 0)
        {
            canShoot = true;
            hasAmmo = true;
        }
        else
        {
            hasAmmo = false;
        }
        if (mags >= 1)
        {
            canReload = true;
        }
    }

    public void reload()
    {
        if (ammo < magCapacity && mags > 0)
        {
            
            //reload anim
            
            magdroptarget.SetActive(true);
            if (!startedWaitFor)//also you can't reload while firing
            {
                isReloading = true;
                StartCoroutine(waitFor(reloadSpeed));
                

            }
            isReloading = false;
            ammo = magCapacity;
            mags -= 1;

           
        }
    }



    public void shoot()
    {
        checkAmmo();

        if (!startedWaitFor)//check to see if the gun is in between rounds
        {

            if (hasAmmo == true && isReloading == false && canShoot == true) //make sure it has ammo and isn't reloading
            {
                if (fireMode == 1 && ai.trigger == true)//full auto, limited to firerate
                {
                    flash();
                    //raycast
                    ammo -= 1;
                    StartCoroutine(waitFor(rateOfFire));
                }
                else if (fireMode == 2 && ai.trigger == true)//semi auto one click one shot
                {
                    //raycast
                    flash();
                    ammo -= 1;
                }
                else if (fireMode == 3) //three round bursts that are limited to burst speed
                {
                    if (ai.trigger == true)
                    {

                        if (startedWaitFor2 == false)
                        {

                            if (shots > 2)
                            {
                                StartCoroutine(waitFor2(burstspeed));
                                shots = 0;
                            }
                            if (startedWaitFor == false)
                            {
                                //raycast
                                ammo -= 1;

                                flash();
                                StartCoroutine(waitFor(rateOfFire));
                                shots++;
                            }
                        }
                    }
            }

                    canShoot = false;
                
            }
        }
    }
    
    

    public void flash()
    {
        isShooting = true;
        casingtarget.SetActive(true);

        //Instantiate(muzzleflashobj);
        muzzleflashobj.SetActive(true);
        audios.PlayOneShot(shoot1);
        muzzleflashobj.transform.localEulerAngles = new Vector3(0, 0, Random.Range(0, 90));
    }

IEnumerator waitFor(float x)
    {
        startedWaitFor = true;
        canShoot = false;
        yield return new WaitForSeconds(x);
        canShoot = true;
        startedWaitFor = false;
    }
    IEnumerator waitFor2(float x)
    {
        startedWaitFor2 = true;
        canShoot = false;
        yield return new WaitForSeconds(x);
        canShoot = true;
        startedWaitFor2 = false;
    }

}


