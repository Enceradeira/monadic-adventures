using System;
using NUnit.Framework;

namespace MonadicAdventures
{
	[TestFixture]
	public class DebuggableFixture
	{
		public Tuple<double, string> Cube(double value)
		{
			return new Tuple<double, string>(value*value, "Cube was called.");
		}

		public Tuple<double, string> Sine(double value)
		{
			return new Tuple<double, string>(Math.Sin(value), "Sine was called.");
		}

		public double SineCube(double value)
		{
			return Sine(Cube(value).Item1).Item1;
		}

		public Func<T, T> Compose<T>(Func<T, T> f, Func<T, T> g)
		{
			return (x) => f(g(x));
		}

		public Func<T, Tuple<T, string>> ComposeDebuggable<T>(Func<T, Tuple<T, string>> f, Func<T, Tuple<T, string>> g)
		{
			return (x) =>
			{
				var resultG = g(x);
				var resultF = f(resultG.Item1);
				return new Tuple<T, string>(resultF.Item1, resultG.Item2 + resultF.Item2);
			};
		}


		private Tuple<double, string> Unit(int value)
		{
			return new Tuple<double, string>(value, "");
		}

		private Func<Tuple<double, string>, Tuple<double, string>> Bind(Func<double, Tuple<double, string>> f)
		{
			return value =>
			{
				var result = f(value.Item1);
				return new Tuple<double, string>(result.Item1, value.Item2 + result.Item2);
			};
		}

		[Test]
		public void Compose()
		{
			Assert.That(SineCube(45), Is.EqualTo(Compose<double>((x) => Sine(x).Item1, (x) => Cube(x).Item1)(45)));

			var result = Compose(Bind(Sine), Bind(Cube))(Unit(12));
			Console.WriteLine(result.Item2);
			Assert.That(SineCube(12), Is.EqualTo(result.Item1));
		}

		[Test]
		public void DebuggableCompose()
		{
			var result = ComposeDebuggable<double>(Sine, Cube)(12);
			Console.WriteLine(result.Item2);
			Assert.That(SineCube(12), Is.EqualTo(result.Item1));
		}
	}
}