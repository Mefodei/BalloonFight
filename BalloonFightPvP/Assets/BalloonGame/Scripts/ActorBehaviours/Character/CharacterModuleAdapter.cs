using System.Collections.Generic;
using UnityEngine;
using UnityTools.UniNodeEditor.Connections;

namespace BalloonGame.Modules.Character
{
    [CreateAssetMenu(menuName = "BalloonGame/Modules/CharacterModule",fileName = "CharacterModule")]
    public class CharacterModuleAdapter : NodeModuleAdapter
    {
        
        private List<string> _characterMessages = new List<string>()
        {
            "MoveLeft",
            "MoveLeft",
            "MoveLeft",
            "MoveLeft",
        };
        
        
        
    }
}
