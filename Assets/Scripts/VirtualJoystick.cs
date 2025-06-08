using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class VirtualJoystick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    public Image joystickBg;
    public Image joystickHandle;
    private Vector2 inputVector;
    private bool isDragging; // Tracks if the joystick is actively being used
    private RectTransform joystickRect; // For bounds checking

    public bool IsDragging => isDragging; // Public property to check if joystick is active
    public Vector2 InputVector => inputVector; // Public property to access the joystick input

    void Awake()
    {
        // Get the RectTransform for bounds checking
        joystickRect = joystickBg.GetComponent<RectTransform>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Check if the initial click/touch is within the joystick bounds
        if (RectTransformUtility.RectangleContainsScreenPoint(joystickRect, eventData.position, eventData.pressEventCamera))
        {
            isDragging = true;
            OnDrag(eventData);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return; // Only process drag if we started within the joystick

        Vector2 pos;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(joystickBg.rectTransform, eventData.position, eventData.pressEventCamera, out pos))
        {
            pos.x = (pos.x / joystickBg.rectTransform.sizeDelta.x);
            pos.y = (pos.y / joystickBg.rectTransform.sizeDelta.y);
            inputVector = new Vector2(pos.x * 2, pos.y * 2);
            inputVector = (inputVector.magnitude > 1.0f) ? inputVector.normalized : inputVector;

            // Move the handle
            joystickHandle.rectTransform.anchoredPosition = new Vector2(inputVector.x * (joystickBg.rectTransform.sizeDelta.x / 2), inputVector.y * (joystickBg.rectTransform.sizeDelta.y / 2));
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (isDragging)
        {
            isDragging = false;
            inputVector = Vector2.zero;
            joystickHandle.rectTransform.anchoredPosition = Vector2.zero;
        }
    }
}