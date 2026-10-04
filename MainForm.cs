using System;
using System.Drawing;
using System.Windows.Forms;

public class MainForm : Form
{
    private ComboBox windowList;
    private Button refreshButton;
    private Button startButton;
    private Label statusLabel;
    private Label addressLabel;

    public MainForm()
    {
        Text = "SimpleCast";

        Width = 520;
        Height = 300;

        StartPosition = FormStartPosition.CenterScreen;

        Label title = new Label
        {
            Text = "SimpleCast",
            Font = new Font("Segoe UI", 20, FontStyle.Bold),
            Left = 25,
            Top = 20,
            AutoSize = true
        };

        Label selectLabel = new Label
        {
            Text = "Window to cast:",
            Left = 25,
            Top = 80,
            AutoSize = true
        };

        windowList = new ComboBox
        {
            Left = 25,
            Top = 105,
            Width = 350,
            DropDownStyle = ComboBoxStyle.DropDownList
        };

        refreshButton = new Button
        {
            Text = "Refresh",
            Left = 385,
            Top = 104,
            Width = 85
        };

refreshButton.Click += RefreshButton_Click;

        startButton = new Button
        {
            Text = "Start Casting",
            Left = 25,
            Top = 155,
            Width = 140,
            Height = 35
        };

        statusLabel = new Label
        {
            Text = "Not casting",
            Left = 185,
            Top = 165,
            AutoSize = true
        };

        addressLabel = new Label
        {
            Text = "Receiver address will appear here",
            Left = 25,
            Top = 215,
            AutoSize = true
        };

        Controls.Add(title);
        Controls.Add(selectLabel);
        Controls.Add(windowList);
        Controls.Add(refreshButton);
        Controls.Add(startButton);
        Controls.Add(statusLabel);
        Controls.Add(addressLabel);
    }
    
    private void RefreshButton_Click(object? sender, EventArgs e)
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

}