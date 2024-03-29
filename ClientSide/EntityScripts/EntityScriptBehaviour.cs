﻿using SNet_Client.PacketCouriers.Entities;
using SNet_Client.Sockets;
using UnityEngine;

namespace SNet_Client.EntityScripts
{
    public class EntityScriptBehaviour : MonoBehaviour
    {
        protected NetworkedEntity networkedEntity;

        [SerializeField]
        protected bool Serialize = false;

        public bool IsToSerialize()
        {
            return Serialize;
        }

        [SerializeField]
        protected string UniqueScriptIdentifingString;

        [SerializeField]
        private bool HasGeneratedScriptId = false;
        [SerializeField]
        private int ScriptID = 0;
        public int GetScriptID()
        {
            if (!HasGeneratedScriptId)
            {
                ScriptID = Utils.Util.GerarHashInt(UniqueScriptIdentifingString);
                HasGeneratedScriptId = true;
            }
            return ScriptID;
        }

        protected virtual void Start()
        {
            networkedEntity = GetComponent<NetworkedEntity>();
        }
        public virtual void OnSerialize(ref PacketWriter writer)
        {
        }
        public virtual void OnDeserialize(ref PacketReader reader, ReceivedPacketData receivedPacketData)
        {
        }
        public virtual void OnReceiveMessage(byte[] messageData, ReceivedPacketData receivedPacketData)
        {
        }
        protected virtual void OnDestroy()
        {
            if (networkedEntity != null)
                networkedEntity.RemoveEntityScript(this);
        }
    }
}
