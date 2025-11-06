using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace TigerFrogGames.Core
{

    public enum GamePauseState
    {
        Gameplay,
        Paused
    }
    
    public class GamePauseManager 
    {
        private static GamePauseManager _instance;
        public static GamePauseManager Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new GamePauseManager();
                }
                return _instance;
            }
        }

        
        private GamePauseManager()
        {
            pausedSources = new Dictionary<string, int>();
        }
        
        public GamePauseState CurrentGamePauseState { get; private set; }
        public event Action<GamePauseState> OnPauseStateChanged;

        private Dictionary<string, int> pausedSources;
        
        public void SetState( string source,  GamePauseState newGamePauseState )
        {
            //if (CurrentGameState == newGameState){ return; }

            if (newGamePauseState == GamePauseState.Paused)
            {
                pausedSources.TryAdd(source, 0);
                
                pausedSources[source]++;

                if (CurrentGamePauseState == GamePauseState.Gameplay)
                {
                    CurrentGamePauseState = GamePauseState.Paused;
                    OnPauseStateChanged?.Invoke(newGamePauseState);
                }
                return;
            }

            if (newGamePauseState == GamePauseState.Gameplay)
            {
                if (pausedSources != null)
                {
                    pausedSources[source]--;

                    if (pausedSources[source] < 0)
                    {
                        pausedSources[source] = 0;
                    }

                    foreach (var dictionaryKey in GamePauseManager.Instance.pausedSources)
                    {
                        if (dictionaryKey.Value != 0)
                        {
                            return;
                        }
                    }
                }

                
                if (CurrentGamePauseState != GamePauseState.Paused) return;
                
                //all sources of pause are removed
                CurrentGamePauseState = GamePauseState.Gameplay;
                OnPauseStateChanged?.Invoke(GamePauseState.Gameplay);

            }
        }

        public void ClearPausedSources()
        {
            pausedSources.Clear();
        }

        #if UNITY_EDITOR
        
        [MenuItem("Tiger Frog Games/DebugGameStateManager")]
        private static void DebugGameStateManager()
        {
            if (GamePauseManager.Instance.pausedSources.Count == 0)
            {
                Debug.Log("Game State Manager is empty");  
            }
            
            foreach (var dictionaryKey in GamePauseManager.Instance.pausedSources)
            {
                Debug.Log(dictionaryKey);
            }
        }

        [InitializeOnEnterPlayMode]
        static void InitializeOnEnterPlayMode()
        {
            Instance.ClearPausedSources();
        }
        
        #endif
        
    }
}