using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace TheKiwiCoder {

    // The context is a shared object every node has access to.
    // Commonly used components and subsytems should be stored here
    // It will be somewhat specfic to your game exactly what to add here.
    // Feel free to extend this class 
    public class Context {
        public GameObject GameObject;
        public Transform Transform;
        public Animator Animator;
        public Rigidbody Physics;
        public NavMeshAgent Agent;
        public SphereCollider SphereCollider;
        public BoxCollider BoxCollider;
        public CapsuleCollider CapsuleCollider;
        public CharacterController CharacterController;

        public Player.Player Player;
        // Add other game specific systems here

        public static Context CreateFromGameObject(GameObject gameObject) {
            // Fetch all commonly used components
            Context context = new Context();
            context.GameObject = gameObject;
            context.Transform = gameObject.transform;
            context.Animator = gameObject.GetComponent<Animator>();
            context.Physics = gameObject.GetComponent<Rigidbody>();
            context.Agent = gameObject.GetComponent<NavMeshAgent>();
            context.SphereCollider = gameObject.GetComponent<SphereCollider>();
            context.BoxCollider = gameObject.GetComponent<BoxCollider>();
            context.CapsuleCollider = gameObject.GetComponent<CapsuleCollider>();
            context.CharacterController = gameObject.GetComponent<CharacterController>();
            
            // Add whatever else you need here...

            return context;
        }
    }
}