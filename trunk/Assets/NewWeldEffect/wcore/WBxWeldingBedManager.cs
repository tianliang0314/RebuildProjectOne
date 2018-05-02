using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RichToolKits;
using System.Xml;
using System.IO;
using System;

namespace wcore
{
    /// <summary>
    /// 负责焊缝成形的控制。
    /// </summary>
    public class WBxWeldingBedManager : MonoBehaviour
    {
        // 是否激活
        protected bool _isOpen = false;

        // 熔池生成点
        protected float _posX = 128;
        protected float _posY = 128;

        // 熔池大小
        protected float _size = 128;

        // 熔池升高速度
        protected float _strength = 0.1f;

        // 焊缝数据贴图的分辨率
        protected int _textureWidth = 4096;
        protected int _textureHeight = 4096;

        // 实体表面的物理宽高
        protected float _planeWidth = 10;
        protected float _planeHeight = 10;

        // 焊缝质量分析结果
        public int[,] _weldingStatus;
        // 熔敷率
        public float _depositionEfficiency
        {
            get { return UnityEngine.Random.Range(0.7f, 0.92f); }
        }

        public virtual bool IsOpen { get { return _isOpen; } set { _isOpen = value; } }
        public virtual float PosX { get { return _posX; } set { _posY = value; } }
        public virtual float PosY { get { return _posY; } set { _posY = value; } }
        public virtual int TextureWidth { get { return _textureWidth; } }
        public virtual int TextureHeight { get { return _textureHeight; } }
        public virtual float Size { get { return _size; } set { _size = value; } }
        public virtual float Strength { get { return _strength; } set { _strength = value; } }

        public virtual void UpdateHitInfo(bool isHit, Vector2 hitInfo) { }
        public virtual void Reset() { }
        public virtual void Defect() { }

    }
}

