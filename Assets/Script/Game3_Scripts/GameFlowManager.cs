using UnityEngine;

public class GameFlowManager : MonoBehaviour
{
    [Header("Final Symbol Validators")]
    public SymbolMatchValidator player1Validator;   // From Player 1's socket
    public SymbolMatchValidator player2Validator;   // From Player 2's socket

    [Header("World Events")]
    public GameObject midWall;                      // The separating wall to destroy
    public BackDoorTrigger backDoorTrigger;         // Optional: door lifting trigger

    private bool gameComplete = false;

    void Update()
    {
        if (gameComplete) return;

        if (player1Validator.isMatched && player2Validator.isMatched)
        {
            gameComplete = true;

            Debug.Log("✅ Both players socketed correct symbols!");

            if (midWall != null)
            {
                Destroy(midWall);
                Debug.Log("💥 Mid-wall destroyed!");
            }

            if (backDoorTrigger != null)
            {
                backDoorTrigger.Unlock();
                Debug.Log("🔓 Back door unlocked!");
            }
        }
    }
}
