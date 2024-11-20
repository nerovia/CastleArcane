using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SadConsole;
using SadRogue.Primitives;
using System.IO.Compression;
using System.Runtime.InteropServices.Marshalling;

namespace CastleArcane.Utility;

/// <summary>
/// An improved version of <see cref="SadConsole.Readers.Playscii"/>
/// </summary>
public class Playscii
{
    /// <summary>
    /// Cashed palettes.
    /// </summary>
    static readonly Dictionary<string, Palette> s_palettes = new();

    /// <summary>
    /// Palette file extensions supported by the Playscii format.
    /// </summary>
    static readonly string[] s_paletteExtensions = new string[] { ".png", ".gif", ".bmp" };

    /// <summary>
    /// Maximum amount of colors supported by the Playscii format.
    /// </summary>
    const int MaxColors = 1024;

    public string? fileName;

    /// <summary>
    /// Name of the font file.
    /// </summary>
    public string charset = string.Empty;

    /// <summary>
    /// Hold all the animation frames.
    /// </summary>
    public Frame[] frames = [];

    /// <summary>
    /// Surface height.
    /// </summary>
    public int height;

    /// <summary>
    /// Name of the palette file.
    /// </summary>
    public string palette = string.Empty;

    /// <summary>
    /// Surface width.
    /// </summary>
    public int width;

    /// <summary>
    /// Json frame object in the <see cref="Playscii"/> file.
    /// </summary>
    public struct Frame
    {
        /// <summary>
        /// Duration for this frame.
        /// </summary>
        public float delay;

        /// <summary>
        /// <see cref="Playscii"/> frame layers that will be converted to <see cref="ScreenSurface"/>.
        /// </summary>
        public Layer[] layers;
    }

    /// <summary>
    /// Json layer in the <see cref="Playscii"/> file.
    /// </summary>
    public struct Layer
    {
        /// <summary>
        /// Layer name.
        /// </summary>
        public string name;

        /// <summary>
        /// <see cref="Playscii"/> tiles that will be converted to <see cref="ColoredGlyphBase"/>.
        /// </summary>
        public Tile[] tiles;

        /// <summary>
        /// Visibility of this layer.
        /// </summary>
        public bool visible;

        /// <summary>
        /// Converts the <see cref="Playscii"/> layer to a SadConsole <see cref="ScreenSurface"/>.
        /// </summary>
        /// <param name="parent"><see cref="ScreenSurface"/> that represents Playscii frame holding this layer.</param>
        /// <param name="colors"><see cref="Palette"/> of colors converted from the <see cref="Playscii"/> format.</param>
        /// <returns><see cref="ScreenSurface"/> containg the given <see cref="Playscii"/> layer.</returns>
        public CellSurface ToCellSurface(Playscii parent, IFont font, Palette colors, Rectangle bounds)
        {
            if (bounds.X < 0) throw new ArgumentException();
            if (bounds.Y < 0) throw new ArgumentException();
            if (bounds.Width > parent.width) throw new ArgumentException();
            if (bounds.Height > parent.height) throw new ArgumentException();

            var surface = new CellSurface(bounds.Width, bounds.Height);

            for (int y = 0; y < surface.Height; y++)
            {
                for (int x = 0; x < surface.Width; x++)
                {
                    int tileIndex = (y + bounds.Y) * parent.width + x + bounds.X;
                    if (tileIndex < tiles.Length)
                    {
                        var glyph = tiles[tileIndex].ToColoredGlyph(font, colors);
                        surface.Surface.SetCellAppearance(x, y, glyph);
                    }
                }
            }

            return surface;
        }
    }

    /// <summary>
    /// Json tile in the <see cref="Playscii"/> file.
    /// </summary>
    public struct Tile
    {
        /// <summary>
        /// Tile background color.
        /// </summary>
        public int bg;

        /// <summary>
        /// Tile character index.
        /// </summary>
        [JsonProperty("char")]
        public int glyph;

        /// <summary>
        /// Tile foreground color.
        /// </summary>
        public int fg;

        /// <summary>
        /// Tile rotation and mirror.
        /// </summary>
        public byte xform;

        /// <summary>
        /// Converts the <see cref="Playscii"/> tile to a SadConsole <see cref="ColoredGlyphBase"/>.
        /// </summary>
        /// <param name="font"><see cref="IFont"/> to be used when creating the <see cref="ScreenSurface"/>.</param>
        /// <param name="colors"><see cref="Palette"/> of colors converted from the <see cref="Playscii"/> format.</param>
        /// <returns><see cref="ColoredGlyphBase"/> equivalent of the <see cref="Playscii"/> tile.</returns>
        public ColoredGlyphBase ToColoredGlyph(IFont font, Palette colors)
        {
            if (bg < 0 || fg < 0 || bg >= colors.Length || fg >= colors.Length) throw new IndexOutOfRangeException("Glyph color out of palette range.");
            if (glyph < 0 || glyph >= font.TotalGlyphs) throw new IndexOutOfRangeException("Glyph index out of font range.");

            Mirror mirror = xform switch
            {
                1 or 2 or 3 => throw new ArgumentException("Rotation is not supported by SadConsole"),
                4 => Mirror.Horizontal,
                5 => Mirror.Vertical,
                _ => Mirror.None
            };
            Color foreground = colors[fg];
            Color background = colors[bg];
            return new ColoredGlyph(foreground, background, glyph, mirror);
        }
    }

    /// <summary>
    /// Converts <see cref="Playscii"/> palette file to a SadConsole <see cref="Palette"/>.
    /// </summary>
    /// <param name="fileName">Name and path of the palette file.</param>
    /// <returns><see cref="Palette"/> of <see cref="Playscii"/> colors.</returns>
    /// <remarks>Place the palette file in the same folder as the playscii file.</remarks>
    static Palette ReadPalette(string fileName)
    {

        if (!File.Exists(fileName)) throw new FileNotFoundException("Palette file doesn't exist.");
        string ext = Path.GetExtension(fileName).ToLower();
        if (!s_paletteExtensions.Contains(ext))
            throw new InvalidOperationException("Palette file extension not supported by the Playscii format.");

        // check if this palette has already been created previously
        string paletteName = Path.GetFileNameWithoutExtension(fileName);
        if (s_palettes.ContainsKey(paletteName))
            return s_palettes[paletteName];

        var colors = new List<Color>() { new(0, 0, 0, 0) };
        using (ITexture image = GameHost.Instance.GetTexture(fileName))
        {
            colors.AddRange(image.GetPixels().Distinct());
        }

        Palette palette = new(colors);
        s_palettes[paletteName] = palette;
        return palette;
    }

    /// <summary>
    /// Reads the <see cref="Playscii"/> Json file.
    /// </summary>
    /// <param name="playsciiFileName">Playscii file.</param>
    /// <param name="zipArchiveName">Zip archive containing playscii file.</param>
    /// <returns>Deserialised object containing <see cref="Playscii"/> save file data.</returns>
    public static Playscii ReadFile(string playsciiFileName, string zipArchiveName = "")
    {
        Playscii output;
        if (!string.IsNullOrEmpty(zipArchiveName))
        {
            if (!File.Exists(zipArchiveName)) throw new FileNotFoundException("Zip archive with given name doesn't exist.");

            // open zip file stream for reading
            using (var zipStream = new FileStream(zipArchiveName, FileMode.Open))
            using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Read))
            {
                if (archive.Entries.Count == 0) throw new FileNotFoundException("Zip file does not contain any files.");

                // get reference for the playscii file from the archive
                ZipArchiveEntry? playsciiZipEntry = archive.GetEntry(playsciiFileName);

                if (playsciiZipEntry is null) throw new FileNotFoundException("Specified playscii file doesn't exist in the zip archive.");

                // create stream, read contents and return playscii instance
                using Stream stream = playsciiZipEntry.Open();
                using StreamReader reader = new(stream);

                output = ReadFromStream(reader);
            }
        }
        else
        {
            if (!File.Exists(playsciiFileName)) throw new FileNotFoundException("Playscii file doesn't exist.");

            using StreamReader streamReader = File.OpenText(playsciiFileName);

            output = ReadFromStream(streamReader);
        }

        ReadPalette(Path.Combine(Path.GetDirectoryName(playsciiFileName), output.palette + ".png"));

        return output;
    }

    static Playscii ReadFromStream(StreamReader streamReader)
    {
        using (JsonTextReader reader = new(streamReader))
        {
            JObject o2 = (JObject)JToken.ReadFrom(reader);
            return o2.ToObject(typeof(Playscii)) as Playscii ?? throw new Exception($"Unable to convert object to {nameof(Playscii)}");
        }
    }

    /// <summary>
    /// Converts a <see cref="Playscii"/> file to a SadConsole <see cref="ScreenSurface"/>.
    /// </summary>
    /// <param name="fileName">Name and path of the <see cref="Playscii"/> file (give only file name if <paramref name="zipArchiveName"/> is used).</param>
    /// <param name="font"><see cref="IFont"/> to be used when converting the <see cref="Playscii"/> file.</param>
    /// <param name="paletteFileName">Path to an alternative palette file rather than the one specified in the playscii records.</param>
    /// <param name="zipArchiveName">If specified, the playscii file will be read from this zip archive.</param>
    /// 
    /// <remarks>SadConsole does not support all the Playscii features at the moment, so the conversion will not be perfect.<br></br>
    /// Do not use tile rotation and set Z-Depth to 0 on all Playscii layers.<br></br>
    /// Transparent glyph foreground is fine, but it will not cut through the <see cref="ColoredGlyphBase"/> background like it does in Playscii.</remarks>
    /// 
    /// <returns><see cref="ScreenSurface"/> containing the first frame from the <see cref="Playscii"/> file.</returns>
    public ScreenObject CreateLayered(Rectangle bounds, int frame = 0)
    {
        var font = GameHost.Instance.Fonts[charset];
        var palette = s_palettes[this.palette];

        var obj = new ScreenObject();

        foreach (var layer in frames[frame].layers)
            obj.Children.Add(new ScreenSurface(layer.ToCellSurface(this, font, palette, bounds)));

        return obj;
    }

    public CellSurface GetCellSurface(Rectangle bounds, int frame = 0, int layer = 0)
    {
        var font = GameHost.Instance.Fonts[charset];
        var palette = s_palettes[this.palette];
        return frames[frame].layers[layer].ToCellSurface(this, font, palette, bounds);
    }

	public CellSurface ToCellSurface(int frame = 0, int layer = 0)
	{
		var font = GameHost.Instance.Fonts[charset];
		var palette = s_palettes[this.palette];
		return frames[frame].layers[layer].ToCellSurface(this, font, palette, new(0, 0, width, height));
	}

	public AnimatedScreenObject CreateAnimated(string name, Rectangle bounds, int layer = 0)
    {
        var font = GameHost.Instance.Fonts[charset];
        var palette = s_palettes[this.palette];
        var duration = this.frames.Sum(it => it.delay);
        var frames = this.frames.Select(it => it.layers[layer].ToCellSurface(this, font, palette, bounds)).ToArray();
        var anim = new AnimatedScreenObject(name, frames)
        {
            AnimationDuration = TimeSpan.FromSeconds(duration)
        };
        return anim;
    }


}