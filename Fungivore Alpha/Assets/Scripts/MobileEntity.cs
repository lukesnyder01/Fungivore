using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobileEntity : MonoBehaviour, IDamageable
{
    float IDamageable.Health { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void Damage(float damageAmount)
    { 
    
    }
}
