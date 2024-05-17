using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour, IDamagable
{
    Controller controller;
    public void SetOwner(Controller _controller, bool mine)
    {
        controller = _controller;
        Collider collider = GetComponent<Collider>();
        if(mine)
            this.gameObject.layer = 12;
        else
            this.gameObject.layer = 6;
        if (collider != null && collider.isTrigger == false)
            collider.isTrigger = true;
    }
    public void TakeDamage(int _damage)
        => controller.Damage(_damage);

    void Start()
    {

    }

}