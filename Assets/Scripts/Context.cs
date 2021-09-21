using UnityEngine;
using UnityEngine.AI;

public class Context
{
        public GameObject   gameObject;
        public Transform    transform;
        public Animator     animator;
        public NavMeshAgent agent;
        public GameObject   player;
        
        public static Context CreateFromGameObject(GameObject gameObject) {
                // Fetch all commonly used components
                Context context = new Context();
                context.gameObject = gameObject;
                context.transform = gameObject.transform;
                context.animator = gameObject.GetComponent<Animator>();
                context.agent = gameObject.GetComponent<NavMeshAgent>();
                context.player = GameObject.FindGameObjectWithTag("Player");

                return context;
        }
}