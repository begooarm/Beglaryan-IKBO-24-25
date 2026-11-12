namespace practic1.models.VFS;

public class Folder : VFSElement
{
	public Folder(string title, Folder? parent) : base(title, parent)
	{
		elementType = ElementType.Folder;
	}
}
