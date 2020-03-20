using UnityEngine;
using UnityEngine.SceneManagement;

namespace ActionCode.Demo
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class GoScene : MonoBehaviour
    {
        public string nextSceneName = "";

        private void Reset()
        {
            BoxCollider2D collider = GetComponent<BoxCollider2D>();
            collider.isTrigger = true;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player") && nextSceneName.Length > 0)
            {
                SceneManager.LoadScene(nextSceneName);
            }            
        }
    }
}