# DotnetSyncer

Dotnet Tool for File synchronization

## Installation

dotnet tool install --global DotnetSyncer.Console

## Usage

```bash
dotnetsync --left "seaweedfs|http://192.168.1.29:8888|Juegos/ROMs/Gameboy" --right "local||home/jmn/Juegos/ROMs/Gameboy"
```


Left part is the reference directory No files will be touched there.
Right part is the directory in which to reflect the changes.

The paths are in a specific format
Eg: seaweedfs|http://192.168.1.29:8888|Juegos/ROMs/Gameboy

- **seaweedfs** is the filesystem plugin. Here it's SeaweedFS
- **http://192.168.1.29:8888** is the server address (specific for SeaweedFS).
- **Juegos/ROMs/Gameboy** this is the path. Paths never start with "/" because it's assumed paths are absolute. In Windows, a valid path would be `C:/Users/User/Desktop`.