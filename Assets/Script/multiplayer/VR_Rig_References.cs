using UnityEngine;

public class VR_Rig_References : MonoBehaviour
{
    public static VR_Rig_References Singleton;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public Transform Root;
    public Transform Head;
    public Transform L_hand;
    public Transform R_hand;

    private void Awake()
    {
        Singleton = this;
    }
}
