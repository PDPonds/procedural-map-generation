using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState
{
    Idle, Chase, Attack
}

public class Enemy : IDamageable
{
    [SerializeField] LayerMask targetMask;
    EnemyState state;
    NavMeshAgent nav;
    Transform target;
    [SerializeField] GameObject attackCollider;

    float curDelayAttack;

    public override int curHP { get; set; }
    public override int maxHP { get; set; }
    public override Faction faction { get; set; }

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
                StartCoroutine(Attack());
                break;
        }
    }

    IEnumerator Attack()
    {
        attackCollider.SetActive(true);
        yield return new WaitForSeconds(0.25f);
        attackCollider.SetActive(false);
        curDelayAttack = 2f;
        SwitchState(EnemyState.Chase);
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
                    if (dis < 0.25f)
                    {
                        if (curDelayAttack == 0)
                        {
                            SwitchState(EnemyState.Attack);
                        }
                        else
                        {
                            nav.SetDestination(target.position);
                        }
                    }
                    else
                    {
                        nav.SetDestination(target.position);
                    }
                }

                Collider[] cols = Physics.OverlapSphere(transform.position, 2f, targetMask);
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
                nav.SetDestination(transform.position);
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

    public override void Death()
    {
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 2f);
    }

}
