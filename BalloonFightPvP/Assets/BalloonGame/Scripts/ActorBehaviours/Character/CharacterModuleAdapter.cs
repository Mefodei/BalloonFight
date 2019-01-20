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
        private Dictionary<string, Type> _portToTypes;
        private Dictionary<Type, Action<IContext>> _inputPortsActions;
        private Dictionary<Type, PortDefinition> _outputMessages;
        private Dictionary<Type, PortDefinition> _inputMessages;

        protected override void OnInitialize()
        {
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
            
            _inputPortsActions = new Dictionary<Type, Action<IContext>>(){
                {typeof(MoveLeftCharacterMessage), context => { context.Publish(new MoveLeftCharacterMessage()); }},
                {typeof(MoveRightCharacterMessage), context => { context.Publish(new MoveRightCharacterMessage()); }},
                {typeof(MoveUpCharacterMessage), context => { context.Publish(new MoveUpCharacterMessage()); }},
                {typeof(FlyLeftCharacterMessage), context => { context.Publish(new FlyLeftCharacterMessage()); }},
                {typeof(FlyRightCharacterMessage), context => { context.Publish(new FlyRightCharacterMessage()); }},
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

                RegisterRuntimeAction(intputName,OnInputPortExecution);
            }

            OnBindOutput<MoveLeftCharacterMessage>();
            OnBindOutput<MoveRightCharacterMessage>();
            OnBindOutput<MoveUpCharacterMessage>();
            OnBindOutput<FlyLeftCharacterMessage>();
            OnBindOutput<FlyRightCharacterMessage>();

            UpdateOutputValue<MoveLeftCharacterMessage>();
            UpdateOutputValue<MoveRightCharacterMessage>();
            UpdateOutputValue<MoveUpCharacterMessage>();
            UpdateOutputValue<FlyLeftCharacterMessage>();
            UpdateOutputValue<FlyRightCharacterMessage>();
        }

        private void OnInputPortExecution(string portName,IContextData<IContext> port, IContext context)
        {
            
            if (!port.HasContext(context))
                return;

            var type = _portToTypes[portName];
            var action = _inputPortsActions[type];
            action(context);

        }

        protected override List<PortDefinition> GetPorts()
        {
            var items = base.GetPorts();

            items.AddRange(_inputMessages.Values);
            items.AddRange(_outputMessages.Values);
            
            return items;
        }

        private void UpdateOutputValue<TData>()
        {
            var type = typeof(TData);
            var port = _outputMessages[type];
            
            RegisterRuntimeAction(port.Name, (portName, portValue, context) =>
            {
                if (!portValue.HasValue<TData>(context)) 
                    return;
                portValue.RemoveContext(context);
            });

        }
        
        private void OnBindOutput<TData>()
        {
            var type = typeof(TData);
            var port = _outputMessages[type];
            
            RegisterBindAction(port.Name,
                (contextData,bindContext) => bindContext.
                        Receive<TData>().
                        Subscribe(x => contextData.UpdateValue(bindContext,x)));
            
        }
        
    }
}
