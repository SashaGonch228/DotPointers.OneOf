using System;
using System.Text;

namespace DotPointers.OneOf.Generator
{
	internal class IndentedWriter
	{
		private const char IndentChar = '\t';
		private readonly StringBuilder _sb = new(4096);
		private int _indent = 0;

		public int Indent => _indent;
		public void Increase() => _indent++;
		public void Decrease()
		{
			if (_indent > 0)
			{
				_indent--;
			}
		}

		public IndentScope EnterScope(bool newLine = true, string endLine = "") => new(this, newLine, endLine);

		public void AppendLine(string line)
		{
			if (string.IsNullOrWhiteSpace(line))
			{
				_sb.AppendLine();
				return;
			}

			AppendIntend();
			_sb.AppendLine(line);
		}

		public void AppendLine() => _sb.AppendLine();

		public void Append(string text)
		{
			if (string.IsNullOrEmpty(text))
			{
				return;
			}
			_sb.Append(text);
		}

		public void AppendIntend()
		{
			_sb.Append(IndentChar, _indent);
		}

		public override string ToString() => _sb.ToString();

		internal readonly struct IndentScope : IDisposable
		{
			private readonly IndentedWriter _writer;
			private readonly bool _newLine;
			private readonly string _endLine;
			public IndentScope(IndentedWriter writer, bool newLine, string endLine)
			{
				_writer = writer;
				_newLine = newLine;
				_endLine = endLine;
				_writer.AppendLine("{");
				_writer.Increase();
			}
			public void Dispose()
			{
				_writer.Decrease();
				_writer.AppendIntend();
				_writer.Append("}");
				_writer.Append(_endLine);
				_writer.AppendLine();
				if (_newLine)
				{
					_writer.AppendLine();
				}
			}
		}
	}
}
