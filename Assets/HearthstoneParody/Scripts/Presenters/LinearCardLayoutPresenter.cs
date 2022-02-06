using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace HearthstoneParody.Presenters
{
    public class LinearCardLayoutPresenter : CardsLayoutPresenter
    {
        [SerializeField, Range(10, 500)] private float spaceBetweenCards = 300f;
        protected override List<(Vector3, Quaternion)> GetPositionsAndRotations(int count)
        {
            var rez = new List<(Vector3, Quaternion)>();
            var trans = transform;
            var spaceInGlobalCoordinates = spaceBetweenCards * trans.lossyScale.x;
            var totalLength = spaceInGlobalCoordinates * (count - 1);
            var deltaVector = spaceInGlobalCoordinates * trans.right;
            var curPos = trans.position - (totalLength / 2) * deltaVector.normalized;
            for (int i = 0; i < count; i++)
            {
                var pos = curPos;
                rez.Add((pos, trans.rotation));
                curPos += deltaVector;
            }

            return rez;
        }
    }
}