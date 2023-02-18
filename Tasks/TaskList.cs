using System;
using System.Collections.Generic;
using System.Linq;

namespace Tasks
{
	public sealed class TaskList
	{
		private const string QUIT = "quit";

		private readonly IDictionary<string, IList<Task>> tasks = new Dictionary<string, IList<Task>>();
		private readonly IConsole console;

		private long lastId = 0;

		public static void Main(string[] args)
		{
			new TaskList(new RealConsole()).Run();
		}

		public TaskList(IConsole console)
		{
			this.console = console;
		}

		public void Run()
		{
			while (true) {
				console.Write("> ");
				var command = console.ReadLine();
				if (command == QUIT) {
					break;
				}
				Execute(command);
			}
		}

		private void Execute(string commandLine)
		{
			var commandRest = commandLine.Split(" ".ToCharArray(), 2);
			var command = commandRest[0];
			switch (command) {
			case "show":
				Show();
				break;
			case "add":
				Add(commandRest[1]);
				break;
			case "check":
				Check(commandRest[1]);
				break;
			case "uncheck":
				Uncheck(commandRest[1]);
				break;
			case "today":
				TodayDue();
				break;
            case "help":
				Help();
				break;
			default:
				Error(command);
				break;
			}
		}

		private void Show()
		{
			foreach (var project in tasks) {
				console.WriteLine(project.Key);
				foreach (var task in project.Value) {

					if (task.Deadline != new DateTime())
					{
                        console.WriteLine("    [{0}] {1}{2}: {3}  {4}", (task.Done ? 'x' : ' '), task.Id, task.IdTag, task.Description, task.Deadline.ToString("dd-MMM-yyyy"));
                    }
					else
					{
                        console.WriteLine("    [{0}] {1}{2}: {3}", (task.Done ? 'x' : ' '), task.Id, task.IdTag, task.Description);
                    }
                }
				console.WriteLine();
			}
		}

		private void TodayDue()
		{
            foreach (var project in tasks)
            {
                console.WriteLine(project.Key);
                foreach (var task in project.Value)
                {
					if (task.Deadline.Date.Equals(DateTime.Now.Date))
					{
						if (String.IsNullOrEmpty(task.IdTag))
						{
							console.WriteLine("    [{0}] {1}: {2}  {3}", (task.Done ? 'x' : ' '), task.Id, task.Description, task.Deadline.ToString("dd-MMM-yyyy"));
						}
						else
						{
							console.WriteLine("    [{0}] {1}{2}: {3}  {4}", (task.Done ? 'x' : ' '), task.Id, task.IdTag, task.Description, task.Deadline.ToString("dd-MMM-yyyy"));
						}
					}
                }
				console.WriteLine();
			}
        }

        private bool HasSpecialChars(string yourString)
        {
            return yourString.Any(ch => !char.IsLetterOrDigit(ch));
        }


        private void Add(string commandLine)
		{
			var subcommandRest = commandLine.Split(" ".ToCharArray(), 2);
			var subcommand = subcommandRest[0];
			if (subcommand == "project") {
				AddProject(subcommandRest[1]);
			} else if (subcommand == "task") {
				var projectTask = subcommandRest[1].Split(" ".ToCharArray(), 2);
				if (projectTask[0].Substring(0, 1) == "#")
				{
					if (!HasSpecialChars(projectTask[0].Substring(1)))
					{
						if (projectTask.Length > 1)
						{
							var projectDescription = projectTask[1].Split(" ".ToCharArray(), 2);
							if (projectDescription.Length >= 2)
							{
								AddTask(projectDescription[0], projectDescription[1], projectTask[0]);
							}
							else
							{
								AddTask(projectDescription[0], "", projectTask[0]);
							}
						}
						else
						{
							AddTask("", "", projectTask[0]);
						}
					}
					else
					{
						AddTask("", "", "#");
					}
				}
				else
				{
					AddTask(projectTask[0], projectTask[1], "");
				}
				//}

			} else if (subcommand == "deadline")
			{
                var projectTask = subcommandRest[1].Split(" ".ToCharArray(), 2);
				AddDeadline(projectTask[0], projectTask[1]);
			}
		}

		private void AddProject(string name)
		{
			tasks[name] = new List<Task>();
		}

		private void AddTask(string project, string description, string idTag)
		{
			if (idTag == "#")
			{
				Console.WriteLine("Identifier Tag should not have any special characters or spaces.");
				return;
			}

			if (!tasks.TryGetValue(project, out IList<Task> projectTasks))
			{
				Console.WriteLine("Could not find a project with the name \"{0}\".", project);
				return;
			}

			projectTasks.Add(new Task { Id = NextId(), IdTag = idTag, Description = description, Done = false });
		}


		private void AddDeadline(string idString, string taskDeadlineStr)
        {
			DateTime parsedDeadline = DateTime.Parse(taskDeadlineStr);

            int id = int.Parse(idString);
            var identifiedTask = tasks
                .Select(project => project.Value.FirstOrDefault(task => task.Id == id))
                .Where(task => task != null)
                .FirstOrDefault();
            if (identifiedTask == null)
            {
                console.WriteLine("Could not find a task with an ID of {0}.", id);
                return;
            }

			identifiedTask.Deadline = parsedDeadline;
        }




        private void Check(string idString)
		{
			SetDone(idString, true);
		}

		private void Uncheck(string idString)
		{
			SetDone(idString, false);
		}

		private void SetDone(string idString, bool done)
		{
			int id = int.Parse(idString);
			var identifiedTask = tasks
				.Select(project => project.Value.FirstOrDefault(task => task.Id == id))
				.Where(task => task != null)
				.FirstOrDefault();
			if (identifiedTask == null) {
				console.WriteLine("Could not find a task with an ID of {0}.", id);
				return;
			}

			identifiedTask.Done = done;
		}

		private void Help()
		{
			console.WriteLine("Commands:");
			console.WriteLine("  show");
			console.WriteLine("  add project <project name>");
			console.WriteLine("  add task <project name> <task description>");
			console.WriteLine("  check <task ID>");
			console.WriteLine("  uncheck <task ID>");
			console.WriteLine("  add deadline <task ID> <deadline yyyy/mm/dd>");
			console.WriteLine("  today");
            console.WriteLine("  add task <#identifierTag> <project name> <task description>");
            console.WriteLine();
		}

		private void Error(string command)
		{
			console.WriteLine("I don't know what the command \"{0}\" is.", command);
		}

		private long NextId()
		{
			return ++lastId;
		}
	}
}
