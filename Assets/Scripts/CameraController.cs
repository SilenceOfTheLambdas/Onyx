using System;
using System.Diagnostics;
using Enemies;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{

    [SerializeField] private LayerMask terrainLayerMask, enemyHitableLayerMask, defaultOrCannotMoveLayerMask;

    private void Update()
    {
        if (GameManager.Instance.player.inventoryOpen == false)
        {
            if (SuperuserUtils.SuperuserUtils.Instance.IsTheMouseHoveringOverGameObject(terrainLayerMask, out var _))
                CursorController.Instance.SetCursor(CursorController.CursorTypes.Move);
            if (SuperuserUtils.SuperuserUtils.Instance.IsTheMouseHoveringOverGameObject(enemyHitableLayerMask, out var _))
                CursorController.Instance.SetCursor(CursorController.CursorTypes.Attack);
            if (SuperuserUtils.SuperuserUtils.Instance.IsTheMouseHoveringOverGameObject(defaultOrCannotMoveLayerMask, out var _))
                CursorController.Instance.SetCursor(CursorController.CursorTypes.CannotMove);
        }
    }

    /// <summary>
    /// This will return the cursor position according to it's position in the world.
    /// </summary>
    /// <returns><code>Vector3</code>: The position of the mouse</returns>
    /// <exception cref="NullReferenceException">Will throw this exception if there is no main camera.</exception>
    public static Vector3 GetMouseWorldPosition()
    {
        if (Camera.main == null) throw new NullReferenceException("There is no Camera assigned to 'main'!");
        var vec = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        vec.y = 1f;
        return vec;
    }

    /// <summary>
    /// Will return the game object that is underneath the cursor.
    /// </summary>
    /// <returns>The GameObject</returns>
    public GameObject GetCursorObject()
    {
        var mousePosition = GetMouseWorldPosition();
        // shoot a raycast
        var hit = Physics2D.Raycast(mousePosition, mousePosition, 1, 1 << 9);
        
        if(hit.collider != null && !(hit.collider.GetComponent<GameObject>() is null))
            // If the ray cast hits a GameObject
        {
            return hit.collider.GetComponent<GameObject>();
        }

        return null;
    }

    /// <summary>
    /// Returns an Enemy that is underneath the cursor.
    /// </summary>
    /// <returns>The Enemy that is under the mouse pointer.</returns>
    private static Enemy GetEnemyOnCursor()
    {
        var mRay = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(mRay, out var mRaycastHit, Mathf.Infinity, LayerMask.GetMask("Enemy")))
        {
            return mRaycastHit.collider.gameObject.GetComponent<Enemy>();
        }

        return default;
    }
}
