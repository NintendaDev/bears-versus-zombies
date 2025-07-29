using System;
using Fusion;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Modules.ObjectsDetection
{
    public sealed class AutoObjectsRaycaster : NetworkBehaviour
    {
        [SerializeField, Required] private ObjectsRaycaster _raycaster;
        [SerializeField, MinValue(0)] private float _scanPeriod = 0.05f;
        
        private CollisionData[] _hits;

        [Networked]
        private TickTimer ScanTimer { get; set; }
        
        public event Action<CollisionData[]> Hit;
        
        public event Action EmptyHit;

        public void Initialize()
        {
            _hits = new CollisionData[_raycaster.MaxHits];
        }
        
        public override void FixedUpdateNetwork()
        {
            if (ScanTimer.ExpiredOrNotRunning(Runner) == false)
                return;

            Scan();
            ScanTimer = TickTimer.CreateFromSeconds(Runner, _scanPeriod);
        }
        
        public void AddCondition(Func<bool> condition) => _raycaster.AddCondition(condition);
        
        public bool TryAddNotValidFilter(Func<HitboxRoot, bool> filter)
        {
            if (_raycaster is LagCompensationObjectsRaycaster detector)
            {
                detector.AddNotValidFilter(filter);
                
                return true;
            }
            
            return false;
        }

        private void Scan()
        {
            if (_raycaster.TryDetect(_hits))
                Hit?.Invoke(_hits);
            else
                EmptyHit?.Invoke();
        }
    }
}