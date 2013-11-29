using System;
using NUnit.Framework;

namespace MonadicAdventures
{
	[TestFixture]
	public class DebuggableExcercise
	{
		public class Person
		{
			public string Name
			{
				get { return "John"; }
			}
		}

		private Tuple<Person, string> GetPersons(int age)
		{
			return new Tuple<Person, string>(new Person(), "GetPersons called");
		}

		private Tuple<string, string> GetName(Person person)
		{
			return new Tuple<string, string>(person.Name, "GetName called");
		}

		private Tuple<T, string> Unit<T>(T value)
		{
			return new Tuple<T, string>(value, "");
		}

		private Func<Tuple<T1, string>, Tuple<T2, string>> Bind<T1, T2>(Func<T1, Tuple<T2, string>> f)
		{
			return input =>
			{
				var result = f(input.Item1);
				return new Tuple<T2, string>(result.Item1, input.Item2 + result.Item2);
			};
		}

		public Func<T1, T3> Compose<T1, T2, T3>(Func<T2, T3> f, Func<T1, T2> g)
		{
			return (x) => f(g(x));
		}

		[Test]
		public void Debuggable()
		{
			// var dummy = Bind<Person, string>(GetName)(Bind<int, Person>(GetPersons))
			var getNameOfAgedPerson = Compose(Bind<Person, string>(GetName), Bind<int, Person>(GetPersons))(Unit(24));

			Console.WriteLine("Debug Output: " + getNameOfAgedPerson.Item2);
			Assert.That(getNameOfAgedPerson.Item1, Is.EqualTo("John"));
		}
	}
}