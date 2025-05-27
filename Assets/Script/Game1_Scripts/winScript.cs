using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class winScript : MonoBehaviour
{
    public List<XRSocketInteractor> allSockets = new List<XRSocketInteractor>();
    public GameObject door;
    public GameObject door2;
    void Update()
    {
        if (AreAllSocketsFilled())
        {
            Debug.Log("All sockets are filled!");
            // You can trigger your win condition here
            door.SetActive(false);
            door2.SetActive(false);
        }
    }

    bool AreAllSocketsFilled()
    {
        foreach (var socket in allSockets)
        {
            if (!socket.hasSelection)
                return false;
        }
        return true;
    }
}
