using System.Collections.Generic;

namespace Acryl.Engine.Graphics.Font
{
    public class FontFormat
    {
        /// <summary>
        /// Gets the name for the format.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the typical file extension for this format (lowercase).
        /// </summary>
        public string FileExtension { get; }

        // ...
        public FontFormat(string name, string ext)
        {
            if (!ext.StartsWith(".")) ext = "." + ext;
            Name = name; FileExtension = ext;
        }
    }
    
    internal class FontFormatCollection : Dictionary<string, FontFormat>
    {
        public void Add(string name, string ext)
        {
            if (!ext.StartsWith(".")) ext = "." + ext;
            Add(ext, new FontFormat(name, ext));
        }

        public bool ContainsExt(string ext)
        {
            return ContainsKey(ext);
        }
    }
}