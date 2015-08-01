using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Graphics.Display;
using Windows.Graphics.Imaging;
using Windows.Media.Capture;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;

namespace Taxomania.ReceiptBank.Util
{
    public static class ImageUtils
    {
        public static async Task<IStorageFile> GetImageAsync()
        {
            var picker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.Thumbnail,
                SuggestedStartLocation = PickerLocationId.PicturesLibrary
            };
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");
            return await picker.PickSingleFileAsync();
        }

        // If showPicker == false => PicturesLibrary capability must be set in client app
        public static async Task<IStorageFile> CreateBitmapFromElement(FrameworkElement uielement, bool showPicker = true, string filenamePrefix = "receipt")
        {
            try
            {
                var renderTargetBitmap = new RenderTargetBitmap();
                await renderTargetBitmap.RenderAsync(uielement);

                var file = await SaveFile(showPicker, filenamePrefix);
                var pixelBuffer = await renderTargetBitmap.GetPixelsAsync();
                using (var stream = await file.OpenAsync(FileAccessMode.ReadWrite))
                {
                    var logicalDpi = DisplayInformation.GetForCurrentView().LogicalDpi;
                    var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, stream);
                    encoder.SetPixelData(
                        BitmapPixelFormat.Bgra8,
                        BitmapAlphaMode.Ignore,
                        (uint) renderTargetBitmap.PixelWidth,
                        (uint) renderTargetBitmap.PixelHeight,
                        logicalDpi,
                        logicalDpi,
                        pixelBuffer.ToArray());

                    await encoder.FlushAsync();
                }
                return file;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return null;
            }
        }

        private static async Task<IStorageFile> SaveFile(bool showPicker = true, string filenamePrefix="receipt")
        {
            if (!showPicker)
            {
                return await KnownFolders.PicturesLibrary.CreateFileAsync(filenamePrefix + DateTime.UtcNow.ToFileTimeUtc() + ".jpg",
                    CreationCollisionOption.GenerateUniqueName); // this way it is not in app data
            }
            var savePicker = new FileSavePicker {SuggestedStartLocation = PickerLocationId.PicturesLibrary};
            savePicker.FileTypeChoices.Add("Images", new List<string> {".jpg", ".jpeg"});
            savePicker.SuggestedFileName = "receipt" + DateTime.UtcNow;
            return await savePicker.PickSaveFileAsync();
        }

        public static async Task<IStorageFile> CameraCapture()
        {
            var cameraUi = new CameraCaptureUI();
            cameraUi.PhotoSettings.AllowCropping = false;
            cameraUi.PhotoSettings.MaxResolution = CameraCaptureUIMaxPhotoResolution.MediumXga;
            return await cameraUi.CaptureFileAsync(CameraCaptureUIMode.Photo);
        }
    }
}