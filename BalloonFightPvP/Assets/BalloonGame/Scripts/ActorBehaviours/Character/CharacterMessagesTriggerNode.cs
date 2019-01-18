using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Modules.UnityToolsModule.Tools.UnityTools.DataFlow;
using Assets.Tools.UnityTools.Interfaces;
using BalloonGame.Messages.Character;
using UniStateMachine;
using UniStateMachine.Nodes;
using XNode;

namespace BalloonGame.Character
{
    public class CharacterMessagesTriggerNode : UniNode
    {
        [NonSerialized] private Dictionary<Type, Action<IContext>> _valueActions;
        [NonSerialized] private Dictionary<Type, UniPortValue> _messages;

        public override void UpdatePortsCache()
        {    
            base.UpdatePortsCache();

            _messages = new Dictionary<Type, UniPortValue>();

            var valuePortTuple = this.UpdatePortValue("MoveLeft", PortIO.Input);
            _messages[typeof(MoveLeftCharacterMessage)] = valuePortTuple.value;

            valuePortTuple = this.UpdatePortValue("MoveRight", PortIO.Input);
            _messages[typeof(MoveRightCharacterMessage)] = valuePortTuple.value;

            valuePortTuple = this.UpdatePortValue("MoveUp", PortIO.Input);
            _messages[typeof(MoveUpCharacterMessage)] = valuePortTuple.value;
        }

        protected override void OnInitialize(IContextData<IContext> localContext)
        {
            _valueActions = new Dictionary<Type, Action<IContext>>()
            {
                {typeof(MoveLeftCharacterMessage), context => { context.Publish(new MoveLeftCharacterMessage()); }},
                {typeof(MoveRightCharacterMessage), context => { context.Publish(new MoveRightCharacterMessage()); }},
                {typeof(MoveUpCharacterMessage), context => { context.Publish(new MoveUpCharacterMessage()); }},
            };
        }

        protected override IEnumerator ExecuteState(IContext context)
        {
            yield return base.ExecuteState(context);
            
            while (IsActive(context))
            {
                UpdatePortsValues(context);
                yield return null;
            }
            
        }

        private void OnCharacterAction(Type type, IContext context)
        {

            var portValue = _messages[type];
                    
            if (!portValue.HasContext(context))
            {
                return;
            }

            var action = _valueActions[type];
            action.Invoke(context);
            
        }
        
        
        private void UpdatePortsValues(IContext context)
        {
            foreach (var value in _messages)
            {
                OnCharacterAction(value.Key,context);
            }
        }
    }
}