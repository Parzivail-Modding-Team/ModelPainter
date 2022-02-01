using ModelPainter.Controls;
using ModelPainter.Util;
using OpenTK;
using OpenTK.Graphics;

namespace ModelPainter.View;

public partial class PainterForm : Form
{
	private const string SETTINGS_FILENAME = "settings.json";

	private readonly ToolStripLabel _statusForgeInfo;

	private readonly ToolStripMenuItem _bOpen;
	private readonly ToolStripMenuItem _bGenerateFilelist;
	private readonly ToolStripMenuItem _bReset3dViewport;
	private readonly ToolStripMenuItem _bReset2dViewport;

	private readonly MinimalSplitContainer _splitContainer;
	private readonly GLControl _modelControl;
	private readonly GLControl _imageControl;

	private ToolStripMenuItem _bEditSettings;

	private ModelPainterSettings _settings;

	private FileWatcher _modelWatcher = new();
	private FileWatcher _imageWatcher = new();

	public PainterForm()
	{
		SuspendLayout();

		AutoScaleMode = AutoScaleMode.Font;
		ClientSize = new Size(800, 450);
		Text = "ModelPainter";

		_settings = ModelPainterSettings.Load(SETTINGS_FILENAME);

		Controls.Add(_splitContainer = new MinimalSplitContainer
		{
			Dock = DockStyle.Fill,
			Panel1 =
			{
				Controls =
				{
					(_modelControl = new GLControl(new GraphicsMode(new ColorFormat(8), 24, 8, 1))
					{
						Dock = DockStyle.Fill
					}),
				}
			},
			Panel2 =
			{
				Controls =
				{
					(_imageControl = new GLControl(new GraphicsMode(new ColorFormat(8), 24, 8, 1))
					{
						Dock = DockStyle.Fill
					})
				}
			}
		});

		Controls.Add(new MenuStrip
		{
			Dock = DockStyle.Top,
			Renderer = new FlatToolStripRenderer(),

			Items =
			{
				new ToolStripDropDownButton
				{
					Text = "&File",
					DropDownItems =
					{
						(_bOpen = new ToolStripMenuItem("&Open")
						{
							ShortcutKeys = Keys.Control | Keys.O
						}),
						new ToolStripSeparator(),
						(_bEditSettings = new ToolStripMenuItem("&Settings")
						{
							ShortcutKeys = Keys.Control | Keys.Oemcomma,
							ShortcutKeyDisplayString = "Ctrl+,"
						}),
					}
				},
				new ToolStripDropDownButton
				{
					Text = "&View",
					DropDownItems =
					{
						(_bReset3dViewport = new ToolStripMenuItem("&Reset 3D Viewport")),
						(_bReset2dViewport = new ToolStripMenuItem("&Reset 2D Viewport")),
					}
				}
			}
		});

		Controls.Add(new StatusStrip
		{
			Dock = DockStyle.Bottom,
			Items =
			{
				(_statusForgeInfo = new ToolStripLabel("Ready"))
			}
		});

		ResumeLayout(false);
		PerformLayout();

		_bOpen.Click += (sender, args) =>
		{
			using var ofd = new OpenFileDialog
			{
				Filter = "Models and Textures|*.obj;*.dcm;*.nem;*.tbl;*.json;*.png"
			};

			if (ofd.ShowDialog() != DialogResult.OK)
				return;

			Open(ofd.FileName);
		};

		_bEditSettings.Click += (sender, args) =>
		{
			if (new SettingsForm(SETTINGS_FILENAME).ShowDialog(this) == DialogResult.OK)
				_settings = ModelPainterSettings.Load(SETTINGS_FILENAME);
		};

		_bReset3dViewport.Click += (sender, args) => _renderer3d.ResetView();
		_bReset2dViewport.Click += (sender, args) => _renderer2d.ResetView();

		_splitContainer.SplitterDistance = _splitContainer.Width / 2;

		SetupRenderer();

		UpdateAbility();
	}

	private void UpdateAbility()
	{
	}

	private void Open(string filename)
	{
		switch (Path.GetExtension(filename))
		{
			case ".png":
			{
				_imageWatcher.Watch(filename);
				break;
			}
		}
	}
}