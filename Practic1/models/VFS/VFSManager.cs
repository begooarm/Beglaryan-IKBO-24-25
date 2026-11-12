namespace practic1.models.VFS;

public class VFSManager
{
	public List<VFSElement> allElements = [];
	public Folder root = new("", null);
	public Folder currentFolder;

	public VFSManager()
	{
		currentFolder = root;
		allElements.Add(root);
	}

	public void Load(string path)
	{
		allElements.Clear();
		root = new Folder("", null);

		currentFolder = root;
		allElements.Add(root);

		LoadDirectory(path, root);
	}

	private void LoadDirectory(string path, Folder parent)
	{
		foreach (string directory in Directory.GetDirectories(path))
		{
			Folder folder = new(Path.GetFileName(directory), parent);
			allElements.Add(folder);
			LoadDirectory(directory, folder);
		}

		foreach (string filePath in Directory.GetFiles(path))
		{
			MyFile file = new(Path.GetFileName(filePath), parent);
			file.content = System.IO.File.ReadAllBytes(filePath);
			allElements.Add(file);
		}
	}

	public void CreateFolder(string title, Folder parent)
	{
		Folder newFolder = new(title, parent);
		allElements.Add(newFolder);
	}

	public void CreateFile(string title, Folder parent)
	{
		MyFile newFile = new(title, parent);
		allElements.Add(newFile);
	}

	public List<VFSElement> GetElementsByParent(string directoryName)
	{
		Folder folder = GetFolder(directoryName);
		List<VFSElement> result = GetElementsByParent(folder);
		return result;
	}

	public List<VFSElement> GetElementsByParent(Folder parent)
	{
		List<VFSElement> result = [];

		foreach (VFSElement element in allElements)
		{
			if (element.parentFolder == parent)
				result.Add(element);
		}

		return result;
	}

	public VFSElement GetElement(string path)
	{
		if (path[0] != '/')
			path = currentFolder.GetPath().TrimEnd('/') + "/" + path;

		path = path.TrimEnd('/');

		foreach (VFSElement element in allElements)
		{
			string elementPath = element.GetPath().TrimEnd('/');

			if (elementPath == path)
				return element;

			if (element.elementType == ElementType.File && path.StartsWith(elementPath + "/", StringComparison.Ordinal))
				throw new InvalidCastException("Not a directory");
		}

		throw new InvalidOperationException("No such file or directory");
	}

	public Folder GetFolder(string path)
	{
		if (GetElement(path) is Folder folder)
			return folder;

		throw new InvalidCastException("Not a directory");
	}

	public void DisplayDirectory(string directoryName)
	{
		Folder folder = GetFolder(directoryName);
		Console.WriteLine(folder.GetPath());

		List<VFSElement> objectList = GetElementsByParent(folder);
		foreach (VFSElement element in objectList)
		{
			Console.WriteLine($"{element.elementType}:    {element.title}");
		}
	}
}
