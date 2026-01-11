using System.Drawing.Imaging;

namespace BOOSE
{
    /// <summary>
    /// Minimal canvas contract required by the BOOSE interpreter.
    /// The WinForms app provides an implementation (CommandCanvas).
    /// </summary>
    public interface ICanvas
    {
        /// <summary>Current pen X position.</summary>
        int Xpos { get; set; }

        /// <summary>Current pen Y position.</summary>
        int Ypos { get; set; }

        /// <summary>Pen colour used for drawing.</summary>
        object PenColour { get; set; }

        /// <summary>Clear the canvas to a blank background.</summary>
        void Clear();

        /// <summary>Reset the canvas (clear + reset pen state).</summary>
        void Reset();

        /// <summary>Resize the underlying drawing surface.</summary>
        void Set(int width, int height);

        /// <summary>Set the pen colour using RGB values (0-255).</summary>
        void SetColour(int red, int green, int blue);

        /// <summary>Move the pen to (x,y) without drawing.</summary>
        void MoveTo(int x, int y);

        /// <summary>Draw a line to (x,y) and update pen position.</summary>
        void DrawTo(int x, int y);

        /// <summary>Draw a circle at the current pen position.</summary>
        void Circle(int radius, bool filled);

        /// <summary>Draw a rectangle from the current pen position.</summary>
        void Rect(int width, int height, bool filled);

        /// <summary>Draw a triangle based on width and height.</summary>
        void Tri(int width, int height);

        /// <summary>Write text at the current pen position.</summary>
        void WriteText(string text);

        /// <summary>Save the drawing surface as an image.</summary>
        void SaveImage(string fileName, ImageFormat format);

        /// <summary>Load an image as the drawing surface.</summary>
        void LoadImage(string fileName);

        /// <summary>Return the underlying bitmap for display.</summary>
        object getBitmap();
    }
}
