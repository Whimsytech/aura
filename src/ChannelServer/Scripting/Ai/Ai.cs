// Copyright (c) Aura development team - Licensed under GNU GPL
// For more information, see license file in the main folder

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using System.Xaml;

namespace Aura.Channel.Scripting.Ai
{
	[ContentProperty("Elements")]
	public class Ai
	{
		private static readonly XamlXmlReaderSettings _readerSettings = new XamlXmlReaderSettings { LocalAssembly = typeof(Ai).Assembly };

		public string Name { get; set; }
		public string BasedOn { get; set; }
		public int AggroRadius { get; set; }

		public AffinityCollection Affinity { get; set; }

		public ElementCollection Elements { get; set; }

		public Ai()
		{
			Affinity = new AffinityCollection();
			Elements = new ElementCollection();
		}

		public static Ai Load(string file)
		{
			var reader = new XamlXmlReader(file, _readerSettings);

			var ai = (Ai)System.Windows.Markup.XamlReader.Load(reader);

			if (!string.IsNullOrEmpty(ai.BasedOn))
			{
				var aiBase = Load(ai.BasedOn);

				MergeAis(aiBase, ai);
			}

			return ai;
		}

		private static void MergeAis(Ai basedOn, Ai derived)
		{
			var newActions = (ElementCollection)(new List<Element>(basedOn.Elements)); // Copy base

			var namedActions = newActions.Where(a => !string.IsNullOrEmpty(a.Name))
				.ToDictionary(a => a.Name.ToUpperInvariant());
			var derivedNamedActions = derived.Elements.Where(a => !string.IsNullOrEmpty(a.Name))
				.ToDictionary(a => a.Name.ToUpperInvariant())
				.Where(n => namedActions.ContainsKey(n.Key));

			// Any elements that are re-specified in derived need to be overwritten
			foreach (var n in derivedNamedActions)
			{
				namedActions[n.Key] = n.Value;
			}

			// Add any new actions from derived
			newActions.AddRange(derived.Elements.Except(derivedNamedActions.Select(a => a.Value)));

			derived.Elements = newActions;
		}
	}
}
