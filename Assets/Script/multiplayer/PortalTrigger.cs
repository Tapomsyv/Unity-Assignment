using UnityEngine;

public class PortalTrigger : MonoBehaviour
{
    public int count = 0;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            count += 1;
            if (count == 2 && networkscenemanager.Instance != null)
            {
                networkscenemanager.Instance.TryLoadNextSceneFromTrigger();
            }
        }
    }
}