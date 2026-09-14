using ObjCRuntime;
using UIKit;
using System;
using System.Threading.Tasks;
using Foundation;

namespace BatiaSuite
{ 
    public class Program {
        // This is the main entry point of the application.
        static void Main(string[] args) {
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                LogCrash("AppDomain.UnhandledException", e.ExceptionObject as Exception);
            };

            TaskScheduler.UnobservedTaskException += (s, e) =>
            {
                LogCrash("TaskScheduler.UnobservedTaskException", e.Exception);
                e.SetObserved();
            };

            try {
                // if you want to use a different Application Delegate class from "AppDelegate"
                // you can specify it here.
                UIApplication.Main(args, null, typeof(AppDelegate));
            } catch(Exception ex) {
                LogCrash("Main try/catch", ex);
                throw;
            }
        }

        static void LogCrash(string source, Exception ex) {
            try {
                var docs = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                var path = System.IO.Path.Combine(docs, "startup_crash.txt");
                var text = $"[{DateTime.Now:O}] {source}\n{ex}\n\n";
                System.IO.File.AppendAllText(path, text);
            } catch {
                // no queremos que el logger cause otro crash
            }
        }
    }
}