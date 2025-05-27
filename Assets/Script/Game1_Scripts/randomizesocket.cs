using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class randomizesocket : MonoBehaviour
{
    public List<XRSocketInteractor> allSockets = new List<XRSocketInteractor>();
    public List<GameObject> indicatorBlocks = new List<GameObject>();
    public GameObject indicatorCube;

    public AudioSource correctA = null;
    public AudioSource wrongA = null;

    public GameObject replacementPrefabL;
    public GameObject replacementPrefabW;
    public GameObject cube;
    public GameObject key;

    public float socketCooldown = 1f;
    public float autoResetTime = 5f;

    private List<XRSocketInteractor> correctSockets = new List<XRSocketInteractor>();
    private List<GameObject> activeIndicatorBlocks = new List<GameObject>();

    private HashSet<XRSocketInteractor> filledCorrectSockets = new HashSet<XRSocketInteractor>();

    private bool canTriggerWin = true;

    void Start()
    {
        foreach (var socket in allSockets)
        {
            socket.selectExited.AddListener(args =>
            {
                OnObjectRemoved(socket);
            });

            // Spawn one cube per socket
            Vector3 spawnPos = socket.attachTransform.position + new Vector3(0, 0.5f, 0);
            GameObject newCube = Instantiate(cube, spawnPos, Quaternion.identity);
            newCube.tag = "socketgame";
        }

        StartGame();
    }

    public void StartGame()
    {
        ResetIndicatorCube();
        GetCorrectSockets();
        

        filledCorrectSockets = new HashSet<XRSocketInteractor>(correctSockets);
        canTriggerWin = true;

        StartCoroutine(AutoResetTimer());
    }

    private void GetCorrectSockets()
    {
        correctSockets.Clear();
        activeIndicatorBlocks.Clear();

        List<int> availableIndices = new List<int>();
        for (int i = 0; i < allSockets.Count; i++)
            availableIndices.Add(i);

        int count = Random.Range(2, 12); // Choose 2 or 3 correct sockets

        for (int i = 0; i < count; i++)
        {
            int randomIndex = Random.Range(0, availableIndices.Count);
            int selectedIndex = availableIndices[randomIndex];
            availableIndices.RemoveAt(randomIndex);

            correctSockets.Add(allSockets[selectedIndex]);

            GameObject indicator = indicatorBlocks[selectedIndex];
            if (indicator.TryGetComponent<Renderer>(out Renderer rend))
            {
                rend.material.color = Color.green;
            }
            activeIndicatorBlocks.Add(indicator);
        }

        Debug.Log($"Correct sockets this round: {correctSockets.Count}");
        foreach (var socket in correctSockets)
        {
            Debug.Log(socket.gameObject.name);
        }
    }

    public void OnObjectRemoved(XRSocketInteractor socket)
    {
        if (!canTriggerWin) return;

        if (correctSockets.Contains(socket))
        {
            filledCorrectSockets.Remove(socket);
            Debug.Log($"Removed from correct socket. Remaining: {filledCorrectSockets.Count}");

            if (filledCorrectSockets.Count == 0)
            {
                Debug.Log("All correct sockets cleared! You win!");
                SetIndicatorColor(Color.green);
                correctA?.Play();
                canTriggerWin = false;
                SpawnKeyAt(socket.attachTransform.position);
            }
        }
        else
        {
            Debug.Log("Removed from wrong socket. No reset triggered.");
            SetIndicatorColor(Color.red);
            wrongA?.Play();
        }
    }

    public XRSocketInteractor itemcheck()
    {
        foreach (XRSocketInteractor socket in allSockets)
        {
            if (!socket.hasSelection)
                return socket;
        }
        return null;
    }

    IEnumerator socketfiller(XRSocketInteractor winnerSocket)
    {
        XRSocketInteractor pickupSocket = itemcheck();

        if (pickupSocket == null)
        {
            Debug.LogWarning("No empty socket found.");
            yield break;
        }

        GameObject result = null;

        if (correctSockets.Contains(pickupSocket))
        {
            filledCorrectSockets.Remove(pickupSocket);
            result = Instantiate(replacementPrefabW, pickupSocket.attachTransform);
            SetIndicatorColor(Color.green);
            correctA?.Play();

            if (filledCorrectSockets.Count == 0)
            {
                yield return new WaitForSeconds(autoResetTime);
                StartGame();
                yield break;
            }
        }
        else
        {
            result = Instantiate(replacementPrefabL, pickupSocket.transform);
            SetIndicatorColor(Color.red);
            wrongA?.Play();
        }

        result.transform.localPosition = Vector3.zero;
        result.tag = "socketgame";

        yield return new WaitForSeconds(0.5f);
        canTriggerWin = true;
    }

    public void WinningSocket()
    {
        if (!canTriggerWin) return;

        canTriggerWin = false;
        XRSocketInteractor winnerSocket = correctSockets[Random.Range(0, correctSockets.Count)];
        StartCoroutine(socketfiller(winnerSocket));
    }

    private void ResetIndicatorCube()
    {
        foreach (var block in indicatorBlocks)
        {
            if (block.TryGetComponent<Renderer>(out Renderer rend))
            {
                rend.material.color = Color.white;
            }
        }
    }


    private void SetIndicatorColor(Color color)
    {
        if (indicatorCube != null && indicatorCube.TryGetComponent<Renderer>(out Renderer rend))
        {
            rend.material.color = color;
        }
    }

    private void SpawnKeyAt(Vector3 position)
    {
        Instantiate(key, position, Quaternion.identity);
    }

    private IEnumerator AutoResetTimer()
    {
        yield return new WaitForSeconds(autoResetTime);
        StartGame();
    }
}
