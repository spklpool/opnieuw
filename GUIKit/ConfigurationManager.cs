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

namespace Opnieuw.Framework
{
	public class ConfigurationManager
	{
		private static string m_ConfigFileName = System.Environment.CurrentDirectory + "\\" + "Refactorings.xml";
		private static OpnieuwConfiguration m_Configuration = new OpnieuwConfiguration();

		public static void Load()
		{
			try
			{
				m_Configuration = new OpnieuwConfiguration();
				m_Configuration.ReadXml(m_ConfigFileName);
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
				Console.WriteLine(e.StackTrace);
			}
		}

		public static void Save()
		{
			try
			{
				m_Configuration.WriteXml(m_ConfigFileName);
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
				Console.WriteLine(e.StackTrace);
			}
		}

		public static OpnieuwConfiguration Configuration {
			get {
				return m_Configuration;
			}
		}

		public static bool ParseBeforeExecute {
			get {
				bool ret = true;
				if (m_Configuration.PropertyList.Count == 1)
				{
					OpnieuwConfiguration.PropertyRow[] properties = m_Configuration.PropertyList[0].GetPropertyRows();
					foreach (OpnieuwConfiguration.PropertyRow property in properties)
					{
						if (property.Name == "ParseBeforeExecute")
						{
							ret = Boolean.Parse(property.Value);
						}
					}
				}
				return ret;
			}
		}

		public static bool ParseAfterExecute {
			get {
				bool ret = true;
				if (m_Configuration.PropertyList.Count == 1)
				{
					OpnieuwConfiguration.PropertyRow[] properties = m_Configuration.PropertyList[0].GetPropertyRows();
					foreach (OpnieuwConfiguration.PropertyRow property in properties)
					{
						if (property.Name == "ParseAfterExecute")
						{
							ret = Boolean.Parse(property.Value);
						}
					}
				}
				return ret;
			}
		}
	}
}
