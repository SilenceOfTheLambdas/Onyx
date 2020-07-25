using System.Collections;
using System.Collections.Generic;
using Scriptable_Objects.Items.Scripts;
using UnityEditor;
using UnityEngine;

public class GroundItem : MonoBehaviour, ISerializationCallbackReceiver
{
    public ItemObject item;
    public void OnBeforeSerialize()
    {
        GetComponent<SpriteRenderer>().sprite = item.PSprite;
        // EditorUtility.SetDirty(GetComponent<SpriteRenderer>());
    }

    public void OnAfterDeserialize()
    {
    }
}
