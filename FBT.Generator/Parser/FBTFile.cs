using System;
using System.IO;
using Antlr4.Runtime;
using FBT.TypeData;
using FBT.TypeManager;

namespace FBT.Parser;

public class FBTFile
{
	private readonly string FilePath;

	public FBTFile(string p_Path)
	{
		FilePath = p_Path;

		using (var s_Reader = new FileStream(FilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
		{
			Parse(s_Reader);
		}
	}

	private string File => Path.GetFileNameWithoutExtension(FilePath);


	private void Parse(Stream p_Stream)
	{
		try
		{
			var s_InputStream = new AntlrInputStream(p_Stream);


			var s_Lexer = new FBTLexer(s_InputStream);
			var s_Tokens = new CommonTokenStream(s_Lexer);

			var s_LexerErrorHandler = new FBTLexerErrorListener(p_Stream);

			s_Lexer.AddErrorListener(s_LexerErrorHandler);


			s_Tokens.Fill();


			/*
			foreach (var s_Token in s_Tokens.GetTokens())
			{
			    if (s_Token.Type == -1)
			        continue;

			    Debug.WriteLine($"({s_Lexer.Vocabulary.GetSymbolicName(s_Token.Type)}) {s_Token.Text}");
			}
			*/

			var s_Parser = new FBTParser(s_Tokens);

			var s_ErrorHandler = new FBTParserErrorListener(p_Stream);

			s_Parser.AddErrorListener(s_ErrorHandler);

			s_Tokens.Fill();

			var s_Visitor = new FBTVisitor();


			var s_Unit = TypeUnitManager.Instance.GetUnit(File);
			if (s_Unit == null)
			{
				s_Unit = new TypeUnit("fb");

				s_Unit.Path = Path.GetDirectoryName(FilePath);

				TypeUnitManager.Instance.AddUnit(File, s_Unit);
			}

			s_Visitor.SetUnit(s_Unit);

			s_Visitor.Visit(s_Parser.frostbiteType());
		}
		catch (Exception e)
		{
		}
	}
}