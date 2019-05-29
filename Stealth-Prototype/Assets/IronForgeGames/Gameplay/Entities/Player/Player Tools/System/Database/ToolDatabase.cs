using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Sirenix.OdinInspector;

namespace AH.Max.System
{
    [Serializable]
    public class ToolDatabaseSlot
    {
        public IdentityType tool;
        public bool locked;
    }

    public class ToolDatabase : Singleton_SerializedMonobehaviour<ToolDatabase>
    {
        [TabGroup(Tabs.Properties)]
        [SerializeField]
        private List<ToolDatabaseSlot> database = new List<ToolDatabaseSlot>();
        public List<ToolDatabaseSlot> Database 
        {
            get 
            {
                return database;
            }
        }

        [TabGroup(Tabs.Events)]
        public static ToolLockedEvent toolLockedEvent = new ToolLockedEvent();

        [TabGroup(Tabs.Events)]
        public static ToolUnLockedEvent toolUnLockedEvent = new ToolUnLockedEvent();

        public static bool IsLocked(IdentityType tool)
        {
            ToolDatabaseSlot _slot = Instance.database.Find(_tool => _tool.tool == tool);

            if(_slot != null)
            {
                return _slot.locked;
            }

            Debug.LogWarning("The given tool: " + tool + " was not found in the tool database");
            return false;
        }

        public static bool DatabaseContains(IdentityType tool)
        {
            ToolDatabaseSlot _slot = Instance.database.Find(_tool => _tool.tool == tool);

            if(_slot != null)
            {
                return true;
            }

            return false;
        }

        public static void UnLockTool(IdentityType tool)
        {
            ToolDatabaseSlot _slot = Instance.database.Find(_tool => _tool.tool == tool);

            if (_slot != null)
            {
                _slot.locked = false;

                if(toolUnLockedEvent != null)
                {
                    toolUnLockedEvent.Invoke(tool);
                }
                return;
            }

            Debug.LogWarning("The tool: " +  tool.name + " does not exist in the tool database");
        }

        public static void LockTool(IdentityType tool)
        {
            ToolDatabaseSlot _slot = Instance.database.Find(_tool => _tool.tool == tool);

            if (_slot != null)
            {
                _slot.locked = true;

                if(toolLockedEvent != null)
                {
                    toolLockedEvent.Invoke(tool);
                }

                return;
            }

            Debug.LogWarning("The tool: " + tool.name + " does not exist in the tool database");
        }

        public static List<IdentityType> GetTools()
        {
            List<IdentityType> _tools = new List<IdentityType>();

            foreach (ToolDatabaseSlot _slot in Instance.database)
            {
                _tools.Add(_slot.tool);
            }

            return _tools;
        }

        public static List<IdentityType> GetAllLockedTools()
        {
            List<IdentityType> _tools = new List<IdentityType>();

            foreach (ToolDatabaseSlot _slot in Instance.database)
            {
                if (_slot.locked)
                {
                    _tools.Add(_slot.tool);
                }
            }

            return _tools;
        }

        public static List<IdentityType> GetAllUnlockedTools()
        {
            List<IdentityType> _tools = new List<IdentityType>();

            foreach (ToolDatabaseSlot _slot in Instance.database)
            {
                if (!_slot.locked)
                {
                    _tools.Add(_slot.tool);
                }
            }

            return _tools;
        }
    }
}
