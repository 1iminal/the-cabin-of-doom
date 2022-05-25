using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class WebButton : Button
{
    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        Cursor.lockState = CursorLockMode.Locked;
        TextMeshProUGUI text = GameObject.Find("Label").GetComponent<TextMeshProUGUI>();
        text.text = "R - return cursor";
    }
}
