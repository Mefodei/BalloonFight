using BalloonGame.Scripts.Models;
using UnityEngine;
using UnityTools.ActorEntityModel;

namespace BalloonGame.Scripts.Info
{
    [CreateAssetMenu(menuName = "BalloonGame/Info/Character",fileName = "CharacterInfo")]
    public class CharacterInfo : ActorInfo
    {
        
        public override ActorModel Create()
        {
            var model = new CharacterModel();
            
            return model;
        }
    }
}
