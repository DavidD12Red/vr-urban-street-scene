using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarRoute1 : MonoBehaviour
{
    public List<Transform> wps;
    public float speed;
    private int targetWP;
    private Rigidbody rb;
    private bool go = true;
    private List<Collider> collidingPedestrians;
    private AudioSource soundEffect;

    private void Awake()
    {
        soundEffect = GetComponent<AudioSource>();
    }

    void SetRoute()
    {
        //initialise position and waypoint counter
        transform.position = new Vector3(wps[0].position.x, transform.position.y, wps[0].position.z);
        targetWP = 1;
    }

    // Start is called before the first frame update
    void Start()
    {
        SetRoute();

        collidingPedestrians = new List<Collider>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!go)
        {
            return;
        }

        var targetRoutePos = wps[targetWP].position;
        var currentPos = transform.position;

        Vector3 displacement = new Vector3(targetRoutePos.x - currentPos.x, 0f, targetRoutePos.z - currentPos.z);

        if (displacement.magnitude < 0.1f)
        {
            targetWP++;
            if (targetWP >= wps.Count)
            {
                SetRoute();
                return;
            }
        }

        //calculate velocity for this frame
        Vector3 velocity = displacement;
        velocity.Normalize();
        velocity *= speed;

        //apply velocity
        Vector3 newPosition = transform.position;
        newPosition += velocity * Time.deltaTime;
        rb.MovePosition(newPosition);

        //align to velocity
        Vector3 desiredForward = Vector3.RotateTowards(transform.forward, velocity,
        10.0f * Time.deltaTime, 0f);
        Quaternion rotation = Quaternion.LookRotation(desiredForward);
        rb.MoveRotation(rotation);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Pedestrian")
        {
            go = false;
            collidingPedestrians.Add(other);
            soundEffect.PlayOneShot(soundEffect.clip);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Pedestrian")
        {
            collidingPedestrians.Remove(other);
            if (collidingPedestrians.Count == 0)
            {
                go = true;
            }
        }
    }
}
