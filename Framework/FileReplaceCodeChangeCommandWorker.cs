#region Copyright (C) Pierre Boudreau
//This file is part of the Opnieuw project.
//
//pierre@opnieuw.com
//http://www.opnieuw.com
//
//This program is free software; you can redistribute it and/or modify it 
//under the terms of the GNU General Public License as published by the Free
//Software Foundation; either version 2 of the License, or (at your option)
//any later version.
//
//This program is distributed in the hope that it will be useful, but 
//WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY 
//or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for 
//more details.
//
//You should have received a copy of the GNU General Public License along 
//with this program; 
//if not, write to the Free Software Foundation, Inc., 
//59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
#endregion

using System;
using System.IO;

namespace Opnieuw.Framework
{
	public class FileReplaceCodeChangeCommandWorker : CodeChangeCommandWorker
	{
		private string m_FileBeforeChange = "";
		private string m_OutputBuffer = "";
		private StreamReader m_InputStream = null;
		private int m_CurrentLine = 1;
		private CodeReplacement m_CurrentCodeReplacement = new MissingCodeReplacement();

		public FileReplaceCodeChangeCommandWorker(ReplaceCodeChangeCommand command) :
			base(command)
		{
		}

		/// <summary>
		/// Executes the change to the file.
		/// </summary>
		public override bool Do()
		{
			bool ret = true;
			try
			{
				BackupFile();
				foreach (CodeReplacement codeReplacement in (m_Command as ReplaceCodeChangeCommand).CodeReplacements)
				{
					m_CurrentLine = 1;
					m_OutputBuffer = "";
					m_CurrentCodeReplacement = codeReplacement;
					if (IsFileValid)
					{
						OpenFile();
                        if (Position is InvalidPosition) {
                            ReplaceEntireFile();
                        } else {
						    BufferToStartLine();
						    ReplaceChangingPart();
						    BufferAfterEndLine();
                        }
						CloseFile();
						WriteFile();
					}
				}
			}
			catch
			{
				CloseFile();
				ret = false;
			}
			return ret;
		}

		/// <summary>
		/// Restores the original file from backup.
		/// </summary>
		public override bool Undo()
		{
			bool ret = true;
			try
			{
				RestoreFileFromBackup();
			}
			catch
			{
				ret = false;
			}
			return ret;
		}

		private string FileName {
			get {
				return (m_Command as ReplaceCodeChangeCommand).FileName;
			}
		}

		private Position Position {
			get {
				return m_CurrentCodeReplacement.Position;
			}
		}

		private string NewCode {
			get {
				return m_CurrentCodeReplacement.NewCode;
			}
		}

		private bool IsFileValid {
			get {
				//TODO:  Return something meaningfull.
				return true;
			}
		}

		private void OpenFile()
		{
			m_InputStream = File.OpenText(FileName);
		}

		private void BufferToStartLine()
		{
			while (m_CurrentLine < Position.StartLine)
			{
				if (m_CurrentLine != 1)
				{
					m_OutputBuffer += "\r\n";
				}
				m_OutputBuffer += m_InputStream.ReadLine();
				m_CurrentLine ++;
			}
		}

		private void ReplaceChangingPart()
		{
			if (m_CurrentLine == Position.StartLine)
			{
				if (m_CurrentLine != 1)
				{
					m_OutputBuffer += "\r\n";
				}

				string tempLine = m_InputStream.ReadLine();
				int realStart = Position.StartCol;

				if (null != tempLine)
				{
					m_OutputBuffer += tempLine.Substring(0, realStart - 1);
				}

				m_OutputBuffer += NewCode;

				while (m_CurrentLine < Position.EndLine)
				{
					tempLine = m_InputStream.ReadLine();
					m_CurrentLine ++;
				}

				int realEnd = Position.EndCol;

				if (null != tempLine)
				{
					m_OutputBuffer += tempLine.Substring(realEnd - 1, tempLine.Length - (realEnd - 1));
				}
			}
		}

		private void BufferAfterEndLine()
		{
			string restOfFile = m_InputStream.ReadToEnd();
			if ("" != restOfFile)
			{
				m_OutputBuffer += "\r\n" + restOfFile;
			}
		}

		private void ReplaceEntireFile()
		{
            m_OutputBuffer = NewCode;
		}

		private void BackupFile()
		{
			TextReader backupStream = File.OpenText(FileName);
			m_FileBeforeChange= backupStream.ReadToEnd();
			backupStream.Close();
		}

		private void RestoreFileFromBackup()
		{
			StreamWriter writerStream = new StreamWriter(FileName);
			TextWriter writer = writerStream as TextWriter;
			writer.Write(m_FileBeforeChange);
			writer.Close();
		}

		private void WriteFile()
		{
			try
			{
				StreamWriter writerStream = new StreamWriter(FileName);
				TextWriter writer = writerStream as TextWriter;
				writer.Write(m_OutputBuffer);
				writer.Close();
			}
			catch (Exception e)
			{
				System.Console.WriteLine(e.StackTrace);
			}
		}

		private void CloseFile()
		{
			m_InputStream.Close();
		}
	}
}
