using System;
using System.Collections.Generic;
using System.Data;
using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace university
{
	enum OBJECT_TYPE
	{
		LESSON,
		GROUP,
		STUDENT,
		INSTRUCTOR,
		UNKNOWN
	}

	class DBMS
	{
		/*
			Path to database.
		*/
		private string _sDatabasePath;

		/*
			State.
		*/
		private List<Lesson> _listLessons = new List<Lesson>();
		private List<Group> _listGroups = new List<Group>();
		private List<Student> _listStudents = new List<Student>();
		private List<Instructor> _listInstructors = new List<Instructor>();

		public DBMS(string sConnectionString)
		{
			/*
				Parsing the connection string.
			*/
			List<string> list = new List<string>(sConnectionString.Split(";"));

			for (int i = 0; i < list.Count; ++i)
				if (list[i].Contains("DatabasePath="))
					this._sDatabasePath = list[i].Replace("DatabasePath=", "");

			/*
				Load database.
			*/
			this._loadDB();
		}

		public void deleteIdenticalInstructorIdsFromLessons(string sInstructorId)
		{
			for (int i = 0; i < this._listLessons.Count; ++i)
				if (this._listLessons[i].instructorId == sInstructorId)
					this._listLessons[i].instructorId = "";

			this._saveDB();
		}

		public void deleteIdenticalGroupIdsFromStudents(string sGroupId)
		{
			for (int i = 0; i < this._listStudents.Count; ++i)
				if (this._listStudents[i].groupId == sGroupId)
					this._listStudents[i].groupId = "";

			this._saveDB();
		}

		public void deleteIdenticalIdsFromGroupsLessonIds(string sLessonId)
		{
			for (int i = 0; i < this._listGroups.Count; ++i)
				if (this._listGroups[i].lessonsIds.IndexOf(sLessonId) != -1)
					this._listGroups[i].lessonsIds.RemoveAt(this._listGroups[i].lessonsIds.IndexOf(sLessonId));

			this._saveDB();
		}

		public List<Lesson> getLessonsByIds(List<string> ids)
		{
			List<Lesson> list = new List<Lesson>();

			for (int i = 0; i < this._listLessons.Count; ++i)
				if (ids.IndexOf(this._listLessons[i].id) != -1)
					list.Add(this._listLessons[i]);

			return list;
		}

		public List<Lesson> getLessonsByInstructorId(string sInstructorId)
		{
			List<Lesson> list = new List<Lesson>();

			for (int i = 0; i < this._listLessons.Count; ++i)
				if (this._listLessons[i].instructorId == sInstructorId)
					list.Add(this._listLessons[i]);

			return list;
		}

		public List<Group> getGroupsByLessonId(string sLessonId)
		{
			List<Group> list = new List<Group>();

			for (int i = 0; i < this._listGroups.Count; ++i)
				if (this._listGroups[i].lessonsIds.IndexOf(sLessonId) != -1)
					list.Add(this._listGroups[i]);

			return list;
		}

		public List<Student> getStudentsByGroupId(string sGroupId)
		{
			List<Student> list = new List<Student>();

			for (int i = 0; i < this._listStudents.Count; ++i)
				if (this._listStudents[i].groupId == sGroupId)
					list.Add(this._listStudents[i]);

			return list;
		}

		public void deleteByIndex<T>(int iIndex)
		{
			List<T> list = this.getObjects<T>();
			list.RemoveAt(iIndex);
			this.setObjects<T>(list);
		}

		public int getIndexById<T>(string sId) where T : Item
		{
			List<T> list = this.getObjects<T>();

			for (int i = 0; i < list.Count; ++i)
				if (((T)list[i]).id == sId)
					return i;
			return -1;
		}

		public void updateObject<T>(int iIndex, Dictionary<string, string> fields) where T : Item
		{
			List<T> list = this.getObjects<T>(); 

			string sId = list[iIndex].id;

			this.insertObject<T>(iIndex, fields);

			list[iIndex].id = sId;

			this.deleteByIndex<T>(iIndex + 1);

			this._saveDB();
		}

		public string insertObject<T>(int iIndex, Dictionary<string, string> fields)
		{
			if (this._isType<T>(OBJECT_TYPE.LESSON))
				return this._insertLesson(iIndex, fields);
			else if (this._isType<T>(OBJECT_TYPE.GROUP))
				return this._insertGroup(iIndex, fields);
			else if (this._isType<T>(OBJECT_TYPE.STUDENT))
				return this._insertStudent(iIndex, fields);
			else if (this._isType<T>(OBJECT_TYPE.INSTRUCTOR))
				return this._insertInstructor(iIndex, fields);

			return null;
		}

		private string _insertLesson(int iIndex, Dictionary<string, string> fields)
		{
			Lesson lesson = new Lesson();
			lesson.name = fields["name"];
			lesson.instructorId = fields["instructorId"];

			this._listLessons.Insert(iIndex, lesson);
			this._saveDB();

			return lesson.id;
		}

		private string _insertGroup(int iIndex, Dictionary<string, string> fields)
		{
			Group group = new Group();
			group.number = fields["number"];

			if (fields["lessonsIds"].Length > 0)
			{
				string[] list = fields["lessonsIds"].Split(" ");
				for (int i = 0; i < list.Length; ++i)
					group.lessonsIds.Add(this._listLessons[int.Parse(list[i]) - 1].id);
			}

			this._listGroups.Insert(iIndex, group);
			this._saveDB();

			return group.id;
		}

		private string _insertStudent(int iIndex, Dictionary<string, string> fields)
		{
			Student student = new Student();
			student.firstName = fields["firstName"];
			student.lastName = fields["lastName"];
			student.birthYear = int.Parse(fields["birthYear"]);
			student.groupId = fields["groupId"];

			this._listStudents.Insert(iIndex, student);
			this._saveDB();

			return student.id;
		}

		private string _insertInstructor(int iIndex, Dictionary<string, string> fields)
		{
			Instructor instructor = new Instructor();
			instructor.firstName = fields["firstName"];
			instructor.lastName = fields["lastName"];
			instructor.birthYear = int.Parse(fields["birthYear"]);
			instructor.experienceYears = int.Parse(fields["expYears"]);

			this._listInstructors.Insert(iIndex, instructor);
			this._saveDB();

			return instructor.id;
		}

		/*
			Getting state.
		*/
		public List<T> getObjects<T>()
		{
			if (this._isType<T>(OBJECT_TYPE.LESSON))
				return (List<T>)Convert.ChangeType(this._listLessons, typeof(List<T>));
			else if (this._isType<T>(OBJECT_TYPE.GROUP))
				return (List<T>)Convert.ChangeType(this._listGroups, typeof(List<T>));
			else if (this._isType<T>(OBJECT_TYPE.STUDENT))
				return (List<T>)Convert.ChangeType(this._listStudents, typeof(List<T>));
			else if (this._isType<T>(OBJECT_TYPE.INSTRUCTOR))
				return (List<T>)Convert.ChangeType(this._listInstructors, typeof(List<T>));

			return new List<T>();
		}

		/*
			Setting state.
		*/
		public void setObjects<T>(List<T> list)
		{
			if (this._isType<T>(OBJECT_TYPE.LESSON))
				this._listLessons = (List<Lesson>)Convert.ChangeType(list, typeof(List<Lesson>));
			else if (this._isType<T>(OBJECT_TYPE.GROUP))
				this._listGroups = (List<Group>)Convert.ChangeType(list, typeof(List<Group>));
			else if (this._isType<T>(OBJECT_TYPE.STUDENT))
				this._listStudents = (List<Student>)Convert.ChangeType(list, typeof(List<Student>));
			else if (this._isType<T>(OBJECT_TYPE.INSTRUCTOR))
				this._listInstructors = (List<Instructor>)Convert.ChangeType(list, typeof(List<Instructor>));

			this._saveDB();
		}

		private bool _isType<T>(OBJECT_TYPE type)
		{
			if (typeof(T) == typeof(Lesson))
				return type == OBJECT_TYPE.LESSON;
			else if (typeof(T) == typeof(Group))
				return type == OBJECT_TYPE.GROUP;
			else if (typeof(T) == typeof(Student))
				return type == OBJECT_TYPE.STUDENT;
			else if (typeof(T) == typeof(Instructor))
				return type == OBJECT_TYPE.INSTRUCTOR;
		
			return false;
		}

		private void _loadDB()
		{
			/*
				Parsing json content.
			*/
			List<List<JObject>> db = Newtonsoft.Json.JsonConvert.DeserializeObject<List<List<JObject>>>(
				System.IO.File.ReadAllText(_sDatabasePath));

			/*
				Setting state.
			*/
			for (int i = 0; i < db[0].Count; ++i)
				this._listLessons.Add(db[0][i].ToObject<Lesson>());

			for (int i = 0; i < db[1].Count; ++i)
				this._listGroups.Add(db[1][i].ToObject<Group>());

			for (int i = 0; i < db[2].Count; ++i)
				this._listStudents.Add(db[2][i].ToObject<Student>());

			for (int i = 0; i < db[3].Count; ++i)
				this._listInstructors.Add(db[3][i].ToObject<Instructor>());
		}

		private void _saveDB()
		{
			/*
				Convert objects to json string.
			*/
			string sLessons = Newtonsoft.Json.JsonConvert.SerializeObject(this._listLessons, Formatting.Indented);
			string sGroups = Newtonsoft.Json.JsonConvert.SerializeObject(this._listGroups, Formatting.Indented);
			string sStudents = Newtonsoft.Json.JsonConvert.SerializeObject(this._listStudents, Formatting.Indented);
			string sInstructors = Newtonsoft.Json.JsonConvert.SerializeObject(this._listInstructors, Formatting.Indented);

			/*
				Save json string to db file.
			*/
			System.IO.File.WriteAllText(this._sDatabasePath, $"[{sLessons}, {sGroups}, {sStudents}, {sInstructors}]");
		}
	}
}