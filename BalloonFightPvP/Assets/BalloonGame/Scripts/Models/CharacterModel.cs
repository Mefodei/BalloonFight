using Assets.Tools.UnityTools.Interfaces;
using Assets.Tools.UnityTools.ObjectPool.Scripts;
using UniRx;
using CharacterInfo = BalloonGame.Scripts.Info.CharacterInfo;

namespace BalloonGame.Scripts.Models
{
    public class CharacterModel : ActorModel
    {
                
        public IntReactiveProperty Health = new IntReactiveProperty();

        public int Id ;
        public CharacterView View;
        public CharacterInfo Info;


        public override void RegisterContext(IContext context)
        {
            context.Add(View);
            context.Add(View.Rigidbody);
            
            context.Add(Info);
            context.Add(this);

            context.LifeTime.AddCleanUpAction(() => this.Despawn());
            
            base.RegisterContext(context);
        }

        public override void Release()
        {
            Id = 0;
            View.Despawn();
            Info = null;
            Health.Value = 0;
            
            base.Release();
        }
    }
}
