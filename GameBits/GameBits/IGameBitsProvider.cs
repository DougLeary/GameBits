using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameBits
{
	interface IGameBitsProvider
	{
		void Load();
		void Save();
	}
}
