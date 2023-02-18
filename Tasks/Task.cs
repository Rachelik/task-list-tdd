using System;
using System.Collections.Generic;

namespace Tasks
{
	public class Task
	{
		public long Id { get; set; }

		public string IdTag { get; set; } = "";

		public string Description { get; set; }

		public bool Done { get; set; }

		public DateTime Deadline { get; set; }
	}


	public class Deadline
	{
		public long Id { get; set; }

		public DateTime DeadlineInput { get; set; }
	}
}
