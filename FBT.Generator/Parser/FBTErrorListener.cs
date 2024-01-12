using System;
using System.IO;
using System.Linq;
using System.Text;
using Antlr4.Runtime;

namespace FBT.Parser;

public class FBTParserErrorListener : BaseErrorListener
{
	private readonly Stream m_FileStream;
	public bool m_HadError;

	public FBTParserErrorListener(Stream p_Stream)
	{
		m_FileStream = p_Stream;
	}

	public override void SyntaxError(TextWriter output,
		IRecognizer recognizer,
		IToken offendingSymbol,
		int line,
		int charPositionInLine,
		string msg,
		RecognitionException e)
	{
		m_HadError = true;

		var s_FileLine = "";
		{
			var s_LastPosition = m_FileStream.Position;

			m_FileStream.Seek(0, SeekOrigin.Begin);

			var FileData = new byte[m_FileStream.Length];
			m_FileStream.Read(FileData, 0, (int)m_FileStream.Length);

			m_FileStream.Seek(s_LastPosition, SeekOrigin.Begin);

			/*
			var s_SplitData = Encoding.Default.GetString( FileData ).Replace("\r", "").Split( '\n' );

			foreach ( var s_Split in s_SplitData )
			    Debug.WriteLine( "\t" + s_Split );
			*/

			s_FileLine = Encoding.Default.GetString(FileData).Replace("\r", "").Split('\n')
				.ElementAt(line >= 1 ? line - 1 : line);
		}


		var Data = $"Error parsing line:\n"
		           + $"\t{s_FileLine}\n"
		           + $"\t{(charPositionInLine > 0 ? new string(' ', charPositionInLine - 1) : "")}^";


		throw new Exception(Data);
	}
}

public class FBTLexerErrorListener
	: IAntlrErrorListener<int>
{
	private readonly Stream m_FileStream;
	public bool m_HadError;

	public FBTLexerErrorListener(Stream p_Stream)
	{
		m_FileStream = p_Stream;
	}

	public void SyntaxError(TextWriter output,
		IRecognizer recognizer,
		int offendingSymbol,
		int line,
		int charPositionInLine,
		string msg,
		RecognitionException e)
	{
		m_HadError = true;

		var s_FileLine = "";
		{
			var s_LastPosition = m_FileStream.Position;

			m_FileStream.Seek(0, SeekOrigin.Begin);

			var FileData = new byte[m_FileStream.Length];
			m_FileStream.Read(FileData, 0, (int)m_FileStream.Length);

			m_FileStream.Seek(s_LastPosition, SeekOrigin.Begin);

			/*
			var s_SplitData = Encoding.Default.GetString( FileData ).Replace("\r", "").Split( '\n' );

			foreach ( var s_Split in s_SplitData )
			    Debug.WriteLine( "\t" + s_Split );
			*/

			s_FileLine = Encoding.Default.GetString(FileData).Replace("\r", "").Split('\n')
				.ElementAt(line >= 1 ? line - 1 : line);
		}


		var Data = $"Error lexing line:\n"
		           + $"\t{s_FileLine}\n"
		           + $"\t{(charPositionInLine > 0 ? new string(' ', charPositionInLine - 1) : "")}^";


		throw new Exception(Data);
	}
}