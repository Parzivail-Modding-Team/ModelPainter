using ModelPainter.Controls;
using ModelPainter.Model.DCM;
using ModelPainter.Model.NEM;
using ModelPainter.Model.OBJ;
using ModelPainter.Model.P3D;
using ModelPainter.Model.TBL;
using ModelPainter.Model.TCN;
using ModelPainter.Resources;
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

	private readonly ToolStripMenuItem _bGenerateUvMap;

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
		Icon = new Icon(ResourceHelper.GetLocalResource("icon.ico"));
		AllowDrop = true;

		DragEnter += OnDragEnter;
		DragDrop += OnDragDrop;

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
						Dock = DockStyle.Fill,
						VSync = true
					}),
				}
			},
			Panel2 =
			{
				Controls =
				{
					(_imageControl = new GLControl(new GraphicsMode(new ColorFormat(8), 24, 8, 1))
					{
						Dock = DockStyle.Fill,
						VSync = true
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
					Text = "&Tools",
					DropDownItems =
					{
						(_bGenerateUvMap = new ToolStripMenuItem("&Generate UV Map"))
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
				Filter = "Models and Textures|*.p3d;*.dcm;*.nem;*.tbl;*.tcn;*.obj;*.png",
				Multiselect = true
			};

			if (ofd.ShowDialog() != DialogResult.OK)
				return;

			foreach (var name in ofd.FileNames)
				Open(name);
		};

		_bEditSettings.Click += (sender, args) =>
		{
			if (new SettingsForm(SETTINGS_FILENAME).ShowDialog(this) == DialogResult.OK)
			{
				_settings = ModelPainterSettings.Load(SETTINGS_FILENAME);
				OnSettingsChanged();
			}
		};

		_bReset3dViewport.Click += (sender, args) => _renderer3d.ResetView();
		_bReset2dViewport.Click += (sender, args) => _renderer2d.ResetView();

		_bGenerateUvMap.Click += (sender, args) => GenerateUvMap();

		_splitContainer.SplitterDistance = _splitContainer.Width / 2;

		SetupRenderer();

		OnSettingsChanged();
		UpdateAbility();
	}

	private void OnDragDrop(object sender, DragEventArgs e)
	{
		if (e.Data?.GetData(DataFormats.FileDrop) is not string[] filenames)
			return;

		foreach (var filename in filenames)
			Open(filename);
	}

	private void OnDragEnter(object sender, DragEventArgs e)
	{
		if (e.Data != null && e.Data.GetDataPresent(DataFormats.FileDrop))
			e.Effect = DragDropEffects.Copy;
	}

	private void UpdateAbility()
	{
	}

	private void Open(string filename)
	{
		try
		{
			switch (Path.GetExtension(filename))
			{
				case ".png":
				{
					_imageWatcher.Watch(filename);
					break;
				}
				case ".nem":
				{
					var nem = NbtEntityModel.Load(filename);
					LoadModelParts(nem.Parts);
					break;
				}
				case ".dcm":
				{
					var dcm = StudioModel.Load(filename);
					LoadStudioModel(dcm);
					break;
				}
				case ".p3d":
				{
					var p3d = P3dModel.Load(filename);
					LoadP3dModel(p3d);
					break;
				}
				case ".tbl":
				{
					var tbl = TabulaModel.Load(filename);
					LoadTabulaModel(tbl);
					break;
				}
				case ".tcn":
				{
					var tcn = TechneModel.Load(filename);
					LoadTechneModel(tcn);
					break;
				}
				case ".obj":
				{
					var obj = ObjModel.Load(filename);
					LoadObjModel(obj);
					break;
				}
			}
		}
		catch (Exception e)
		{
			MessageBox.Show(this, e.ToString(), "Error while opening file", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
	}
}