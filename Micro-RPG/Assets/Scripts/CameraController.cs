using System;
using System.Collections;
using System.Collections.Generic;
using Enemies;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject target;
    [SerializeField] private float t = 5;

    private void Update()
    {
        var position = target.transform.position;
        transform.position = Vector3.Lerp(transform.position, new Vector3(position.x, position.y, -10), t);
        
        // Update cursor texture when hovering over an enemy
        if (GetEnemyOnCursor(1) != null && GetEnemyOnCursor(1).CompareTag("Enemy"))
        {
            CursorController.Instance.SetCursor(CursorController.CursorTypes.Attack);
        } else CursorController.Instance.SetCursor(CursorController.CursorTypes.Default);
    }

    /// <summary>
    /// This will return the cursor position according to it's position in the world.
    /// </summary>
    /// <returns><code>Vector3</code>: The position of the mouse</returns>
    /// <exception cref="NullReferenceException">Will throw this exception if there is no main camera.</exception>
    public static Vector3 GetMouseWorldPosition()
    {
        if (Camera.main == null) throw new NullReferenceException("There is no Camera assigned to 'main'!");
        var vec = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        vec.z = 0f;
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
    public Enemy GetEnemyOnCursor(float distance)
    {
        var mousePosition = GetMouseWorldPosition();
        // shoot a raycast
        var hit = Physics2D.Raycast(mousePosition, mousePosition, distance, 1 << 9);
        
        if(hit.collider != null && !(hit.collider.GetComponent<Enemy>() is null))
            // If the ray cast hits a GameObject
        {
            return hit.collider.GetComponent<Enemy>();
        }

        return null;
    }
}
