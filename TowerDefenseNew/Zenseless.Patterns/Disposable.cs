using System;
using System.Collections;
using System.Linq;
using System.Reflection;

namespace Zenseless.Patterns
{
	/// <summary>
	/// Implements the default disposing behavior as recommended by Microsoft.
	/// If you have resources that need disposing, subclass this class.
	/// </summary>
	public abstract class Disposable : IDisposable
	{
		/// <summary>
		/// Will be called from the default Dispose method.
		/// Implementers should dispose all their resources her.
		/// </summary>
		protected abstract void DisposeResources();

		/// <summary>
		/// Dispose status of the instance.
		/// </summary>
		public bool Disposed => disposed;

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Calls <see cref="IDisposable.Dispose()"/> on all fields of type <see cref="IDisposable"/> found on the given object.
		/// Also calls <see cref="IDisposable.Dispose()"/> on each item of fields of type <see cref="IEnumerable"/>
		/// </summary>
		/// <param name="obj"></param>
		public static void DisposeAllFields(object obj)
		{
			// get all fields, including backing fields for properties
			var allFields = obj.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			foreach (var field in allFields.Where(field => typeof(IDisposable).IsAssignableFrom(field.FieldType)))
			{
				((IDisposable?)field.GetValue(obj))?.Dispose();
			}
			foreach (var field in allFields.Where(field => typeof(IEnumerable).IsAssignableFrom(field.FieldType)))
			{
				var enumerable = (IEnumerable?)field.GetValue(obj);
				if (enumerable is null) break;
				foreach (var d in enumerable.OfType<IDisposable>()) d.Dispose();
			}
		}


		/// <summary>
		/// Leave out the finalizer altogether if this class doesn't
		/// own unmanaged resources, but leave the other methods
		/// exactly as they are.
		/// </summary>
		~Disposable()
		{
			// Finalizer calls Dispose(false)
			Dispose(false);
		}

		private bool disposed = false;

		private void Dispose(bool disposing)
		{
			if (disposed) return;
			if (disposing)
			{
				DisposeResources();
				disposed = true;
			}
		}
	}
}
