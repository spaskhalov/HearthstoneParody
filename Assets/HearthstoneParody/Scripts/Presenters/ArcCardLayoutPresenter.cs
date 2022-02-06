using System.Collections.Generic;
using Cysharp.Threading.Tasks.Triggers;
using UnityEngine;

namespace HearthstoneParody.Presenters
{
    public class ArcCardLayoutPresenter : CardsLayoutPresenter
    {
        [SerializeField, Range(100, 5000)] private float arcRadius = 1000;
        [SerializeField, Range(0,Mathf.PI / 6)] private float radiansBetweenCards = Mathf.PI / 30;
        
        private float ArcRadiusInGlobal => arcRadius * gameObject.transform.lossyScale.x;

        protected override List<(Vector3, Quaternion)> GetPositionsAndRotations(int count)
        {
            var rez = new List<(Vector3, Quaternion)>(count);
            var objectTransform = gameObject.transform;
            var rootObjectRotation = objectTransform.rotation.eulerAngles.z;
            
            var pivot = objectTransform.position + objectTransform.up * -1 * ArcRadiusInGlobal;
            var angle = rootObjectRotation * Mathf.Deg2Rad + Mathf.PI / 2 + radiansBetweenCards * (count - 1) / 2;

            for (int i = 0; i < count; i++)
            {
                Vector3 pos = GetPointOnCircle(angle, pivot);
                pos.z = gameObject.transform.position.z;
                var rot = Quaternion.LookRotation(transform.forward, pos - pivot);
                angle -= radiansBetweenCards;
                rez.Add((pos, rot));
            }

            return rez;
        }
        
        private Vector2 GetPointOnCircle(float angle, Vector3 pivot)
        {
            var x = pivot.x + ArcRadiusInGlobal * Mathf.Cos(angle);
            var y = pivot.y + ArcRadiusInGlobal * Mathf.Sin(angle);
            
            return new Vector2(x, y);
        }
    }
}