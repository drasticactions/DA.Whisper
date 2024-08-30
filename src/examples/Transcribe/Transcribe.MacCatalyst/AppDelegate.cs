// <copyright file="AppDelegate.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using Transcribe.Apple;

namespace Transcribe.MacCatalyst;

/// <summary>
/// App Delegate.
/// </summary>
[Register("AppDelegate")]
public class AppDelegate : UIApplicationDelegate
{
    /// <inheritdoc/>
    public override UIWindow? Window
    {
        get;
        set;
    }

    /// <inheritdoc/>
    public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
    {
        // create a new window instance based on the screen size
        this.Window = new UIWindow(UIScreen.MainScreen.Bounds);

        // create a UIViewController with a single UILabel
        var vc = new MainViewController();
        this.Window.RootViewController = vc;

        // make the window visible
        this.Window.MakeKeyAndVisible();

        return true;
    }
}
