using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BeastState
{
    public static int WAITING = 0;
    public static int TRAVEL = 1;
    public static int NO_TARGET = 2;
}

public class BeastController : MonoBehaviour
{
    public float speed = 1.0f;
    private static GameObject graph;
    private DoorController target;
    private int state;
    private DoorController[] doors;
    private AudioSource audioSource;
    private int currentNode;
    private GameObject[] path;
    private float waitTime;
    private GameController game;
    // Start is called before the first frame update
    void Start()
    {
        graph = GameObject.Find("Path");
        state = BeastState.NO_TARGET;
        doors = new DoorController[4];
        for (int i = 0; i < 4; i++)
        {
            doors[i] = GameObject.Find("Door" + (i + 1)).GetComponent<DoorController>();
        }
        audioSource = GetComponent<AudioSource>();
        game = GameObject.Find("GameController").GetComponent<GameController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (game.state != GameState.PLAYING)
        {
            return;
        }
        if (state == BeastState.NO_TARGET)
        {
            bool foundTarget = false;
            while (!foundTarget)
            {
                target = doors[Random.Range(0, doors.Length)];
                if (!target.occupied)
                {
                    foundTarget = true;
                }
            }
            // build path to target
            GameObject closestNode = getClosestNode(transform.gameObject);
            GameObject closestDoorNode = getClosestNode(target.gameObject);
            path = getPath(closestNode, closestDoorNode);
            currentNode = 0;
            state = BeastState.TRAVEL;
            audioSource.loop = true;
            audioSource.Play();
        } else if (state == BeastState.TRAVEL)
        {
            Vector3 rot = transform.rotation.eulerAngles;
            Vector3 dir = path[currentNode].transform.rotation.eulerAngles - rot;
            rot.y = dir.y;
            transform.Rotate(rot);
            transform.position = Vector3.MoveTowards(transform.position, path[currentNode].transform.position, speed * Time.deltaTime);
            if (Vector3.Distance(transform.position, path[currentNode].transform.position) < 0.01)
            {
                currentNode++;
                if(currentNode > path.Length - 1)
                {
                    state = BeastState.WAITING;
                    waitTime = 10;
                    audioSource.Stop();
                    target.open();
                }
            }
        } else if (state == BeastState.WAITING)
        {
            waitTime -= Time.deltaTime;
            if (waitTime <= 0)
            {
                state = BeastState.NO_TARGET;
            }
        }
    }

    private static GameObject getClosestNode(GameObject obj)
    {
        float dist = Mathf.Infinity;
        GameObject node = obj;
        Queue<GameObject> queue = new Queue<GameObject>();
        for (int i = 0; i < graph.transform.childCount; i++)
        {
            queue.Enqueue(graph.transform.GetChild(i).gameObject);
        }
        List<GameObject> searched = new List<GameObject>();
        while (queue.Count > 0)
        {
            GameObject n = queue.Dequeue();
            if (!searched.Contains(n))
            {
                searched.Add(n);
                if (Vector3.Distance(n.transform.position, obj.transform.position) < dist)
                {
                    dist = Vector3.Distance(n.transform.position, obj.transform.position);
                    node = n;
                }
                GameObject[] neighbours = n.GetComponent<NodeController>().neighbours;
                for (int i = 0; i < neighbours.Length; i++)
                {
                    queue.Enqueue(neighbours[i]);
                }
            }
        }
        return node;
    }

    private static GameObject[] getPath(GameObject startNode, GameObject endNode)
    {
        Queue<GameObject> nodes = new Queue<GameObject>();
        nodes.Enqueue(startNode);
        if (startNode == endNode)
        {
            return nodes.ToArray();
        }
        var queue = new Queue<KeyValuePair<GameObject, Queue<GameObject>>>();
        GameObject[] neighbours = startNode.GetComponent<NodeController>().neighbours;
        for (int i = 0; i < neighbours.Length; i++)
        {
            queue.Enqueue(new KeyValuePair<GameObject, Queue<GameObject>>(neighbours[i], nodes));
        }
        List<GameObject> searched = new List<GameObject>();
        while (queue.Count > 0)
        {
            var n = queue.Dequeue();
            if (!searched.Contains(n.Key))
            {
                searched.Add(n.Key);
                if (n.Key == endNode)
                {
                    n.Value.Enqueue(n.Key);
                    return n.Value.ToArray();
                } else
                {
                    Queue<GameObject> parents = new Queue<GameObject>(n.Value);
                    parents.Enqueue(n.Key);
                    neighbours = n.Key.GetComponent<NodeController>().neighbours;
                    for (int i = 0; i < neighbours.Length; i++)
                    {
                        queue.Enqueue(new KeyValuePair<GameObject, Queue<GameObject>>(neighbours[i], parents));
                    }
                }
            }
        }
        return new GameObject[0];
    }

    public void doorClosed()
    {
        if (state == BeastState.WAITING)
        {
            state = BeastState.NO_TARGET;
        }
    }

    public void stop()
    {
        audioSource.Stop();
    }
}
