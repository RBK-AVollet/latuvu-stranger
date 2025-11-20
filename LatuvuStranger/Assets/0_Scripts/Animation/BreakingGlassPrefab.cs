using UnityEngine;

namespace Latuvu
{
    public class BreakingGlassPrefab : MonoBehaviour
    {
        void Start()
        {
            var animator = GetComponent<Animator>();
            float delay = animator.GetCurrentAnimatorStateInfo(0).length;
            Destroy(gameObject, delay);
        }
    }
}
