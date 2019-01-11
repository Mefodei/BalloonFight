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
        
        public override void UpdatePorts()
        {
            
            base.UpdatePorts();

            _messages = new Dictionary<Type, UniPortValue>();
            
            var valuePortTuple = this.UpdatePortValue("MoveLeft", NodePort.IO.Input);
            _messages[typeof(MoveLeftCharacterMessage)] = valuePortTuple.value;
            
            valuePortTuple = this.UpdatePortValue("MoveRight", NodePort.IO.Input);
            _messages[typeof(MoveRightCharacterMessage)] = valuePortTuple.value;
            
            valuePortTuple = this.UpdatePortValue("MoveUp", NodePort.IO.Input);
            _messages[typeof(MoveUpCharacterMessage)] = valuePortTuple.value;
            
        }

        protected override void OnInitialize(IContextData<IContext> localContext)
        {
            _valueActions = new Dictionary<Type, Action<IContext>>()
            {
    
                { typeof(MoveLeftCharacterMessage), context =>
                {
                    context.Publish( new MoveLeftCharacterMessage());
                }},
                { typeof(MoveRightCharacterMessage), context =>
                {
                    context.Publish( new MoveRightCharacterMessage());
                }},
                { typeof(MoveUpCharacterMessage), context =>
                {
                    context.Publish( new MoveUpCharacterMessage());
                }},

            };

        }

        protected override IEnumerator ExecuteState(IContext context)
        {
            yield return base.ExecuteState(context);

            var lifeTime = GetLifeTime(context);
            
            MapAction<MoveLeftCharacterMessage>(context,lifeTime);
            MapAction<MoveRightCharacterMessage>(context,lifeTime);
            MapAction<MoveUpCharacterMessage>(context,lifeTime);
            
        }

        private void MapAction<TValue>(IContext context, ILifeTime lifeTime)
        {
            
            var type = typeof(TValue);
            var value = _messages[type];
            var action = _valueActions[type];
            var disposable = value.Subscribe<TValue>(context, x => action(context));
            lifeTime.AddDispose(disposable);

        }
    }
}