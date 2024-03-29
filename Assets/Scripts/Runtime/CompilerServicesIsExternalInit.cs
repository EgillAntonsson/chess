namespace System.Runtime.CompilerServices
{
	/// <summary>
	/// Unity needs this for full record support, as record uses init only setters.
	/// https://docs.unity3d.com/2022.3/Documentation/Manual/CSharpCompiler.html
	/// </summary>
	public static class IsExternalInit {}
}