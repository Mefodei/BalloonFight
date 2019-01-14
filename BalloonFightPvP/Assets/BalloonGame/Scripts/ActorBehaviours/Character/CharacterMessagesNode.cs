using System.Collections;
using System.Collections.Generic;
using BalloonGame.Messages.Character;
using UniStateMachine;
using UnityEngine;

namespace BalloonGame.Character
{
    public class CharacterMessagesNode : UniNode
    {
        
        
        public override void UpdatePortsCache()
        {
            base.UpdatePortsCache();

            this.UpdatePortValue("MoveLeft");
            this.UpdatePortValue("MoveRight");
            this.UpdatePortValue("MoveUp");
            
        }
    }
}
