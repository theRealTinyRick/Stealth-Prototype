using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AH.Max.Gameplay.AI;
using AH.Max.System;

namespace AH.Max.System
{
    public class EntityManager :  Singleton_MonoBehavior <EntityManager>
    {
        ///<Summary>
        /// A reference to the player entity in the scene.
        ///</Summary>
        [SerializeField]
        private Entity _player;

        ///<Summary>
        /// A reference to the player entity in the scene.
        ///</Summary>
        public Entity Player
        {
            get
            { 
                if(!_player)
                {
                    Entity _player = FindPlayer();

                    if(_player == null)
                    {
                        Debug.LogError( "The player has not initialized or is not present in the scene. You may also want to make sure that your player object has the indentity type: Player." );
                        return null;
                    }
                }
                return _player; 
            }
            private set { _player = value; }
        }

        [SerializeField]
        private Entity gameCamera;

        public Entity GameCamera
        {
            get
            {
                if(!gameCamera)
                {
                    gameCamera = GetEntity(UsageType.Camera);
                    if(!gameCamera)
                    {
                        Debug.LogError( "The game camera is not present in the scene. You may need to initialize the resources scene" );
                    }
                }
                return gameCamera;
            }
            private set {gameCamera = value;}
        }

        [SerializeField]
        private List <Entity> _enemies = new List <Entity> ();
        public List <Entity> Enemies
        {
            get { return _enemies; }
        }

        [SerializeField]
        private List <Entity> entities = new List <Entity>();
        public List <Entity> Entities
        {
            get { return entities; }
        }

        public void RegisterEntity( Entity entity )
        {
            if(!entities.Contains(entity))
            {
                entities.Add(entity);
            }

            switch (entity.IdentityType.type)
            {
                case UsageType.Player:
                    _player = entity;
                    break;

                case UsageType.Enemy:
                    if(!_enemies.Contains(entity))
                    {
                        _enemies.Add(entity);
                    }
                    break;
            }
        }

        public void RemoveEntity( Entity entity )
        {
            if(entities.Contains(entity))
            {
                entities.Remove(entity);
            }

            if(_player == entity)
            {
                _player = null;
            }

            if(_enemies.Contains(entity))
            {
                _enemies.Remove(entity);
            }
        }

        private Entity FindPlayer()
        {
            foreach(Entity e in entities)
            {
                if(e.IdentityType.type == UsageType.Player)
                {
                    return e;
                }
            }

            return null;
        }

        ///<Summary>
        /// Returns a list of entities of a given Identity.
        ///</Summary>
        public static List <Entity> GetEntities( IdentityType type )
        {
            List <Entity> _entities = new List <Entity>();

            foreach(Entity e in Instance.entities)
            {
                if(e.IdentityType == type)
                {
                    _entities.Add(e);
                }
            }

            return _entities;
        }

        /// <summary>
        /// Get a list of entities in thhe game with the given list of identities
        /// </summary>
        /// <param name="types"></param>
        /// <returns></returns>
        public static List <Entity> GetEntities ( List <IdentityType> types )
        {
            List<Entity> _entities = new List<Entity>();

            foreach(IdentityType _type in types)
            {
                foreach (Entity e in Instance.entities)
                {
                    if (e.IdentityType == _type)
                    {
                        _entities.Add(e);
                    }
                }
            }

            //clean up the list just in case someone fucked it up

            return _entities;
        }

        public static void CleanUpTheEntityList(List<Entity> list)
        {

        }

        ///<Summary>
        ///Returns an entity of the given type. The first one found will returned.
        ///</Summary>
        public static Entity GetEntity( IdentityType type )
        {
            foreach(Entity _e in Instance.entities)
            {
                if(_e.IdentityType == type)
                {
                    return _e;
                }
            }
            return null;
        }

        ///<Summary>
        ///Find the entity in the list with the IdentityTypes
        ///</Summary>
        public static Entity GetEntity (UsageType type)
        {
            foreach(Entity _e in Instance.entities)
            {
                if(_e.IdentityType.type == type)
                {
                    return _e;
                }
            }

            return null;
        }
    }
}
