using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public static class GameState
{
    public static int PLAYING = 0;
    public static int LOST = 1;
    public static int WON = 2;
}

public class GameController : MonoBehaviour
{
    public int roundTime = 120;
    public int state;
    public AudioClip clipWon;
    public AudioClip clipLost;
    private GameObject label;
    private float countdown;
    private DoorController[] doors;
    private Image fill;
    private BeastController beast;
    // Start is called before the first frame update
    void Start()
    {
        state = GameState.PLAYING;
        label = GameObject.Find("Countdown");
        countdown = roundTime;
        doors = new DoorController[4];
        for (int i = 0; i < 4; i++)
        {
            doors[i] = GameObject.Find("Door" + (i + 1)).GetComponent<DoorController>();
        }
        fill = GameObject.Find("Fill").GetComponent<Image>();
        beast = GameObject.Find("Beast").GetComponent<BeastController>();
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            TextMeshProUGUI text = GameObject.Find("Label").GetComponent<TextMeshProUGUI>();
            text.text = "R - return cursor";
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                Cursor.lockState = CursorLockMode.None;
                TextMeshProUGUI text = GameObject.Find("Label").GetComponent<TextMeshProUGUI>();
                text.text = "Lock Cursor";
            }
        }
        if (state == GameState.PLAYING)
        {
            TextMeshProUGUI text = label.GetComponent<TextMeshProUGUI>();
            countdown -= Time.deltaTime;
            if (countdown <= 0)
            {
                state = GameState.WON;
                text.text = "YOU WON\nPress [Enter] to restart";
                AudioSource rooster = GameObject.Find("Rooster").GetComponent<AudioSource>();
                rooster.Play();
                AudioSource bgm = GetComponent<AudioSource>();
                bgm.clip = clipWon;
                bgm.Play();
                beast.stop();
            } else
            {
                TimeSpan span = TimeSpan.FromSeconds(countdown);
                text.text = "Time until dawn:\n" + String.Format("{0:D2}:{1:D2}", span.Minutes, span.Seconds);
                for (int i = 0; i < doors.Length; i++)
                {
                    if (doors[i].state == DoorStates.OPEN)
                    {
                        state = GameState.LOST;
                        text.text = "YOU LOSE\nPress [Enter] to restart";
                        AudioSource bgm = GetComponent<AudioSource>();
                        bgm.clip = clipLost;
                        bgm.Play();
                        beast.stop();
                    }
                }
            }
        } else if (state == GameState.LOST)
        {
            Color color = fill.color;
            if (color.a < 1)
            {
                color.a = color.a + Time.deltaTime;
                fill.color = color;
            }
        }
    }
}
