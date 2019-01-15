using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Tools.UnityTools.Interfaces;
using BalloonGame.Messages.Character;
using UniStateMachine;
using UniStateMachine.Nodes;
using UnityEngine;
using UniRx;

namespace BalloonGame.Character
{
    public class CharacterMessagesNode : UniNode
    {
        #region inspector

        private Dictionary<Type, UniPortValue> _outputCharacterPorts = 
            new Dictionary<Type, UniPortValue>();

        [SerializeField]
        private float _throttleMs = 32;
        
        #endregion
        
        public override void UpdatePortsCache()
        {
            
            base.UpdatePortsCache();
            
            if(_outputCharacterPorts == null)
                _outputCharacterPorts = new Dictionary<Type, UniPortValue>();

            _outputCharacterPorts.Clear();
            
            var pair = this.UpdatePortValue("MoveLeft");
            _outputCharacterPorts[typeof(MoveLeftCharacterMessage)] = pair.value;
            
            pair = this.UpdatePortValue("MoveRight");
            _outputCharacterPorts[typeof(MoveRightCharacterMessage)] = pair.value;
            
            pair = this.UpdatePortValue("MoveUp");
            _outputCharacterPorts[typeof(MoveUpCharacterMessage)] = pair.value;      
            
        }

        protected override IEnumerator ExecuteState(IContext context)
        {
            yield return base.ExecuteState(context);

            UpdateContextSubscriptions(context);
            
            while (IsActive(context))
            {
                
                foreach (var outputCharacterPort in _outputCharacterPorts)
                {
                    var value = outputCharacterPort.Value;
                    value.Release();
                }

                yield return null;
            }
            
        }

        private void UpdateContextSubscriptions(IContext context)
        {
            var timeline = GetLifeTime(context);
            
            var disposable = context.Receive<MoveLeftCharacterMessage>()
                .Subscribe(x => OnMessage(context, x));
            timeline.AddDispose(disposable);

            disposable = context.Receive<MoveLeftCharacterMessage>()
                .Subscribe(x => OnMessage(context, x));
            timeline.AddDispose(disposable);
            
            disposable = context.Receive<MoveUpCharacterMessage>()
                .Subscribe(x => OnMessage(context, x));
            timeline.AddDispose(disposable);
            
        }
        
        private void OnMessage<T>(IContext context, T message)
        {
            var type = typeof(T);
            var value = _outputCharacterPorts[type];
            value.UpdateValue(context,message);
        }
        
    }
}
