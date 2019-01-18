using System;
using System.Collections.Generic;
using Assets.Modules.UnityToolsModule.Tools.UnityTools.DataFlow;
using Assets.Tools.UnityTools.Common;
using Assets.Tools.UnityTools.Interfaces;
using BalloonGame.Messages.Character;
using UnityEngine;
using UnityTools.UniNodeEditor.Connections;
using UniRx;
using UniStateMachine;
using UnityTools.UniVisualNodeSystem;
using XNode;

namespace BalloonGame.Modules.Character
{
    [CreateAssetMenu(menuName = "BalloonGame/Modules/CharacterModule",fileName = "CharacterModule")]
    public class CharacterModuleAdapter : NodeModuleAdapter
    {
                
        [NonSerialized] 
        private Dictionary<Type, Action<IContext>> _inputActions;
        private ContextData<IContext> _data;
        
        private Dictionary<Type, PortDefinition> _outputMessages;
        private Dictionary<Type, PortDefinition> _inputMessages;

        public override void Bind(IContext context, ILifeTime timeline)
        {
            OnBindOutput(context, timeline);
        }

        public override void Execute(IContext context, ILifeTime lifeTime)
        {
            foreach (var message in _inputMessages)
            {
                if (message.Value.Direction == PortIO.Input)
                {
                    UpdateInputValue(context, message.Key, message.Value);
                }
                else
                {
                   UpdateOutputValue(context, message.Key, message.Value); 
                }
                
            }
            _data.RemoveContext(context);
        }

        protected override void OnInitialize()
        {
            _data = new ContextData<IContext>();
            
            _outputMessages = new Dictionary<Type, PortDefinition>() {
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

            _inputMessages = new Dictionary<Type, PortDefinition>();
            foreach (var message in _outputMessages)
            {
                var port = message.Value;
                _inputMessages[message.Key] = new PortDefinition()
                {
                    Name = UniNode.GetFormatedInputName(port.Name),
                    Direction = PortIO.Input,
                };
            }
            
            _inputActions = new Dictionary<Type, Action<IContext>>(){
                {typeof(MoveLeftCharacterMessage), context => { context.Publish(new MoveLeftCharacterMessage()); }},
                {typeof(MoveRightCharacterMessage), context => { context.Publish(new MoveRightCharacterMessage()); }},
                {typeof(MoveUpCharacterMessage), context => { context.Publish(new MoveUpCharacterMessage()); }},
                {typeof(FlyLeftCharacterMessage), context => { context.Publish(new FlyLeftCharacterMessage()); }},
                {typeof(FlyRightCharacterMessage), context => { context.Publish(new FlyRightCharacterMessage()); }},
            };
            
        }

        protected override List<PortDefinition> GetPorts()
        {
            var items = base.GetPorts();

            items.AddRange(_inputMessages.Values);
            items.AddRange(_outputMessages.Values);
            
            return items;
        }

        private void OnMessage<T>(IContext context, T message)
        {
            var type = typeof(T);
            var portDefinition = _inputMessages[type];
            var value = portDefinition.Value;
            value.UpdateValue(context,message);
        }

        private void UpdateInputValue(IContext context, Type type, PortDefinition portDefinition)
        {
            
        }

        private void UpdateOutputValue(IContext context,Type type,PortDefinition portDefinition)
        {
            var value = portDefinition.Value;
            if (!_data.HasValue(context, type))
            {
                value.RemoveContext(context);
            }
        }

        private void OnBindOutput(IContext context, ILifeTime timeline)
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
        
    }
}
