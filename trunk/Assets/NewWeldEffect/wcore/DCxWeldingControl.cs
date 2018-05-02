using UnityEngine;
using System.Collections;
//using RichToolKits;

namespace wcore
{
    public class DCxWeldingControl : MonoBehaviour
    {
        // 全局直接访问
        public static DCxWeldingControl Instance { get; private set; }

        DCxWeldingControlObject _weldingControlObject;

        // 是否碰到焊板
        public bool ishit
        {
            get { return _weldingControlObject.ishit; }
            set { _weldingControlObject.ishit = value; }
        }
        // 碰撞信息
        public RaycastHit hitInfo
        {
            get { return _weldingControlObject.hitInfo; }
            set { _weldingControlObject.hitInfo = value; }
        }
        // 是否碰到焊板
        public bool lengthishit
        {
            get { return _weldingControlObject.lengthishit; }
            set { _weldingControlObject.lengthishit = value; }
        }
        // 碰撞信息
        public RaycastHit lengthhitInfo
        {
            get { return _weldingControlObject.lengthhitInfo; }
            set { _weldingControlObject.lengthhitInfo = value; }
        }
        // 是否正在焊接
        public bool IsWelding
        {
            get { return _weldingControlObject.IsWelding; }
            set { _weldingControlObject.IsWelding = value; }
        }
        // 是否可以焊
        public bool IfCanWelding
        {
            get { return _weldingControlObject.IfCanWelding; }
            set { _weldingControlObject.IfCanWelding = value; }
        }
        public float maxRayLength
        {
            get { return _weldingControlObject.maxRayLength; }
            set { _weldingControlObject.maxRayLength = value; }
        }
        public float maxArcLength
        {
            get { return _weldingControlObject.maxArcLength; }
            set { _weldingControlObject.maxArcLength = value; }
        }
        public int debugLevel
        {
            get { return _weldingControlObject.debugLevel; }
            set { _weldingControlObject.debugLevel = value; }
        }

        void Awake()
        {
            Instance = this;
            
            _weldingControlObject = new DCxCO2WeldingControl();
            _weldingControlObject.Awake();       
            
        }
        void Start()
        {
            _weldingControlObject.Start();
        }

        void Update()
        {
            _weldingControlObject.Update();
        }
    }
}