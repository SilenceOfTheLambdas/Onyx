using UnityEngine;
using Player;

public class EnemyHitDetection : MonoBehaviour
{
    public AbilitiesSystem playerAbilitySystem;

    private void Start()
    {
        playerAbilitySystem = GameManager.Instance.player.GetComponent<AbilitiesSystem>();
    }
}
