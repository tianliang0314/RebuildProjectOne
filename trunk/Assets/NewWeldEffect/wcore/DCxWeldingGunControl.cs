using UnityEngine;
using System;
using System.Collections;
//using RichToolKits;

namespace wcore
{
    public class DCxWeldingGunControl : MonoBehaviour//, IFxDataListener
    {
        public static DCxWeldingGunControl Instance { get; private set; }

        private string _ID = "0";

        private Vector3 _position;
        private Quaternion _orientation;

        private Transform _gunNozzle;

        // 枪口位置
        public Vector3 GunNozzlePosition { get; private set; }
        // 枪口朝向
        public Vector3 GunNozzleDirection { get; private set; }

        public float _smoothVelocityTime = 0.2F;

        void Awake()
        {
            Instance = this;
            gameObject.tag = "DCxWeldingGunControl";
        }

        void Start()
        {
            if (_gunNozzle == null) _gunNozzle = gameObject.transform;

            _ID = "0";
        }

        void Update()
        {

            GunNozzlePosition = _gunNozzle.position;
            GunNozzleDirection = _gunNozzle.forward;
        }

        public void OnDeviceConnected(string strGUID, Color color)
        {

        }
        public void OnLocationChanged(string strGUID, Vector3 position, Quaternion orient)
        {
            if (_ID == strGUID)
            {
                _position = position;
                _orientation = orient;
            }
        }
        public void OnDeviceDisconnect(string strGUID)
        {

        }
        void OnDestroy()
        {
            try
            {
                //DCxDataDeviceManager.Instance.UnSubscribeDataListener(this);
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
            }
        }
    }

}