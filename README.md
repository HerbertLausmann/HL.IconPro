# ICON PRO
### Written in C# + WPF (.Net 4.8)
### Create Windows Icons easily!
<a href="https://www.youtube.com/watch?v=5yy3UKVHmVc" target="_blank"><img src="iconproyoutubevideoprint.png?raw=tru" 
alt="Icon Pro Presentation" width="480" height="280" border="10" /></a>

### This project is divided in three layers.

## The first layer is the core library: HL.IconPro.Lib.Core;
This layer works directly with bytes and bits. It's a .NET STANDARD 2.0 Class library, so, it's multi platform!

## The second layer is the WPF library: HL.IconPro.Lib.WPF;
This layer provides Encoders and Decoders for opening and writing Icons/Cursors in Windows Presentation Foundation.
It's, basically, a wrapper around the Core lib.
It makes the conversion between the bytes world of the core lib to a more friendly approach, like WIC.

## The third and last layer is the Icon Pro application itself!
It's a simple and objective GUI that allows you to create icons with easy!

By the way, even not implemented in the GUI, the libraries can also create and read cursors files!
An Animated Cursor File support might come in the future, but I'm not sure if I'm going to develop this, due to the fact that is really tough to find documentation about these old file formats.

Any way, I hope you guys find this tool useful!


This project uses the SVG.NET Library, from the following Git:
[SVG.NET](https://github.com/vvvv/SVG)
