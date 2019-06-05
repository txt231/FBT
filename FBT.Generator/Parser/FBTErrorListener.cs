using Antlr4.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FBT.Parser
{
    public class FBTParserErrorListener : BaseErrorListener
    {
        public FBTParserErrorListener( Stream p_Stream )
        {
            this.m_FileStream = p_Stream;
        }


        Stream m_FileStream;
        public bool m_HadError = false;

        public override void SyntaxError( TextWriter output,
                                          IRecognizer recognizer,
                                          IToken offendingSymbol,
                                          Int32 line,
                                          Int32 charPositionInLine,
                                          String msg,
                                          RecognitionException e )
        {
            this.m_HadError = true;

            string s_FileLine = "";
            {

                var s_LastPosition = this.m_FileStream.Position;

                this.m_FileStream.Seek( 0, SeekOrigin.Begin );

                byte[] FileData = new byte[this.m_FileStream.Length];
                this.m_FileStream.Read( FileData, 0, ( int )this.m_FileStream.Length );

                this.m_FileStream.Seek( s_LastPosition, SeekOrigin.Begin );

                /*
                var s_SplitData = Encoding.Default.GetString( FileData ).Replace("\r", "").Split( '\n' );

                foreach ( var s_Split in s_SplitData )
                    Debug.WriteLine( "\t" + s_Split );
                */

                s_FileLine = Encoding.Default.GetString( FileData ).Replace( "\r", "" ).Split( '\n' ).ElementAt( ( line >= 1 ? line - 1 : line ) );
            }


            string Data = $"Error parsing line:\n"
                        + $"\t{s_FileLine}\n"
                        + $"\t{( charPositionInLine > 0 ? new String( ' ', charPositionInLine - 1 ) : "" )}^";


            throw new Exception( Data );
        }
    }


    public class FBTLexerErrorListener
        : IAntlrErrorListener<System.Int32>
    {
        public FBTLexerErrorListener( Stream p_Stream )
        {
            this.m_FileStream = p_Stream;
        }


        Stream m_FileStream;
        public bool m_HadError = false;

        public void SyntaxError( TextWriter output,
                                          IRecognizer recognizer,
                                          Int32 offendingSymbol,
                                          Int32 line,
                                          Int32 charPositionInLine,
                                          String msg,
                                          RecognitionException e )
        {
            this.m_HadError = true;

            string s_FileLine = "";
            {

                var s_LastPosition = this.m_FileStream.Position;

                this.m_FileStream.Seek( 0, SeekOrigin.Begin );

                byte[] FileData = new byte[this.m_FileStream.Length];
                this.m_FileStream.Read( FileData, 0, ( int )this.m_FileStream.Length );

                this.m_FileStream.Seek( s_LastPosition, SeekOrigin.Begin );

                /*
                var s_SplitData = Encoding.Default.GetString( FileData ).Replace("\r", "").Split( '\n' );

                foreach ( var s_Split in s_SplitData )
                    Debug.WriteLine( "\t" + s_Split );
                */

                s_FileLine = Encoding.Default.GetString( FileData ).Replace( "\r", "" ).Split( '\n' ).ElementAt( ( line >= 1 ? line - 1 : line ) );
            }


            string Data = $"Error lexing line:\n"
                        + $"\t{s_FileLine}\n"
                        + $"\t{( charPositionInLine > 0 ? new String( ' ', charPositionInLine - 1 ) : "" )}^";


            throw new Exception( Data );
        }
    }
}
