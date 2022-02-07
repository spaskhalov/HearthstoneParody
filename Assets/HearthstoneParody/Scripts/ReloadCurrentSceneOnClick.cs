using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace HearthstoneParody
{
    [RequireComponent(typeof(Button))]
    public class ReloadCurrentSceneOnClick : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            GetComponent<Button>().onClick.AddListener(() =>
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            });
        }
        
    }
}
