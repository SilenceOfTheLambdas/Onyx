using SuperuserUtils;
using UnityEngine;

public class EnemyAISettings : GenericSingletonClass<EnemyAISettings>
{
    [Tooltip("The movement speed of the enemy")]
    [SerializeField] private float enemySpeed = 2f;
    public static float EnemySpeed => Instance.enemySpeed;

    [Tooltip("The range at which the enemy can sight the player")]
    [SerializeField] private float aggroRadius = 6f;
    public static float AggroRadius => Instance.aggroRadius;

    [Tooltip("The range at which the enemy will melee the player")]
    [SerializeField] private float meleeAttackRange = 3f;
    public static float MeleeAttackRange => Instance.meleeAttackRange;

    [Tooltip("The range at which the enemy will use their skill")]
    [SerializeField] private float skillAttackRange = 4f;
    public static float SkillAttackRange => Instance.skillAttackRange;
}
