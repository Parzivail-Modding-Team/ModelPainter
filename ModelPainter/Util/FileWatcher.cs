namespace ModelPainter.Util;

public class FileWatcher
{
	private FileSystemWatcher _watcher;

	public event EventHandler<FileStream> FileChanged;

	public void Watch(string filename)
	{
		filename = Path.GetFullPath(filename);
		// Create a new FileSystemWatcher and set its properties.
		_watcher?.Dispose();
		_watcher = new FileSystemWatcher
		{
			Path = Path.GetDirectoryName(filename),
			NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Attributes | NotifyFilters.FileName,
			Filter = Path.GetFileName(filename)
		};

		// Add event handlers.
		_watcher.Changed += OnChanged;

		// Begin watching.
		_watcher.EnableRaisingEvents = true;
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

	private void OnChanged(object sender, FileSystemEventArgs e)
	{
		Console.WriteLine("OnChanged");
		WaitAndNotify(e.FullPath);
	}

	private void WaitAndNotify(string filename)
	{
		var fs = WaitForFile(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
		FileChanged?.Invoke(this, fs);
	}
}