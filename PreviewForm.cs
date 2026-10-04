using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

public class PreviewForm : Form
{
    private readonly PictureBox previewBox;
    private readonly System.Windows.Forms.Timer refreshTimer;
    private readonly ComboBox windowList;
    private readonly Button sendButton;
    private readonly Button bringBackButton;

    public PreviewForm()
    {
        Text = "SimpleCast — Display 2 Preview";

        Width = 700;
        Height = 500;

        StartPosition = FormStartPosition.CenterScreen;

        windowList = new ComboBox
        {
            Dock = DockStyle.Top,
            DropDownStyle = ComboBoxStyle.DropDownList,
            Height = 30
        };

        sendButton = new Button
        {
            Text = "Send to Display 2",
            Dock = DockStyle.Top,
            Height = 35
        };

        bringBackButton = new Button
        {
            Text = "Bring Back to Display 1",
            Dock = DockStyle.Top,
            Height = 35
        };

        sendButton.Click += SendButton_Click;
        bringBackButton.Click += BringBackButton_Click;

        previewBox = new PictureBox
        {
            Dock = DockStyle.Fill,
            BackColor = Color.Black,
            SizeMode = PictureBoxSizeMode.Zoom
        };

        previewBox.MouseClick += PreviewBox_MouseClick;

        Controls.Add(previewBox);
        Controls.Add(bringBackButton);
        Controls.Add(sendButton);
        Controls.Add(windowList);

        RefreshWindowList();

        refreshTimer = new System.Windows.Forms.Timer
        {
            Interval = 500
        };

        refreshTimer.Tick += RefreshPreview;
        refreshTimer.Start();
    }

    private void RefreshWindowList()
    {
        windowList.Items.Clear();

        var windows = WindowFinder.GetWindows();

        foreach (var window in windows)
        {
            windowList.Items.Add(window);
        }

        if (windowList.Items.Count > 0)
        {
            windowList.SelectedIndex = 0;
        }
    }

    private void SendButton_Click(
        object? sender,
        EventArgs e
    )
    {
        if (windowList.SelectedItem
            is not WindowFinder.WindowInfo selectedWindow)
        {
            return;
        }

        Screen? display2 = MonitorFinder.SelectedScreen;

if (display2 == null)
{
    MessageBox.Show("No casting display was selected.");
    return;
}

        WindowFinder.MoveWindowToMonitor(
            selectedWindow.Handle,
            display2.Bounds
        );
    }

    private void BringBackButton_Click(
        object? sender,
        EventArgs e
    )
    {
        if (windowList.SelectedItem
            is not WindowFinder.WindowInfo selectedWindow)
        {
            return;
        }

        Screen display1 = Screen.PrimaryScreen!;

        WindowFinder.MoveWindowToMonitor(
            selectedWindow.Handle,
            display1.WorkingArea
        );
    }

        private void PreviewBox_MouseClick(
        object? sender,
        MouseEventArgs e
    )
    {
        Screen? display2 = MonitorFinder.SelectedScreen;

if (display2 == null)
    return;

        if (previewBox.Image == null)
            return;

        float imageRatio =
            (float)previewBox.Image.Width /
            previewBox.Image.Height;

        float boxRatio =
            (float)previewBox.ClientSize.Width /
            previewBox.ClientSize.Height;

        int shownWidth;
        int shownHeight;
        int offsetX;
        int offsetY;

        if (imageRatio > boxRatio)
        {
            shownWidth = previewBox.ClientSize.Width;

            shownHeight =
                (int)(shownWidth / imageRatio);

            offsetX = 0;

            offsetY =
                (previewBox.ClientSize.Height -
                shownHeight) / 2;
        }
        else
        {
            shownHeight = previewBox.ClientSize.Height;

            shownWidth =
                (int)(shownHeight * imageRatio);

            offsetX =
                (previewBox.ClientSize.Width -
                shownWidth) / 2;

            offsetY = 0;
        }

        if (
            e.X < offsetX ||
            e.X >= offsetX + shownWidth ||
            e.Y < offsetY ||
            e.Y >= offsetY + shownHeight
        )
        {
            return;
        }

        int x =
            display2.Bounds.Left +
            (e.X - offsetX) *
            display2.Bounds.Width /
            shownWidth;

        int y =
            display2.Bounds.Top +
            (e.Y - offsetY) *
            display2.Bounds.Height /
            shownHeight;

        Cursor.Position = new Point(x, y);

        MouseInput.LeftClick();
    }

    private void RefreshPreview(
        object? sender,
        EventArgs e
    )
    {
        byte[]? data =
            GraphicsCapture.GetLatestPng();

        if (data == null)
            return;

        try
        {
            using MemoryStream stream =
                new MemoryStream(data);

            using Image temporary =
                Image.FromStream(stream);

            Image newImage =
                new Bitmap(temporary);

            Image? oldImage =
                previewBox.Image;

            previewBox.Image = newImage;

            oldImage?.Dispose();
        }
        catch
        {
            // Ignore a frame if it is being updated.
        }
    }

    protected override void OnFormClosed(
        FormClosedEventArgs e
    )
    {
        refreshTimer.Stop();

        previewBox.Image?.Dispose();

        base.OnFormClosed(e);
    }
}