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
    [CreateAssetMenu(menuName = "BalloonGame/Modules/CharacterModule",fileName = "CharacterModuleTest")]
    public class CharacterModuleAdapterTest : NodeModuleAdapter
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

        public override void Execute(IContext context, ILifeTime lifeTime)
        {
            foreach (var message in _characterMessages)
            {
                var type = message.Key;
                if (!_data.HasValue(context, type))
                {
                    var port = GetConnection(message.Value);
                    var value = port.Value;
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

        private void OnMessage<T>(IContext context, T message)
        {
            var type = typeof(T);
            var key = _characterMessages[type];
            var port = GetConnection(key);
            port.Value.UpdateValue(context,message);
        }
    }
}
