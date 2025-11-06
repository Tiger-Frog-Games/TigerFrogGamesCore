using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TigerFrogGames.Core
{
    public class SceneLoadManager 
    {
        public event Action<String> OnSceneLoaded = delegate { };
        public event Action<String> OnSceneUnloaded = delegate { };
        public event Action<String> OnSceneGroupLoaded = delegate { };
        
        SceneGroup ActiveSceneGroup;

        public async Task LoadSceneGroupAsync(SceneGroup sceneGroupIn, IProgress<float> progress, bool unloadOtherScenes = false, bool unloadGameplayScene = false)
        {
            if (unloadOtherScenes || ActiveSceneGroup == null)
            {
                ActiveSceneGroup = sceneGroupIn;
            }
            
            if (unloadOtherScenes || unloadGameplayScene)
            {
                await UnloadScenes(unloadGameplayScene);
            }
            
            var loadedScenes = new List<string>();
            int sceneCount = SceneManager.sceneCount;

            for (int i = 0; i < sceneCount; i++)
            {
                loadedScenes.Add(SceneManager.GetSceneAt(i).name);
            }
            
            var totalScenesToLoad = sceneGroupIn.Scenes.Count;
            var operationGroup = new AsyncOperationsGroup(totalScenesToLoad);
            
            for (int i = 0; i < totalScenesToLoad; i++)
            {
                var sceneData = sceneGroupIn.Scenes[i];
                if(loadedScenes.Contains(sceneData.Name)) continue;
                
                var operation = SceneManager.LoadSceneAsync(sceneData.Name, LoadSceneMode.Additive);
                
                operationGroup.Operations.Add(operation);
                
                OnSceneLoaded?.Invoke(sceneData.Name);
            }
            
            
            while (!operationGroup.IsDone)
            {
                progress.Report(operationGroup.Progress);
                await Task.Delay(100);
            }
            
            Scene activeScene = SceneManager.GetSceneByName(ActiveSceneGroup.FindSceneByType(SceneType.ActiveScene));

            if (activeScene.IsValid())
            {
                SceneManager.SetActiveScene(activeScene);
            }
            
            OnSceneGroupLoaded?.Invoke(ActiveSceneGroup.GroupName);
        }
        
        public async Task UnloadScenes(bool unloadActiveScene = false)
        {
            var scenes = new List<string>();
            var activeScene = SceneManager.GetActiveScene().name;
            int sceneCount = SceneManager.sceneCount;

            for (int i = 0; i < sceneCount; i++)
            {
                var sceneAt = SceneManager.GetSceneAt(i);
                if(!sceneAt.isLoaded) continue;
                
                var sceneName = sceneAt.name;
                if((sceneName.Equals(activeScene) && !unloadActiveScene) || sceneName == "Core") continue;
                
                scenes.Add(sceneName);
            }
            
            var operationGroup = new AsyncOperationsGroup(scenes.Count);

            foreach (var scene in scenes)
            {
                var operation = SceneManager.UnloadSceneAsync(scene);
                if(operation == null) continue;
                
                operationGroup.Operations.Add(operation);
                OnSceneUnloaded?.Invoke(scene);
            }

            while (!operationGroup.IsDone)
            {
                await Task.Delay(100); //slight delay to prevent looping too fast
            }
        }
        
    }

    public readonly struct AsyncOperationsGroup
    {
        public readonly List<AsyncOperation> Operations;

        public float Progress => Operations.Count == 0 ? 0 : Operations.Average(o => o.progress );
        public bool IsDone => Operations.All(o => o.isDone);
        
        public AsyncOperationsGroup(int initialCapacity)
        {
            Operations = new List<AsyncOperation>(initialCapacity);
        }
    }
    
}