using UnityEngine;

public class Door : MonoBehaviour
{
    public Transform doorEntryPosition;

    public void OpenDoor()
    {
        FindObjectOfType<Player>().transform.position = doorEntryPosition.position;
    }
}
