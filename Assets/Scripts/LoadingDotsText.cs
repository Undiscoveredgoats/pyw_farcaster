using UnityEngine;
using UnityEngine.UI;

public class LoadingDots : MonoBehaviour
{
    [Header("UI Reference")]
    public Text loadingText;         // Assign in Inspector

    [Header("Dot Animation Settings")]
    public float dotInterval = 0.5f; // Time between dot updates
    public int maxDots = 3;          // Max dots before reset

    private float timer = 0f;
    private int dotCount = 0;
    private bool isAnimating = false;

    void Update()
    {
        if (!isAnimating || loadingText == null)
            return;

        timer += Time.deltaTime;

        if (timer >= dotInterval)
        {
            dotCount = (dotCount + 1) % (maxDots + 1);
            loadingText.text = "Loading" + new string('.', dotCount);
            timer = 0f;
        }
    }

    public void StartLoading()
    {
        timer = 0f;
        dotCount = 0;
        isAnimating = true;
        if (loadingText != null)
            loadingText.text = "Loading";
    }

    public void StopLoading()
    {
        isAnimating = false;
        if (loadingText != null)
            loadingText.text = "";
    }
}
