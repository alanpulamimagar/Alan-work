using BOOSE;
using System.Drawing;
using System.Drawing.Imaging;

namespace boosecli;

/// <summary>
/// Simple bitmap-backed canvas that can be used from a console app.
/// Implements the same ICanvas contract as the WinForms canvas, but without UI.
/// </summary>
public sealed class BitmapCanvas : ICanvas, IDisposable
{
    private int _x;
    private int _y;
    private Color _penColor = Color.Black;

    private Bitmap _bitmap;
    private Graphics _graphics;

    public BitmapCanvas(int width = 800, int height = 600)
    {
        _bitmap = new Bitmap(width, height);
        _graphics = Graphics.FromImage(_bitmap);
        _graphics.Clear(Color.White);
    }

    public int Xpos
    {
        get => _x;
        set => _x = value;
    }

    public int Ypos
    {
        get => _y;
        set => _y = value;
    }

    public object PenColour
    {
        get => _penColor;
        set
        {
            if (value is Color c)
                _penColor = c;
            else
                throw new ArgumentException("PenColour must be a System.Drawing.Color");
        }
    }

    public void Clear()
    {
        _graphics.Clear(Color.White);
        _x = 0;
        _y = 0;
    }

    public void Reset()
    {
        Clear();
        _penColor = Color.Black;
    }

    public void Set(int width, int height)
    {
        if (width <= 0 || height <= 0)
            throw new ArgumentException("Canvas width/height must be positive.");

        _graphics.Dispose();
        _bitmap.Dispose();

        _bitmap = new Bitmap(width, height);
        _graphics = Graphics.FromImage(_bitmap);
        _graphics.Clear(Color.White);

        _x = 0;
        _y = 0;
    }

    public void SetColour(int red, int green, int blue)
    {
        if (red is < 0 or > 255) throw new ArgumentOutOfRangeException(nameof(red));
        if (green is < 0 or > 255) throw new ArgumentOutOfRangeException(nameof(green));
        if (blue is < 0 or > 255) throw new ArgumentOutOfRangeException(nameof(blue));
        _penColor = Color.FromArgb(red, green, blue);
    }

    public void MoveTo(int x, int y)
    {
        _x = x;
        _y = y;
    }

    public void DrawTo(int x, int y)
    {
        using var p = new Pen(_penColor);
        _graphics.DrawLine(p, _x, _y, x, y);
        _x = x;
        _y = y;
    }

    public void Circle(int radius, bool filled)
    {
        if (radius < 0) throw new ArgumentOutOfRangeException(nameof(radius));
        var rect = new Rectangle(_x - radius, _y - radius, radius * 2, radius * 2);

        if (filled)
        {
            using var b = new SolidBrush(_penColor);
            _graphics.FillEllipse(b, rect);
        }
        else
        {
            using var p = new Pen(_penColor);
            _graphics.DrawEllipse(p, rect);
        }
    }

    public void Rect(int width, int height, bool filled)
    {
        if (width < 0) throw new ArgumentOutOfRangeException(nameof(width));
        if (height < 0) throw new ArgumentOutOfRangeException(nameof(height));
        var rect = new Rectangle(_x, _y, width, height);

        if (filled)
        {
            using var b = new SolidBrush(_penColor);
            _graphics.FillRectangle(b, rect);
        }
        else
        {
            using var p = new Pen(_penColor);
            _graphics.DrawRectangle(p, rect);
        }
    }

    public void Tri(int width, int height)
    {
        // Simple right-angled triangle from current position.
        var p1 = new Point(_x, _y);
        var p2 = new Point(_x + width, _y);
        var p3 = new Point(_x, _y + height);
        using var p = new Pen(_penColor);
        _graphics.DrawPolygon(p, new[] { p1, p2, p3 });
    }

    public void WriteText(string text)
    {
        using var b = new SolidBrush(_penColor);
        _graphics.DrawString(text, SystemFonts.DefaultFont, b, _x, _y);
    }

    public void SaveImage(string fileName, ImageFormat format)
    {
        _bitmap.Save(fileName, format);
    }

    public void LoadImage(string fileName)
    {
        _graphics.Dispose();
        _bitmap.Dispose();

        _bitmap = new Bitmap(fileName);
        _graphics = Graphics.FromImage(_bitmap);
        _x = 0;
        _y = 0;
    }

    public object getBitmap() => _bitmap;

    public void Dispose()
    {
        _graphics.Dispose();
        _bitmap.Dispose();
    }

    // Convenience helper for CLI
    public void SavePng(string path) => SaveImage(path, ImageFormat.Png);
}
