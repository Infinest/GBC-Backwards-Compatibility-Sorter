# GBC Backwards Compatibility Sorter
 Sorts GBC ROMs by their backwards compatibility flag

This application will sort all files with the file ending ".gbc" depending on the byte at offset 0x143.
This offset is used in a GBC Rom to determine whether or not a Gameboy Color game is backwards compatible with the Gameboy or not. [See this article over on the Gameboy Development Wiki](https://gbdev.gg8.se/wiki/articles/The_Cartridge_Header#0143_-_CGB_Flag)

The program will either sort all ROMs in it's own directory or you can specify a path when starting it via a command line.
On startup you will need to specify whether or not the appication should recursively sort ROMs in subdirectories as well.
ROMs will be sorted into the directories "gb_compatible" and "gb_incompatible". These directories will be ignored for recursive sorting.
