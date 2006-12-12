/***************************************************************************
 *  Log.cs
 *
 *  Written by Trey Ethridge <tale@juno.com>
 ****************************************************************************/

/*  THIS FILE IS LICENSED UNDER THE MIT LICENSE AS OUTLINED IMMEDIATELY BELOW: 
 *
 *  Permission is hereby granted, free of charge, to any person obtaining a
 *  copy of this software and associated documentation files (the "Software"),  
 *  to deal in the Software without restriction, including without limitation  
 *  the rights to use, copy, modify, merge, publish, distribute, sublicense,  
 *  and/or sell copies of the Software, and to permit persons to whom the  
 *  Software is furnished to do so, subject to the following conditions:
 *
 *  The above copyright notice and this permission notice shall be included in 
 *  all copies or substantial portions of the Software.
 *
 *  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 *  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 *  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
 *  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 *  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
 *  FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
 *  DEALINGS IN THE SOFTWARE.
 */

using Banshee.Base;
 
namespace Banshee.Plugins.Cleanup
{
   /**
    * Wrapper class to the LogCore class.
    * Author: Trey Ethridge
    */ 
   public class Log
   {
       public static void Debug(string msg)
       {
           LogCore.Instance.PushDebug(msg, "");
       }
       
       public static void Debug(string msg, string details)
       {
           LogCore.Instance.PushDebug(msg, details);
       }
       
       public static void Debug(string msg, string details, bool showUser)
       {
           LogCore.Instance.PushDebug(msg, details, showUser);
       }
       
       public static void Warning(string msg)
       {
           LogCore.Instance.PushWarning(msg, "");
       }
       
       public static void Warning(string msg, string details)
       {
           LogCore.Instance.PushWarning(msg, details);
       }
       
       public static void Warning(string msg, string details, bool showUser)
       {
           LogCore.Instance.PushWarning(msg, details, showUser);
       }
       
       public static void Error(string msg)
       {
           LogCore.Instance.PushError(msg, "");
       }
       
       public static void Error(string msg, string details)
       {
           LogCore.Instance.PushError(msg, details);
       }
       
       public static void Error(string msg, string details, bool showUser)
       {
           LogCore.Instance.PushError(msg, details, showUser);
      }
   }
}
