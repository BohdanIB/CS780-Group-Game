using Godot;
using System;

public partial class OptionSelector : Panel
{
    [Export] private TextureButton leftButton, rightButton;
    [Export] private Label selectionLabel, nameLabel;

    private string[] options;
    public int currentIndex;

    [Signal] public delegate void OnIndexChangedEventHandler(int index);


    public void SetOptions(string[] options, string name, int index=0)
    {
        this.options = options;
        nameLabel.Text = name;
        index = Math.Max(0, Math.Min(options.Length-1, index));

        if (options != null && options.Length > 0)
        {
            Visible = true;
            UpdateDisplay();
        } else
        {
            Visible = false;
        }
        
    }

    private void UpdateDisplay()
    {
        selectionLabel.Text = options[currentIndex];
        leftButton.Visible = currentIndex > 0;
        rightButton.Visible = currentIndex < options.Length-1;
    }

    public void IncreaseIndex()
    {
        currentIndex = Math.Min(options.Length-1, currentIndex+1);
        EmitSignal(SignalName.OnIndexChanged, currentIndex);
        UpdateDisplay();
    }

    public void DecreaseIndex()
    {
        currentIndex = Math.Max(0, currentIndex-1);
        EmitSignal(SignalName.OnIndexChanged, currentIndex);
        UpdateDisplay();
    }
}
