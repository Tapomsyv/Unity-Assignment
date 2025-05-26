using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class SymbolMatchValidator : MonoBehaviour
{
    public XRSocketInteractor socket;
    public GameObject expectedObject; // This is the correct object shown on the monitor

    public bool isMatched { get; private set; } = false;

    void Update()
    {
        var selected = socket.GetOldestInteractableSelected();

        // No object in socket
        if (selected == null)
        {
            isMatched = false;
            return;
        }

        // Compare the correct object
        if (selected.transform.gameObject == expectedObject)
        {
            isMatched = true;
        }
        else
        {
            isMatched = false;
        }
    }
}
