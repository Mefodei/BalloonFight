using System.Collections;
using Assets.Tools.UnityTools.Extension;
using Assets.Tools.UnityTools.Interfaces;
using UniStateMachine;
using UnityEngine;
using UnityTools.ActorEntityModel;

namespace BalloonGame.Nodes
{
    public class ActorSpawnNode : UniNode
    {

        public ActorInfo Info;

        public int Count = 1;

        public Vector3 Position;

        public float Delay;
        
        protected override IEnumerator ExecuteState(IContext context)
        {
            for (int i = 0; i < Count; i++)
            {
                var actor = new Actor();
                var model = Info.Create();
                actor.SetModel(model);
                actor.SetEnabled(true);
                
                context.LifeTime.AddCleanUpAction(actor.Release);
                
                yield return this.WaitForSecond(Delay);
                
            }    
            yield return base.ExecuteState(context);
        }
        
        public override bool Validate(IContext context)
        {
            return Info && base.Validate(context);
        }
    }
}
