using Enemies;
using UnityEngine;

public class AttackPlayerEvent : MonoBehaviour
{
    private Enemy _enemy;

    private void Start()
    {
        _enemy = GetComponentInParent<Enemy>();
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
    private void AttackPlayer()
    {
        _enemy.Attack();
    }
}
