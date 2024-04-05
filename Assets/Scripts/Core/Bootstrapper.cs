using ARKitect.Items.Import;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ARKitect.Core
{
    /// <summary>
    /// Init app resources at launch
    /// </summary>
    [AddComponentMenu("ARkitect/Bootstrapper")]
    public class Bootstrapper : MonoBehaviour
    {
        [SerializeField]
        private InternalImporter internalItemsImporter;

        public void Awake()
        {
            Application.runInBackground = true;

            if(internalItemsImporter == null )
                internalItemsImporter = FindObjectOfType<InternalImporter>();

            // Load assets
            internalItemsImporter?.Import();

            if (SceneManager.loadedSceneCount == 1)
                SceneManager.LoadScene("ARkitectEditor", LoadSceneMode.Additive);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Init()
        {
#if UNITY_EDITOR
            var currentlyLoadedEditorScene = SceneManager.GetActiveScene();
#endif

            if (SceneManager.GetSceneByName("ARkitectBootstrapper").isLoaded != true)
                SceneManager.LoadScene("ARkitectBootstrapper");

#if UNITY_EDITOR
            if (currentlyLoadedEditorScene.IsValid())
                SceneManager.LoadSceneAsync(currentlyLoadedEditorScene.name, LoadSceneMode.Additive);
#else
        SceneManager.LoadSceneAsync("ARkitectEditor", LoadSceneMode.Additive);
#endif
        }
    }

}