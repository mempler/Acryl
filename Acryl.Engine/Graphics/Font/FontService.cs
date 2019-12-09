using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using Acryl.Engine.Graphics.Extension;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpFont;
using Color = System.Drawing.Color;

namespace Acryl.Engine.Graphics.Font
{
	// https://github.com/Robmaister/SharpFont/blob/master/Source/Examples/FontService.cs
	internal class FontService : DependencyContainer, IDisposable
	{
		private Library _lib;

		#region Dependencies

		[DependencyResolved]
		private GraphicsDevice GraphicsDevice { get; set; }
		
		#endregion
		
		#region Properties
		internal Face FontFace
		{
			get => _fontFace;
			set => SetFont(value);
		}
		private Face _fontFace;

		internal float Size
		{
			get => _size;
			set => SetSize(value);
		}
		private float _size;

		internal FontFormatCollection SupportedFormats { get; }

		#endregion // Properties

		#region Constructor

		/// <summary>
		/// If multithreading, each thread should have its own FontService.
		/// </summary>
		internal FontService(Library lib)
		{
			_lib = lib;
			_size = 8.25f;
			SupportedFormats = new FontFormatCollection();
			AddFormat("TrueType", "ttf");
			AddFormat("OpenType", "otf");
			// Not so sure about these...
			//AddFormat("TrueType Collection", "ttc");
			//AddFormat("Type 1", "pfa"); // pfb?
			//AddFormat("PostScript", "pfm"); // ext?
			//AddFormat("FNT", "fnt");
			//AddFormat("X11 PCF", "pcf");
			//AddFormat("BDF", "bdf");
			//AddFormat("Type 42", "");
		}

		private void AddFormat(string name, string ext)
		{
			SupportedFormats.Add(name, ext);
		}

		#endregion

		#region Setters

		internal void SetFont(Face face)
		{
			_fontFace = face;
			SetSize(Size);
		}

		internal void SetFont(string filename)
		{
			FontFace = new Face(_lib, filename);
			SetSize(Size);
		}

		internal void SetSize(float size)
		{
			_size = size;
			FontFace?.SetCharSize(0, size, 0, 96);
		}

		#endregion // Setters

		#region FileEnumeration

		internal IEnumerable<FileInfo> GetFontFiles(DirectoryInfo folder, bool recurse)
		{
			var option = recurse ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
			return folder.GetFiles("*.*", option).Where(file => SupportedFormats.ContainsExt(file.Extension)).ToList();
		}

		#endregion // FileEnumeration

		#region RenderString

		/// <summary>
		/// Render the string into a bitmap with <see cref="SystemColors.ControlText"/> text color and a transparent background.
		/// </summary>
		/// <param name="text">The string to render.</param>
		internal virtual Texture2D RenderString(string text)
		{
			try
			{
				return RenderString(FontFace, text, Color.White, Color.Transparent);
			}
			catch
			{
				// Ignored
			}
			return null;
		}

		/// <summary>
		/// Render the string into a bitmap with a transparent background.
		/// </summary>
		/// <param name="text">The string to render.</param>
		/// <param name="foreColor">The color of the text.</param>
		/// <returns></returns>
		internal virtual Texture2D RenderString(string text, Color foreColor)
		{
			return RenderString(FontFace, text, foreColor, Color.Transparent);
		}

		/// <summary>
		/// Render the string into a bitmap with an opaque background.
		/// </summary>
		/// <param name="text">The string to render.</param>
		/// <param name="foreColor">The color of the text.</param>
		/// <param name="backColor">The color of the background behind the text.</param>
		/// <returns></returns>
		internal virtual Texture2D RenderString(string text, Color foreColor, Color backColor)
		{
			return RenderString(FontFace, text, foreColor, backColor);
		}

		internal Texture2D RenderString(Face face, string text, Color foreColor, Color backColor)
		{
			var measuredChars = new List<DebugChar>();
			var renderedChars = new List<DebugChar>();
			var pen = new Vector2();
			var stringSize = new SizeF();
			float overrun = 0;
			float underrun = 0;
			float kern;
			var spacingError = 0;
			var trackingUnderrun = true;
			var rightEdge = 0; // tracking rendered right side for debugging

			// Bottom and top are both positive for simplicity.
			// Drawing in .Net has 0,0 at the top left corner, with positive X to the right
			// and positive Y downward.
			// Glyph metrics have an origin typically on the left side and at baseline
			// of the visual data, but can draw parts of the glyph in any quadrant, and
			// even move the origin (via kerning).
			float top = 0, bottom = 0;

			// Measure the size of the string before rendering it. We need to do this so
			// we can create the proper size of bitmap (canvas) to draw the characters on.
			for (var i = 0; i < text.Length; i++)
			{
				#region Load character
				var c = text[i];

				// Look up the glyph index for this character.
				var glyphIndex = face.GetCharIndex(c);

				// Load the glyph into the font's glyph slot. There is usually only one slot in the font.
				face.LoadGlyph(glyphIndex, LoadFlags.Default, LoadTarget.Normal);
				
				// Refer to the diagram entitled "Glyph Metrics" at http://www.freetype.org/freetype2/docs/tutorial/step2.html.
				// There is also a glyph diagram included in this example (glyph-dims.svg).
				// The metrics below are for the glyph loaded in the slot.
				var gAdvanceX = (float)face.Glyph.Advance.X; // same as the advance in metrics
				var gBearingX = (float)face.Glyph.Metrics.HorizontalBearingX;

				var gWidth = face.Glyph.Metrics.Width.ToSingle();
				var rc = new DebugChar(c, gAdvanceX, gBearingX, gWidth);
				#endregion
				#region Underrun
				// Negative bearing would cause clipping of the first character
				// at the left boundary, if not accounted for.
				// A positive bearing would cause empty space.
				underrun += -gBearingX;
				if (stringSize.Width <= 0)
					stringSize.Width += underrun;
				if (trackingUnderrun)
					rc.Underrun = underrun;
				if (trackingUnderrun && underrun <= 0)
				{
					underrun = 0;
					trackingUnderrun = false;
				}
				#endregion
				#region Overrun
				// Accumulate overrun, which coould cause clipping at the right side of characters near
				// the end of the string (typically affects fonts with slanted characters)
				if (gBearingX + gWidth > 0 || gAdvanceX > 0)
				{
					overrun -= Math.Max(gBearingX + gWidth, gAdvanceX);
					if (overrun <= 0) overrun = 0;
				}
				overrun += gBearingX <= 0 && gWidth <= 0 ? 0 : gBearingX + gWidth - gAdvanceX;
				// On the last character, apply whatever overrun we have to the overall width.
				// Positive overrun prevents clipping, negative overrun prevents extra space.
				if (i == text.Length - 1)
					stringSize.Width += overrun;
				rc.Overrun = overrun; // accumulating (per above)
				#endregion

				#region Top/Bottom
				// If this character goes higher or lower than any previous character, adjust
				// the overall height of the bitmap.
				var glyphTop = (float)face.Glyph.Metrics.HorizontalBearingY;
				var glyphBottom = (float)(face.Glyph.Metrics.Height - face.Glyph.Metrics.HorizontalBearingY);
				if (glyphTop > top)
					top = glyphTop;
				if (glyphBottom > bottom)
					bottom = glyphBottom;
				#endregion

				// Accumulate the distance between the origin of each character (simple width).
				stringSize.Width += gAdvanceX;
				rc.RightEdge = stringSize.Width;
				measuredChars.Add(rc);

				#region Kerning (for NEXT character)
				// Calculate kern for the NEXT character (if any)
				// The kern value adjusts the origin of the next character (positive or negative).
				if (!face.HasKerning || i >= text.Length - 1)
					continue;
				
				var cNext = text[i + 1];
				kern = (float)face.GetKerning(glyphIndex, face.GetCharIndex(cNext), KerningMode.Default).X;
				// sanity check for some fonts that have kern way out of whack
				if (kern > gAdvanceX * 5 || kern < -(gAdvanceX * 5))
					kern = 0;
				rc.Kern = kern;
				stringSize.Width += kern;

				#endregion
			}

			stringSize.Height = top + bottom;

			// If any dimension is 0, we can't create a bitmap
			if (stringSize.Width <= 0 || stringSize.Height <= 0)
				return null;

			// Create a new bitmap that fits the string.
			var bmp = new Bitmap((int)Math.Ceiling(stringSize.Width), (int)Math.Ceiling(stringSize.Height));
			trackingUnderrun = true;
			underrun = 0;
			overrun = 0;
			stringSize.Width = 0;
			
			using (var g = System.Drawing.Graphics.FromImage(bmp))
			{
				#region Set up graphics
				// HighQuality and GammaCorrected both specify gamma correction be applied (2.2 in sRGB)
				// https://msdn.microsoft.com/en-us/library/windows/desktop/ms534094(v=vs.85).aspx
				g.CompositingQuality = CompositingQuality.HighQuality;
				// HighQuality and AntiAlias both specify antialiasing
				g.SmoothingMode = SmoothingMode.HighQuality;
				// If a background color is specified, blend over it.
				g.CompositingMode = CompositingMode.SourceOver;

				g.Clear(backColor);
				#endregion

				// Draw the string into the bitmap.
				// A lot of this is a repeat of the measuring steps, but this time we have
				// an actual bitmap to work with (both canvas and bitmaps in the glyph slot).
				for (var i = 0; i < text.Length; i++)
				{
					#region Load character
					var c = text[i];

					// Same as when we were measuring, except RenderGlyph() causes the glyph data
					// to be converted to a bitmap.
					var glyphIndex = face.GetCharIndex(c);
					face.LoadGlyph(glyphIndex, LoadFlags.Default, LoadTarget.Normal);
					face.Glyph.RenderGlyph(RenderMode.Normal);
					var ftbmp = face.Glyph.Bitmap;

					var gAdvanceX = (float)face.Glyph.Advance.X;
					var gBearingX = (float)face.Glyph.Metrics.HorizontalBearingX;
					var gWidth = (float)face.Glyph.Metrics.Width;

					var rc = new DebugChar(c, gAdvanceX, gBearingX, gWidth);
					#endregion
					#region Underrun
					// Underrun
					underrun += -gBearingX;
					if (pen.X <= 0)
						pen.X += underrun;
					if (trackingUnderrun)
						rc.Underrun = underrun;
					if (trackingUnderrun && underrun <= 0)
					{
						underrun = 0;
						trackingUnderrun = false;
					}
					#endregion
					#region Draw glyph
					// Whitespace characters sometimes have a bitmap of zero size, but a non-zero advance.
					// We can't draw a 0-size bitmap, but the pen position will still get advanced (below).
					if (ftbmp.Width > 0 && ftbmp.Rows > 0)
					{
						// Get a bitmap that .Net can draw (GDI+ in this case).

						var cBmp = ftbmp.ToGdipBitmap(foreColor);
						rc.Width = cBmp.Width;
						rc.BearingX = face.Glyph.BitmapLeft;
						var x = (int)Math.Round(pen.X + face.Glyph.BitmapLeft);
						var y = (int)Math.Round(pen.Y + top - (float)face.Glyph.Metrics.HorizontalBearingY);
						//Not using g.DrawImage because some characters come out blurry/clipped. (Is this still true?)
						g.DrawImageUnscaled(cBmp, x, y);
						rc.Overrun = face.Glyph.BitmapLeft + cBmp.Width - gAdvanceX;
						// Check if we are aligned properly on the right edge (for debugging)
						rightEdge = Math.Max(rightEdge, x + cBmp.Width);
					}
					else
					{
						rightEdge = (int)(pen.X + gAdvanceX);
					}
					#endregion

					#region Overrun
					if (gBearingX + gWidth > 0 || gAdvanceX > 0)
					{
						overrun -= Math.Max(gBearingX + gWidth, gAdvanceX);
						if (overrun <= 0) overrun = 0;
					}
					overrun += gBearingX <= 0 && gWidth <= 0 ? 0 : gBearingX + gWidth - gAdvanceX;
					if (i == text.Length - 1) pen.X += overrun;
					rc.Overrun = overrun;
					#endregion

					// Advance pen positions for drawing the next character.
					pen.X += (float)face.Glyph.Advance.X; // same as Metrics.HorizontalAdvance?
					pen.Y += (float)face.Glyph.Advance.Y;

					rc.RightEdge = pen.X;
					spacingError = bmp.Width - (int)Math.Round(rc.RightEdge);
					renderedChars.Add(rc);

					#region Kerning (for NEXT character)
					// Adjust for kerning between this character and the next.
					if (face.HasKerning && i < text.Length - 1)
					{
						var cNext = text[i + 1];
						kern = (float)face.GetKerning(glyphIndex, face.GetCharIndex(cNext), KerningMode.Default).X;
						if (kern > gAdvanceX * 5 || kern < -(gAdvanceX * 5))
							kern = 0;
						rc.Kern = kern;
						pen.X += kern;
					}
					#endregion

				}
			}
			
			var printedHeader = false;
			if (spacingError != 0)
			{
				for (var i = 0; i < renderedChars.Count; i++)
				{
					//if (measuredChars[i].RightEdge != renderedChars[i].RightEdge)
					//{
					if (!printedHeader)
						DebugChar.PrintHeader();
					printedHeader = true;
					Debug.Print(measuredChars[i].ToString());
					Debug.Print(renderedChars[i].ToString());
					//}
				}
				var msg = string.Format("Right edge: {0,3} ({1}) {2}",
					spacingError,
					spacingError == 0 ? "perfect" : spacingError > 0 ? "space  " : "clipped",
					face.FamilyName);
				Debug.Print(msg);
				//throw new ApplicationException(msg);
			}
			
			return GraphicsDevice.GetTexture2DFromBitmap(bmp);
		}

		#endregion // RenderString

		#region IDisposable Support
		private bool _disposedValue; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if (!_disposedValue)
			{
				if (disposing)
				{
					if (FontFace != null && !FontFace.IsDisposed)
						try
						{
							FontFace.Dispose();
						}
						catch
						{
							// Ignored
						}
				}

				// TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
				// TODO: set large fields to null.

				_disposedValue = true;
			}
		}

		// TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
		// ~FontService() {
		//   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
		//   Dispose(false);
		// }

		// This code added to correctly implement the disposable pattern.
		public void Dispose()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(true);
			// TODO: uncomment the following line if the finalizer is overridden above.
			// GC.SuppressFinalize(this);
		}
		#endregion

		#region class DebugChar

		private class DebugChar
		{
			public char Char { get; set; }
			public float AdvanceX { get; set; }
			public float BearingX { get; set; }
			public float Width { get; set; }
			public float Underrun { get; set; }
			public float Overrun { get; set; }
			public float Kern { get; set; }
			public float RightEdge { get; set; }
			internal DebugChar(char c, float advanceX, float bearingX, float width)
			{
				Char = c; AdvanceX = advanceX; BearingX = bearingX; Width = width;
			}

			public override string ToString()
			{
				return string.Format("'{0}' {1,5:F0} {2,5:F0} {3,5:F0} {4,5:F0} {5,5:F0} {6,5:F0} {7,5:F0}",
					Char, AdvanceX, BearingX, Width, Underrun, Overrun,
					Kern, RightEdge);
			}
			public static void PrintHeader()
			{
				Debug.Print("    {0,5} {1,5} {2,5} {3,5} {4,5} {5,5} {6,5}",
					"adv", "bearing", "wid", "undrn", "ovrrn", "kern", "redge");
			}
		}

		#endregion
	}
}