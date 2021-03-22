using System;

namespace Base
{
	public class ReadyEventArgs : EventArgs
	{
		public bool Ready { get; }

		public ReadyEventArgs(bool ready)
		{
			Ready = ready;
		}
	}
}