using RPG.Control;

namespace Control
{
    public interface IRaycastable
    {
        CursorType getCursorType();
        bool HandleRaycast(PlayerController callingController);
    }
}