using System;
using System.Linq;
using System.Collections.Generic;

namespace university
{
	class UI
	{
		public static Dictionary<string, string> inputFields(string[] fields, string[] defaultFields = null)
		{
			Dictionary<string, string> obj = new Dictionary<string, string>();

			Console.Write("\n");

			for (int i = 0; i < fields.Length; ++i)
			{
				Console.Write($"# <Object>.{fields[i]}: ");
				
				if (defaultFields == null)
					obj.Add(fields[i], Console.ReadLine());
				else
					obj.Add(fields[i], ReadLine(defaultFields[i]));
			}

			return obj;
		}

		public static void showReport(int iLessonsCount, int iGroupsCount, int iStudentsCount, int iInstructorsCount)
		{
			Console.WriteLine("\n[report]:\n");
			Console.WriteLine($"Lessons Count: {iLessonsCount}");
			Console.WriteLine($"Groups Count: {iGroupsCount}");
			Console.WriteLine($"Students Count: {iStudentsCount}");
			Console.WriteLine($"Instructors Count: {iInstructorsCount}");
		}

		public static void showLesson(Lesson lesson, int iIndex, List<Group> listGroups, Instructor instructor,
			int iInstructorIndex, ref DBMS dbms)
		{
			Console.WriteLine($"\n[lesson | i={iIndex + 1}]:\n");
			Console.WriteLine($"Name: {lesson.name}");

			Console.WriteLine("Instructor: " + (iInstructorIndex == -1 ?
				"-" : $"{instructor.firstName} {instructor.lastName} | i={iInstructorIndex + 1}"));
			
			Console.WriteLine($"Groups ({listGroups.Count}):\n");

			if (listGroups.Count > 0)
				for (int i = 0; i < listGroups.Count; ++i)
					Console.WriteLine($"\ti={dbms.getIndexById<Group>(listGroups[i].id) + 1} | Group №{listGroups[i].number}");
			else
				Console.WriteLine("\t~ there are no groups that attend this lesson ~");
		}

		public static void showGroup(Group group, int iIndex, List<Lesson> listLessons, List<Student> listStudents,
			ref DBMS dbms)
		{
			Console.WriteLine($"\n[group | i={iIndex + 1}]:\n");
			Console.WriteLine($"Group Number: {group.number}");
			Console.WriteLine($"Lessons ({listLessons.Count}):\n");

			if (listLessons.Count > 0)
			{
				for (int i = 0; i < listLessons.Count; ++i)
					Console.WriteLine($"\ti={dbms.getIndexById<Lesson>(listLessons[i].id) + 1} | {listLessons[i].name}");
			}
			else
				Console.WriteLine("\t~ this group not attend any lesson ~");

			Console.WriteLine($"\nStudents ({listStudents.Count}):\n");

			if (listStudents.Count > 0)
			{
				for (int i = 0; i < listStudents.Count; ++i)
					Console.WriteLine($"\ti={dbms.getIndexById<Student>(listStudents[i].id) + 1} | {listStudents[i].firstName} {listStudents[i].lastName}");
			}
			else
				Console.WriteLine("\t~ this group has no students ~");
		}

		public static void showStudent(Student student, int iIndex, Group group, int iGroupIndex)
		{
			Console.WriteLine($"\n[student | i={iIndex + 1}]:\n");
			Console.WriteLine($"First Name: {student.firstName}");
			Console.WriteLine($"Last Name: {student.lastName}");
			Console.WriteLine($"Year of Birth: {student.birthYear} ({DateTime.Now.Year - student.birthYear} y.o.)");			
			Console.WriteLine("Group: " + (iGroupIndex == -1 ? "-" : $"№{group.number} | i={iGroupIndex + 1}"));
		}

		public static void showInstructor(Instructor instructor, int iIndex, List<Lesson> listLessons, ref DBMS dbms)
		{
			Console.WriteLine($"\n[instructor | i={iIndex + 1}]:\n");
			Console.WriteLine($"First Name: {instructor.firstName}");
			Console.WriteLine($"Last Name: {instructor.lastName}");
			Console.WriteLine($"Year of Birth: {instructor.birthYear} ({DateTime.Now.Year - instructor.birthYear} y.o.)");
			Console.WriteLine($"Experience: {instructor.experienceYears} years");
			Console.WriteLine($"Lessons ({listLessons.Count}):\n");

			if (listLessons.Count > 0)
				for (int i = 0; i < listLessons.Count; ++i)
					Console.WriteLine($"\ti={dbms.getIndexById<Lesson>(listLessons[i].id) + 1} | {listLessons[i].name}");
			else
				Console.WriteLine("\t~ this instructor does not teach any lessons ~");
		}

		public static void showLessons(List<Lesson> listLessons)
		{
			Console.WriteLine("\n[lessons]:\n");

			if (listLessons.Count > 0)
				for (int i = 0; i < listLessons.Count; ++i)
				{
					string sLine = $"{i + 1}) {listLessons[i].name}";

					Console.WriteLine(sLine);
				}
			else
				Console.WriteLine("~ no lessons ~");
		}

		public static void showGroups(List<Group> listGroups)
		{
			Console.WriteLine("\n[groups]:\n");

			if (listGroups.Count > 0)
				for (int i = 0; i < listGroups.Count; ++i)
				{
					string sLine = $"{i + 1}) Group №{listGroups[i].number}";

					Console.WriteLine(sLine);
				}
			else
				Console.WriteLine("~ no groups ~");
		}

		public static void showStudents(List<Student> listStudents)
		{
			Console.WriteLine("\n[students]:\n");

			if (listStudents.Count > 0)
				showPeoples<Student>(listStudents);
			else
				Console.WriteLine("~ no students ~");
		}

		public static void showInstructors(List<Instructor> listInstructors)
		{
			Console.WriteLine("\n[instructors]:\n");

			if (listInstructors.Count > 0)
				showPeoples<Instructor>(listInstructors);
			else
				Console.WriteLine("~ no instructors ~");
		}

		public static void showPeoples<T>(List<T> list) where T : People
		{
			for (int i = 0; i < list.Count; ++i)
			{
				string sLine = $"{i + 1}) {list[i].firstName} {list[i].lastName}, ";
				sLine += $"{DateTime.Now.Year - list[i].birthYear} y.o.";
				
				Console.WriteLine(sLine);
			}
		}

		static string ReadLine(string Default)
		{
			int pos = Console.CursorLeft;
			
			Console.Write(Default);
			
			ConsoleKeyInfo info;
			
			List<char> chars = new List<char> ();
			
			if (string.IsNullOrEmpty(Default) == false)
				chars.AddRange(Default.ToCharArray());

			while (true)
			{
				info = Console.ReadKey(true);
				
				if (info.Key == ConsoleKey.Backspace && Console.CursorLeft > pos)
				{
					chars.RemoveAt(chars.Count - 1);
					Console.CursorLeft--;
					Console.Write(' ');
					Console.CursorLeft--;

				}
				else if (info.Key == ConsoleKey.Enter)
				{
					Console.Write(Environment.NewLine); 
					break;
				}
				else if (info.Key == ConsoleKey.Spacebar)
				{
					Console.Write(' '); 
					chars.Add(' ');
				}
				else if (char.IsLetterOrDigit(info.KeyChar))
				{
					Console.Write(info.KeyChar);
					chars.Add(info.KeyChar);
				}
			}

			return new string(chars.ToArray());
		}
	}
}