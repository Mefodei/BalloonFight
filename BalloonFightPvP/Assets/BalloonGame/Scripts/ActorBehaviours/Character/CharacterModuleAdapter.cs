using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Assets.Modules.UnityToolsModule.Tools.UnityTools.DataFlow;
using Assets.Tools.UnityTools.Common;
using Assets.Tools.UnityTools.Interfaces;
using Assets.Tools.UnityTools.ObjectPool.Scripts;
using BalloonGame.Messages.Character;
using UnityEngine;
using UnityTools.UniNodeEditor.Connections;
using UniRx;
using UniStateMachine;
using UniStateMachine.Nodes;
using UnityEditor.Build.Pipeline;
using UnityTools.UniVisualNodeSystem;
using XNode;

namespace BalloonGame.Modules.Character
{
    [CreateAssetMenu(menuName = "BalloonGame/Modules/CharacterModule",fileName = "CharacterModule")]
    public class CharacterModuleAdapter : NodeModuleAdapter
    {
        private ContextData<IContext> _moduleData;
        private Dictionary<string, Type> _portToTypes;
        private Dictionary<PortDefinition, PortDefinition> _portPairs;
        private Dictionary<Type, PortDefinition> _outputMessages;
        private Dictionary<Type, PortDefinition> _inputMessages;

        protected override void OnInitialize()
        {
            _portPairs = new Dictionary<PortDefinition, PortDefinition>();
            _moduleData = new ContextData<IContext>();
            _portToTypes = new Dictionary<string, Type>();
            _inputMessages = new Dictionary<Type, PortDefinition>();
            
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
            
            foreach (var message in _outputMessages)
            {
                
                var port = message.Value;
                var intputName = UniNode.GetFormatedInputName(port.Name);
                
                var definition = ClassPool.Spawn<PortDefinition>();
                definition.Direction = PortIO.Input;
                definition.Name = intputName;
                
                _inputMessages[message.Key] = definition;
                _portToTypes[port.Name] = message.Key;
                _portToTypes[definition.Name] = message.Key;
                _portPairs[definition] = port;
            }

            BindInputUpdate<MoveLeftCharacterMessage>();
            BindInputUpdate<MoveRightCharacterMessage>();
            BindInputUpdate<MoveUpCharacterMessage>();
            BindInputUpdate<FlyLeftCharacterMessage>();
            BindInputUpdate<FlyRightCharacterMessage>();
            
            OnBindOutput<MoveLeftCharacterMessage>();
            OnBindOutput<MoveRightCharacterMessage>();
            OnBindOutput<MoveUpCharacterMessage>();
            OnBindOutput<FlyLeftCharacterMessage>();
            OnBindOutput<FlyRightCharacterMessage>();

            BindOutputUpdate<MoveLeftCharacterMessage>();
            BindOutputUpdate<MoveRightCharacterMessage>();
            BindOutputUpdate<MoveUpCharacterMessage>();
            BindOutputUpdate<FlyLeftCharacterMessage>();
            BindOutputUpdate<FlyRightCharacterMessage>();
        }

        protected override List<PortDefinition> GetPorts()
        {
            var items = base.GetPorts();

            items.AddRange(_inputMessages.Values);
            items.AddRange(_outputMessages.Values);
            
            return items;
        }

        private void BindInputUpdate<TData>() where TData : new()
        {
            var type = typeof(TData);
            var port = _inputMessages[type];
            
            RegisterRuntimeAction(port.Name, (portName, portValue, context) =>
            {
                if (!portValue.HasContext(context))
                    return;
                var data = new TData();
                context.Publish(data);
            });
        }

        private void BindOutputUpdate<TData>()
        {
            var type = typeof(TData);
            var port = _outputMessages[type];
            
            RegisterRuntimeAction(port.Name, (portName, portValue, context) =>
            {
                
                var valueUpdated = _moduleData.HasValue<TData>(context);
                if (valueUpdated)
                {
                    var value = _moduleData.Get<TData>(context);
                    portValue.UpdateValue(context,value);
                }
                else
                {
                    portValue.RemoveContext(context);
                }
                
                _moduleData.Remove<TData>(context);
                
            });

        }
        
        private void OnBindOutput<TData>()
        {
            var type = typeof(TData);
            var port = _outputMessages[type];
            
            RegisterBindAction(port.Name,
                (contextData,bindContext) => 
                    bindContext.Receive<TData>().
                        Subscribe(x => 
                            _moduleData.UpdateValue(bindContext,x)));
            
        }
        
    }
}
