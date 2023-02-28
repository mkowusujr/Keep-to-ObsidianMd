![Keep.md Logo](keepmd.png)

A CLI to help you convert your google keep zip archives into an obsidian valut.

# How to use
This only works on google keep zip archives. You can get your own google keep archive by requesting it from google.

## Avaialble Commands
```shell
keep.md {keep-archive.zip}

keep.md {keep-archive.zip path} -d {destination path}
```

# Credits
- The CLI relies on the c# library [Command Line Parser](https://github.com/commandlineparser/commandline).
- I used [Angle Sharp](https://github.com/AngleSharp/AngleSharp) to parse through the google keep html files
- I got the font from this [site](https://fontmeme.com/uno-card-game-font/).