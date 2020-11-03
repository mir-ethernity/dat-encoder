# Mir Ethernity - Dat Encoder

Install package from nuget:
```sh
Install-Package Mir.Ethernity.Dat
```

## How to use

```cs
using Mir.Ethernity.Dat;

var inputEncodedData = await File.ReadAllBytesAsync(@"example.dat");
var decodedData = DatEncoder.Decode(inputEncodedData);
var encodedData = DatEncoder.Encode(decodedData);
```