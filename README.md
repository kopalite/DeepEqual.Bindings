# DeepEqual.Bindings

DeepEqual.Bindings is extension of excelent DeepEqual library for comparing objects by values.

It compares 2 object graphs by matching node names (reference property names) and values only. 
Thy type of node object is totally ignored, as long as each node has its match in other tree.

Example: Instances a1 and a2 of class A1 and A2 below are considered equal.
It doesn't matter that typeof(A1) != typeof(A2) or typeof(B1) != typeof(B2).
Property names and correspondent values are matched. 
	
	var a1 = new A1 { B = new B1 { Y = "value" } };
	var a2 = new A2 { B = new B2 { Y = "value" } };
	var result = a1.IsDeepEqual(a2); //result is true.
	
If type A1 had property B named differently (e.g. 'DifferentName'), these 2 instances would not be equal. 
DeepEqual.Bindings overcomes that obstacle - you can bind properties with diferent names during comparison:

	var comparer = ExtendedComparer<A1, A2>.New().Bind(x1 => x1.DifferentName, x2 => B);
	var result = comparer.Compare(a1, a2); //result is true.
	
You can also specify custom comparison expression:

	var comparer = ExtendedComparer<A1, A2>.New().Bind(x1 => x1.DifferentName, x2 => B, 
			(x1, x2) => x1.DifferentName.Y == "anything you say" && x2.Y == "you take my breath away");
							
Unmatched properties in any of 2 types will make any 2 instances not equal.
You can skip equality check by that property with method Skip(), a wrapper around DeepEqual's IgnoreProperty()


	var comparer = ExtendedComparer<A1, A2>.New().Bind(x1 => x1.DifferentName, x2 => B)
												 .Skip(x1 => x1.SomeOtherName)
												 
The whole idea works extremly well in unit test for mappers (like AutoMapper, ExpressMapper etc.)
E.G. AutoMapper.CreateMap() creates a way how to make A2 instance out of A1 in very custom way.

	var a2 = AutoMapper.Map<A1, A2>(a1);
	Assert.IsTrue(comparer.Compare(a1, a2, out differences), differences);
	
Enjoy using it and let me know if I can make it better.

 


