using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Xml;

namespace CommentStripper
{
    static class Program
    {
        static int Main (string [] args)
        {
            Console.WriteLine ("XML Comment Stripper 1.0");

            if (args.Length != 2)
            {
                Console.WriteLine ("Usage: CommentStripper <assemblyFileName> <xmlFileName>");
                Console.WriteLine ("Upon success, the XML file is overwritten.");
                return -1;
            }

            string assemblyFileName = args [0];
            string xmlFileName = args [1];

            if (FileNotFound (assemblyFileName)) return -1;
            if (FileNotFound (xmlFileName)) return -1;

            Console.Write ("Stripping XML comments for internal members... ");

            DoTheStripping (assemblyFileName, xmlFileName);

            Console.WriteLine ("done.");

            return 0;
        }

        private static bool FileNotFound (string filename)
        {
            if (System.IO.File.Exists (filename)) return false;
            Console.WriteLine ("File not found: '" + filename + "'");
            return true;
        }

        private static void DoTheStripping (string assemblyFileName, string xmlFileName)
        {
            List <string> visibleMembers = CollectVisibleMembers (assemblyFileName);
            visibleMembers.Sort (); // sort so we can use BinarySearch later

            XmlDocument xmlDoc = new XmlDocument ();
            xmlDoc.Load (xmlFileName);
            XmlNodeList nodes = xmlDoc.DocumentElement.SelectNodes ("members/member");
            foreach (XmlNode node in nodes)
            {
                string name = node.Attributes ["name"].Value;
                name = name.Substring (2); // Clip off the preceding "T:" or "M:", etc
                name = name.Replace ("#ctor", ".ctor");

                // Treat overloaded methods as one method
                if (name.EndsWith (")")) name = name.Substring (0, name.IndexOf ('('));

                if (visibleMembers.BinarySearch (name) >= 0) continue;
                node.ParentNode.RemoveChild (node);
            }
            xmlDoc.Save (xmlFileName);
        }

        private static List <string> CollectVisibleMembers (string assemblyFileName)
        {
            Assembly assembly = Assembly.LoadFrom (assemblyFileName);
            var visibleMembers = new List <string> ();
            foreach (Type type in assembly.GetExportedTypes ())
            {
                string typeName = type.FullName.Replace ('+', '.');

                visibleMembers.Add (typeName);

                MemberInfo [] members = type.FindMembers (MemberTypes.All,
                    BindingFlags.Public | BindingFlags.DeclaredOnly |
                    BindingFlags.Instance | BindingFlags.Static, null, null);

                foreach (MemberInfo member in members)
                {
                    if (IsNotEditorBrowsable (member)) continue;
                    visibleMembers.Add (typeName + '.' + member.Name);
                }
            }
            return visibleMembers;
        }

        static bool IsNotEditorBrowsable (MemberInfo member)
        {
            object [] editorBrowsableAttributes = member.GetCustomAttributes (
                typeof (EditorBrowsableAttribute), false);

            if (editorBrowsableAttributes.Length == 0) return false;

            EditorBrowsableAttribute editorBrowsableAttribute =
                (EditorBrowsableAttribute) editorBrowsableAttributes [0];

            return (editorBrowsableAttribute.State == EditorBrowsableState.Never);
        }
    }
}
