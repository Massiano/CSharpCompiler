// add reference: System.Windows.Forms
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

class QuickPaste
{
    [DllImport("user32.dll")] static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);
    const uint KEYDOWN = 0, KEYUP = 2;

    static void Main()
    {
        Console.Write("3-sec pause… focus the box now!\n");
        System.Threading.Thread.Sleep(3000);

        SendKeys.SendWait("test text");   // paste text
        System.Threading.Thread.Sleep(100);
        keybd_event(0x0D, 0, KEYDOWN, UIntPtr.Zero); // Enter down
        keybd_event(0x0D, 0, KEYUP, UIntPtr.Zero);   // Enter up
    }
}
