/***************************************************************************
 *  CleanupPlugin.cs
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

using Banshee;
using Banshee.Base;
using Banshee.Sources;
using Banshee.Widgets;
using Gtk;
using System;
using System.Collections;
using System.Data;
using System.IO;
using System.Threading;
using Mono.Unix; 
 
namespace Banshee.Plugins.Cleanup
{
    public class CleanupPlugin : Banshee.Plugins.Plugin
    {
          private ActionGroup actions;
          private uint ui_manager_id;
        
        protected override string ConfigurationName 
        { 
                get 
                { 
                    return "Cleanup"; 
                } 
        }
        
        public override string DisplayName 
        { 
                get 
                { 
                    return "Cleanup"; 
                } 
        }
        
        public override string Description 
        {
            get 
            {
                return Catalog.GetString(
                    "A Banshee plugin that removes tracks that have been " +
                    "deleted from the file system or moved to another location."
                );
            }
        }
        
        public override string[] Authors {
            get {
                return new string[] {
                    "Trey Ethridge"
                };
            }
        }
 
        // --------------------------------------------------------------- //
        
        protected override void PluginInitialize() 
        {
            InstallInterfaceActions();
        }
        
        protected override void PluginDispose() 
        {
            Globals.ActionManager.UI.RemoveUi(ui_manager_id);
            Globals.ActionManager.UI.RemoveActionGroup(actions);

            actions = null;
        }
                        
        private void InstallInterfaceActions() 
        {
            actions = new ActionGroup("Cleanup");
            actions.Add(new ActionEntry [] {                                       
                             new ActionEntry("CleanupAction", null /* icon goes here */,
                                              Catalog.GetString("Clean Library"), null,
                                              Catalog.GetString("Remove Tracks That Are Missing"), 
                                              OnStartCleanupHandler),                                      
                         });

            Globals.ActionManager.UI.InsertActionGroup(actions, 0);
            ui_manager_id = Globals.ActionManager.UI.AddUiFromResource("CleanupMenu.xml");
        }
        
        private void OnStartCleanupHandler(object sender, EventArgs args) 
        {
                  CleanupWorker worker = new CleanupWorker(new OnCleanupFinished(this.CleanupComplete));
            
            Thread t = new Thread(new ThreadStart(worker.Cleanup));
            t.Start();
            
            // Must sleep to yield on uniprocessor boxes.
            Thread.Sleep(0); 
        }
        
       private void ShowMsg(string msg, string details) 
       {
            ThreadAssist.ProxyToMain(delegate {
               HigMessageDialog dialog = new HigMessageDialog(InterfaceElements.MainWindow, 
                   DialogFlags.Modal,
                   MessageType.Info,
                   ButtonsType.Ok,
                   msg,
                   details);
                   
               dialog.Run();
               dialog.Destroy();     
            });               
       }
       
       private void CleanupComplete(int numTracksRemoved)
       {
            string msg = Catalog.GetString("Finshed Cleanup");
            string details = null;
            
              if (numTracksRemoved == 0) 
              {
                  details = Catalog.GetString("Library is clean.  There is nothing to remove.");
              }
              else if (numTracksRemoved == 1)
              {
                  details = Catalog.GetString("Removed " + numTracksRemoved + " Track.");                 
              }
              else 
              {
                  details = Catalog.GetString("Removed " + numTracksRemoved + " Tracks.");
              }
               
               Log.Debug(msg, details);
               ShowMsg(msg, details);            
       }
   }
   
   public delegate void OnCleanupFinished(int numTracksRemoved);
   
   public class CleanupWorker
   {
      private OnCleanupFinished cleanupFinished;
      
      public CleanupWorker(OnCleanupFinished cleanupFinished) 
      {
         this.cleanupFinished = cleanupFinished;
      }
      
      public void Cleanup() 
      {
         Log.Debug(Catalog.GetString("Starting Cleanup"), "");
         ArrayList tracksToRemove = new ArrayList();            

         Log.Debug(Catalog.GetString("Starting Query"), "");
         string query = "SELECT TrackID, Uri FROM Tracks";                
         IDataReader reader = Globals.Library.Db.Query(query);
         Log.Debug(Catalog.GetString("Finished Query"), "");
         
         if (reader == null) 
         {
             Log.Warning(Catalog.GetString("CleanupPlugin: Unable to retrieve tracks.  Aborting."), "");
             return;
         }

         while(reader.Read()) 
         {
               try 
               {
                    int trackId = (int) reader["TrackID"];
                    SafeUri uri = new SafeUri((string) reader["Uri"]);
                    string path = SafeUri.UriToFilename(uri); 
                    
                    if (!Banshee.IO.IOProxy.File.Exists(uri)) 
                    {
                        Log.Debug(Catalog.GetString("Adding '") + path + 
                            Catalog.GetString("' to remove list."), "");
                        tracksToRemove.Add(Globals.Library.Tracks[trackId]);
                    }
               } 
               catch(Exception e) 
               {
                     Log.Warning(Catalog.GetString("CleanupPlugin: Error reading track. ("),
                        (reader["Uri"] as string) + "): " + e.Message, false);
               }
         }

         reader.Dispose();

         if (tracksToRemove.Count > 0) 
         {
             Log.Debug(Catalog.GetString("Starting Remove"), "");
             Globals.Library.Remove(tracksToRemove);
             Log.Debug(Catalog.GetString("Finished Remove"), "");
         } 
         else 
         {
             Log.Debug(Catalog.GetString("Library is clean.  Nothing to remove."), "");                    
         }
           
           // Call delegate to notify the plugin that cleanup is complete.
           Log.Debug(Catalog.GetString("Finished Cleanup"), "Thread Finished.");
             cleanupFinished(tracksToRemove.Count);
      }   
   }
   
}
