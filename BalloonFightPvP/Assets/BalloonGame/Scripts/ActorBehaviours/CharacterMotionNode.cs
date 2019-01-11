using System.Collections;
using Assets.Tools.UnityTools.Interfaces;
using UniStateMachine;
using UnityEngine;

namespace BalloonGame.Character
{
    public class CharacterMotionNode : UniNode
    {

        #region inspector property
        
        public Vector2 AddForce = new Vector2();

        public ForceMode ForceMode = ForceMode.VelocityChange;
        
        #endregion

        protected override IEnumerator ExecuteState(IContext context)
        {
            return base.ExecuteState(context);
        }
    }
}
