using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Security.Cryptography;

class Program
{
    private static readonly string[] ColumnNames = { "Start", "Stop", "Continue" };
    
    private static readonly Dictionary<string, string> ColumnColors = new Dictionary<string, string>
    {
        { "Start", "#4caf50" },    // Green
        { "Stop", "#f44336" },     // Red
        { "Continue", "#ffeb3b" }  // Yellow
    };

    // Distinct colors for sticky notes
    private static readonly string[] StickyNoteColors = new[]
    {
        "#e6e6fa", // Lavender
        "#b0e0e6", // Powder Blue
        "#f0e68c", // Khaki
        "#ffb6c1", // Light Pink
        "#98fb98", // Pale Green
        "#d8bfd8", // Thistle
        "#ffdab9", // Peach
        "#e0ffff", // Light Cyan
        "#f5f5dc", // Beige
        "#ffe4e1", // Misty Rose
        "#dda0dd", // Plum
        "#f0f8ff", // Alice Blue
        "#faebd7", // Antique White
        "#7fffd4", // Aquamarine
        "#f0ffff", // Azure
        "#f5f5dc", // Beige
        "#ffe4c4", // Bisque
        "#ffebcd", // Blanched Almond
        "#8a2be2", // Blue Violet
        "#a52a2a", // Brown
        "#deb887", // Burlywood
        "#5f9ea0", // Cadet Blue
        "#d2691e", // Chocolate
        "#ff7f50", // Coral
        "#6495ed", // Cornflower Blue
        "#fff8dc", // Cornsilk
        "#dc143c", // Crimson
        "#00ffff", // Cyan
        "#00008b", // Dark Blue
        "#008b8b", // Dark Cyan
        "#b8860b", // Dark Goldenrod
        "#a9a9a9", // Dark Gray
        "#006400", // Dark Green
        "#bdb76b", // Dark Khaki
        "#8b008b", // Dark Magenta
        "#556b2f", // Dark Olive Green
        "#ff8c00", // Dark Orange
        "#9932cc", // Dark Orchid
        "#8b0000", // Dark Red
        "#e9967a", // Dark Salmon
        "#8fbc8f", // Dark Sea Green
        "#483d8b", // Dark Slate Blue
        "#2f4f4f", // Dark Slate Gray
        "#00ced1", // Dark Turquoise
        "#9400d3", // Dark Violet
        "#ff1493", // Deep Pink
        "#00bfff", // Deep Sky Blue
        "#696969", // Dim Gray
        "#1e90ff", // Dodger Blue
        "#b22222", // Firebrick
        "#fffaf0", // Floral White
        "#228b22", // Forest Green
        "#ff00ff", // Fuchsia
        "#dcdcdc", // Gainsboro
        "#f8f8ff", // Ghost White
        "#ffd700", // Gold
        "#daa520", // Goldenrod
        "#808080", // Gray
        "#008000", // Green
        "#adff2f", // Green Yellow
        "#f0fff0", // Honeydew
        "#ff69b4", // Hot Pink
        "#cd5c5c", // Indian Red
        "#4b0082", // Indigo
        "#fffff0", // Ivory
        "#f0e68c", // Khaki
        "#e6e6fa", // Lavender
        "#fff0f5", // Lavender Blush
        "#7cfc00", // Lawn Green
        "#fffacd", // Lemon Chiffon
        "#add8e6", // Light Blue
        "#f08080", // Light Coral
        "#e0ffff", // Light Cyan
        "#fafad2", // Light Goldenrod Yellow
        "#d3d3d3", // Light Gray
        "#90ee90", // Light Green
        "#ffb6c1", // Light Pink
        "#ffa07a", // Light Salmon
        "#20b2aa", // Light Sea Green
        "#87cefa", // Light Sky Blue
        "#778899", // Light Slate Gray
        "#b0c4de", // Light Steel Blue
        "#ffffe0", // Light Yellow
        "#00ff00", // Lime
        "#32cd32", // Lime Green
        "#faf0e6", // Linen
        "#ff00ff", // Magenta
        "#800000", // Maroon
        "#66cdaa", // Medium Aquamarine
        "#0000cd", // Medium Blue
        "#ba55d3", // Medium Orchid
        "#9370db", // Medium Purple
        "#3cb371", // Medium Sea Green
        "#7b68ee", // Medium Slate Blue
        "#00fa9a", // Medium Spring Green
        "#48d1cc", // Medium Turquoise
        "#c71585", // Medium Violet Red
        "#191970", // Midnight Blue
        "#f5fffa", // Mint Cream
        "#ffe4e1", // Misty Rose
        "#ffe4b5", // Moccasin
        "#ffdead", // Navajo White
        "#000080", // Navy
        "#fdf5e6", // Old Lace
        "#808000", // Olive
        "#6b8e23", // Olive Drab
        "#ffa500", // Orange
        "#ff4500", // Orange Red
        "#da70d6", // Orchid
        "#eee8aa", // Pale Goldenrod
        "#98fb98", // Pale Green
        "#afeeee", // Pale Turquoise
        "#db7093", // Pale Violet Red
        "#ffefd5", // Peach Puff
        "#cd853f", // Peru
        "#ffc0cb", // Pink
        "#dda0dd", // Plum
        "#b0e0e6", // Powder Blue
        "#800080", // Purple
        "#ff0000", // Red
        "#bc8f8f", // Rosy Brown
        "#4169e1", // Royal Blue
        "#8b4513", // Saddle Brown
        "#fa8072", // Salmon
        "#f4a460", // Sandy Brown
        "#2e8b57", // Sea Green
        "#fff5ee", // Seashell
        "#a0522d", // Sienna
        "#c0c0c0", // Silver
        "#87ceeb", // Sky Blue
        "#6a5acd", // Slate Blue
        "#708090", // Slate Gray
        "#fffafa", // Snow
        "#00ff7f", // Spring Green
        "#4682b4", // Steel Blue
        "#d2b48c", // Tan
        "#008080", // Teal
        "#d8bfd8", // Thistle
        "#ff6347", // Tomato
        "#40e0d0", // Turquoise
        "#ee82ee", // Violet
        "#f5deb3", // Wheat
        "#ffffff", // White
        "#f5f5f5", // White Smoke
        "#ffff00", // Yellow
        "#9acd32"  // Yellow Green
    };

    private static Dictionary<string, string> fileColors = new Dictionary<string, string>();

    static string GetFileColor(string fileName)
    {
        if (!fileColors.ContainsKey(fileName))
        {
            // Use a hash of the filename to get a consistent index
            using (var md5 = MD5.Create())
            {
                var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(fileName));
                var index = Math.Abs(BitConverter.ToInt32(hash, 0)) % StickyNoteColors.Length;
                fileColors[fileName] = StickyNoteColors[index];
            }
        }
        return fileColors[fileName];
    }

    static void Main(string[] args)
    {
        string sitePath = Path.Combine(Directory.GetCurrentDirectory(), "site");
        string outputPath = Path.Combine(Directory.GetCurrentDirectory(), "output");

        // Ensure output directory exists
        Directory.CreateDirectory(outputPath);

        // Get all markdown files recursively
        var allFiles = Directory.GetFiles(sitePath, "*.md", SearchOption.AllDirectories);

        // Group files by their parent directory (event)
        var eventGroups = allFiles
            .GroupBy(f => Path.GetDirectoryName(f) ?? "root")
            .ToDictionary(g => g.Key, g => g.ToList());

        foreach (var eventGroup in eventGroups)
        {
            var eventName = Path.GetFileName(eventGroup.Key);
            var columnNotes = new Dictionary<string, List<string>>();
            foreach (var column in ColumnNames)
            {
                columnNotes[column] = new List<string>();
            }

            // Process each column separately
            foreach (var column in ColumnNames)
            {
                // Dictionary to track notes with the same tag in this column
                var taggedNotes = new Dictionary<string, List<(string Note, string Person, string File)>>();

                // Read all notes from files in this event
                foreach (var file in eventGroup.Value)
                {
                    var content = File.ReadAllText(file);
                    var personName = Path.GetFileNameWithoutExtension(file);
                    
                    // Split content into sections based on headings
                    var sections = Regex.Split(content, @"(?=^# )", RegexOptions.Multiline);
                    
                    foreach (var section in sections)
                    {
                        var lines = section.Split('\n');
                        if (lines.Length == 0) continue;

                        var heading = lines[0].TrimStart('#', ' ').Trim();
                        if (heading != column) continue;

                        // Process notes in this section
                        foreach (var line in lines.Skip(1))
                        {
                            var trimmedLine = line.Trim();
                            if (!trimmedLine.StartsWith("*")) continue;

                            var noteParts = trimmedLine.TrimStart('*', ' ').Split('>');
                            var note = noteParts[0].Trim();
                            var tag = noteParts.Length > 1 ? noteParts[1].Trim() : null;

                            if (tag != null)
                            {
                                if (!taggedNotes.ContainsKey(tag))
                                {
                                    taggedNotes[tag] = new List<(string, string, string)>();
                                }
                                taggedNotes[tag].Add((note, personName, file));
                            }
                            else
                            {
                                // Add untagged note directly
                                columnNotes[column].Add(
                                    $"<div class='sticky-note' data-person='{personName}' style='background-color: {GetFileColor(file)}'>{note}</div>"
                                );
                            }
                        }
                    }
                }

                // Process tagged notes for this column
                foreach (var tagGroup in taggedNotes)
                {
                    var notesInGroup = tagGroup.Value;
                    if (notesInGroup.Count > 1)
                    {
                        // Get the first note's color
                        var firstNote = notesInGroup[0];
                        var color = GetFileColor(firstNote.File);
                        
                        // Create HTML for all notes in the group
                        var allNotesHtml = string.Join("\n", notesInGroup.Select(n => 
                            $"<div class='grouped-note' style='background-color: {GetFileColor(n.File)}'><span class='note-content'>{n.Note}</span><span class='note-author'>{n.Person}</span></div>"
                        ));
                        
                        // Create a stacked note with popup
                        var stackedNote = $@"<div class='sticky-note stacked' style='background-color: {color}' data-count='{notesInGroup.Count}'>
                            <div class='note-content'>{firstNote.Note}</div>
                            <div class='note-tag'>#{tagGroup.Key}</div>
                            <div class='note-count'>+{notesInGroup.Count - 1}</div>
                            <div class='note-popup'>
                                <div class='popup-content'>
                                    <div class='popup-tag'>Tag: {tagGroup.Key}</div>
                                    {allNotesHtml}
                                </div>
                            </div>
                        </div>";
                        columnNotes[column].Add(stackedNote);
                    }
                    else
                    {
                        // Single note with tag, add it normally
                        var note = notesInGroup[0];
                        columnNotes[column].Add(
                            $"<div class='sticky-note' data-person='{note.Person}' style='background-color: {GetFileColor(note.File)}'>{note.Note}</div>"
                        );
                    }
                }
            }

            // Generate HTML for this event
            var html = GenerateHtml(eventName, columnNotes);
            var outputFile = Path.Combine(outputPath, $"{eventName}.html");
            File.WriteAllText(outputFile, html);
        }
    }

    static string GenerateHtml(string eventName, Dictionary<string, List<string>> columnNotes)
    {
        return $@"<!DOCTYPE html>
<html>
<head>
    <title>Whiteboard - {eventName}</title>
    <style>
        body {{
            font-family: Arial, sans-serif;
            margin: 0;
            padding: 20px;
            background: #f0f0f0;
        }}
        .whiteboard {{
            background: white;
            padding: 20px;
            border-radius: 10px;
            box-shadow: 0 0 10px rgba(0,0,0,0.1);
            min-height: 80vh;
            display: grid;
            grid-template-columns: repeat(3, 1fr);
            gap: 20px;
        }}
        .column {{
            display: flex;
            flex-direction: column;
            gap: 20px;
            padding: 15px;
            border-radius: 8px;
        }}
        .column-title {{
            text-align: center;
            font-size: 24px;
            font-weight: bold;
            color: white;
            padding: 10px;
            border-radius: 5px;
            margin-bottom: 10px;
            text-shadow: 1px 1px 2px rgba(0,0,0,0.3);
        }}
        .sticky-note {{
            padding: 15px;
            border-radius: 5px;
            box-shadow: 2px 2px 5px rgba(0,0,0,0.2);
            min-height: 100px;
            display: flex;
            align-items: center;
            justify-content: center;
            text-align: center;
            font-size: 14px;
            word-wrap: break-word;
            position: relative;
            transform: rotate({new Random().Next(-3, 4)}deg);
            transition: transform 0.2s;
        }}
        .sticky-note.stacked {{
            cursor: pointer;
        }}
        .sticky-note.stacked:hover {{
            transform: rotate(0deg) scale(1.05);
            z-index: 2;
        }}
        .sticky-note.stacked:hover .note-count {{
            display: none;
        }}
        .note-tag {{
            font-size: 12px;
            color: rgba(0,0,0,0.5);
            margin-top: 5px;
            font-style: italic;
        }}
        .sticky-note.stacked:hover::after {{
            content: 'Click to expand';
            position: absolute;
            bottom: 5px;
            right: 5px;
            font-size: 10px;
            color: rgba(0,0,0,0.5);
        }}
        .note-count {{
            position: absolute;
            top: 5px;
            right: 5px;
            background: rgba(0,0,0,0.1);
            padding: 2px 6px;
            border-radius: 10px;
            font-size: 12px;
        }}
        .sticky-note:not(.stacked):hover {{
            transform: rotate(0deg) scale(1.05);
            z-index: 1;
        }}
        .sticky-note:not(.stacked)::after {{
            content: attr(data-person);
            position: absolute;
            bottom: 5px;
            right: 5px;
            font-size: 10px;
            color: rgba(0,0,0,0.5);
        }}
        .note-popup {{
            display: none;
            position: fixed;
            top: 0;
            left: 0;
            right: 0;
            bottom: 0;
            background: rgba(0,0,0,0.5);
            z-index: 1000;
            justify-content: center;
            align-items: center;
        }}
        .popup-content {{
            background: white;
            padding: 20px;
            border-radius: 10px;
            max-width: 80%;
            max-height: 80vh;
            overflow-y: auto;
            display: flex;
            flex-direction: column;
            gap: 10px;
        }}
        .popup-tag {{
            font-size: 16px;
            font-weight: bold;
            color: #666;
            padding: 5px 0;
            border-bottom: 2px solid #eee;
            margin-bottom: 10px;
        }}
        .grouped-note {{
            padding: 10px;
            border-radius: 5px;
            display: flex;
            flex-direction: column;
            gap: 5px;
        }}
        .note-author {{
            font-size: 12px;
            color: rgba(0,0,0,0.5);
            text-align: right;
        }}
        h1 {{
            text-align: center;
            color: #333;
            margin-bottom: 20px;
        }}
    </style>
    <script>
        document.addEventListener('DOMContentLoaded', function() {{
            document.querySelectorAll('.sticky-note.stacked').forEach(note => {{
                note.addEventListener('click', function() {{
                    const popup = this.querySelector('.note-popup');
                    if (popup.style.display === 'flex') {{
                        popup.style.display = 'none';
                    }} else {{
                        popup.style.display = 'flex';
                    }}
                }});
            }});
        }});
    </script>
</head>
<body>
    <h1>{eventName}</h1>
    <div class='whiteboard'>
        {string.Join("\n        ", ColumnNames.Select(column => $@"
        <div class='column' style='background-color: {ColumnColors[column]}'>
            <div class='column-title'>{column}</div>
            {string.Join("\n            ", columnNotes[column])}
        </div>"))}
    </div>
</body>
</html>";
    }
}
