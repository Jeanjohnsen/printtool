using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

class ScreenPrinter
{
    [DllImport("user32.dll")]
    static extern bool SetProcessDPIAware();

    [DllImport("user32.dll")]
    static extern IntPtr GetDC(IntPtr hwnd);

    [DllImport("user32.dll")]
    static extern int ReleaseDC(IntPtr hwnd, IntPtr hdc);

    [DllImport("gdi32.dll")]
    static extern IntPtr CreateCompatibleDC(IntPtr hdc);

    [DllImport("gdi32.dll")]
    static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int cx, int cy);

    [DllImport("gdi32.dll")]
    static extern IntPtr SelectObject(IntPtr hdc, IntPtr h);

    [DllImport("gdi32.dll")]
    static extern bool BitBlt(IntPtr hdcDst, int x, int y, int cx, int cy, IntPtr hdcSrc, int x1, int y1, uint rop);

    [DllImport("gdi32.dll")]
    static extern bool DeleteDC(IntPtr hdc);

    [DllImport("gdi32.dll")]
    static extern bool DeleteObject(IntPtr ho);

    const uint SRCCOPY    = 0x00CC0020;
    const uint CAPTUREBLT = 0x40000000;

    [STAThread]
    static void Main()
    {
        SetProcessDPIAware();

        Point cursor = Cursor.Position;
        Screen screen = Screen.FromPoint(cursor);
        Rectangle bounds = screen.Bounds;

        // Brug BitBlt med CAPTUREBLT så hardware-accelererede lag også fanges
        IntPtr screenDC = GetDC(IntPtr.Zero);
        IntPtr memDC    = CreateCompatibleDC(screenDC);
        IntPtr hBmp     = CreateCompatibleBitmap(screenDC, bounds.Width, bounds.Height);
        IntPtr old      = SelectObject(memDC, hBmp);

        BitBlt(memDC, 0, 0, bounds.Width, bounds.Height, screenDC, bounds.Left, bounds.Top, SRCCOPY | CAPTUREBLT);

        SelectObject(memDC, old);
        DeleteDC(memDC);
        ReleaseDC(IntPtr.Zero, screenDC);

        using Bitmap bmp = Image.FromHbitmap(hBmp);
        DeleteObject(hBmp);

        // Print screenshot skaleret til siden
        using PrintDocument pd = new PrintDocument();

        pd.DefaultPageSettings.Landscape = bmp.Width > bmp.Height;
        pd.OriginAtMargins = true;
        pd.DefaultPageSettings.Margins = new Margins(25, 25, 25, 25);

        pd.PrintPage += (sender, e) =>
        {
            Rectangle target = e.MarginBounds;

            float scaleX = (float)target.Width / bmp.Width;
            float scaleY = (float)target.Height / bmp.Height;
            float scale = Math.Min(scaleX, scaleY);

            int drawWidth = (int)(bmp.Width * scale);
            int drawHeight = (int)(bmp.Height * scale);

            int drawX = target.Left + (target.Width - drawWidth) / 2;
            int drawY = target.Top + (target.Height - drawHeight) / 2;

            e.Graphics.DrawImage(bmp, drawX, drawY, drawWidth, drawHeight);
            e.HasMorePages = false;
        };

        pd.Print();
    }
}