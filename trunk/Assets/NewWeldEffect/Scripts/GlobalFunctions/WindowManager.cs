using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class WindowManager : MonoBehaviour
{
    public enum EWindowStyle
    {
        eFullScreen,
        eWindowedFullScreen,
        eWindowed,
        eWindowedWithoutBorder
    }
    public enum EZDepth
    {
        eNormal,
        eTop,
        eTopMost
    }
    private const uint SWP_SHOWWINDOW = 64u;
    private const int GWL_STYLE = -16;
    private const int WS_BORDER = 1;
    private const int GWL_EXSTYLE = -20;
    private const int WS_CAPTION = 12582912;
    private const int WS_POPUP = 8388608;
    private const int SM_CXSCREEN = 0;
    private const int SM_CYSCREEN = 1;
    public WindowManager.EWindowStyle AppWindowStyle = WindowManager.EWindowStyle.eWindowedWithoutBorder;
    public WindowManager.EZDepth ScreenDepth = EZDepth.eTopMost;
    public int windowLeft = 10;
    public int windowTop = 10;
    public int windowWidth = 1024;
    public int windowHeight = 768;
    private Rect screenPosition;
    private IntPtr HWND_TOP = new IntPtr(0);
    private IntPtr HWND_TOPMOST = new IntPtr(-1);
    private IntPtr HWND_NORMAL = new IntPtr(-2);
    private int Xscreen;
    private int Yscreen;
    private int i;
    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hPos, int x, int y, int cx, int cy, uint nflags);
    [DllImport("User32.dll")]
    private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
    [DllImport("User32.dll")]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
    [DllImport("User32.dll")]
    private static extern int GetWindowLong(IntPtr hWnd, int dwNewLong);
    [DllImport("User32.dll")]
    private static extern bool MoveWindow(IntPtr hWnd, int x, int y, int width, int height, bool repaint);
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern int ShowWindow(IntPtr hwnd, int nCmdShow);
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern int SendMessage(IntPtr hwnd, int msg, IntPtr wP, IntPtr IP);
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern IntPtr SetParent(IntPtr hChild, IntPtr hParent);
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern IntPtr GetParent(IntPtr hChild);
    [DllImport("User32.dll")]
    public static extern IntPtr GetSystemMetrics(int nIndex);


    void Awake()
    {
        QualitySettings.SetQualityLevel(3);
        //if (GlobalFunctions.GetConfigXMLNodeValue("Is_DoubleScreen") == "true")
        //{
        //    this.windowWidth = 2880;
        //    this.windowHeight = 900;
        //    //Screen.SetResolution(2880, 900, false);
        //}
        //else
        //{
        //    this.windowWidth = 1440;
        //    this.windowHeight = 900;
        //    //Screen.SetResolution(1440, 900, false);
        //}

        //this.Xscreen = (int)UTxWindowManager.GetSystemMetrics(0);
        //this.Yscreen = (int)UTxWindowManager.GetSystemMetrics(1);

        //if (this.AppWindowStyle == UTxWindowManager.EWindowStyle.eFullScreen)
        //{
        //    Screen.SetResolution(this.Xscreen, this.Yscreen, true);
        //}
        //if (this.AppWindowStyle == UTxWindowManager.EWindowStyle.eWindowedFullScreen)
        //{
        //    Screen.SetResolution(this.Xscreen - 1, this.Yscreen - 1, false);
        //    this.screenPosition = new Rect(0.0f, 0.0f, (float)(this.Xscreen - 1), (float)(this.Yscreen - 1));
        //}
        //if (this.AppWindowStyle == UTxWindowManager.EWindowStyle.eWindowed)
        //{
        //    Screen.SetResolution(this.windowWidth, this.windowWidth, false);
        //}
        //if (this.AppWindowStyle == UTxWindowManager.EWindowStyle.eWindowedWithoutBorder)
        //{
        //    Screen.SetResolution(this.windowWidth, this.windowWidth, false);
        //    this.screenPosition = new Rect((float)this.windowLeft, (float)this.windowTop, (float)this.windowWidth, (float)this.windowWidth);
        //}

        if (!Application.isEditor)
        {
            //while (UTxWindowManager.SetWindowLong(UTxWindowManager.GetForegroundWindow(), -16, 369164288) == 0)
            //{
            //    break;
            //}

            //IntPtr ptr = UTxWindowManager.GetForegroundWindow();
            IntPtr ptr = WindowManager.FindWindow(null, "MechanicalArm");
            if (ptr.Equals(IntPtr.Zero))
            {
                Application.Quit();
            }
            WindowManager.SetWindowPos(ptr, this.HWND_TOPMOST, 0, 0, windowWidth, windowHeight, 64u);
            while (true)
            {
                if (WindowManager.SetWindowLong(ptr, -16, 369164288) != 0)
                {
                    break;
                }
            }
            WindowManager.SetWindowPos(ptr, this.HWND_TOPMOST, 0, 0, windowWidth, windowHeight, 64u);
        }
    }

    private void Update()
    {
        if (Application.isEditor)
        {
            return;
        }

        return;

        if (this.i < 5)
        {
            if (this.AppWindowStyle == WindowManager.EWindowStyle.eWindowedFullScreen)
            {
                WindowManager.SetWindowLong(WindowManager.GetForegroundWindow(), -16, 369164288);
                if (this.ScreenDepth == WindowManager.EZDepth.eNormal)
                {
                    WindowManager.SetWindowPos(WindowManager.GetForegroundWindow(), this.HWND_NORMAL, (int)this.screenPosition.x, (int)this.screenPosition.y, (int)this.screenPosition.width, (int)this.screenPosition.height, 64u);
                }
                if (this.ScreenDepth == WindowManager.EZDepth.eTop)
                {
                    WindowManager.SetWindowPos(WindowManager.GetForegroundWindow(), this.HWND_TOP, (int)this.screenPosition.x, (int)this.screenPosition.y, (int)this.screenPosition.width, (int)this.screenPosition.height, 64u);
                }
                if (this.ScreenDepth == WindowManager.EZDepth.eTopMost)
                {
                    WindowManager.SetWindowPos(WindowManager.GetForegroundWindow(), this.HWND_TOPMOST, (int)this.screenPosition.x, (int)this.screenPosition.y, (int)this.screenPosition.width, (int)this.screenPosition.height, 64u);
                }
                WindowManager.ShowWindow(WindowManager.GetForegroundWindow(), 3);
            }
            if (this.AppWindowStyle == WindowManager.EWindowStyle.eWindowed)
            {
                if (this.ScreenDepth == WindowManager.EZDepth.eNormal)
                {
                    WindowManager.SetWindowPos(WindowManager.GetForegroundWindow(), this.HWND_NORMAL, 0, 0, 0, 0, 3u);
                    WindowManager.SetWindowPos(WindowManager.GetForegroundWindow(), this.HWND_NORMAL, 0, 0, 0, 0, 35u);
                }
                if (this.ScreenDepth == WindowManager.EZDepth.eTop)
                {
                    WindowManager.SetWindowPos(WindowManager.GetForegroundWindow(), this.HWND_TOP, 0, 0, 0, 0, 3u);
                    WindowManager.SetWindowPos(WindowManager.GetForegroundWindow(), this.HWND_TOP, 0, 0, 0, 0, 35u);
                }
                if (this.ScreenDepth == WindowManager.EZDepth.eTopMost)
                {
                    WindowManager.SetWindowPos(WindowManager.GetForegroundWindow(), this.HWND_TOPMOST, 0, 0, 0, 0, 3u);
                    WindowManager.SetWindowPos(WindowManager.GetForegroundWindow(), this.HWND_TOPMOST, 0, 0, 0, 0, 35u);
                }
            }
            if (this.AppWindowStyle == WindowManager.EWindowStyle.eWindowedWithoutBorder)
            {
                WindowManager.SetWindowLong(WindowManager.GetForegroundWindow(), -16, 369164288);
                if (this.ScreenDepth == WindowManager.EZDepth.eNormal)
                {
                    WindowManager.SetWindowPos(WindowManager.GetForegroundWindow(), this.HWND_NORMAL, (int)this.screenPosition.x, (int)this.screenPosition.y, (int)this.screenPosition.width, (int)this.screenPosition.height, 64u);
                }
                if (this.ScreenDepth == WindowManager.EZDepth.eTop)
                {
                    WindowManager.SetWindowPos(WindowManager.GetForegroundWindow(), this.HWND_TOP, (int)this.screenPosition.x, (int)this.screenPosition.y, (int)this.screenPosition.width, (int)this.screenPosition.height, 64u);
                }
                if (this.ScreenDepth == WindowManager.EZDepth.eTopMost)
                {
                    WindowManager.SetWindowPos(WindowManager.GetForegroundWindow(), this.HWND_TOPMOST, (int)this.screenPosition.x, (int)this.screenPosition.y, (int)this.screenPosition.width, (int)this.screenPosition.height, 64u);
                }
            }
        }
        this.i++;
    }
}
