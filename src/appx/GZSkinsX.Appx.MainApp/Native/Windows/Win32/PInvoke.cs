// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Windows.Win32
{
    static partial class PInvoke
    {
        /// <summary>Retrieves information about the specified window. (GetWindowLongW)</summary>
        /// <param name="hWnd">
        /// <para>Type: <b>HWND</b> A handle to the window and, indirectly, the class to which the window belongs.</para>
        /// <para><see href="https://docs.microsoft.com/windows/win32/api/winuser/nf-winuser-getwindowlongw#parameters">Read more on docs.microsoft.com</see>.</para>
        /// </param>
        /// <param name="nIndex">Type: <b>int</b></param>
        /// <returns>
        /// <para>Type: <b>LONG</b> If the function succeeds, the return value is the requested value. If the function fails, the return value is zero. To get extended error information, call <a href="https://docs.microsoft.com/windows/desktop/api/errhandlingapi/nf-errhandlingapi-getlasterror">GetLastError</a>. If <a href="https://docs.microsoft.com/windows/desktop/api/winuser/nf-winuser-setwindowlonga">SetWindowLong</a> has not been called previously, <b>GetWindowLong</b> returns zero for values in the extra window or class memory.</para>
        /// </returns>
        /// <remarks>
        /// <para>Reserve extra window memory by specifying a nonzero value in the <b>cbWndExtra</b> member of the <a href="https://docs.microsoft.com/windows/desktop/api/winuser/ns-winuser-wndclassexa">WNDCLASSEX</a> structure used with the <a href="https://docs.microsoft.com/windows/desktop/api/winuser/nf-winuser-registerclassexa">RegisterClassEx</a> function.</para>
        /// <para><see href="https://docs.microsoft.com/windows/win32/api/winuser/nf-winuser-getwindowlongw#">Read more on docs.microsoft.com</see>.</para>
        /// </remarks>
        [LibraryImport("USER32.dll", EntryPoint = "GetWindowLongW", SetLastError = true)]
        [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
        [SupportedOSPlatform("windows5.0")]
        private static partial int GetWindowLongPtr32(nint hWnd, int nIndex);

        /// <summary>Retrieves information about the specified window. The function also retrieves the value at a specified offset into the extra window memory. (Unicode)</summary>
        /// <param name="hWnd">
        /// <para>Type: <b>HWND</b> A handle to the window and, indirectly, the class to which the window belongs.</para>
        /// <para><see href="https://docs.microsoft.com/windows/win32/api/winuser/nf-winuser-getwindowlongptrw#parameters">Read more on docs.microsoft.com</see>.</para>
        /// </param>
        /// <param name="nIndex">Type: <b>int</b></param>
        /// <returns>
        /// <para>Type: <b>LONG_PTR</b> If the function succeeds, the return value is the requested value. If the function fails, the return value is zero. To get extended error information, call <a href="https://docs.microsoft.com/windows/desktop/api/errhandlingapi/nf-errhandlingapi-getlasterror">GetLastError</a>. If <a href="https://docs.microsoft.com/windows/desktop/api/winuser/nf-winuser-setwindowlonga">SetWindowLong</a> or <a href="https://docs.microsoft.com/windows/desktop/api/winuser/nf-winuser-setwindowlongptra">SetWindowLongPtr</a> has not been called previously, <b>GetWindowLongPtr</b> returns zero for values in the extra window or class memory.</para>
        /// </returns>
        /// <remarks>
        /// <para>Reserve extra window memory by specifying a nonzero value in the <b>cbWndExtra</b> member of the <a href="https://docs.microsoft.com/windows/desktop/api/winuser/ns-winuser-wndclassexa">WNDCLASSEX</a> structure used with the <a href="https://docs.microsoft.com/windows/desktop/api/winuser/nf-winuser-registerclassexa">RegisterClassEx</a> function.</para>
        /// <para>> [!NOTE] > The winuser.h header defines GetWindowLongPtr as an alias which automatically selects the ANSI or Unicode version of this function based on the definition of the UNICODE preprocessor constant. Mixing usage of the encoding-neutral alias with code that not encoding-neutral can lead to mismatches that result in compilation or runtime errors. For more information, see [Conventions for Function Prototypes](/windows/win32/intl/conventions-for-function-prototypes).</para>
        /// <para><see href="https://docs.microsoft.com/windows/win32/api/winuser/nf-winuser-getwindowlongptrw#">Read more on docs.microsoft.com</see>.</para>
        /// </remarks>
        [LibraryImport("USER32.dll", EntryPoint = "GetWindowLongPtrW", SetLastError = true)]
        [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
        [SupportedOSPlatform("windows5.0")]
        private static partial nint GetWindowLongPtr64(nint hWnd, int nIndex);

        /// <summary>Changes an attribute of the specified window. The function also sets the 32-bit (long) value at the specified offset into the extra window memory. (Unicode)</summary>
        /// <param name="hWnd">
        /// <para>Type: <b>HWND</b> A handle to the window and, indirectly, the class to which the window belongs.</para>
        /// <para><see href="https://docs.microsoft.com/windows/win32/api/winuser/nf-winuser-setwindowlongw#parameters">Read more on docs.microsoft.com</see>.</para>
        /// </param>
        /// <param name="nIndex">Type: <b>int</b></param>
        /// <param name="dwNewLong">
        /// <para>Type: <b>LONG</b> The replacement value.</para>
        /// <para><see href="https://docs.microsoft.com/windows/win32/api/winuser/nf-winuser-setwindowlongw#parameters">Read more on docs.microsoft.com</see>.</para>
        /// </param>
        /// <returns>
        /// <para>Type: <b>LONG</b> If the function succeeds, the return value is the previous value of the specified 32-bit integer. If the function fails, the return value is zero. To get extended error information, call <a href="https://docs.microsoft.com/windows/desktop/api/errhandlingapi/nf-errhandlingapi-getlasterror">GetLastError</a>. If the previous value of the specified 32-bit integer is zero, and the function succeeds, the return value is zero, but the function does not clear the last error information. This makes it difficult to determine success or failure. To deal with this, you should clear the last error information by calling <a href="https://docs.microsoft.com/windows/desktop/api/errhandlingapi/nf-errhandlingapi-setlasterror">SetLastError</a> with 0 before calling <b>SetWindowLong</b>. Then, function failure will be indicated by a return value of zero and a <a href="https://docs.microsoft.com/windows/desktop/api/errhandlingapi/nf-errhandlingapi-getlasterror">GetLastError</a> result that is nonzero.</para>
        /// </returns>
        /// <remarks>
        /// <para>Certain window data is cached, so changes you make using <b>SetWindowLong</b> will not take effect until you call the <a href="https://docs.microsoft.com/windows/desktop/api/winuser/nf-winuser-setwindowpos">SetWindowPos</a> function. Specifically, if you change any of the frame styles, you must call <b>SetWindowPos</b> with the <b>SWP_FRAMECHANGED</b> flag for the cache to be updated properly. If you use <b>SetWindowLong</b> with the <b>GWL_WNDPROC</b> index to replace the window procedure, the window procedure must conform to the guidelines specified in the description of the <a href="https://docs.microsoft.com/previous-versions/windows/desktop/legacy/ms633573(v=vs.85)">WindowProc</a> callback function. If you use <b>SetWindowLong</b> with the <b>DWL_MSGRESULT</b> index to set the return value for a message processed by a dialog procedure, you should return <b>TRUE</b> directly afterward. Otherwise, if you call any function that results in your dialog procedure receiving a window message, the nested window message could overwrite the return value you set using <b>DWL_MSGRESULT</b>. Calling <b>SetWindowLong</b> with the <b>GWL_WNDPROC</b> index creates a subclass of the window class used to create the window. An application can subclass a system class, but should not subclass a window class created by another process. The <b>SetWindowLong</b> function creates the window subclass by changing the window procedure associated with a particular window class, causing the system to call the new window procedure instead of the previous one. An application must pass any messages not processed by the new window procedure to the previous window procedure by calling <a href="https://docs.microsoft.com/windows/desktop/api/winuser/nf-winuser-callwindowproca">CallWindowProc</a>. This allows the application to create a chain of window procedures. Reserve extra window memory by specifying a nonzero value in the <b>cbWndExtra</b> member of the <a href="https://docs.microsoft.com/windows/desktop/api/winuser/ns-winuser-wndclassexa">WNDCLASSEX</a> structure used with the <a href="https://docs.microsoft.com/windows/desktop/api/winuser/nf-winuser-registerclassexa">RegisterClassEx</a> function. You must not call <b>SetWindowLong</b> with the <b>GWL_HWNDPARENT</b> index to change the parent of a child window. Instead, use the <a href="https://docs.microsoft.com/windows/desktop/api/winuser/nf-winuser-setparent">SetParent</a> function. If the window has a class style of <b>CS_CLASSDC</b> or <b>CS_OWNDC</b>, do not set the extended window styles <b>WS_EX_COMPOSITED</b> or <b>WS_EX_LAYERED</b>. Calling <b>SetWindowLong</b> to set the style on a progressbar will reset its position.</para>
        /// <para><see href="https://docs.microsoft.com/windows/win32/api/winuser/nf-winuser-setwindowlongw#">Read more on docs.microsoft.com</see>.</para>
        /// </remarks>
        [LibraryImport("USER32.dll", EntryPoint = "SetWindowLongW", SetLastError = true)]
        [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
        [SupportedOSPlatform("windows5.0")]
        private static partial int SetWindowLongPtr32(nint hWnd, int nIndex, int dwNewLong);

        /// <summary>Changes an attribute of the specified window. (Unicode)</summary>
        /// <param name="hWnd">
        /// <para>Type: <b>HWND</b> A handle to the window and, indirectly, the class to which the window belongs. The <b>SetWindowLongPtr</b> function fails if the process that owns the window specified by the <i>hWnd</i> parameter is at a higher process privilege in the UIPI hierarchy than the process the calling thread resides in. <b>Windows XP/2000:  </b> The <b>SetWindowLongPtr</b> function fails if the window specified by the <i>hWnd</i> parameter does not belong to the same process as the calling thread.</para>
        /// <para><see href="https://docs.microsoft.com/windows/win32/api/winuser/nf-winuser-setwindowlongptrw#parameters">Read more on docs.microsoft.com</see>.</para>
        /// </param>
        /// <param name="nIndex">Type: <b>int</b></param>
        /// <param name="dwNewLong">
        /// <para>Type: <b>LONG_PTR</b> The replacement value.</para>
        /// <para><see href="https://docs.microsoft.com/windows/win32/api/winuser/nf-winuser-setwindowlongptrw#parameters">Read more on docs.microsoft.com</see>.</para>
        /// </param>
        /// <returns>
        /// <para>Type: <b>LONG_PTR</b> If the function succeeds, the return value is the previous value of the specified offset. If the function fails, the return value is zero. To get extended error information, call <a href="https://docs.microsoft.com/windows/desktop/api/errhandlingapi/nf-errhandlingapi-getlasterror">GetLastError</a>. If the previous value is zero and the function succeeds, the return value is zero, but the function does not clear the last error information. To determine success or failure, clear the last error information by calling <a href="https://docs.microsoft.com/windows/desktop/api/errhandlingapi/nf-errhandlingapi-setlasterror">SetLastError</a> with 0, then call <b>SetWindowLongPtr</b>. Function failure will be indicated by a return value of zero and a <a href="https://docs.microsoft.com/windows/desktop/api/errhandlingapi/nf-errhandlingapi-getlasterror">GetLastError</a> result that is nonzero.</para>
        /// </returns>
        /// <remarks>
        /// <para>Certain window data is cached, so changes you make using <b>SetWindowLongPtr</b> will not take effect until you call the <a href="https://docs.microsoft.com/windows/desktop/api/winuser/nf-winuser-setwindowpos">SetWindowPos</a> function. If you use <b>SetWindowLongPtr</b> with the <b>GWLP_WNDPROC</b> index to replace the window procedure, the window procedure must conform to the guidelines specified in the description of the <a href="https://docs.microsoft.com/previous-versions/windows/desktop/legacy/ms633573(v=vs.85)">WindowProc</a> callback function. If you use <b>SetWindowLongPtr</b> with the <b>DWLP_MSGRESULT</b> index to set the return value for a message processed by a dialog box procedure, the dialog box procedure should return <b>TRUE</b> directly afterward. Otherwise, if you call any function that results in your dialog box procedure receiving a window message, the nested window message could overwrite the return value you set by using <b>DWLP_MSGRESULT</b>. Calling <b>SetWindowLongPtr</b> with the <b>GWLP_WNDPROC</b> index creates a subclass of the window class used to create the window. An application can subclass a system class, but should not subclass a window class created by another process. The <b>SetWindowLongPtr</b> function creates the window subclass by changing the window procedure associated with a particular window class, causing the system to call the new window procedure instead of the previous one. An application must pass any messages not processed by the new window procedure to the previous window procedure by calling <a href="https://docs.microsoft.com/windows/desktop/api/winuser/nf-winuser-callwindowproca">CallWindowProc</a>. This allows the application to create a chain of window procedures. Reserve extra window memory by specifying a nonzero value in the <b>cbWndExtra</b> member of the <a href="https://docs.microsoft.com/windows/desktop/api/winuser/ns-winuser-wndclassexa">WNDCLASSEX</a> structure used with the <a href="https://docs.microsoft.com/windows/desktop/api/winuser/nf-winuser-registerclassexa">RegisterClassEx</a> function. Do not call <b>SetWindowLongPtr</b> with the <b>GWLP_HWNDPARENT</b> index to change the parent of a child window. Instead, use the <a href="https://docs.microsoft.com/windows/desktop/api/winuser/nf-winuser-setparent">SetParent</a> function. If the window has a class style of <b>CS_CLASSDC</b> or <b>CS_PARENTDC</b>, do not set the extended window styles <b>WS_EX_COMPOSITED</b> or <b>WS_EX_LAYERED</b>. Calling <b>SetWindowLongPtr</b> to set the style on a progressbar will reset its position.</para>
        /// <para>> [!NOTE] > The winuser.h header defines SetWindowLongPtr as an alias which automatically selects the ANSI or Unicode version of this function based on the definition of the UNICODE preprocessor constant. Mixing usage of the encoding-neutral alias with code that not encoding-neutral can lead to mismatches that result in compilation or runtime errors. For more information, see [Conventions for Function Prototypes](/windows/win32/intl/conventions-for-function-prototypes).</para>
        /// <para><see href="https://docs.microsoft.com/windows/win32/api/winuser/nf-winuser-setwindowlongptrw#">Read more on docs.microsoft.com</see>.</para>
        /// </remarks>
        [LibraryImport("USER32.dll", EntryPoint = "SetWindowLongPtrW", SetLastError = true)]
        [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
        [SupportedOSPlatform("windows5.0")]
        private static partial nint SetWindowLongPtr64(nint hWnd, int nIndex, nint dwNewLong);

        /// <inheritdoc cref="GetWindowLongPtr64(nint, int)"/>
        public static nint GetWindowLong(nint hWnd, int nIndex)
        {
            return Environment.Is64BitProcess ? GetWindowLongPtr64(hWnd, nIndex) : GetWindowLongPtr32(hWnd, nIndex);
        }

        /// <inheritdoc cref="SetWindowLongPtr64(nint, int, nint)"/>
        public static nint SetWindowLong(nint hWnd, int nIndex, nint dwNewLong)
        {
            return Environment.Is64BitProcess ? SetWindowLongPtr64(hWnd, nIndex, dwNewLong) : SetWindowLongPtr32(hWnd, nIndex, dwNewLong.ToInt32());
        }
    }
}
