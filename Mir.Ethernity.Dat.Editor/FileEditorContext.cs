namespace Mir.Ethernity.Dat.Editor
{
    public class FileEditorContext
    {
        public string? FilePath { get; set; } = null;
        public string? OriginalContent { get; set; } = null;
        public string Content { get; set; } = string.Empty;
        public string FileName => FilePath != null ? System.IO.Path.GetFileName(FilePath) : "New file.dat";
        public bool IsNew => FilePath == null;
        public bool IsModified => IsNew || OriginalContent == null || Content != OriginalContent;
    }
}
