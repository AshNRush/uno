﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Uno.UI.DataBinding;
using Windows.UI.Xaml.Markup;

namespace Windows.UI.Xaml
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public class NameScope : INameScope
	{
		private Dictionary<string, ManagedWeakReference> _names = new Dictionary<string, ManagedWeakReference>();

		public NameScope()
		{
		}

		public object FindName(string name)
		{
			return _names.TryGetValue(name, out var element)
				? element.Target
				: null;
		}

		public void RegisterName(string name, object scopedElement)
		{
			_names.Add(name, WeakReferencePool.RentWeakReference(this, scopedElement));
		}

		public void UnregisterName(string name)
		{
			_names.Remove(name);
		}

		#region NameScope attached property

		/// <summary>
		/// Provides the attached property set accessor for the NameScope attached property.
		/// </summary>
		/// <param name="dependencyObject">Object to change XAML namescope for.</param>
		/// <param name="value">The new XAML namescope, using an interface cast.</param>
		public static void SetNameScope(DependencyObject dependencyObject, INameScope value)
		{
			dependencyObject.SetValue(NameScopeProperty, value);
		}

		/// <summary>
		/// Provides the attached property get accessor for the NameScope attached property.
		/// </summary>
		/// <param name="dependencyObject">The object to get the XAML namescope from.</param>
		/// <returns>A XAML namescope, as an INameScope instance.</returns>
		public static INameScope GetNameScope(DependencyObject dependencyObject)
		{
			return (INameScope)dependencyObject.GetValue(NameScopeProperty);
		}

		/// <summary>
		/// Identifies the NameScope attached property.
		/// </summary>
		public static readonly DependencyProperty NameScopeProperty =
			DependencyProperty.RegisterAttached(
				"NameScope",
				typeof(INameScope),
				typeof(NameScope),
				new FrameworkPropertyMetadata(
					null,
					// This property is inherited to ensure the NameScope is available to all children.
					// This differs from WPF's implementation, which doesn't seem to inherit this property, 
					// but still appears to have the effect we want.
					FrameworkPropertyMetadataOptions.Inherits
				)
			);

		#endregion

		/// <summary>
		/// Search for a named element in all available namescopes, preferring scopes that are 'closest' in the hierarchy.
		/// </summary>
		internal static object FindInNamescopes(DependencyObject caller, string name)
		{
			var parent = caller;
			while (parent != null)
			{
				var scope = NameScope.GetNameScope(parent);
				var target = scope?.FindName(name);

				if (target != null)
				{
					return target;
				}

				parent = parent.GetParent() as DependencyObject;
			}

			return null;
		}
	}
}
