using FlowBoard.Classes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Input.Inking;
using Windows.Storage.Provider;
using Windows.Storage.Pickers;
using Windows.Storage;
using static FlowBoard.Classes.FileClass;

namespace FlowBoard.Helpers
{
    public class FileHelper
    {
        public static async Task<FileClass> OpenFilePicker()
        {
            var picker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.Thumbnail,
                SuggestedStartLocation = PickerLocationId.PicturesLibrary
            };
            picker.FileTypeFilter.Add(".flowx");
            return JsonConvert.DeserializeObject<FileClass>(await FileIO.ReadTextAsync(await picker.PickSingleFileAsync()));
        }

        public static async Task<bool> SaveNewFile(Color canvasColor, List<FileInkStroke> inkStrokes)
        {
            FileClass File = new FileClass();
            File.CanvasColor = canvasColor;
            File.Version = 1;
            File.InkStrokes = inkStrokes;
            var savePicker = new FileSavePicker();
            savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            // Dropdown of file types the user can save the file as
            savePicker.FileTypeChoices.Add("FlowBoard Project", new List<string>() { ".flowx" });
            // Default file name if the user does not type one in or select a file to replace
            savePicker.SuggestedFileName = "New Project";
            StorageFile file = await savePicker.PickSaveFileAsync();
            if (file != null)
            {
                CachedFileManager.DeferUpdates(file);
                await FileIO.WriteTextAsync(file, JsonConvert.SerializeObject(File));
                FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(file);
                return status == FileUpdateStatus.Complete ? true : false;
            }
            else
            {
                return false;
            }
        }

        public static FileInkStroke ConvertToFileInkStroke(InkStroke ink)
        {
            List<FileInkPoint> Points = new List<FileInkPoint>();
            foreach(var i in ink.GetInkPoints())
            {
                Points.Add(new FileInkPoint
                {
                    Position = i.Position,
                    TiltX = i.TiltX,
                    TiltY = i.TiltY,
                    TimeStamp = i.Timestamp,
                    Pressure = i.Pressure
                });
            }
            return new FileInkStroke
            {
                IsPenTipCircle = ink.DrawingAttributes.PenTip == PenTipShape.Circle,
                IgnorePressure = ink.DrawingAttributes.IgnorePressure,
                IgnoreTilt = ink.DrawingAttributes.IgnoreTilt,
                Color = ink.DrawingAttributes.Color,
                DrawAsHighlighter = ink.DrawingAttributes.DrawAsHighlighter,
                Size = ink.DrawingAttributes.Size,
                PointTransform = ink.PointTransform,
                InkPoints = Points
            };
        }
    }
}
