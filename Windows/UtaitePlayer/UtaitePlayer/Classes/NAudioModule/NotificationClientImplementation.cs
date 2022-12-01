using NAudio.CoreAudioApi;
using RHYANetwork.UtaitePlayer.ExceptionHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtaitePlayer.Classes.Core;

namespace UtaitePlayer.Classes.NAudioModule
{
    class NotificationClientImplementation : NAudio.CoreAudioApi.Interfaces.IMMNotificationClient
    {

        public void OnDefaultDeviceChanged(DataFlow dataFlow, Role deviceRole, string defaultDeviceId)
        {
            //Do some Work
            //Console.WriteLine("OnDefaultDeviceChanged --> {0}", dataFlow.ToString());
        }

        public void OnDeviceAdded(string deviceId)
        {
            //Do some Work
            //Console.WriteLine("OnDeviceAdded -->");
        }

        public void OnDeviceRemoved(string deviceId)
        {

            //Console.WriteLine("OnDeviceRemoved -->");
            //Do some Work
        }

        public async void OnDeviceStateChanged(string deviceId, DeviceState newState)
        {
            //Do some Work
            if (newState == DeviceState.Active)
            {
                try
                {
                    await Task.Run(() =>
                    {
                        try
                        {
                            PlayerService.getInstance().changeAudioDevice();
                        }
                        catch (Exception ex)
                        {
                            ExceptionManager.getInstance().showMessageBox(ex);
                        }
                    });
                }
                catch (Exception) { }
            }
            else if (newState == DeviceState.Unplugged)
            {
                await Task.Run(() =>
                {
                    try
                    {
                        PlayerService.getInstance().changeAudioDevice();
                    }
                    catch (Exception ex)
                    {
                        ExceptionManager.getInstance().showMessageBox(ex);
                    }
                });
            }
        }

        public NotificationClientImplementation()
        {
            //_realEnumerator.RegisterEndpointNotificationCallback();
            if (System.Environment.OSVersion.Version.Major < 6)
            {
                throw new NotSupportedException("This functionality is only supported on Windows Vista or newer.");
            }
        }

        public void OnPropertyValueChanged(string deviceId, PropertyKey propertyKey)
        {
            //Do some Work
            //fmtid & pid are changed to formatId and propertyId in the latest version NAudio
            //Console.WriteLine("OnPropertyValueChanged: formatId --> {0}  propertyId --> {1}", propertyKey.formatId.ToString(), propertyKey.propertyId.ToString());
        }
    }
}
