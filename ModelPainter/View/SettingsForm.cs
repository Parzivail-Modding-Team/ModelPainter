using ModelPainter.Resources;

namespace ModelPainter.View;

public class SettingsForm : Form
{
	public string Filename { get; }

	public SettingsForm(string filename)
	{
		Filename = filename;
		SuspendLayout();

		AutoScaleMode = AutoScaleMode.Font;
		ClientSize = new Size(530, 500);
		Text = "ModelPainter";
		Icon = new Icon(ResourceHelper.GetLocalResource("icon.ico"));

		PropertyGrid pgSettings;

		Controls.Add(pgSettings = new PropertyGrid
		{
			Dock = DockStyle.Fill,
			SelectedObject = ModelPainterSettings.Load(filename)
		});

		Button bSave;
		Button bCancel;

		Controls.Add(new FlowLayoutPanel
		{
			Dock = DockStyle.Bottom,
			FlowDirection = FlowDirection.RightToLeft,
			AutoSize = true,
			Controls =
			{
				(bSave = new Button
				{
					Text = "Save",
					DialogResult = DialogResult.OK
				}),
				(bCancel = new Button
				{
					Text = "Cancel",
					DialogResult = DialogResult.Cancel
				})
			}
		});

		bSave.Click += (sender, args) =>
		{
			if (pgSettings.SelectedObject is not ModelPainterSettings po)
				return;

			po.Save(filename);

			Close();
		};

		bCancel.Click += (sender, args) => Close();

		ResumeLayout(true);
	}
}