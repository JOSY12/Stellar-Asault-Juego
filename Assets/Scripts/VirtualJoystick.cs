using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class VirtualJoystick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    private Image container;
    private Image handle;
    [HideInInspector] public Vector2 InputVector;

    [Header("Ajustes Visuales")]
    [Range(1f, 5f)] public float rangoMovimiento = 2.5f; // Qué tanto se aleja la esfera

    void Start()
    {
        container = GetComponent<Image>();
        // Obtiene el primer hijo (la esfera del centro)
        handle = transform.GetChild(0).GetComponent<Image>();
    }

    public void OnDrag(PointerEventData ped)
    {
        Vector2 pos;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(container.rectTransform, ped.position, ped.pressEventCamera, out pos))
        {
            // Calcula la posición relativa al tamaño del contenedor
            pos.x = (pos.x / container.rectTransform.sizeDelta.x);
            pos.y = (pos.y / container.rectTransform.sizeDelta.y);

            InputVector = new Vector2(pos.x * 2, pos.y * 2);
            InputVector = (InputVector.magnitude > 1.0f) ? InputVector.normalized : InputVector;

            // ESTA LÍNEA MUEVE LA ESFERA VISUALMENTE
            handle.rectTransform.anchoredPosition = new Vector2(
                InputVector.x * (container.rectTransform.sizeDelta.x / rangoMovimiento), 
                InputVector.y * (container.rectTransform.sizeDelta.y / rangoMovimiento)
            );
        }
    }

    public void OnPointerDown(PointerEventData ped) => OnDrag(ped);

    public void OnPointerUp(PointerEventData ped)
    {
        InputVector = Vector2.zero;
        handle.rectTransform.anchoredPosition = Vector2.zero; // Vuelve al centro al soltar
    }

    public float Horizontal => InputVector.x;
    public float Vertical => InputVector.y;
}