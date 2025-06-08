using UnityEngine;
using System.Collections;

public class CanvasTransition : MonoBehaviour
{
    public Canvas currentCanvas;
    public float fadeDuration = 0.5f;

    public void TransitionTo(Canvas nextCanvas)
    {
        StartCoroutine(TransitionCanvas(currentCanvas, nextCanvas));
    }

    private IEnumerator TransitionCanvas(Canvas fromCanvas, Canvas toCanvas)
    {
        // Get CanvasGroups
        CanvasGroup fromGroup = fromCanvas.GetComponent<CanvasGroup>();
        CanvasGroup toGroup = toCanvas.GetComponent<CanvasGroup>();

        if (fromGroup == null || toGroup == null)
        {
            Debug.LogError("Both Canvases must have a CanvasGroup attached.");
            yield break;
        }

        // Fade out current
        yield return StartCoroutine(FadeCanvasGroup(fromGroup, 1, 0));
        fromCanvas.gameObject.SetActive(false);

        // Fade in next
        toCanvas.gameObject.SetActive(true);
        yield return StartCoroutine(FadeCanvasGroup(toGroup, 0, 1));

        // Update current reference
        currentCanvas = toCanvas;
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup group, float start, float end)
    {
        float elapsed = 0f;
        group.alpha = start;

        while (elapsed < fadeDuration)
        {
            group.alpha = Mathf.Lerp(start, end, elapsed / fadeDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        group.alpha = end;
        group.interactable = end > 0;
        group.blocksRaycasts = end > 0;
    }

    public void SetCurrentCanvas(Canvas current)
    {
        Debug.Log("Setting");
        currentCanvas = current;
        Debug.Log(currentCanvas);
    }
}
