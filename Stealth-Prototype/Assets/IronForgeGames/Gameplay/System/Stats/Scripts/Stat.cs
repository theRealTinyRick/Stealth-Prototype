using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Sirenix.OdinInspector;

using AH.Max.System;

namespace AH.Max.System.Stats
{
    [Serializable]
    public class Stat
    {
        /// <summary>
        /// 
        /// </summary>
        [TabGroup(Tabs.Stats)]
        [SerializeField]
        public StatType statType;

        ///<Summary>
        /// The amount shown in the inspector to rep the statType
        ///</Summary>
        [TabGroup(Tabs.Stats)]
        [SerializeField]
        private float amount;
        public float Amount
        {
            get
            {
                return amount;
            }
            set
            {
                amount = value;
            }
        }

        ///<Summary>
        ///
        ///</Summary>
        [TabGroup(Tabs.Stats)]
        [SerializeField]
        private float minimumAmount;
        public float MinimumAmount
        {
            get 
            {
                return minimumAmount;
            }
            set
            {
                minimumAmount = value;
            }
        }

        ///<Summary>
        ///
        ///</Summary>
        [TabGroup(Tabs.Stats)]
        [SerializeField]
        private float maximumAmount;
        public float MaximumAmount
        {
            get 
            {
                return maximumAmount;
            }
            set
            {
                maximumAmount = value;
            }
        }

        public void Add()
        {
            Amount ++;
        }

        public void Subtract()
        {
            Amount --;
        }

        public void Add(float amountToAdd)
        {
            Amount += amountToAdd;

            if(Amount > MaximumAmount)
            {
                Amount = MaximumAmount;
            }
        }

        public void Subtract(float amountToSubtract)
        {
            Amount -= amountToSubtract;

            if(Amount < MinimumAmount)
            {
                Amount = MinimumAmount;
            }
        }

        public void Reset()
        {
            Amount = MaximumAmount;
        }

        public void RemoveAll()
        {
            Amount = MinimumAmount;
        }
    }
}
