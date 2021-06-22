using System;

namespace Zenseless.Patterns
{
	/// <summary>
	/// Class that implements the dirty flag pattern http://gameprogrammingpatterns.com/dirty-flag.html.
	/// A value is cached and only recalculated, if invalidated.
	/// </summary>
	/// <typeparam name="ValueType">The type of the cached value.</typeparam>
	public class DirtyFlag<ValueType>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DirtyFlag{ValueType}"/> class.
		/// </summary>
		/// <param name="calculateValue">Functor for calculating the value.</param>
		/// <exception cref="ArgumentNullException">calculateValue</exception>
		public DirtyFlag(Func<ValueType> calculateValue)
		{
			_calculateValue = calculateValue ?? throw new ArgumentNullException(nameof(calculateValue));
			_value = _calculateValue();
			IsCacheDirty = false;
		}


		/// <summary>
		/// Invalidates the cached value.
		/// </summary>
		public void Invalidate() => IsCacheDirty = true;

		/// <summary>
		/// Gets the cached value. If value is not valid (isDirty == true) it will get recalculated.
		/// </summary>
		/// <value>
		/// The value.
		/// </value>
		public ValueType Value
		{
			get
			{
				if(IsCacheDirty)
				{
					_value = _calculateValue();
					IsCacheDirty = false;
				}
				return _value;
			}
		}

		/// <summary>
		/// Gets a value indicating whether the cache is dirty (needs to be recalculated).
		/// </summary>
		/// <value>
		///   <c>true</c> if this instance cache is dirty; otherwise, <c>false</c>.
		/// </value>
		public bool IsCacheDirty { get; private set; } = true;

		private ValueType _value;
		private readonly Func<ValueType> _calculateValue;
	}
}
