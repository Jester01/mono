//
// Copyright (C) 2010 Novell Inc. http://novell.com
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Markup;
using System.Xaml;
using System.Xaml.Schema;
using System.Xml;
using NUnit.Framework;

using Category = NUnit.Framework.CategoryAttribute;

namespace MonoTests.System.Xaml
{
	[TestFixture]
	public class XamlXmlReaderTest
	{

		// read test

		XamlReader GetReader (string filename)
		{
			return new XamlXmlReader (XmlReader.Create (Path.Combine ("Test/XmlFiles", filename), new XmlReaderSettings () { CloseInput =true }));
		}

		void ReadTest (string filename)
		{
			var r = GetReader (filename);
			while (!r.IsEof)
				r.Read ();
		}

		void LoadTest (string filename, Type type)
		{
			var obj = XamlServices.Load (GetReader (filename));
			Assert.AreEqual (type, obj.GetType (), "type");
		}

		[Test]
		public void Read_String ()
		{
			ReadTest ("String.xml");
			//LoadTest ("String.xml", typeof (string));
		}

		[Test]
		public void Read_Int32 ()
		{
			ReadTest ("Int32.xml");
			//LoadTest ("Int32.xml", typeof (int));
		}

		[Test]
		public void Read_DateTime ()
		{
			ReadTest ("DateTime.xml");
			//LoadTest ("DateTime.xml", typeof (DateTime));
		}

		[Test]
		public void Read_TimeSpan ()
		{
			ReadTest ("TimeSpan.xml");
			//LoadTest ("TimeSpan.xml", typeof (TimeSpan));
		}

		[Test]
		public void Read_ArrayInt32 ()
		{
			ReadTest ("Array_Int32.xml");
			//LoadTest ("Array_Int32.xml", typeof (int []));
		}

		[Test]
		[Category ("NotWorking")]
		public void Read_ListInt32 ()
		{
			ReadTest ("List_Int32.xml");
			LoadTest ("List_Int32.xml", typeof (List<int>));
		}

		[Test]
		public void Read1 ()
		{
			var r = GetReader ("Int32.xml");

			Assert.IsTrue (r.Read (), "ns#1");
			Assert.AreEqual (XamlNodeType.NamespaceDeclaration, r.NodeType, "ns#2");
			Assert.AreEqual (XamlLanguage.Xaml2006Namespace, r.Namespace.Namespace, "ns#3");

			Assert.IsTrue (r.Read (), "so#1");
			Assert.AreEqual (XamlNodeType.StartObject, r.NodeType, "so#2");
			Assert.AreEqual (XamlLanguage.Int32, r.Type, "so#3");

			Assert.IsTrue (r.Read (), "sbase#1");
			Assert.AreEqual (XamlNodeType.StartMember, r.NodeType, "sbase#2");
			Assert.AreEqual (XamlLanguage.Base, r.Member, "sbase#3");

			Assert.IsTrue (r.Read (), "vbase#1");
			Assert.AreEqual (XamlNodeType.Value, r.NodeType, "vbase#2");
			Assert.IsTrue (r.Value is string, "vbase#3");

			Assert.IsTrue (r.Read (), "ebase#1");
			Assert.AreEqual (XamlNodeType.EndMember, r.NodeType, "ebase#2");

			Assert.IsTrue (r.Read (), "sinit#1");
			Assert.AreEqual (XamlNodeType.StartMember, r.NodeType, "sinit#2");
			Assert.AreEqual (XamlLanguage.Initialization, r.Member, "sinit#3");

			Assert.IsTrue (r.Read (), "vinit#1");
			Assert.AreEqual (XamlNodeType.Value, r.NodeType, "vinit#2");
			Assert.AreEqual ("5", r.Value, "vinit#3"); // string

			Assert.IsTrue (r.Read (), "einit#1");
			Assert.AreEqual (XamlNodeType.EndMember, r.NodeType, "einit#2");

			Assert.IsTrue (r.Read (), "eo#1");
			Assert.AreEqual (XamlNodeType.EndObject, r.NodeType, "eo#2");

			Assert.IsFalse (r.Read (), "end");
		}

		[Test]
		public void Read2 ()
		{
			var r = GetReader ("DateTime.xml");

			Assert.IsTrue (r.Read (), "ns#1");
			Assert.AreEqual (XamlNodeType.NamespaceDeclaration, r.NodeType, "ns#2");
			Assert.AreEqual ("clr-namespace:System;assembly=mscorlib", r.Namespace.Namespace, "ns#3");

			Assert.IsTrue (r.Read (), "so#1");
			Assert.AreEqual (XamlNodeType.StartObject, r.NodeType, "so#2");
			Assert.AreEqual (r.SchemaContext.GetXamlType (typeof (DateTime)), r.Type, "so#3");

			Assert.IsTrue (r.Read (), "sbase#1");
			Assert.AreEqual (XamlNodeType.StartMember, r.NodeType, "sbase#2");
			Assert.AreEqual (XamlLanguage.Base, r.Member, "sbase#3");

			Assert.IsTrue (r.Read (), "vbase#1");
			Assert.AreEqual (XamlNodeType.Value, r.NodeType, "vbase#2");
			Assert.IsTrue (r.Value is string, "vbase#3");

			Assert.IsTrue (r.Read (), "ebase#21");

			Assert.IsTrue (r.Read (), "sinit#1");
			Assert.AreEqual (XamlNodeType.StartMember, r.NodeType, "sinit#2");
			Assert.AreEqual (XamlLanguage.Initialization, r.Member, "sinit#3");

			Assert.IsTrue (r.Read (), "vinit#1");
			Assert.AreEqual (XamlNodeType.Value, r.NodeType, "vinit#2");
			Assert.AreEqual ("2010-04-14", r.Value, "vinit#3"); // string

			Assert.IsTrue (r.Read (), "einit#1");
			Assert.AreEqual (XamlNodeType.EndMember, r.NodeType, "einit#2");

			Assert.IsTrue (r.Read (), "eo#1");
			Assert.AreEqual (XamlNodeType.EndObject, r.NodeType, "eo#2");
			Assert.IsFalse (r.Read (), "end");
		}
	}
}