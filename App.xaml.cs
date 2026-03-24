using BatiaSuite.Controls;
using BatiaSuite.Utils;
using BatiaSuite.Views;
using System.Drawing;
using SQLitePCL;
#if IOS
using UIKit;
using Foundation;
#endif

namespace BatiaSuite;

public partial class App : Application {

    public App() {
        SQLitePCL.Batteries_V2.Init();
        InitializeComponent();

        if(UserSession.IdPersonal != 0) {
            MainPage = new AppShell();
        } else {
            MainPage = new Logueo();
        }

        CreateControls();
    }

    void CreateControls() {
        Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping(nameof(TransparentEntry), (handler, view) => {
            if(view is TransparentEntry) {
#if ANDROID
                handler.PlatformView.SetBackgroundColor(Android.Graphics.Color.Transparent);
#elif IOS
                handler.PlatformView.BackgroundColor = UIKit.UIColor.Clear;
                handler.PlatformView.BorderStyle = UIKit.UITextBorderStyle.None;
#endif
            }
        });

        Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("Done", (handler, view) => {
#if IOS
            UIToolbar toolbar = new UIToolbar(new RectangleF(0.0f, 0.0f, 50.0f, 44.0f));
            toolbar.BackgroundColor = UIColor.LightGray;
            UIBarButtonItem doneButton = new UIBarButtonItem(UIBarButtonSystemItem.Done, delegate {
                handler.PlatformView.ResignFirstResponder();
            });

            toolbar.Items = new UIBarButtonItem[] {
                new UIBarButtonItem (UIBarButtonSystemItem.FlexibleSpace),
                doneButton
            };

            handler.PlatformView.InputAccessoryView = toolbar;
#endif
        });

        Microsoft.Maui.Handlers.EditorHandler.Mapper.AppendToMapping(nameof(TransparentEditor), (handler, view) => {
            if(view is TransparentEditor) {
#if ANDROID
                handler.PlatformView.SetBackgroundColor(Android.Graphics.Color.Transparent);
#elif IOS
                handler.PlatformView.BackgroundColor = UIKit.UIColor.Clear;
#endif
            }
        });

        Microsoft.Maui.Handlers.PickerHandler.Mapper.AppendToMapping(nameof(TransparentPicker), (handler, view) => {
            if(view is TransparentPicker) {
#if ANDROID
                handler.PlatformView.SetBackgroundColor(Android.Graphics.Color.Transparent);
#elif IOS
                handler.PlatformView.BackgroundColor = UIKit.UIColor.Clear;
                handler.PlatformView.BorderStyle = UIKit.UITextBorderStyle.None;
#endif
            }
        });

        Microsoft.Maui.Handlers.DatePickerHandler.Mapper.AppendToMapping(nameof(TransparentDatePicker), (handler, view) => {
#if ANDROID
            handler.PlatformView.SetBackgroundColor(Android.Graphics.Color.Transparent);
#elif IOS
            handler.PlatformView.BackgroundColor = UIKit.UIColor.Clear;
            handler.PlatformView.BorderStyle = UIKit.UITextBorderStyle.None;
#endif
        });
    }
}