using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeProjectile : MonoBehaviour
{
    public static event EventHandler OnAnyGrenadeExploded;

    [SerializeField] private Transform grenadeExplodeVFXPrefab;

    private Vector3 targetPosition;
    private Action OnGrenadeBehaviourComplete;

    private void Update()
    {
        Vector3 moveDirection = (targetPosition - transform.position).normalized;
        float moveSpeed = 15f;

        transform.position += moveDirection * moveSpeed * Time.deltaTime;

        float reachedTargetDistance = .2f;
        if (Vector3.Distance(transform.position, targetPosition) < reachedTargetDistance)
        {
            float damageRadius = 4f;
            Collider[] colliderArray = Physics.OverlapSphere(targetPosition, damageRadius);

            foreach(Collider collider in colliderArray)
            {
                if (collider.TryGetComponent<Unit>(out Unit targetUnit))
                {
                    targetUnit.Damage(40);
                }
            }

            OnAnyGrenadeExploded?.Invoke(this, EventArgs.Empty);

            Instantiate(grenadeExplodeVFXPrefab, targetPosition + Vector3.up * 1f, Quaternion.identity);

            Destroy(gameObject);

            OnGrenadeBehaviourComplete();
        }
    }

    public void Setup(GridPosition targetGridPosition, Action OnGrenadeBehaviourComplete)
    {
        this.OnGrenadeBehaviourComplete = OnGrenadeBehaviourComplete;
        targetPosition = LevelGrid.Instance.GetWorldPosition(targetGridPosition);
    }
}
