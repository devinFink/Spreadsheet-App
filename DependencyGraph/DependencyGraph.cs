// Skeleton implementation written by Joe Zachary for CS 3500, September 2013.
// Version 1.1 (Fixed error in comment for RemoveDependency.)
// Version 1.2 - Daniel Kopta 
//               (Clarified meaning of dependent and dependee.)
//               (Clarified names in solution/project structure.)

using System.Diagnostics;

namespace SpreadsheetUtilities
{
    /// <summary>
    /// Author:    Devin Fink
    /// Partner:   None
    /// Date:      01/26/23
    /// Course:    CS 3500, University of Utah, School of Computing
    /// Copyright: CS 3500 and Devin Fink - This work may not 
    ///            be copied for use in Academic Coursework.
    ///
    /// I, Devin Fink, certify that I wrote this code from scratch
    /// (Except for the given methods and headers) and
    /// did not copy it in part or whole from another source.  All 
    /// references used in the completion of the assignments are cited 
    /// in my README file.
    ///
    /// File Contents
    ///
    ///    This File provides the methods to keep track of a list
    ///    of all needed dependencies in a spreadsheet
    /// </summary>

    /// <summary>
    /// (s1,t1) is an ordered pair of strings
    /// t1 depends on s1; s1 must be evaluated before t1
    /// 
    /// A DependencyGraph can be modeled as a set of ordered pairs of strings.  Two ordered pairs
    /// (s1,t1) and (s2,t2) are considered equal if and only if s1 equals s2 and t1 equals t2.
    /// Recall that sets never contain duplicates.  If an attempt is made to add an element to a 
    /// set, and the element is already in the set, the set remains unchanged.
    /// 
    /// Given a DependencyGraph DG:
    /// 
    ///    (1) If s is a string, the set of all strings t such that (s,t) is in DG is called dependents(s).
    ///        (The set of things that depend on s)    
    ///        
    ///    (2) If s is a string, the set of all strings t such that (t,s) is in DG is called dependees(s).
    ///        (The set of things that s depends on) 
    //
    // For example, suppose DG = {("a", "b"), ("a", "c"), ("b", "d"), ("d", "d")}
    //     dependents("a") = {"b", "c"}
    //     dependents("b") = {"d"}
    //     dependents("c") = {}
    //     dependents("d") = {"d"}
    //     dependees("a") = {}
    //     dependees("b") = {"a"}
    //     dependees("c") = {"a"}
    //     dependees("d") = {"b", "d"}
    /// </summary>
    public class DependencyGraph
    {
        //The following variables represent the key, value pairs for both 
        //Dependents and dependees. A dictionary exists for both, 
        //where each value holds a hashset (for constant time access) 
        //for holding data.
        private int size; 
        private Dictionary<String, HashSet<String>> dependees;
        private Dictionary<String, HashSet<String>> dependents;
        /// <summary>
        /// Creates an empty DependencyGraph.
        /// </summary>
        public DependencyGraph()
        {
            dependees = new Dictionary<String, HashSet<String>>();
            dependents = new Dictionary<String, HashSet<String>>();
            this.size = 0;
        }


        /// <summary>
        /// The number of ordered pairs in the DependencyGraph.
        /// </summary>
        public int Size
        {
            get { return size; }
        }


        /// <summary>
        /// The size of dependees(s).
        /// This property is an example of an indexer.  If dg is a DependencyGraph, you would
        /// invoke it like this:
        /// dg["a"]
        /// It should return the size of dependees("a")
        /// </summary>
        public int this[string s]
        { 
            get {
                if(dependees.ContainsKey(s))
                    return dependees[s].Count();
                else 
                    return 0;
            }
        }


        /// <summary>
        /// Reports whether dependents(s) is non-empty.
        /// <param name="s"/>: String to pull dependents<param>
        /// </summary>
        public bool HasDependents(string s)
        {
            if (dependents.ContainsKey(s))
            {
                if (dependents[s].Count > 0)
                    return true;
                else return false;
            }
            return false;
        }


        /// <summary>
        /// Reports whether dependees(s) is non-empty.
        /// <param name="s"/>: String to pull dependees<param>
        /// </summary>
        public bool HasDependees(string s)
        {
            if(dependees.ContainsKey(s))
            {
                if (dependees[s].Count > 0)
                    return true;
                else return false;
            }
            return false;
        }


        /// <summary>
        /// Enumerates dependents(s).
        /// <param name = "s" />: String to pull dependents<param>
        /// <retur>Any IEnumerable of Strings</retur>:
        /// </summary>
        public IEnumerable<string> GetDependents(string s)
        {
            List<String> dependentList = new List<String>();
            if (dependents.TryGetValue(s, out HashSet<String> values))
            {
                foreach (String key in values)
                {
                    dependentList.Add(key);
                }
            }
            return dependentList;
        }

        /// <summary>
        /// Enumerates dependees(s).
        /// <param name="s"/>: String to pull dependees<param>
        /// <retur>Any IEnumerable of Strings</retur>:
        /// </summary>
        public IEnumerable<string> GetDependees(string s)
        {
            List<String> dependeeList = new List<String>();
            if (dependees.TryGetValue(s, out HashSet<String> values))
            {
                foreach (String key in values)
                {
                    dependeeList.Add(key);
                }
            }
            return dependeeList;
        }


        /// <summary>
        /// <para>Adds the ordered pair (s,t), if it doesn't exist</para>
        /// 
        /// <para>This should be thought of as:</para>   
        /// 
        ///   t depends on s
        ///
        /// </summary>
        /// <param name="s"> s must be evaluated first. T depends on S</param>
        /// <param name="t"> t cannot be evaluated until s is</param>        /// 
        public void AddDependency(string s, string t)
        {
            if (!dependents.ContainsKey(s))
            {
                dependents.Add(s, new HashSet<String>());
                dependents[s].Add(t);
                size++;
            }
            else
            {
                if(dependents[s].Add(t))
                     size++;
            }

            if (!dependees.ContainsKey(t))
            {
                dependees.Add(t, new HashSet<String>());
                dependees[t].Add(s);
            }
            else
            {
                dependees[t].Add(s);
            }


        }


        /// <summary>
        /// Removes the ordered pair (s,t), if it exists
        /// </summary>
        /// <param name="s"></param>
        /// <param name="t"></param>
        public void RemoveDependency(string s, string t)
        {
            if (dependents.ContainsKey(s))
            {
                if (dependents[s].Contains(t))
                {
                    dependents[s].Remove(t);
                    dependees[t].Remove(s);
                    size--;
                }
            }
        }


        /// <summary>
        /// Removes all existing ordered pairs of the form (s,r).  Then, for each
        /// t in newDependents, adds the ordered pair (s,t).
        /// <param name="s"/>String to replace</param>
        /// <param name="s"/>New List of Dependents</param>
        /// </summary>
        public void ReplaceDependents(string s, IEnumerable<string> newDependents)
        {
            if (dependents.ContainsKey(s))
            {
                foreach(string r in dependents[s])
                {
                    RemoveDependency(s, r);
                }
                foreach (String c in newDependents.ToList())
                {
                    if(c != null)
                    {
                        AddDependency(s, c);
                    }
                }
            }
            else
            {
                foreach (String c in newDependents.ToList())
                {
                    AddDependency(s, c);
                }
            }
        }


        /// <summary>
        /// Removes all existing ordered pairs of the form (r,s).  Then, for each 
        /// t in newDependees, adds the ordered pair (t,s).
        /// <param name="s"/>String to replace</param>
        /// <param name="s"/>New List of Dependees</param>
        /// </summary>
        public void ReplaceDependees(string s, IEnumerable<string> newDependees)
        {
            if (dependees.ContainsKey(s))
            {
                foreach (string r in dependees[s])
                {
                    RemoveDependency(r, s);
                }
                foreach (String c in newDependees.ToList())
                {
                    if (c != null)
                    {
                        AddDependency(c, s);
                    }
                }
            }
            else
            {
                foreach (String c in newDependees.ToList())
                {
                    AddDependency(c, s);
                }
            }
        }

    }

}