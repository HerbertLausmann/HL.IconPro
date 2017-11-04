/*
 *  Icon Pro
 *  Copyright (C) 2017 Herbert Lausmann. All rights reserved.
 *  http://herbertdotlausmann.wordpress.com/
 *  herbert.lausmann@hotmail.com
 *
 *  Redistribution and use in source and binary forms, with or without
 *  modification, are permitted provided that the following conditions
 *  are met:
 *
 *   1. Redistributions of source code must retain the above copyright
 *      notice, this list of conditions and the following disclaimer.
 *   2. Redistributions in binary form must reproduce the above copyright
 *      notice and this list of conditions.    
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;

namespace HL.IconPro.Lib.Wpf.Dialogs
{
#pragma warning disable 0414
    public sealed class FolderBrowserDialog
    {
        #region Shell32
        // Constants for sending and receiving messages in BrowseCallBackProc
        private const int WM_USER = 0x400;
        private const int BFFM_INITIALIZED = 1;
        private const int BFFM_SELCHANGED = 2;
        private const int BFFM_VALIDATEFAILEDA = 3;
        private const int BFFM_VALIDATEFAILEDW = 4;
        private const int BFFM_IUNKNOWN = 5; // provides IUnknown to client. lParam: IUnknown*
        private const int BFFM_SETSTATUSTEXTA = WM_USER + 100;
        private const int BFFM_ENABLEOK = WM_USER + 101;
        private const int BFFM_SETSELECTIONA = WM_USER + 102;
        private const int BFFM_SETSELECTIONW = WM_USER + 103;
        private const int BFFM_SETSTATUSTEXTW = WM_USER + 104;
        private const int BFFM_SETOKTEXT = WM_USER + 105; // Unicode only
        private const int BFFM_SETEXPANDED = WM_USER + 106; // Unicode only

        // Browsing for directory.
        private uint BIF_RETURNONLYFSDIRS = 0x0001;  // For finding a folder to start document searching
        private uint BIF_DONTGOBELOWDOMAIN = 0x0002;  // For starting the Find Computer
        private uint BIF_STATUSTEXT = 0x0004;  // Top of the dialog has 2 lines of text for BROWSEINFO.lpszTitle and one line if
        // this flag is set.  Passing the message BFFM_SETSTATUSTEXTA to the hwnd can set the
        // rest of the text.  This is not used with BIF_USENEWUI and BROWSEINFO.lpszTitle gets
        // all three lines of text.
        private uint BIF_RETURNFSANCESTORS = 0x0008;
        private uint BIF_EDITBOX = 0x0010;   // Add an editbox to the dialog
        private uint BIF_VALIDATE = 0x0020;   // insist on valid result (or CANCEL)

        private uint BIF_NEWDIALOGSTYLE = 0x0040;   // Use the new dialog layout with the ability to resize
        // Caller needs to call OleInitialize() before using this API
        private uint BIF_USENEWUI = 0x0040 + 0x0010; //(BIF_NEWDIALOGSTYLE | BIF_EDITBOX);

        private uint BIF_BROWSEINCLUDEURLS = 0x0080;   // Allow URLs to be displayed or entered. (Requires BIF_USENEWUI)
        private uint BIF_UAHINT = 0x0100;   // Add a UA hint to the dialog, in place of the edit box. May not be combined with BIF_EDITBOX
        private uint BIF_NONEWFOLDERBUTTON = 0x0200;   // Do not add the "New Folder" button to the dialog.  Only applicable with BIF_NEWDIALOGSTYLE.
        private uint BIF_NOTRANSLATETARGETS = 0x0400;  // don't traverse target as shortcut

        private uint BIF_BROWSEFORCOMPUTER = 0x1000;  // Browsing for Computers.
        private uint BIF_BROWSEFORPRINTER = 0x2000;// Browsing for Printers
        private uint BIF_BROWSEINCLUDEFILES = 0x4000; // Browsing for Everything
        private uint BIF_SHAREABLE = 0x8000;  // sharable resources displayed (remote shares, requires BIF_USENEWUI)

        [DllImport("shell32.dll")]
        private static extern IntPtr SHBrowseForFolder(ref BROWSEINFO lpbi);

        // Note that the BROWSEINFO object's pszDisplayName only gives you the name of the folder.
        // To get the actual path, you need to parse the returned PIDL
        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        // static extern uint SHGetPathFromIDList(IntPtr pidl, [MarshalAs(UnmanagedType.LPWStr)] 
        //StringBuilder pszPath);
        private static extern bool SHGetPathFromIDList(IntPtr pidl, IntPtr pszPath);

        [DllImport("user32.dll", PreserveSig = true)]
        private static extern IntPtr SendMessage(HandleRef hWnd, uint Msg, int wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(HandleRef hWnd, int msg, int wParam, string lParam);

        private delegate int BrowseCallBackProc(IntPtr hwnd, int msg, IntPtr lp, IntPtr wp);
        private struct BROWSEINFO
        {
            public IntPtr hwndOwner;
            public IntPtr pidlRoot;
            public string pszDisplayName;
            public string lpszTitle;
            public uint ulFlags;
            public BrowseCallBackProc lpfn;
            public IntPtr lParam;
            public int iImage;
        }
        private int OnBrowseEvent(IntPtr hWnd, int msg, IntPtr lp, IntPtr lpData)
        {
            switch (msg)
            {
                case BFFM_INITIALIZED: // Required to set initialPath
                    {
                        //Win32.SendMessage(new HandleRef(null, hWnd), BFFM_SETSELECTIONA, 1, lpData);
                        // Use BFFM_SETSELECTIONW if passing a Unicode string, i.e. native CLR Strings.
                        SendMessage(new HandleRef(null, hWnd), BFFM_SETSELECTIONW, 1, _FolderPath);
                        break;
                    }
                case BFFM_SELCHANGED:
                    {
                        IntPtr pathPtr = Marshal.AllocHGlobal((int)(260 * Marshal.SystemDefaultCharSize));
                        if (SHGetPathFromIDList(lp, pathPtr))
                            SendMessage(new HandleRef(null, hWnd), BFFM_SETSTATUSTEXTW, 0, pathPtr);
                        Marshal.FreeHGlobal(pathPtr);
                        break;
                    }
            }

            return 0;
        }
        private string SelectFolder(IntPtr parentHandle)
        {
            StringBuilder sb = new StringBuilder(256);
            IntPtr bufferAddress = Marshal.AllocHGlobal(256); ;
            IntPtr pidl = IntPtr.Zero;
            BROWSEINFO bi = new BROWSEINFO();
            bi.hwndOwner = parentHandle;
            bi.pidlRoot = IntPtr.Zero;
            bi.lpszTitle = _Caption;
            bi.ulFlags = BIF_NEWDIALOGSTYLE | BIF_SHAREABLE;
            bi.lpfn = new BrowseCallBackProc(OnBrowseEvent);
            bi.lParam = IntPtr.Zero;
            bi.iImage = 0;

            try
            {
                pidl = SHBrowseForFolder(ref bi);
                if (!SHGetPathFromIDList(pidl, bufferAddress))
                {
                    return null;
                }
                sb.Append(Marshal.PtrToStringAuto(bufferAddress));
            }
            finally
            {
                // Caller is responsible for freeing this memory.
                Marshal.FreeCoTaskMem(pidl);
            }

            return sb.ToString();
        }
        #endregion
        private string _Caption;
        private string _FolderPath;
        public string Caption
        {
            get
            {
                return _Caption;
            }
            set
            {
                _Caption = value;
            }
        }
        public string FolderPath
        {
            get
            {
                return _FolderPath;
            }
            set
            {
                _FolderPath = value;
            }
        }

        public bool? ShowDialog(System.Windows.Window Parent = null)
        {
            IntPtr parentHwnd = IntPtr.Zero;
            if (Parent != null)
            {
                WindowInteropHelper interop = new WindowInteropHelper(Parent);
                parentHwnd = interop.EnsureHandle();
            }
            string result = SelectFolder(IntPtr.Zero);
            if (result == null)
            {
                _FolderPath = null;
                return false;
            }
            else
            {
                _FolderPath = result;
                return true;
            }
        }
    }
}
