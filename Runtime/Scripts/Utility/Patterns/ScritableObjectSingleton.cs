using UnityEngine;

namespace TigerFrogGames.Core
{
    //Has to be in the resources folder for the singleton to be found
    
    public class ScriptableObjectSingleton<T> : ScriptableObject where T : ScriptableObject
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    T[] assets = Resources.LoadAll<T>("");
                    if(assets == null || assets.Length < 1)
                    {
                        throw new System.Exception($"Not found Singleton Scriptable Object of type: {typeof(T)}");
                    } else if (assets.Length > 1)
                    {
                        throw new System.Exception($"More than 1 instance of Singleton Scriptable Object of type: {typeof(T)} found");
                    }
                    _instance = assets[0];
                }
                return _instance;
            }
        }
    }
}