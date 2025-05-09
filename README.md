# Whiteboard Generator

A C# program that processes markdown files containing sticky notes and generates an interactive HTML whiteboard visualization.

## Features

- Processes markdown files from a `site` directory
- Organizes sticky notes into columns (Start, Stop, Continue)
- Color-codes notes based on their source file
- Groups notes with the same tag together
- Interactive popup view for grouped notes
- Responsive and modern UI design

## Directory Structure

```
whiteboard/
├── site/           # Input markdown files
│   └── [event]/    # Event-specific folders
│       └── *.md    # Markdown files for each participant
├── output/         # Generated HTML files
└── Program.cs      # Main program file
```

## Markdown Format

Each markdown file should follow this format:

```markdown
# Start
* Note 1
* Note 2 > tag1
* Note 3 > tag2

# Stop
* Note 4
* Note 5 > tag1

# Continue
* Note 6
* Note 7 > tag2
```

- Notes are prefixed with `*`
- Tags are added after `>` symbol
- Notes with the same tag are grouped together

## Building and Running

1. Ensure you have .NET SDK installed
2. Clone the repository
3. Run the program:
   ```bash
   dotnet run
   ```
4. Generated HTML files will appear in the `output` directory

## Features in Detail

### Note Grouping
- Notes with the same tag are grouped together
- Click on a grouped note to see all notes in that group
- Each note in the group shows its author and original color

### Color Coding
- Each markdown file gets its own unique color
- Colors are consistent across all views
- Colors are visually distinct and readable

### Interactive Features
- Hover over notes to see author information
- Click grouped notes to expand/collapse the group
- Responsive design works on different screen sizes

## Contributing

Feel free to submit issues and enhancement requests! 