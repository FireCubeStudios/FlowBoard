using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Input;
using Windows.UI.Xaml.Input;

namespace FlowBoard.Helpers
{
    public class FlowMatrixHelper
    {
        public static Matrix4x4 ToMatrix4x4(Matrix3x2 matrix)
        {
            return new Matrix4x4(
               matrix.M11, matrix.M12, 0, 0,
               matrix.M21, matrix.M22, 0, 0,
               0, 0, 1, 0,
               matrix.M31, matrix.M32, 0, 1);
        }

        public static Matrix3x2 DivideMatrix(Matrix3x2 matrix, Matrix3x2 Divisor)
        {
            return new Matrix3x2(
               matrix.M11, matrix.M12,
               matrix.M21, matrix.M22,
               matrix.M31, matrix.M32);
        }

        public static Matrix3x2 GetScale(float e) => Matrix3x2.CreateScale(e);

        public static Matrix3x2 GetScale(ManipulationDeltaRoutedEventArgs e) => Matrix3x2.CreateScale(e.Delta.Scale);

        public static Matrix3x2 GetTranslation(ManipulationDeltaRoutedEventArgs e) 
            => Matrix3x2.CreateTranslation((float)-e.Position.X, (float)-e.Position.Y) *
            GetScale(e) *
            Matrix3x2.CreateTranslation((float)e.Position.X, (float)e.Position.Y) *
            Matrix3x2.CreateTranslation((float)e.Delta.Translation.X, (float)e.Delta.Translation.Y);

        public static Matrix3x2 GetTranslation(PointerRoutedEventArgs e, float scale, PointerPoint point)
         => Matrix3x2.CreateTranslation((float)-point.RawPosition.X, (float)-point.RawPosition.X) *
         GetScale(scale) *
         Matrix3x2.CreateTranslation((float)point.RawPosition.X, (float)point.RawPosition.X) *
         Matrix3x2.CreateTranslation(0, 0);

        /*  Code for rotation, Kept for reference
            Matrix3x2.CreateRotation((float)(e.Delta.Rotation / 180 * Math.PI)) *
        */

        //  Pointer methods
        public static Matrix3x2 GetScale(int Delta) => Matrix3x2.CreateScale(Delta);

        public static Matrix3x2 GetTranslation(PointerPoint e, double Width, double Height) => Matrix3x2.CreateTranslation((float)-e.Position.X, (float)-e.Position.Y) *
                            GetScale(e.Properties.MouseWheelDelta) *
                            Matrix3x2.CreateTranslation((float)e.Position.X, (float)e.Position.Y) *
                            Matrix3x2.CreateTranslation(GetPointerTranslation(e.Position.X, Width), GetPointerTranslation(e.Position.Y, Height));

        public static float GetPointerTranslation(double pos, double Length) => -(float)((Length / 2) - pos);
        }
    }
