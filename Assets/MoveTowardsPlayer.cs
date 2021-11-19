using UnityEngine;

public class MoveTowardsPlayer : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    private Vector3 _playerPosition;

    private void Start()
    {
        _playerPosition = GameManager.Instance.player.transform.position;
    }

    private void Update()
    {
        _playerPosition = GameManager.Instance.player.transform.position;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var direction = transform.position - _playerPosition;
            // transform.position += direction * moveSpeed * Time.deltaTime;
            // transform.position += Vector3.Lerp(transform.position, _playerPosition, Time.deltaTime * moveSpeed);
            transform.SetPositionAndRotation(Vector3.Lerp(transform.position, _playerPosition, Time.deltaTime * moveSpeed), Quaternion.identity);
        }
    }
}
