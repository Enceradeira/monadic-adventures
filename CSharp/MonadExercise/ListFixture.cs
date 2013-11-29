using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace MonadicAdventures
{
	[TestFixture]
	public class ListFixture
	{
		[SetUp]
		public void SetUp()
		{
			_root = new Element("A", new[]
			{
				new Element("A1", new[]
				{
					new Element("A11"),
					new Element("A12")
				}),
				new Element("A2", new[]
				{
					new Element("A21"),
				})
			});
		}

		private Element _root;

		public class Element
		{
			public Element(string name, IEnumerable<Element> children = null)
			{
				Children = children ?? new Element[] {};
				Name = name;
			}

			public string Name { get; private set; }

			public IEnumerable<Element> Children { get; private set; }
		}

		public Func<T, T> Compose<T>(Func<T, T> f, Func<T, T> g)
		{
			return x => f(g(x));
		}

		public IEnumerable<Element> Children(Element parent)
		{
			return parent.Children;
		}

		private IEnumerable<T> Unit<T>(T root)
		{
			return new[] {root};
		}

		private Func<IEnumerable<Element>, IEnumerable<Element>> Bind(Func<Element, IEnumerable<Element>> children)
		{
			return input =>
			{
				var result = new List<Element>();
				foreach (var element in input)
				{
					result.AddRange(children(element));
				}
				return result;
			};
		}

		[Test]
		public void GetAllGrandchildren()
		{
			var grandchildren = Compose(Bind(Children), Bind(Children));
			var result = grandchildren(Unit(_root));

			var signatureString = string.Join(";", result.Select(e => e.Name));
			Assert.That(signatureString, Is.EqualTo("A11;A12;A21"));
		}
	}
}