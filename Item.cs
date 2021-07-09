using System;
using System.Linq;
using System.Collections.Generic;

namespace university
{
	public class Item
	{
		public int iIndex;

		public string id;

		public Item()
		{
			Random random = new Random();
			string s = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
			this.id = new string(Enumerable.Repeat(s, 100).Select(s => s[random.Next(s.Length)]).ToArray());
		}
	}
}