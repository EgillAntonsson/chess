using System;

namespace Chess
{
	[Flags]
	public enum MoveCaptureFlag
	{
		Move = 1 << 0,
		Capture = 2 << 1
	}
}
