using System;
using System.Collections.Generic;

namespace university
{
	public class ValidateResult
	{
		public bool bSuccess;
		public string sMessage;

		public ValidateResult(bool bSuccess, string sMessage)
		{
			this.bSuccess = bSuccess;
			this.sMessage = sMessage;
		}
	}

	public class Validator
	{
		public static ValidateResult validateIndex(string sIndex, int iCeil)
		{
			if (!isNumber(sIndex))
				return new ValidateResult(false, "invalid index");

			int iIndex = int.Parse(sIndex) - 1;

			if (!(iIndex >= 0 && iIndex < iCeil))
				return new ValidateResult(false, "index out of range");

			return new ValidateResult(true, "all is ok");
		}

		public static ValidateResult validateLesson(Dictionary<string, string> fields, int iInstructorsCount)
		{
			if (fields["name"].Length == 0)
				return new ValidateResult(false, "name is an empty string");

			if (fields["instructorId"].Length > 0)
			{
				if (!isNumber(fields["instructorId"]))
					return new ValidateResult(false, "incorrect instructorId");

				int iIndex = int.Parse(fields["instructorId"]) - 1;
				if (!(iIndex >= 0 && iIndex < iInstructorsCount))
					return new ValidateResult(false, "instructorId is out of range");
			}

			return new ValidateResult(true, "all is ok");
		}

		public static ValidateResult validateGroup(Dictionary<string, string> fields, int iLessonsCount)
		{
			if (!(fields["number"].Length > 0 && isNumber(fields["number"])))
				return new ValidateResult(false, "incorrect number");

			if (fields["lessonsIds"].Length > 0)
			{
				string[] list = fields["lessonsIds"].Split(" ");
				for (int i = 0; i < list.Length; ++i)
				{
					if (!isNumber(list[i]))
						return new ValidateResult(false, "one of the lessonsIds is incorrect");

					int iIndex = int.Parse(list[i]) - 1;
					if (!(iIndex >= 0 && iIndex < iLessonsCount))
						return new ValidateResult(false, "one of the lessonsIds is out of range");
				
					if (fields["lessonsIds"].Split(list[i]).Length - 1 > 1)
						return new ValidateResult(false, "one of the lessonsIds is repeated");
				}
			}

			return new ValidateResult(true, "all is ok");
		}

		public static ValidateResult validateStudent(Dictionary<string, string> fields, int iGroupsCount)
		{
			ValidateResult result = validatePeople(fields);
			if (!result.bSuccess)
				return result;

			if (fields["groupId"].Length > 0)
			{
				if (!isNumber(fields["groupId"]))
					return new ValidateResult(false, "incorrect groupId");

				int iIndex = int.Parse(fields["groupId"]) - 1;
				if (!(iIndex >= 0 && iIndex < iGroupsCount))
					return new ValidateResult(false, "groupId is out of range");
			}

			return new ValidateResult(true, "all is ok");
		}

		public static ValidateResult validateInstructor(Dictionary<string, string> fields)
		{
			ValidateResult result = validatePeople(fields);
			if (!result.bSuccess)
				return result;

			if (!(fields["expYears"].Length > 0 && isNumber(fields["expYears"])))
				return new ValidateResult(false, "incorrect expYears");

			return new ValidateResult(true, "all is ok");
		}

		public static ValidateResult validatePeople(Dictionary<string, string> fields)
		{
			if (fields["firstName"].Length == 0)
				return new ValidateResult(false, "firstName is an empty string");

			if (fields["lastName"].Length == 0)
				return new ValidateResult(false, "lastName is an empty string");

			if (!(fields["birthYear"].Length > 0 && isNumber(fields["birthYear"])))
				return new ValidateResult(false, "incorrect birthYear");

			if (DateTime.Now.Year - int.Parse(fields["birthYear"]) < 16)
				return new ValidateResult(false, $"birthYear should be <= {DateTime.Now.Year - 16}");

			return new ValidateResult(true, "all is ok");
		}

		public static bool isNumber(string s) {return int.TryParse(s, out int iOut);}
	}
}