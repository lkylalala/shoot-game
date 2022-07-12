using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMissile : PlayerProjectileOverdrive
{
    [SerializeField] AudioData targetAcquiredVoice = null;

    [Header("---- SPEED CHANGE ----")]
    [SerializeField] float lowSpeed = 8f;
    [SerializeField] float highSpeed = 25f;
    [SerializeField] float variableSpeedDelay = 0.5f;

    [Header("---- EXPLOSION ----")]
    [SerializeField] GameObject explosionVFX = null;
    [SerializeField] AudioData explosionSFX = null;
    [SerializeField] float explosionRadius = 3f;
    [SerializeField] LayerMask enemyLayerMask = default;
    [SerializeField] float explosionDamage = 100f;

    WaitForSeconds waitVariableSpeedDelay;
    
    protected override void Awake()
    {
        base.Awake();
        waitVariableSpeedDelay = new WaitForSeconds(variableSpeedDelay);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        StartCoroutine(nameof(VariableSpeedCoroutine));
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);
        PoolManager.Release(explosionVFX, transform.position);

        AudioManager.Instance.PlayRandomSFX(explosionSFX);

        var colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius, enemyLayerMask);

        foreach(var collider in colliders)
        {
            if(collider.TryGetComponent<Enemy>(out Enemy enemy))
            {
                enemy.TakeDamage(explosionDamage);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }

    IEnumerator VariableSpeedCoroutine()
    {
        moveSpeed = lowSpeed;

        yield return waitVariableSpeedDelay;

        moveSpeed = highSpeed;

        if (target != null)
        {
            AudioManager.Instance.PlayRandomSFX(targetAcquiredVoice);
        }
    }
}
