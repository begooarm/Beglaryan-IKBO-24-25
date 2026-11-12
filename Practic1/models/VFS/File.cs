namespace practic1.models.VFS;

public class MyFile : VFSElement
{
	public byte[] content = [];

	public MyFile(string title, Folder? parent) : base(title, parent)
	{
		elementType = ElementType.File;
	}
}
