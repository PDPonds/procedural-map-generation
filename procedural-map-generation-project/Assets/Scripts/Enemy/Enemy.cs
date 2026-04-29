using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState
{
    Idle, Chase, Attack
}

public enum EnemyType
{
    Close, Range
}

public class Enemy : IDamageable
{
    [SerializeField] LayerMask targetMask;
    EnemyState state;
    NavMeshAgent nav;
    Transform target;
    [Header("----- Type -----")]
    [SerializeField] EnemyType type;
    [Header("----- Close -----")]
    [SerializeField] GameObject attackCollider;
    [Header("----- Range -----")]
    [SerializeField] Transform bulletSpawnpoint;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] float rangeAttackBulletTime;
    [SerializeField] float rangeAttackBulletSpeed;

    [SerializeField] float attackDelay;
    [SerializeField] float attackDuration;
    [SerializeField] float changeTargetLength;
    [SerializeField] float attackRange;

    float curDelayAttack;
    float curAttackDuration;

    public override int curHP { get; set; }
    public override int maxHP { get; set; }
    public override Faction faction { get; set; }

    public int curDamage;

    private void Awake()
    {
        faction = Faction.Enemy;
        curHP = startHP;
        maxHP = startHP;
    }

    float curIdleTime;

    private void Start()
    {
        nav = GetComponent<NavMeshAgent>();
        SwitchState(EnemyState.Idle);
    }

    public void SwitchState(EnemyState state)
    {
        this.state = state;
        switch (state)
        {
            case EnemyState.Idle:
                curIdleTime = 2f;

                if (GameManager.Instance.ship != null)
                {
                    target = GameManager.Instance.ship.transform;
                }

                break;
            case EnemyState.Chase:

                if (target == null)
                {
                    if (GameManager.Instance.ship != null)
                    {
                        target = GameManager.Instance.ship.transform;
                    }
                    else
                    {
                        SwitchState(EnemyState.Idle);
                    }
                }

                break;
            case EnemyState.Attack:
                curAttackDuration = attackDuration;
                if (type == EnemyType.Close) { attackCollider.SetActive(true); }
                else
                {
                    GameObject bulletObj = Instantiate(bulletPrefab.gameObject, bulletSpawnpoint.position, Quaternion.identity);
                    Bullet bullet = bulletObj.GetComponent<Bullet>();
                    bullet.Setup(Faction.Enemy, transform.forward, rangeAttackBulletSpeed, rangeAttackBulletTime, curDamage);
                }
                break;
        }
    }

    void UpdateState()
    {
        switch (state)
        {
            case EnemyState.Idle:

                if (target != null)
                {
                    curIdleTime -= Time.deltaTime;
                    if (curIdleTime < 0)
                    {
                        SwitchState(EnemyState.Chase);
                    }
                }

                break;
            case EnemyState.Chase:

                if (target != null)
                {

                    float dis = Vector3.Distance(transform.position, target.position);
                    if (dis < attackRange)
                    {
                        nav.velocity = Vector3.zero;
                        if (curDelayAttack <= 0)
                        {
                            SwitchState(EnemyState.Attack);
                        }
                    }
                    else
                    {
                        nav.SetDestination(target.position);
                    }
                }
                else
                {
                    if (GameManager.Instance.ship != null)
                    {
                        target = GameManager.Instance.ship.transform;
                    }
                }

                Collider[] cols = Physics.OverlapSphere(transform.position, changeTargetLength, targetMask);
                if (cols.Length > 0)
                {
                    Collider col = cols[0];
                    if (col.transform.TryGetComponent<IDamageable>(out IDamageable idamageable))
                    {
                        target = col.transform;
                    }
                }

                break;
            case EnemyState.Attack:
                nav.velocity = Vector3.zero;
                if (curAttackDuration > 0)
                {
                    curAttackDuration -= Time.deltaTime;

                    if (curAttackDuration < attackDuration - attackDelay)
                    {
                        if (type == EnemyType.Close) { attackCollider.SetActive(false); }
                        curDelayAttack = attackDelay;
                    }

                    if (curAttackDuration <= 0)
                    {
                        SwitchState(EnemyState.Chase);
                    }
                }
                break;
        }
    }

    public bool IsState(EnemyState state)
    {
        return this.state == state;
    }

    private void Update()
    {
        UpdateState();

        if (curDelayAttack > 0)
        {
            curDelayAttack -= Time.deltaTime;
            if (curDelayAttack <= 0)
            {
                curDelayAttack = 0;
            }
        }

    }

    public override void TakeDamageEffect()
    {

    }

    public override void Death()
    {
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, changeTargetLength);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

}
