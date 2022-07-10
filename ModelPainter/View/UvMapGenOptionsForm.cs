using ModelPainterCore.Util;

namespace ModelPainter.View;

public class UvMapGenOptionsForm : Form
{
	public UvMapGenOptionsForm(UvMapGenOptions genOptions)
	{
		SuspendLayout();

		AutoScaleMode = AutoScaleMode.Font;
		ClientSize = new Size(530, 500);
		Text = "ModelPainter";
		Icon = new Icon(ResourceHelper.GetLocalResource("icon.ico"));

		PropertyGrid pgSettings;

		Controls.Add(pgSettings = new PropertyGrid
		{
			Dock = DockStyle.Fill,
			SelectedObject = genOptions
		});

		Button bOk;
		Button bCancel;

		Controls.Add(new FlowLayoutPanel
		{
			Dock = DockStyle.Bottom,
			FlowDirection = FlowDirection.RightToLeft,
			AutoSize = true,
			Controls =
			{
				(bOk = new Button
				{
					Text = "OK",
					DialogResult = DialogResult.OK
				}),
				(bCancel = new Button
				{
					Text = "Cancel",
					DialogResult = DialogResult.Cancel
				})
			}
		});

		bOk.Click += (sender, args) => Close();
		bCancel.Click += (sender, args) => Close();

		ResumeLayout(true);
	}
}