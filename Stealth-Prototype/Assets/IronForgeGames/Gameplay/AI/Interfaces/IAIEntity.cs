using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Sirenix.OdinInspector;

namespace AH.Max.Gameplay.AI
{
    public interface IAIEntity
    {
        NavMeshAgent navMeshAgent { get; }

        void Move( Vector3 _target, float range );
    }
}
