using SuperuserUtils;
using System;
using UnityEngine;

public class CursorController : GenericSingletonClass<CursorController>
{
    public Texture2D moveHereCursor;
    public Texture2D cannotMoveCursor;
    public Texture2D normalCursor;
    public Texture2D equipCursor, dequipCursor, attackCursor;

    public enum CursorTypes
    {
        Default,
        Attack,
        Move,
        CannotMove,
        Equip,
        Dequip
    }

    public void SetCursor(CursorTypes cursorType)
    {
        Vector2 cursorOffset;
        switch (cursorType)
        {
            case CursorTypes.Default:
                Cursor.SetCursor(normalCursor, Vector2.zero, CursorMode.Auto);
                break;
            case CursorTypes.Attack:
                cursorOffset = new Vector2(attackCursor.width / 2, attackCursor.height / 2);
                Cursor.SetCursor(attackCursor, cursorOffset, CursorMode.Auto);
                break;
            case CursorTypes.Equip:
                Cursor.SetCursor(equipCursor, Vector2.zero, CursorMode.Auto);
                break;
            case CursorTypes.Dequip:
                Cursor.SetCursor(dequipCursor, Vector2.zero, CursorMode.Auto);
                break;
            case CursorTypes.Move:
                cursorOffset = new Vector2(moveHereCursor.width / 2, moveHereCursor.height / 2);
                Cursor.SetCursor(moveHereCursor, cursorOffset, CursorMode.Auto);
                break;
            case CursorTypes.CannotMove:
                cursorOffset = new Vector2(cannotMoveCursor.width / 2, cannotMoveCursor.height / 2);
                Cursor.SetCursor(cannotMoveCursor, cursorOffset, CursorMode.Auto);
                break;
            default:
                Cursor.SetCursor(normalCursor, Vector2.zero, CursorMode.Auto);
                break;
        }
    }
    
}
