using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Sirenix.OdinInspector;

namespace AH.Max.Gameplay.AI
{
    [Serializable]
    public class Action
    {
        [SerializeField]
        [TabGroup(Tabs.SetUp)]
        private ActionType type;
        public ActionType Type
        {
            get { return type; }
            set { type = value; }
        }

        [SerializeField]
        [TabGroup(Tabs.SetUp)]
        private string animation;
        public string Animation
        {
            get { return animation; }
            set { animation = value; }
        }

        [SerializeField]
        [TabGroup(Tabs.SetUp)]
        private ActionInteruptType[] possibleInterrupts;
        public ActionInteruptType[] PossibleInterrupts
        {
            //this is read only and can only be set in the inspector
            get { return possibleInterrupts; }
        }

#region Constructors
        public Action()
        {

        }

        public Action(string _animation, ActionType _type)
        {
            this.animation = _animation;
            this.type = _type;
        }

        public Action(string _animation, ActionType _type, ActionInteruptType[] _possibleInterrupts)
        {
            this.animation = _animation;
            this.type = _type;
            possibleInterrupts = _possibleInterrupts;
        }

#endregion
    }
}
