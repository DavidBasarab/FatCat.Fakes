using FluentAssertions;
using Xunit;

namespace FatCat.Fakes.Tests.Primitives
{
	public abstract class FakeNumberItems<T> : PrimitiveTests<T> where T : struct
	{
		[Fact]
		public void ANumberCanBeFaked()
		{
			var value = Faker.Create<T>();

			VerifyRange(value);
		}

		[Fact]
		public void CanFakeANullableNumber()
		{
			var value = Faker.Create<T?>();

			value.Should().NotBeNull();

			// ReSharper disable once PossibleInvalidOperationException
			VerifyRange(value.Value);
		}

		[Fact]
		public void CanFakeByType()
		{
			var value = (T)Faker.Create(typeof(T));

			VerifyRange(value);
		}

		protected abstract void VerifyRange(T value);
	}
}