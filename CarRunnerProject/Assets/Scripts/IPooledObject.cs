using System;
using UnityEngine;

public interface IPooledObject
{
    public GameObject GameObject
    {
        get;
    }

    public event Action<IPooledObject> ReturnToPoolAction;

    public virtual void ReturnToPool()
    {
        
    }


}
