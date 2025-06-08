using UnityEngine;
using UnityEngine.UI;

public class UISpriteAnimator : MonoBehaviour
{
    public Image image;             // UI Image component
    public Sprite[] frames;         // Animation frames
    public float frameRate = 10f;   // Speed of animation

    private int currentFrame;
    private float timer;

    void Update()
    {
        if (frames.Length == 0 || image == null) return;

        timer += Time.deltaTime;
        if (timer >= 1f / frameRate)
        {
            timer = 0f;
            currentFrame = (currentFrame + 1) % frames.Length;
            image.sprite = frames[currentFrame];
        }
    }
}
