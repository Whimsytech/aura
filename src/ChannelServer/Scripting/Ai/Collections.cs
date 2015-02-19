// Copyright (c) Aura development team - Licensed under GNU GPL
// For more information, see license file in the main folder

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aura.Channel.Scripting.Ai
{
	// Intellisense doesn't like the use of generics in
	// project references and xaml, for some odd reason.
	// So these let us cheat and fool it.

	public class ElementCollection : List<Element>
	{
		
	}

	public class AffinityCollection : List<Affinity>
	{
		
	}

	public class CaseCollection : List<Case>
	{
		
	}
}
