using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.Data.Dto.ChartOfAccountDto
{
 
    public class JsData
    {
        public string id { get; set; }
        public string text { get; set; }
        public string icon { get; set; }
        public State state { get; set; }
        public string parentId { get; set; }
        public List<JsData> children { get; set; }
        public Dictionary<string, object> li_attr { get; set; }
        public Dictionary<string, object> a_attr { get; set; }

        public static List<JsData> BuildTree(List<JsData> flatList)
        {
            var idToDataLookup = flatList.ToLookup(data => data.id);

            var roots = flatList
                .OrderBy(data => data.id.Length)
                .GroupBy(data => data.id.Substring(0, Math.Min(2, data.id.Length)))
                .Select(group => group.First())  // Select the shortest Id in each group as a root
                .ToList();

            foreach (var data in flatList)
            {
                var parent = idToDataLookup[data.parentId].FirstOrDefault(); // Use FirstOrDefault to handle cases where there is no parent with the specified id

                if (parent != null)
                {
                    if (parent.children == null)
                    {
                        parent.children = new List<JsData>();
                    }

                    parent.children.Add(data);
                }
            }

            return roots;
        }


        public static List<JsData> BuildTreeWithOrphanTagging(List<JsData> flatList)
        {
            var idToDataLookup = flatList.ToLookup(data => data.id);
            var orphanLeaves = new HashSet<JsData>();

            var roots = flatList
                .OrderBy(data => data.id.Length)
                .GroupBy(data => data.id.Substring(0, Math.Min(2, data.id.Length)))
                .Select(group => group.First())  // Select the shortest Id in each group as a root
                .ToList();

            foreach (var data in flatList)
            {
                var parent = idToDataLookup[data.parentId].FirstOrDefault(); // Use FirstOrDefault to handle cases where there is no parent with the specified id

                if (parent != null)
                {
                    if (parent.children == null)
                    {
                        parent.children = new List<JsData>();
                    }

                    parent.children.Add(data);
                }
                else
                {
                    // Node has no parent, consider it an orphan leaf
                    orphanLeaves.Add(data);
                }
            }

            // Tag orphan leaves during tree construction
            foreach (var orphanLeaf in orphanLeaves)
            {
                orphanLeaf.icon =  "mdi mdi-file-document-outline";
            }

            return roots;
        }

        public static void TagOrphanLeaves(List<JsData> tree)
        {
            // Use a stack for iterative tree traversal
            var stack = new Stack<JsData>(tree);

            while (stack.Count > 0)
            {
                var currentNode = stack.Pop();

                // Check if the current node is a leaf (has no children) and does not have a parent
                if ((currentNode.children == null || currentNode.children.Count == 0) && string.IsNullOrEmpty(currentNode.parentId))
                {
                    // Tag the orphan leaf node (modify as needed)
                    currentNode.icon  = "mdi mdi-file-document-outline";
                }

                // Add children to the stack for further processing
                if (currentNode.children != null)
                {
                    foreach (var child in currentNode.children)
                    {
                        stack.Push(child);
                    }
                }
            }
        }
    }

    public class State
    {
        public bool opened { get; set; }
        public bool disabled { get; set; }
        public bool selected { get; set; }
    }

    public class TreeNodeDto
    {
        public string id { get; set; }
        public string text { get; set; }
        public string icon { get; set; }
        public State state { get; set; }
    
        public List<JsData> children { get; set; }
        public Dictionary<string, object> li_attr { get; set; }
        public Dictionary<string, object> a_attr { get; set; }

    }
}
