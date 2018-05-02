using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using RichToolKits;
using System.Xml;
using System.IO;
using System;
using RichToolKits;

namespace wcore
{
    public class WBxWeldingBedManagerCO2 : WBxWeldingBedManager
    {
        public bool sss = true;

        public ComputeShader _computeShader;

        private const int _kernelWidth = 32;
        private const int _kernelHeight = 32;

        public float _posPoolX = 128;
        public float _posPoolY = 128;
        public float _posPoolR = 128;
        public float _posPoolH = 0.1f;

        public float _fluctuateRange = 30;

        public float _warmingSpeed = 2.0f;
        public float _coolingSpeed = 0.2f;

        public float _maxHeight = 0.01f;

        public float _smoothRatio = 10;

        private RenderTexture _dataIn;
        private RenderTexture _dataOut;
        private RenderTexture _normalIn;
        private RenderTexture _normalOut;

        //	public float _gapTime = 1.0f/30.0f;
        //	private float _lestTime = 0;

        private int _kernelCSMain;
        private int _kernelCSReset;
        private int _kernelCSDefect;
        private int _kernelCSTest;

        public Texture2D _mainTex;
        public Texture2D _edgeTex;
        public Texture2D _temperatureColorMap;
        public Texture2D _warmingMap;
        public Texture2D _coolingMap;
        public Texture2D _maxTemperatureMap;

        public AnimationCurve _brushStrengthCurve;
        public AnimationCurve _brushVCurve;
        public AnimationCurve _brushACurve;

        private struct DebugStruct
        {
            public int i;
            public float f;
            public Vector3 f3;
            public Vector4 f4;
        }

        public  void Awake()
        {
            MessagerCenter.Register("RestoreWeldingBed", Reset);
            MessagerCenter<bool, Vector2>.Register("UpdateHitInfo", UpdateHitInfo);
            LoadConfig();
        }
        public  void Start()
        {
            Init();
        }
        public  void OnDestroy()
        {
            MessagerCenter.UnRegister("RestoreWeldingBed", Reset);
            MessagerCenter<bool, Vector2>.UnRegister("UpdateHitInfo", UpdateHitInfo);
            Release();
        }

        private void LoadConfig()
        {
            try
            {
                // 配置文件打包到场景包里面
                //TextAsset ta = scene.assetBundle.Load(scene.sceneData.weldlineConfig, typeof(TextAsset)) as TextAsset;
                // 配置文件放在资源文件夹下面
                //TextAsset ta = Resources.Load("SceneConfig/" + scene.sceneData.config, typeof(TextAsset)) as TextAsset;

                XmlDocument config = new XmlDocument();
                //config.LoadXml(ta.text);

                config.LoadXml(File.ReadAllText(Application.dataPath + "/Configs/WeldlineConfig.xml"));

                foreach (XmlNode xnc in config.DocumentElement.ChildNodes)
                {
                    switch (xnc.Attributes["name"].Value)
                    {
                        case "ComputeShader":
                            _computeShader = Resources.Load(xnc.Attributes["value"].Value, typeof(ComputeShader)) as ComputeShader;
                            break;
                        case "MainTex":
                            _mainTex = Resources.Load(xnc.Attributes["value"].Value, typeof(Texture2D)) as Texture2D;
                            break;
                        case "EdgeTex":
                            _edgeTex = Resources.Load(xnc.Attributes["value"].Value, typeof(Texture2D)) as Texture2D;
                            break;
                        case "TemperatureColorMap":
                            _temperatureColorMap = Resources.Load(xnc.Attributes["value"].Value, typeof(Texture2D)) as Texture2D;
                            break;
                        case "WarmingMap":
                            _warmingMap = Resources.Load(xnc.Attributes["value"].Value, typeof(Texture2D)) as Texture2D;
                            break;
                        case "CoolingMap":
                            _coolingMap = Resources.Load(xnc.Attributes["value"].Value, typeof(Texture2D)) as Texture2D;
                            break;
                        case "MaxTemperatureMap":
                            _maxTemperatureMap = Resources.Load(xnc.Attributes["value"].Value, typeof(Texture2D)) as Texture2D;
                            break;
                        case "TextureWidth":
                            _textureWidth = int.Parse(xnc.Attributes["value"].Value);
                            break;
                        case "TextureHeight":
                            _textureHeight = int.Parse(xnc.Attributes["value"].Value);
                            break;
                        case "PlaneWidth":
                            _planeWidth = float.Parse(xnc.Attributes["value"].Value);
                            break;
                        case "PlaneHeight":
                            _planeHeight = float.Parse(xnc.Attributes["value"].Value);
                            break;
                        case "Size":
                            _size = float.Parse(xnc.Attributes["value"].Value);
                            break;
                        case "Strength":
                            _strength = float.Parse(xnc.Attributes["value"].Value);
                            break;
                        case "FluctuateRange":
                            _fluctuateRange = float.Parse(xnc.Attributes["value"].Value);
                            break;
                        case "WarmingSpeed":
                            _warmingSpeed = float.Parse(xnc.Attributes["value"].Value);
                            break;
                        case "CoolingSpeed":
                            _coolingSpeed = float.Parse(xnc.Attributes["value"].Value);
                            break;
                        case "MaxHeight":
                            _maxHeight = float.Parse(xnc.Attributes["value"].Value);
                            break;
                        case "SmoothRatio":
                            _smoothRatio = float.Parse(xnc.Attributes["value"].Value);
                            break;
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex.ToString());
            }
        }

        private void Init()
        {
            InitKernel();
            InitTexture();
            InitMap();
        }

        private void InitKernel()
        {
            _kernelCSMain = _computeShader.FindKernel("CSMain");
            _kernelCSReset = _computeShader.FindKernel("CSReset");
            _kernelCSDefect = _computeShader.FindKernel("CSDefect");
            _kernelCSTest = _computeShader.FindKernel("CSTest");
        }

        private void InitTexture()
        {
            _dataIn = new RenderTexture(_textureWidth, _textureHeight, 24, RenderTextureFormat.ARGBFloat);
            _dataIn.enableRandomWrite = true;
            _dataIn.Create();
            _dataOut = new RenderTexture(_textureWidth, _textureHeight, 24, RenderTextureFormat.ARGBFloat);
            _dataOut.enableRandomWrite = true;
            _dataOut.Create();
            _normalIn = new RenderTexture(_textureWidth, _textureHeight, 24, RenderTextureFormat.ARGBFloat);
            _normalIn.enableRandomWrite = true;
            _normalIn.Create();
            _normalOut = new RenderTexture(_textureWidth, _textureHeight, 24, RenderTextureFormat.ARGBFloat);
            _normalOut.enableRandomWrite = true;
            _normalOut.Create();

            _computeShader.SetTexture(_kernelCSMain, "data_in", _dataIn);
            _computeShader.SetTexture(_kernelCSMain, "data_out", _dataOut);
            _computeShader.SetTexture(_kernelCSMain, "normal_in", _normalIn);
            _computeShader.SetTexture(_kernelCSMain, "normal_out", _normalOut);

            _computeShader.SetTexture(_kernelCSReset, "data_out", _dataOut);
            _computeShader.SetTexture(_kernelCSReset, "normal_out", _normalOut);

            GetComponent<Renderer>().material.SetTexture("_MainTex", _mainTex);
            GetComponent<Renderer>().material.SetTexture("_EdgeTex", _edgeTex);
            GetComponent<Renderer>().material.SetTexture("_DataMap", _dataIn);
            GetComponent<Renderer>().material.SetTexture("_NormalMap", _normalIn);
            GetComponent<Renderer>().material.SetTexture("_TemperatureColorMap", _temperatureColorMap);
        }

        private void InitMap()
        {
            _warmingMap = new Texture2D(128, 128);
            _coolingMap = new Texture2D(128, 128);
            _maxTemperatureMap = new Texture2D(128, 128);

            _computeShader.SetTexture(_kernelCSMain, "warming_map", _warmingMap);
            _computeShader.SetTexture(_kernelCSMain, "cooling_map", _coolingMap);
            _computeShader.SetTexture(_kernelCSMain, "max_tem_map", _maxTemperatureMap);

            _brushVCurve = new AnimationCurve();
            _brushVCurve.AddKey(0, 0.7f);
            _brushVCurve.AddKey(15, 0.8f);
            _brushVCurve.AddKey(25, 1.0f);
            _brushVCurve.AddKey(35, 1.2f);
            _brushVCurve.AddKey(100, 1.3f);
            _brushACurve = new AnimationCurve();
            _brushACurve.AddKey(0, 0.7f);
            _brushACurve.AddKey(150, 0.8f);
            _brushACurve.AddKey(250, 1.0f);
            _brushACurve.AddKey(350, 1.2f);
            _brushACurve.AddKey(400, 1.3f);
        }

        private void Release()
        {
            if(_dataIn != null)
            {
                _dataIn.Release();
                _dataIn = null;
            }
            if(_dataOut != null)
            {
                _dataOut.Release();
                _dataOut = null;
            }
            if(_normalIn != null)
            {
                _normalIn.Release();
                _normalIn = null;
            }
            if(_normalOut != null)
            {
                _normalOut.Release();
                _normalOut = null;
            }
        }

        private int _poolStatus = 0;
        private float _poolMaxSpeed = 40f;
        private float _poolMinSpeed = 20f;
        private float _poolMaxDistance = 80;
        private float _poolMinDistance = 30;
        private void PoolUpdate()
        {
            switch (_poolStatus)
            {
                case 0:
                    _posPoolX += (_posX > _posPoolX ? _poolMinSpeed * Time.deltaTime : -_poolMinSpeed * Time.deltaTime);// + (_posX - _posPoolX)/(_poolMaxDistance - _poolMinDistance)*_poolMinSpeed;
                    _posPoolY += (_posY > _posPoolY ? _poolMinSpeed * Time.deltaTime : -_poolMinSpeed * Time.deltaTime);// + (_posY - _posPoolY)/(_poolMaxDistance - _poolMinDistance)*_poolMinSpeed;
                    _posPoolR = _size * _brushVCurve.Evaluate(20.0f);
                    _posPoolH = _strength * _brushACurve.Evaluate(220.0f);
                    if (Vector2.Distance(new Vector2(_posPoolX, _posPoolY), new Vector2(_posX, _posY)) > _poolMaxDistance)
                    {
                        _poolStatus = 1;
                    }
                    break;
                case 1:
                    _posPoolX += (_posX > _posPoolX ? _poolMaxSpeed * Time.deltaTime : -_poolMaxSpeed * Time.deltaTime);// + (_posX - _posPoolX)/(_poolMaxDistance - _poolMinDistance)*_poolMinSpeed;
                    _posPoolY += (_posY > _posPoolY ? _poolMaxSpeed * Time.deltaTime : -_poolMaxSpeed * Time.deltaTime);// + (_posY - _posPoolY)/(_poolMaxDistance - _poolMinDistance)*_poolMinSpeed;
                    _posPoolR = _size * _brushVCurve.Evaluate(20.0f);
                    _posPoolH = _strength * _brushACurve.Evaluate(220.0f);
                    if (Vector2.Distance(new Vector2(_posPoolX, _posPoolY), new Vector2(_posX, _posY)) < _poolMinDistance)
                    {
                        _poolStatus = 0;
                    }
                    if (Vector2.Distance(new Vector2(_posPoolX, _posPoolY), new Vector2(_posX, _posY)) > _poolMaxDistance * 2)
                    {
                        _posPoolX = _posX;
                        _posPoolY = _posY;
                        _poolStatus = 0;
                    }
                    break;
            }
        }

        private void DataUpdate()
        {
            PoolUpdate();
            //MoltenDropUpdate ();

            _computeShader.SetInt("activation", _isOpen ? 1 : 0);

            _computeShader.SetFloat("pos_x", _posX);
            _computeShader.SetFloat("pos_y", _posY);
            _computeShader.SetFloat("pos_r", UnityEngine.Random.Range(_size - _fluctuateRange, _size + _fluctuateRange));

            _computeShader.SetFloat("pos_pool_x", _posPoolX);
            _computeShader.SetFloat("pos_pool_y", _posPoolY);
            _computeShader.SetFloat("pos_pool_r", _posPoolR);

            _computeShader.SetFloat("molten_drop_h", _posPoolH * Time.deltaTime);

            _computeShader.SetInt("texture_width", _textureWidth);
            _computeShader.SetInt("texture_height", _textureHeight);

            _computeShader.SetFloat("warming_speed", _warmingSpeed * Time.deltaTime);
            _computeShader.SetFloat("cooling_speed", _coolingSpeed * Time.deltaTime);

            _computeShader.SetFloat("smooth_ratio", _smoothRatio * Time.deltaTime);
            
            _computeShader.SetFloat("width_scale", _planeWidth / _textureWidth);
            _computeShader.SetFloat("height_scale", _planeHeight / _textureHeight);

            _computeShader.SetFloat("max_height", _maxHeight);

            _computeShader.Dispatch(_kernelCSMain, _textureWidth / _kernelWidth, _textureHeight / _kernelHeight, 1);

            Graphics.Blit(_dataOut, _dataIn);
            Graphics.Blit(_normalOut, _normalIn);
        }

        public override void Defect()
        {
            _computeShader.Dispatch(_kernelCSDefect, 1, 1, 1);

            Graphics.Blit(_dataOut, _dataIn);
        }

        public override void Reset()
        {
            _computeShader.Dispatch(_kernelCSReset, _textureWidth / _kernelWidth, _textureHeight / _kernelHeight, 1);

            Graphics.Blit(_dataOut, _dataIn);
            Graphics.Blit(_normalOut, _normalIn);
        }
        public override void UpdateHitInfo(bool isHit, Vector2 hitInfo)
        {
            _isOpen = isHit;
            _posX = hitInfo.x * _textureWidth;
            _posY = hitInfo.y * _textureHeight;
        }

        public void Test()
        {
            DebugStruct[] indata = new DebugStruct[5];
            DebugStruct[] outdata = new DebugStruct[5];

            ComputeBuffer buffer = new ComputeBuffer(indata.Length, System.Runtime.InteropServices.Marshal.SizeOf(typeof(DebugStruct)));

            buffer.SetData(indata);

            _computeShader.SetBuffer(_kernelCSTest, "debug_buffer", buffer);
            _computeShader.Dispatch(_kernelCSTest, indata.Length, 1, 1);

            buffer.GetData(outdata);

            Debug.Log(outdata[0].f4);
        }
        void Update()
        {
            if (!false)
            //if (!MultiMotionComplex.isSwitchMotion)
            {
                _size = 64;
            }
            else
            {
                _size = 16;
            }


            if(sss)
            DataUpdate();

            //Debug.Log(Time.frameCount);
        }
    }
}