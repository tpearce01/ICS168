using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

public class ButtonBehavior : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler/*, ISelectHandler, IDeselectHandler*/ {

    [SerializeField]
    private float _resizeSpeed = 0.0f;
    [SerializeField]
    private float _selectedWidth = 0.0f;

    private float _baseWidth = 0.0f;
    private float _currentWidth = 0.0f;
    private bool _isSelected = false;

    private void OnDisable() {
        _isSelected = false;
    }

    private void Start() {
        _baseWidth = GetComponent<LayoutElement>().minWidth;
    }

    private void Update() {

        _currentWidth = GetComponent<LayoutElement>().preferredWidth;

        if (_isSelected && _currentWidth < _selectedWidth) {
            GetComponent<LayoutElement>().preferredWidth += _resizeSpeed;
        }

        else if (!_isSelected && _currentWidth > _baseWidth) {
            GetComponent<LayoutElement>().preferredWidth -= _resizeSpeed;
        }
    }

    public void OnPointerEnter(PointerEventData data) {
        _isSelected = true;
    }

    public void OnPointerExit(PointerEventData data) {
        _isSelected = false;
    }
}
