// <copyright file="MainViewController.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System.Runtime.InteropServices;
using AudioToolbox;
using CoreFoundation;
using CoreGraphics;
using DA.UI.TableView;
using DA.UI.TableView.Elements;
using Foundation;
using UIKit;

namespace Transcribe.Apple;

/// <summary>
/// Main View Controller.
/// </summary>
public sealed class MainViewController : UITableViewController
{
    private bool isRecording;
    private Microphone microphone;

    /// <summary>
    /// Initializes a new instance of the <see cref="MainViewController"/> class.
    /// </summary>
    public MainViewController()
    {
        this.Title = "Transcribe";
        this.microphone = new Microphone();
        this.microphone.OnBroadcast += this.OnBroadcast;
        this.TableView = new Root()
        {
            new Section()
            {
                ActionElement.Create("Toggle Recording", this.ToggleRecording),
            },
        };
    }

    private void OnBroadcast(object? sender, BroadcastEventArgs e)
    {
        System.Diagnostics.Debug.WriteLine($"Broadcast: {e.AudioData.Length}");
    }

    private void ToggleRecording()
    {
        if (this.isRecording)
        {
            this.microphone.Stop();
            this.isRecording = false;
        }
        else
        {
            this.microphone.Start(16000);
            this.isRecording = true;
        }
    }
}