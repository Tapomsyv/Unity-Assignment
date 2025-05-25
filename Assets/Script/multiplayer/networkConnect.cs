using Unity.Netcode;
using UnityEngine;

public class networkConnect : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void create()
    {
        NetworkManager.Singleton.StartHost();
    }
    public void Join()
    {
        NetworkManager.Singleton.StartClient();
    }
    
}
