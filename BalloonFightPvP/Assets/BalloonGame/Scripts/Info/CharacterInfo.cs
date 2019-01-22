using Assets.Tools.UnityTools.ObjectPool.Scripts;
using BalloonGame.Scripts.Models;
using UnityEngine;
using UnityTools.ActorEntityModel;

namespace BalloonGame.Scripts.Info
{
    [CreateAssetMenu(menuName = "BalloonGame/Info/Character",fileName = "CharacterInfo")]
    public class CharacterInfo : ActorInfo
    {
        private static int _id = 1;
        
        public CharacterView CharacterView;
        
        protected override ActorModel CreateModel()
        {
            var model = ClassPool.Spawn<CharacterModel>();
            
            model.Id = ++_id;
            model.Info = this;
            model.View = ObjectPool.Spawn(CharacterView, Vector3.zero, Quaternion.identity);

            return model;
        }
    }
}
