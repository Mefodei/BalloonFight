using UnityEngine;
using UniRx;
using CharacterInfo = BalloonGame.Scripts.Info.CharacterInfo;

namespace BalloonGame.Scripts.Models
{
    public class CharacterModel : ActorModel
    {
        
        public IntReactiveProperty Id = new IntReactiveProperty();
        public IntReactiveProperty Health = new IntReactiveProperty();

        public CharacterView View;

        public CharacterInfo Info;

    }
}
