using BOOSE;
using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace booseapp
{
    /// <summary>
    /// Canvas implementation used for drawing shapes in the BOOSE app.
    /// Stores the current pen position, pen colour, and an in-memory bitmap surface.
    /// </summary>
    public class CommandCanvas : ICanvas, IDisposable
    {
        private int _x;
        private int _y;
        private Color _penColor = Color.Black;

        private Bitmap _bitmap;
        private Graphics _graphics;
        private bool _disposed;

        // Default size (keeps older code happy: new CommandCanvas())
        public CommandCanvas() : this(640, 480) { }

        // Constructor that tests usually require: new CommandCanvas(width, height)
        public CommandCanvas(int width, int height)
        {
            if (width < 1) throw new ArgumentOutOfRangeException(nameof(width));
            if (height < 1) throw new ArgumentOutOfRangeException(nameof(height));

            _bitmap = new Bitmap(width, height);
            _graphics = Graphics.FromImage(_bitmap);
            _graphics.Clear(Color.White);

            _x = 0;
            _y = 0;
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

        // BOOSE interface uses object; internally we store Color
        public object PenColour
        {
            get => _penColor;
            set
            {
                ThrowIfDisposed();
                if (value is Color c)
                {
                    _penColor = c;
                }
                // else ignore (keeps it safe and compatible)
            }
        }

        public void Clear()
        {
            ThrowIfDisposed();
            _graphics.Clear(Color.White);
        }

        public void Reset()
        {
            ThrowIfDisposed();
            Clear();
            _x = 0;
            _y = 0;
            _penColor = Color.Black;
        }

        public void Set(int width, int height)
        {
            ThrowIfDisposed();
            if (width < 1) throw new ArgumentOutOfRangeException(nameof(width));
            if (height < 1) throw new ArgumentOutOfRangeException(nameof(height));

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
            ThrowIfDisposed();
            if (red < 0 || red > 255) throw new ArgumentOutOfRangeException(nameof(red));
            if (green < 0 || green > 255) throw new ArgumentOutOfRangeException(nameof(green));
            if (blue < 0 || blue > 255) throw new ArgumentOutOfRangeException(nameof(blue));

            _penColor = Color.FromArgb(red, green, blue);
        }

        public void MoveTo(int x, int y)
        {
            ThrowIfDisposed();
            _x = x;
            _y = y;
        }

        public void DrawTo(int x, int y)
        {
            ThrowIfDisposed();
            using (var p = new Pen(_penColor))
            {
                _graphics.DrawLine(p, _x, _y, x, y);
            }
            _x = x;
            _y = y;
        }

        public void Circle(int radius, bool filled)
        {
            ThrowIfDisposed();
            if (radius < 0) throw new ArgumentOutOfRangeException(nameof(radius));

            var rect = new Rectangle(_x - radius, _y - radius, radius * 2, radius * 2);

            if (filled)
            {
                using (var b = new SolidBrush(_penColor))
                {
                    _graphics.FillEllipse(b, rect);
                }
            }
            else
            {
                using (var p = new Pen(_penColor))
                {
                    _graphics.DrawEllipse(p, rect);
                }
            }
        }

        public void Rect(int width, int height, bool filled)
        {
            ThrowIfDisposed();
            if (width < 0) throw new ArgumentOutOfRangeException(nameof(width));
            if (height < 0) throw new ArgumentOutOfRangeException(nameof(height));

            var rect = new Rectangle(_x, _y, width, height);

            if (filled)
            {
                using (var b = new SolidBrush(_penColor))
                {
                    _graphics.FillRectangle(b, rect);
                }
            }
            else
            {
                using (var p = new Pen(_penColor))
                {
                    _graphics.DrawRectangle(p, rect);
                }
            }
        }

        public void Tri(int width, int height)
        {
            ThrowIfDisposed();
            if (width < 0) throw new ArgumentOutOfRangeException(nameof(width));
            if (height < 0) throw new ArgumentOutOfRangeException(nameof(height));

            Point[] pts =
            {
                new Point(_x, _y),
                new Point(_x + width, _y),
                new Point(_x + width / 2, _y - height)
            };

            using (var p = new Pen(_penColor))
            {
                _graphics.DrawPolygon(p, pts);
            }
        }

        public void WriteText(string text)
        {
            ThrowIfDisposed();
            if (text == null) throw new ArgumentNullException(nameof(text));

            using (var b = new SolidBrush(_penColor))
            {
                _graphics.DrawString(text, SystemFonts.DefaultFont, b, _x, _y);
            }
        }

        // Useful for your UI buttons
        public void SaveImage(string fileName, ImageFormat format)
        {
            ThrowIfDisposed();
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException("Invalid file name.", nameof(fileName));
            if (format == null)
                throw new ArgumentNullException(nameof(format));

            _bitmap.Save(fileName, format);
        }

        public void LoadImage(string fileName)
        {
            ThrowIfDisposed();
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException("Invalid file name.", nameof(fileName));

            _graphics.Dispose();
            _bitmap.Dispose();

            _bitmap = new Bitmap(fileName);
            _graphics = Graphics.FromImage(_bitmap);

            _x = 0;
            _y = 0;
        }

        // BOOSE templates often expect this exact lowercase name
        public object getBitmap()
        {
            ThrowIfDisposed();
            return _bitmap;
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            _graphics?.Dispose();
            _bitmap?.Dispose();
        }

        private void ThrowIfDisposed()
        {
            if (_disposed) throw new ObjectDisposedException(nameof(CommandCanvas));
        }
    }
}
