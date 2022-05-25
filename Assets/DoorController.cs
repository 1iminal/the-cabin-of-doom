using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DoorStates
{
    public static int CLOSED = 0;
    public static int OPENING = 1;
    public static int OPEN = 2;
}

public class DoorController : MonoBehaviour
{
    public AudioClip soundOpen;
    public AudioClip soundClose;
    public float openSpeed = 10f;
    public int state = DoorStates.CLOSED;
    public bool occupied = false;
    private float originalRot;
    private AudioSource audioSource;
    private BeastController beast;
    private GameController game;
    // Start is called before the first frame update
    void Start()
    {
        originalRot = transform.rotation.eulerAngles.y;
        audioSource = GetComponent<AudioSource>();
        beast = GameObject.Find("Beast").GetComponent<BeastController>();
        game = GameObject.Find("GameController").GetComponent<GameController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (game.state != GameState.PLAYING)
        {
            return;
        }
        if (state == DoorStates.OPENING)
        {

            transform.Rotate(new Vector3(0, -10 * openSpeed * Time.deltaTime, 0));
            Vector3 rot = transform.rotation.eulerAngles;
            if (Mathf.DeltaAngle(rot.y, originalRot) >= 90)
            {
                state = DoorStates.OPEN;
            }
        } 
    }

    public void open()
    {
        state = DoorStates.OPENING;
        audioSource.clip = soundOpen;
        audioSource.Play();
    }

    public void close()
    {
        state = DoorStates.CLOSED;
        audioSource.clip = soundClose;
        audioSource.Play();
        Vector3 rot = transform.rotation.eulerAngles;
        transform.Rotate(new Vector3(rot.x, originalRot - rot.y, rot.z));
        beast.doorClosed();
    }

    void OnTriggerEnter(Collider collider)
    {
        occupied = true;
        if (state == DoorStates.OPENING)
        {
            close();
        }
    }

    void OnTriggerExit(Collider collider)
    {
        occupied = false;
    }
}
