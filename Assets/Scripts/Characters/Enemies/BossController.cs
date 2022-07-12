using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : EnemyController
{
    [SerializeField] float continuousFireDuration = 1.5f;

    [Header("---- PLAYER DETECTION ----")]
    [SerializeField] Transform playerDetectionTransform;
    [SerializeField] Vector3 playerDetectionSize;
    [SerializeField] LayerMask playerLayer;

    [Header("---- BEAM ----")]
    [SerializeField] float beamCooldownTime = 12f;
    [SerializeField] AudioData beamChargingSFX;
    [SerializeField] AudioData beamLaunchSFX;

    bool isBeamReady;

    int launchBeamID = Animator.StringToHash("launchBeam");

    WaitForSeconds waitForContinuousFireInterval;
    WaitForSeconds waitForFireInterval;
    WaitForSeconds waitForBeamCooldownTime;

    List<GameObject> magazine;

    AudioData launchSFX;
    Animator animator;

    Transform playerTransform;

    protected override void Awake()
    {
        base.Awake();

        animator = GetComponent<Animator>();

        waitForContinuousFireInterval = new WaitForSeconds(minFireInerval);
        waitForFireInterval = new WaitForSeconds(maxFireInterval);
        waitForBeamCooldownTime = new WaitForSeconds(beamCooldownTime);

        magazine = new List<GameObject>(projectiles.Length);

        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    protected override void OnEnable()
    {
        isBeamReady = false;
        muzzleVFX.Stop();
        StartCoroutine(nameof(BeamCooldownCoroutine));
        base.OnEnable();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(playerDetectionTransform.position, playerDetectionSize);
    }

    void ActiveBeamWeapon()
    {
        isBeamReady = false;
        animator.SetTrigger(launchBeamID);
        AudioManager.Instance.PlayRandomSFX(beamChargingSFX);
    }

    void AnimationEventLaunchBeam()
    {
        AudioManager.Instance.PlayRandomSFX(beamLaunchSFX);
    }

    void AnimationEventStopBeam()
    {
        StopCoroutine(nameof(ChasingPlayerCoroutine));
        StartCoroutine(nameof(BeamCooldownCoroutine));
        StartCoroutine(nameof(RandomlyFireCoroutine));
    }

    void LoadProjectiles()
    {
        magazine.Clear();

        if(Physics2D.OverlapBox(playerDetectionTransform.position, playerDetectionSize, 0f, playerLayer))
        {
            magazine.Add(projectiles[0]);
            launchSFX = projectileLaunchSFX[0];
        }
        else
        {
            if(Random.value < 0.5f)
            {
                magazine.Add(projectiles[1]);
                launchSFX = projectileLaunchSFX[1];
            }
            else
            {
                for(int i=2;i<projectiles.Length;i++)
                {
                    magazine.Add(projectiles[i]);
                }
                launchSFX = projectileLaunchSFX[2];
            }
        }
    }

    protected override IEnumerator RandomlyFireCoroutine()
    {
        while(isActiveAndEnabled)
        {
            if(GameManager.GameState == GameState.GameOver) yield break;

            if(isBeamReady)
            {
                ActiveBeamWeapon();
                StartCoroutine(nameof(ChasingPlayerCoroutine));

                yield break;
            }

            yield return waitForFireInterval;
            yield return StartCoroutine(nameof(ContinuousFireCoroutine));
        }

    }

    IEnumerator ContinuousFireCoroutine()
    {
        LoadProjectiles();
        muzzleVFX.Play();

        float continuousFireTimer = 0f;

        while(continuousFireTimer < continuousFireDuration)
        {
            foreach(var projectile in magazine)
            {
                PoolManager.Release(projectile, muzzle.position);
            }

            continuousFireTimer += minFireInerval;
            AudioManager.Instance.PlayRandomSFX(launchSFX);

            yield return waitForContinuousFireInterval;
        }

        muzzleVFX.Stop();
    }

    IEnumerator BeamCooldownCoroutine()
    {
        yield return waitForBeamCooldownTime;

        isBeamReady = true;
    }

    IEnumerator ChasingPlayerCoroutine()
    {
        while(isActiveAndEnabled)
        {
            targetPosition.x = Viewport.Instance.MaxX - paddingX;
            targetPosition.y = playerTransform.position.y;

            yield return null;
        }
    }
}
