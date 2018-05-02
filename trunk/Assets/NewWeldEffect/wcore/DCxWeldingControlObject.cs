using UnityEngine;
using System.Collections;
//using RichToolKits;

namespace wcore
{
    public class DCxWeldingControlObject
    {
        // 是否碰到焊板
        public virtual bool ishit { get; set; }
        // 碰撞信息
        public virtual RaycastHit hitInfo { get; set; }
        // 是否碰到焊板
        public virtual bool lengthishit { get; set; }
        // 碰撞信息
        public virtual RaycastHit lengthhitInfo { get; set; }
        // 是否正在焊接
        public virtual bool IsWelding { get; set; }
        // 是否可以焊接
        public virtual bool IfCanWelding { get; set; }

        public float maxRayLength = 0.02F;
        public float maxArcLength = 20.0F;
        public int debugLevel = 0;

        public virtual void Awake() { }
        public virtual void Start() { }
        public virtual void Update() { }
    }
}
