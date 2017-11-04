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
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;

namespace HL.IconPro.Lib.Wpf.Dialogs
{
    public class IconBrowserDialog
    {
        [DllImport("shell32.dll", EntryPoint = "#62", CharSet = CharSet.Unicode, SetLastError = true)]
        [SuppressUnmanagedCodeSecurity]
        private static extern bool PickIconDialog(
            IntPtr hWnd, StringBuilder pszFilename, int cchFilenameMax, out int pnIconIndex);
        
        private const int MAX_PATH = 260;

        private string _FileName;
        private int _Index;
        public string FileName
        {
            get { return _FileName; }
            set { _FileName = value; }
        }
        public int Index
        {
            get { return _Index; }
            set { _Index = value; }
        }

        public bool? ShowDialog(System.Windows.Window Parent = null)
        {
            StringBuilder bufffer = new StringBuilder(_FileName, MAX_PATH);
            IntPtr parentHwnd = IntPtr.Zero;
            if(Parent!= null)
            {
                WindowInteropHelper interop = new WindowInteropHelper(Parent);
                parentHwnd = interop.EnsureHandle();
            }
            bool result = PickIconDialog(parentHwnd, bufffer, MAX_PATH, out _Index);
            if (result)
            {
                FileName = Environment.ExpandEnvironmentVariables(bufffer.ToString());
            }
            return result;
        }
    }
}
