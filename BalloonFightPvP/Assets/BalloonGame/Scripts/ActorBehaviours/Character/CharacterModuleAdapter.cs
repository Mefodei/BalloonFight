﻿using System;
using System.Collections.Generic;
using Assets.Modules.UnityToolsModule.Tools.UnityTools.DataFlow;
using Assets.Tools.UnityTools.Common;
using Assets.Tools.UnityTools.Interfaces;
using BalloonGame.Messages.Character;
using UnityEngine;
using UnityTools.UniNodeEditor.Connections;
using UniRx;
using UnityTools.UniVisualNodeSystem;
using XNode;

namespace BalloonGame.Modules.Character
{
    [CreateAssetMenu(menuName = "BalloonGame/Modules/CharacterModule",fileName = "CharacterModule")]
    public class CharacterModuleAdapter : NodeModuleAdapter
    {
        private ContextData<IContext> _data;
        private Dictionary<Type, PortDefinition> _characterMessages;
        
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
                var portDefinition = message.Value;
                var value = portDefinition.Value;
                if (!_data.HasValue(context, type))
                {
                    value.RemoveContext(context);
                }
            }
            _data.RemoveContext(context);
        }

        protected override void OnInitialize()
        {
            _data = new ContextData<IContext>();
            
            _characterMessages = new Dictionary<Type, PortDefinition>()
            {
                {typeof(MoveLeftCharacterMessage),new PortDefinition()
                {
                    Name = "MoveLeft",
                    Direction = PortIO.Output,
                }},
                {typeof(MoveRightCharacterMessage),new PortDefinition()
                {
                    Name = "MoveRight",
                    Direction = PortIO.Output,
                }},
                {typeof(MoveUpCharacterMessage),new PortDefinition()
                {
                    Name = "MoveUp",
                    Direction = PortIO.Output,
                }},
                {typeof(FlyLeftCharacterMessage),new PortDefinition()
                {
                    Name = "FlyLeft",
                    Direction = PortIO.Output,
                }},
                {typeof(FlyRightCharacterMessage),new PortDefinition()
                {
                    Name = "FlyRight",
                    Direction = PortIO.Output,
                }},
            };

        }

        protected override List<PortDefinition> GetPorts()
        {
            var items = base.GetPorts();

            items.AddRange(_characterMessages.Values);
            
            return items;
        }

        private void OnMessage<T>(IContext context, T message)
        {
            var type = typeof(T);
            var portDefinition = _characterMessages[type];
            var value = portDefinition.Value;
            value.UpdateValue(context,message);
        }
    }
}
