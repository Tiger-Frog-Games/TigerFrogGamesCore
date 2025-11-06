using UnityEngine;

namespace TigerFrogGames.Core
{
    public class AudioPlayer : MonoBehaviour
    {
        /* ------- Variables ------- */

        [SerializeField] private SoundDataName soundToPlay;

        /* ------- Unity Methods ------- */
        
       

        /* ------- Methods ------- */
        public void PlayAudioOnHover() 
        {
            SoundManager.Instance.CreateSoundBuilder()
                .WithRandomPitch()
                .Play(SoundSFXLibrary.Instance.GetSoundByEnum(soundToPlay));
        }
        
    }
}