using UnityEngine;
using System.Collections;
using RichToolKits;

namespace wcore
{
    public class DCxCO2WeldingControl : DCxWeldingControlObject
    {
        // 干伸长度可视物体
        public GameObject gunNozzleCube;

        // 是否自动
        public bool isAuto = false;
        private int pressCount = 0;

        // 扣扳机
        public static bool isPress = false;

        // 碰撞信息
        private RaycastHit _hitInfo;
        public override RaycastHit hitInfo { get { return _hitInfo; } set { } }
        private RaycastHit _lengthhitInfo;
        public override RaycastHit lengthhitInfo { get { return _lengthhitInfo; } set { } }


        // 是否正在焊接
        public override bool IsWelding
        {
            get
            {
                return ishit && isPress && IfCanWelding;
                //&& RSxReplaySystem.Instance.Status != RSxReplaySystem.EReplaySystemStatus.PauseReplaying;
            }
            set
            {
                isPress = value;
                pressCount = 0;
            }
        }

        public override void Awake()
        {
            //GameObject.Find("Target_2").AddComponent<DCxWeldingGunControl>();           
            IfCanWelding = true;
            maxRayLength = 5.03f;
        }

        public override void Start()
        {
            // 得到调试等级
            debugLevel = 2;//配置文件
            // 得到调试等级
            isAuto = true;
            // 如果干伸长度关联物体没指定，自动搜索
            if (gunNozzleCube == null) gunNozzleCube = DCxWeldingControl.Instance.gameObject.transform.GetChild(0).gameObject;
        }

        public override void Update()
        {
            // 碰撞检测
            ishit = Physics.Raycast(DCxWeldingGunControl.Instance.GunNozzlePosition,
                DCxWeldingGunControl.Instance.GunNozzleDirection,
                out _hitInfo, maxRayLength, 1 << 12);

            // 碰撞检测 
            lengthishit = Physics.Raycast(DCxWeldingGunControl.Instance.GunNozzlePosition,
                DCxWeldingGunControl.Instance.GunNozzleDirection,
                out _lengthhitInfo, maxRayLength * 3, 1 << 12);

            if (isAuto)
            {
                if (ishit)
                {
                    //if (Input.GetKeyDown(KeyCode.Space))
                    //{
                    //    isPress = true;
                    //}
                    //else if (Input.GetKeyUp(KeyCode.Space))
                    //{
                    //        isPress = false;

                    //}
                }
                else
                {
                    isPress = false;
                }
            }
            else
            {
                //// 按键检测
                //isPress = Input.GetKey(KeyCode.Space);
            }

            if (IsWelding)
            {
                MessagerCenter<bool, Vector2>.Broadcast("UpdateHitInfo", true, hitInfo.textureCoord);
                //EFxEffectManager.Instance.StartAllEffect();
                //RSxReplaySystem.Instance.AddRecord("z", "z");
            }
            else
            {
                MessagerCenter<bool, Vector2>.Broadcast("UpdateHitInfo", false, Vector2.zero);
                //EFxEffectManager.Instance.StopAllEffect();
            }

            // 焊枪嘴颜色改变
            if (ishit)
            {
                float pv = Vector3.Distance(DCxWeldingGunControl.Instance.GunNozzlePosition, DCxWeldingControl.Instance.hitInfo.point);
                gunNozzleCube.GetComponent<Renderer>().material.color = Color.green;
                //gunNozzleCube.transform.localPosition = new Vector3(0, 0, pv - 0.04f);
            }
            else
            {
                gunNozzleCube.GetComponent<Renderer>().material.color = Color.red;
                //gunNozzleCube.transform.localPosition = new Vector3(0, 0, maxRayLength - 0.04f);
            }
        }
    }
}