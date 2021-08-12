using UnityEngine;

public class CursorController : MonoBehaviour
{
    public static CursorController Instance;
    public Texture2D defaultCursor, attackCursor, interactCursor;

    public enum CursorTypes
    {
        Default,
        Attack,
        Interact
    }
    
    private void Awake()
    {
        Instance = this;
    }

    public void SetCursor(CursorTypes cursorType)
    {
        switch (cursorType)
        {
            case CursorTypes.Default:
                Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
                break;
            case CursorTypes.Attack:
                Cursor.SetCursor(attackCursor, Vector2.zero, CursorMode.Auto);
                break;
            case CursorTypes.Interact:
                Cursor.SetCursor(interactCursor, Vector2.zero, CursorMode.Auto);
                break;
            default:
                Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
                break;
        }
    }
    
}
