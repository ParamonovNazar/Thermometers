using UnityEngine.EventSystems;

namespace Core.Level.Input
{
    public interface IInputState
    {
        public void Activate();
        public void OnPointerDown(PointerEventData eventData);

        public void OnPointerUp(PointerEventData eventData);

        public void OnPointerMove(PointerEventData eventData);
        public void Deactivate();
    }
}