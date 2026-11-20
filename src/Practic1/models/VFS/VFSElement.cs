namespace Practic1.Models.VFS;

public abstract class VFSElement
{
	public string title;
	public Folder? parentFolder = null;
	public ElementType elementType;

	public VFSElement(string title, Folder? parent)
	{
		this.title = title;
		parentFolder = parent;
	}

	public string GetPath()
	{
		Folder? lastFolder = parentFolder;
		string result = title;

		while (lastFolder != null)
		{
			if (lastFolder.title != "")
				result = lastFolder.title + "/" + result;

			lastFolder = lastFolder.parentFolder;
		}

		return "/" + result;
	}

	public override string ToString() { return title; }
}
