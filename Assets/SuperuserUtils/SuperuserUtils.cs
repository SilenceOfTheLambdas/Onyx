using UnityEngine;
using UnityEngine.InputSystem;

namespace SuperuserUtils
{
    public class SuperuserUtils : GenericSingletonClass<SuperuserUtils>
    {
        private Camera _mainCamera;

        private void Start()
        {
            _mainCamera = Camera.main;
        }

        /// <summary>
        /// Is the mouse hovering over a <see cref="GameObject"/> with a matching tagToCompare?
        /// </summary>
        /// <param name="tagToCompare">The tagToCompare of the <see cref="GameObject"/> to find</param>
        /// <param name="obj">The <see cref="GameObject"/> found, null if none was found</param>
        /// <returns>If there is an object matching the tagToCompare given under the mouse pointer</returns>
        public bool IsTheMouseHoveringOverGameObject(string tagToCompare, out GameObject obj)
        {
            if (GetGameObjectAtMousePosition() != null)
            {
                var go = GetGameObjectAtMousePosition();
                if (go.CompareTag(tagToCompare))
                {
                    obj = go;
                    return true;
                }
            }
            obj = null;
            return false;
        }

        /// <summary>
        /// Is the mouse hovering over a <see cref="GameObject"/> on a matching <see cref="LayerMask"/>?
        /// </summary>
        /// <param name="layer">The <see cref="LayerMask"/> the game object is on</param>
        /// <param name="obj">The object found, will be null of none is found</param>
        /// <returns>If there is an object matching the layer given under the mouse pointer</returns>
        public bool IsTheMouseHoveringOverGameObject(LayerMask layer, out GameObject obj)
        {
            if (GetGameObjectAtMousePosition(layer) != null)
            {
                obj = GetGameObjectAtMousePosition();
                return true;
            }
            obj = null;
            return false;
        }

        /// <summary>
        /// Returns an Enemy that is underneath the cursor.
        /// </summary>
        /// <returns>The Enemy that is under the mouse pointer.</returns>
        public LayerMask GetLayerFromMouseHover()
        {
            var mRay = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            Physics.Raycast(mRay, out RaycastHit hit, Mathf.Infinity);
            if (hit.transform != null)
                return hit.transform.gameObject.layer;

            return LayerMask.GetMask("Default");
        }

        #region MouseRaycast

        /// <summary>
        /// Performs a raycast from the mouse world position and gets the <see cref="GameObject"/> that is underneath the mouse pointer.
        /// </summary>
        /// <returns>The <see cref="GameObject"/> found, null is none was found</returns>
        public static GameObject GetGameObjectAtMousePosition()
        {
            if (Camera.main is { })
            {
                var mRay = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
                if (Physics.Raycast(mRay, out var mRaycastHit, Mathf.Infinity))
                {
                    return mRaycastHit.transform.gameObject;
                }
            }

            return default;
        }

        /// <summary>
        /// Performs a raycast from the mouse world position and gets the <see cref="GameObject"/> that is underneath the mouse pointer at a given <see cref="LayerMask"/>.
        /// </summary>
        /// <returns>The <see cref="GameObject"/> found, null is none was found</returns>
        public GameObject GetGameObjectAtMousePosition(LayerMask layerMask)
        {
            if (_mainCamera != null)
            {
                var mRay = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
                if (Physics.Raycast(mRay, out var mRaycastHit, Mathf.Infinity, layerMask))
                {
                    return mRaycastHit.transform.gameObject;
                }
            }

            return null;
        }

        #endregion
    }
}
