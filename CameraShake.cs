using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour {

    public Transform target;
    Player player;
    float moveSpeed = 30.0f;
    float rotateSpeed = 3;
    Animator anim;
    bool isRolling = false;
    float stopRollTime;
    // Use this for initialization
    void Start () {
        //transform.position = target.position;
        //transform.rotation = target.rotation;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
         anim = player.GetComponent<Animator>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 nodepos;
        Vector3 aipos;
        nodepos = new Vector3(target.position.x, 0, target.position.z);
        aipos = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 direction = nodepos - aipos;
        if (!isRolling)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 100 * Time.deltaTime);
        }
        // transform.LookAt(target);

        if (Input.GetKey(KeyCode.LeftControl))
        {
            stopRollTime = Time.time;
            isRolling = true;
        }

        if (Time.time >= stopRollTime + .7f)
        {
            isRolling = false;
        }
    }
   void stopRoll () {
        isRolling = false;
      
    }
}


