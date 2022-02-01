#nullable enable
namespace ModelPainter.Util;

public class FileWatcher
{
	private PollingFileSystemWatcher _watcher;
	private string _filename;

	public event EventHandler<FileStream> FileChanged;

	public void Watch(string filename)
	{
		_filename = Path.GetFullPath(filename);
		// Create a new FileSystemWatcher and set its properties.
		_watcher?.Dispose();
		_watcher = new PollingFileSystemWatcher(Path.GetDirectoryName(filename), Path.GetFileName(filename))
		{
			PollingInterval = 500
		};

		_watcher.Changed += OnChanged;
		_watcher.Start();

		WaitAndNotify(filename);
	}

	private static FileStream WaitForFile(string fullPath, FileMode mode, FileAccess access, FileShare share)
	{
		for (var numTries = 0; numTries < 10; numTries++)
		{
			FileStream fs = null;
			try
			{
				fs = new FileStream(fullPath, mode, access, share);
				return fs;
			}
			catch (IOException)
			{
				fs?.Dispose();
				Thread.Sleep(50);
			}
		}

		return null;
	}

	private void OnChanged(object? sender, EventArgs eventArgs)
	{
		WaitAndNotify(_filename);
	}

	private void WaitAndNotify(string filename)
	{
		var fs = WaitForFile(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
		FileChanged?.Invoke(this, fs);
	}
}