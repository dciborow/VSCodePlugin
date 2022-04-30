// Copyright(c) Loupedeck.All rights reserved.

namespace Loupedeck.VSCodePlugin
{
    using System;
    using System.Timers;
    using VSCodeAPI.Web.Models;

    internal class VSCodeVolumeAdjustment : PluginDynamicAdjustment
    {
        private VSCodePlugin VSCodePlugin => this.Plugin as VSCodePlugin;

        private Boolean _volumeBlocked;

        private Timer _volumeBlockedTimer;

        public VSCodeVolumeAdjustment()
            : base(
                "VSCode Volume",
                "VSCode Volume description",
                "VSCode Volume",
                true)
        {
        }

        protected override void ApplyAdjustment(String actionParameter, Int32 ticks)
        {
            try
            {
                var modifiedVolume = 0;
                if (this._volumeBlocked)
                {
                    modifiedVolume = this.VSCodePlugin.PreviousVolume + ticks;
                }
                else
                {
                    var playback = this.VSCodePlugin.Api.GetPlayback();
                    if (playback?.Device == null)
                    {
                        // Set plugin status and message
                        this.VSCodePlugin.CheckStatusCode(System.Net.HttpStatusCode.NotFound, "Cannot adjust volume, no device");
                        return;
                    }
                    else
                    {
                        this.InitVolumeBlockedTimer();
                        modifiedVolume = playback.Device.VolumePercent + ticks;
                    }
                }

                this.VSCodePlugin.PreviousVolume = modifiedVolume;
                this.VSCodePlugin.CheckVSCodeResponse(this.SetVolume, modifiedVolume);
            }
            catch (Exception e)
            {
                Tracer.Trace($"VSCode VSCodeVolumeAdjustment action obtain an error: ", e);
            }
        }

        // Overwrite the RunCommand method that is called every time the user presses the encoder to which this command is assigned
        protected override void RunCommand(String actionParameter)
        {
            try
            {
                this.VSCodePlugin.CheckVSCodeResponse(this.TogglePlayback);
            }
            catch (Exception e)
            {
                Tracer.Trace($"VSCode VSCodeVolumeAdjustment action obtain an error: ", e);
            }
        }

        protected override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize)
        {
            // Dial strip 50px
            var bitmapImage = EmbeddedResources.ReadImage("Loupedeck.VSCodePlugin.Icons.Width50.Volume.png");
            return bitmapImage;
        }

        public ErrorResponse TogglePlayback()
        {
            var playback = this.VSCodePlugin.Api.GetPlayback();
            return playback.IsPlaying
                ? this.VSCodePlugin.Api.PausePlayback(this.VSCodePlugin.CurrentDeviceId)
                : this.VSCodePlugin.Api.ResumePlayback(this.VSCodePlugin.CurrentDeviceId, String.Empty, null, String.Empty, 0);
        }

        private void InitVolumeBlockedTimer()
        {
            if (this._volumeBlockedTimer == null)
            {
                this._volumeBlockedTimer = new Timer(2000);
                this._volumeBlockedTimer.Elapsed += this.VolumeBlockExpired;
            }

            this._volumeBlocked = true;
            if (this._volumeBlockedTimer.Enabled)
            {
                this._volumeBlockedTimer.Stop();
            }

            this._volumeBlockedTimer.Start();
        }

        public ErrorResponse SetVolume(Int32 percents)
        {
            if (percents > 100)
            {
                percents = 100;
            }

            if (percents < 0)
            {
                percents = 0;
            }

            var response = this.VSCodePlugin.Api.SetVolume(percents, this.VSCodePlugin.CurrentDeviceId);
            return response;
        }

        private void VolumeBlockExpired(Object o, ElapsedEventArgs e) => this._volumeBlocked = false;
    }
}
