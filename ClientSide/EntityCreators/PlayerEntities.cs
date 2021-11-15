﻿using UnityEngine;

using SNet_Client.PacketCouriers.Entities;
using SNet_Client.PacketCouriers;
using SNet_Client.EntityScripts.TransfromSync;
using SNet_Client.Utils;

namespace SNet_Client.EntityCreators
{
    public class PlayerEntities : MonoBehaviour
    {
        bool hasSpawnedPlayer = false;
        public void Start()
        {
            EntityInitializer.client_EntityInitializer.AddGameObjectPrefab("PlayerEntity", CreatePlayerEntity);
            ServerInteraction.OnReceiveOwnerID += ServerInteraction_OnReceiveOwnerID;

            GlobalMessenger<DeathType>.AddListener("PlayerDeath", OnPlayerDeath);
            GlobalMessenger<int>.AddListener("StartOfTimeLoop", OnStartOfTimeLoop);
            GlobalMessenger.AddListener("ResumeSimulation", OnResumeSimulation);
        }
        public void OnDestroy()
        {
            GlobalMessenger<DeathType>.RemoveListener("PlayerDeath", OnPlayerDeath);
            GlobalMessenger<int>.RemoveListener("StartOfTimeLoop", OnStartOfTimeLoop);
            GlobalMessenger.RemoveListener("ResumeSimulation", OnResumeSimulation);
        }
        private void OnStartOfTimeLoop(int loop)
        {
            if(!hasSpawnedPlayer)
                SpawnPlayer();
        }
        private void OnResumeSimulation()
        {
            if (!hasSpawnedPlayer)
                SpawnPlayer();
        }
        private void OnPlayerDeath(DeathType deathType)
        {
            hasSpawnedPlayer = false;
        }
        private void ServerInteraction_OnReceiveOwnerID()
        {
            if (!hasSpawnedPlayer)
                SpawnPlayer();
        }

        private void SpawnPlayer()
        {
            Transform playerTransf = Locator.GetPlayerTransform();
            if (playerTransf != null)
                EntityInitializer.client_EntityInitializer.InstantiateEntity("PlayerEntity", playerTransf.position, playerTransf.rotation, EntityInitializer.InstantiateType.Buffered);

            hasSpawnedPlayer = true;
        }

        public NetworkedEntity CreatePlayerEntity(Vector3 position, Quaternion rotation, int ownerID, object[] InitializationData)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);

            go.layer = LayerMask.NameToLayer("Primitive");

            CapsuleCollider c = go.GetComponent<CapsuleCollider>();
            c.radius = 0.5f;
            c.height = 2f;
            c.enabled = false;

            Rigidbody rigidbody = go.AddComponent<Rigidbody>();
            rigidbody.mass = 0.001f;
            rigidbody.drag = 0f;
            rigidbody.angularDrag = 0f;

            go.AddComponent<OWRigidbody>();

            go.transform.position = position;
            go.transform.rotation = rotation;

            if (ownerID == ServerInteraction.GetOwnerID())
            {
                go.transform.parent = Locator.GetPlayerTransform();
                rigidbody.isKinematic = false;
            }
            

            NetworkedEntity networkedEntity = go.AddComponent<NetworkedEntity>();

            TransformEntitySync transformEntitySync = networkedEntity.AddEntityScript<TransformEntitySync>();
            transformEntitySync.syncTransformType = SyncTransform.PositionAndRotationOnly;
            transformEntitySync.referenceFrame = ReferenceFrames.Timber_Hearth;

            RigidbodyEntitySync rigibodyEntitySync = networkedEntity.AddEntityScript<RigidbodyEntitySync>();
            rigibodyEntitySync.syncRigidbodyType = SyncRigidbody.Both;

            return networkedEntity;
        }
    }
}