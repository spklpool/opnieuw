// © 2004 IDesign Inc. All rights reserved 
//Questions? Comments? go to 
//http://www.idesign.net

using System;

namespace Opnieuw.Framework
{
   public class CancleEventArgs : EventArgs
   {
      protected bool m_Cancel; 
      public bool Cancel
      {
         get
         {
            return m_Cancel;
         }   
         set
         {
            m_Cancel = value;
         }   
      }

   }
   public class DoWorkEventArgs : CancleEventArgs
   {
      object m_Result;
      public object Result
      {
         get
         {
            return m_Result;
         }   
         set
         {
            m_Result = value;
         }   
      }
      public readonly object Argument; 
      public DoWorkEventArgs(object argument)
      {
         Argument = argument;
      }
   }
   public class ProgressChangedEventArgs : EventArgs
   {
      public readonly int ProgressPercentage; 
      public ProgressChangedEventArgs(int percentage)
      {
         ProgressPercentage = percentage;
      }
   }

   public class AsyncCompletedEventArgs : EventArgs
   {
      public readonly Exception Error;
      public readonly bool Cancelled;

      public AsyncCompletedEventArgs(Exception error,bool cancel)
      {
         Error = error;
         Cancelled = cancel;
      }
   }

   public class RunWorkerCompletedEventArgs : AsyncCompletedEventArgs
   {
      public readonly object Result;
      public RunWorkerCompletedEventArgs(object result,Exception error,bool cancel) : base(error,cancel)
      {
         Result = result;
      }
   }
    
   public delegate void DoWorkEventHandler(object sender,DoWorkEventArgs e);
   public delegate void ProgressChangedEventHandler(object sender,ProgressChangedEventArgs e);
   public delegate void RunWorkerCompletedEventHandler(object sender,RunWorkerCompletedEventArgs e);
}