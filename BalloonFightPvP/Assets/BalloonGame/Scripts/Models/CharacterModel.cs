using UnityEngine;
using UniRx;

namespace BalloonGame.Scripts.Models
{
    public class CharacterModel : ActorModel
    {
        
        public IntReactiveProperty Id = new IntReactiveProperty();
        public IntReactiveProperty Health = new IntReactiveProperty();
        
    }
}
