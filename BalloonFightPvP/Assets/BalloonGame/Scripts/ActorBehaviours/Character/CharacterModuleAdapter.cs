using System;
using System.Collections.Generic;
using Assets.Modules.UnityToolsModule.Tools.UnityTools.DataFlow;
using Assets.Tools.UnityTools.Common;
using Assets.Tools.UnityTools.Interfaces;
using BalloonGame.Messages.Character;
using UnityEngine;
using UnityTools.UniNodeEditor.Connections;
using UniRx;

namespace BalloonGame.Modules.Character
{
    [CreateAssetMenu(menuName = "BalloonGame/Modules/CharacterModule",fileName = "CharacterModule")]
    public class CharacterModuleAdapter : NodeModuleAdapter
    {
        private ContextData<IContext> _data;
        private Dictionary<Type, string> _characterMessages;


        public override void Bind(IContext context, ILifeTime timeline)
        {
                                    
            var disposable = context.Receive<MoveLeftCharacterMessage>()
                .Subscribe(x => OnMessage(context, x));
            timeline.AddDispose(disposable);

            disposable = context.Receive<MoveRightCharacterMessage>()
                .Subscribe(x => OnMessage(context, x));
            timeline.AddDispose(disposable);
            
            disposable = context.Receive<MoveUpCharacterMessage>()
                .Subscribe(x => OnMessage(context, x));
            timeline.AddDispose(disposable);
            
            disposable = context.Receive<FlyLeftCharacterMessage>()
                .Subscribe(x => OnMessage(context, x));
            timeline.AddDispose(disposable);
            
            disposable = context.Receive<FlyRightCharacterMessage>()
                .Subscribe(x => OnMessage(context, x));
            timeline.AddDispose(disposable);

            timeline.AddCleanUpAction(() => _data.RemoveContext(context));
        }

        public override void Update(IContext context, ILifeTime lifeTime)
        {
            foreach (var message in _characterMessages)
            {
                var type = message.Key;
                if (!_data.HasValue(context, type))
                {
                    var value = GetConnection(message.Value);
                    value.RemoveContext(context);
                }
            }
            _data.RemoveContext(context);
        }

        protected override void OnInitialize()
        {
            _data = new ContextData<IContext>();
            
            _characterMessages = new Dictionary<Type, string>()
            {
                {typeof(MoveLeftCharacterMessage),"MoveLeft"},
                {typeof(MoveRightCharacterMessage),"MoveRight"},
                {typeof(MoveUpCharacterMessage),"MoveUp"},
                {typeof(FlyLeftCharacterMessage),"FlyLeft"},
                {typeof(FlyRightCharacterMessage),"FlyRight"},
            };

        }
        
        protected override IReadOnlyCollection<string> GetPorts()
        {
            return _characterMessages.Values;
        }

        private void OnMessage<T>(IContext context, T message)
        {
            var type = typeof(T);
            var key = _characterMessages[type];
            var value = GetConnection(key);
            value.UpdateValue(context,message);
        }
    }
}
