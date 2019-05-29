using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

using AH.Max.System;
using AH.Max.System.Stats;

///<Summary>
///The base for every "Actor" in the scene. This will be on every object that we want to interact with. 
///</Summary>
namespace AH.Max
{
    public class Entity : SerializedMonoBehaviour, IEntity
    {
        /// <summary>
        /// 
        /// </summary>
        [TabGroup(Tabs.Entity)]
        [SerializeField]
        [InlineEditor]
        private IdentityType identityType;
        public IdentityType IdentityType
        {
            get { return identityType; }
        }

        /// <summary>
        /// 
        /// </summary>
        [TabGroup(Tabs.Entity)]
        [SerializeField]
        private bool doNotDestroyOnLoad;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        [TabGroup(Tabs.Stats)]
        private List<Stat> stats = new List<Stat>();

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        [TabGroup(Tabs.Stats)]
        private List<Modifier> modifiers = new List<Modifier>();

        public void OnEnable()
        {
            if(identityType == null)
            {
                Debug.LogError("All Entities must have a valid identity type" + gameObject);
                return;
            }
        
            EntityManager.Instance.RegisterEntity(this);

            if(doNotDestroyOnLoad)
            {
                DontDestroyOnLoad(gameObject);
            }

            Enable();
        }

        public void OnDisable() 
        {
            if(identityType == null)
            {
                Debug.LogError("All Entities must have a valid identity type");
                return;
            }

            EntityManager.Instance.RemoveEntity(this);

            Disable();
        }

        ///<Summary>
        ///Override this method to add additional OnEnable logic
        ///</Summary>
        protected virtual void Enable()
        {
        }

        ///<Summary>
        ///Override this method to add additional OnDisable logic.
        ///</Summary>
        protected virtual void Disable()
        {
        }

        public Stat GetStat(StatType statType)
        {
            return stats.Find(_stat => _stat.statType == statType);
        }

        public void SetStat(StatType statType, float amount)
        {
            GetStat(statType).Amount = amount;
        }

        public bool HasStat(StatType statType)
        {
            return stats.Find(_stat => _stat.statType == statType) != null;
        }
    }
}
