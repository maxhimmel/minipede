using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Minipede.Utility
{
	[Serializable]
	public class TypeReference
	{
		public bool IsValid => !string.IsNullOrEmpty( Fullname );
		public Type RefType => Type.GetType( Fullname );

		[HideInInspector]
		public string Fullname;
	}

	public class TypeReferenceAttribute : Attribute
	{
		private readonly Type _typeConstraint;

		public TypeReferenceAttribute( Type typeConstraint )
		{
			_typeConstraint = typeConstraint;
		}

		public IEnumerable<Type> GetTypes()
		{
			var query = _typeConstraint.Assembly.GetTypes()
				.Where( x => !x.IsInterface )                           // Excludes BaseClass
				.Where( x => !x.IsAbstract )                            // Excludes BaseClass
				.Where( x => !x.IsGenericTypeDefinition )               // Excludes C1<>
				.Where( x => _typeConstraint.IsAssignableFrom( x ) );   // Excludes classes not inheriting from BaseClas

			return query;
		}
	}
}