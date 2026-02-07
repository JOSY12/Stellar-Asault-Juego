using UnityEngine;
using UnityEngine.EventSystems;

public class TouchZoneController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public enum ZoneType { Movement, Aiming }
    public ZoneType zoneType;
    
    private Vector2 touchStartPosition;
    private Vector2 currentTouchPosition;
    private bool isTouching = false;
    private RectTransform rectTransform;
    private Canvas canvas;
    
    [HideInInspector] public Vector2 InputVector;
    
    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        isTouching = true;
        
        // Guardar punto inicial del toque (CENTRO del joystick invisible)
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform, 
            eventData.position, 
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera, 
            out touchStartPosition
        );
        
        currentTouchPosition = touchStartPosition;
        UpdateInput();
    }
    
    public void OnPointerUp(PointerEventData eventData)
    {
        isTouching = false;
        InputVector = Vector2.zero;
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        if (isTouching)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform, 
                eventData.position, 
                canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera, 
                out currentTouchPosition
            );
            
            UpdateInput();
        }
    }
    
    void UpdateInput()
    {
        // ✅ AMBOS funcionan igual: dirección desde donde tocaste inicialmente
        Vector2 direction = currentTouchPosition - touchStartPosition;
        
        float maxDistance = 150f;
        if (direction.magnitude > maxDistance)
        {
            direction = direction.normalized * maxDistance;
        }
        
        InputVector = direction.magnitude > 10f ? direction.normalized : Vector2.zero;
    }
    
    public bool IsTouching()
    {
        return isTouching;
    }
    
    public void ResetZone()
    {
        InputVector = Vector2.zero;
        isTouching = false;
    }
    
    public float Horizontal => InputVector.x;
    public float Vertical => InputVector.y;
}