using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitRagdollSpawner : MonoBehaviour
{
    [SerializeField] private Transform ragdollPrefab;

    private HealthSystem healthSystem;

    private void Awake()
    {
        healthSystem = GetComponent<HealthSystem>();

        healthSystem.OnDead += healthSystem_OnDead;
    }

    private void healthSystem_OnDead(object sender, EventArgs e)
    {
        Instantiate(ragdollPrefab, transform.position, transform.rotation);
    }
}
