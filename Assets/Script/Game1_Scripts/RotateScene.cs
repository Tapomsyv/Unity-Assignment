using UnityEngine;
using System.Collections;

public class RotateScene : MonoBehaviour
{
    public GameObject targetObject;

    private int clickCount = 0;
    private float totalX = 0f;
    private float totalZ = 0f;
    private bool isRotating = false;

    public float rotationDuration = 0.4f;

    public void onPoke()
    {
        if (targetObject == null || isRotating) return;

        if (clickCount % 2 == 0)
            totalX += 90f;
        else
            totalZ += 90f;

        // This ensures Y stays at 0
        Quaternion targetRotation = Quaternion.Euler(totalX, 0f, totalZ);
        StartCoroutine(AnimateRotation(targetObject.transform.rotation, targetRotation));

        clickCount++;
    }

    IEnumerator AnimateRotation(Quaternion fromRotation, Quaternion toRotation)
    {
        isRotating = true;

        float elapsed = 0f;
        while (elapsed < rotationDuration)
        {
            float t = elapsed / rotationDuration;
            targetObject.transform.rotation = Quaternion.Slerp(fromRotation, toRotation, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        targetObject.transform.rotation = toRotation;
        isRotating = false;
    }
}
