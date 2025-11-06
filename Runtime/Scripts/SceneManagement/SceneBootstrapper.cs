using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace TigerFrogGames.Core
{
    public class SceneBootstrapper : MonoBehaviour
    {
        
        //This force unloads all other scenes and just loads the "core" scene which lives for the lifetime of the entire game
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Initialize()
        {
            Debug.Log("SceneBootstrapper initialized");
#if UNITY_EDITOR
            EditorSceneManager.playModeStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(EditorBuildSettings.scenes[0].path);
#endif
        }
    }
}