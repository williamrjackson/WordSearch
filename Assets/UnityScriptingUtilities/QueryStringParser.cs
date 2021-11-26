using UnityEngine.Events;
using UnityEngine;

namespace Wrj
{        
    public class QueryStringParser : MonoBehaviour
    {
        [SerializeField]
        private QueryStringCommand[] queryStrings;

        void Start()
        {
    #if UNITY_WEBGL
            string url = Application.absoluteURL;
            // Test string:
            // string url = "www.test.com/index.htm?EnableHints=true&WordList=Engagement,In,On,Oh&w=30&h=16&float=1.75";
            var splitUrl = url.Split(new[] { '?' }, 2);
            if (splitUrl.Length > 1)
            {
                var querySubstring = splitUrl[1];
                var results = System.Web.HttpUtility.ParseQueryString(querySubstring);
                foreach (var qs in queryStrings)
                {
                    foreach (var item in results)
                    {
                        if (item.ToString().ToLower() == qs.queryString.ToLower())
                        {
                            if (qs.action.GetPersistentEventCount() > 0)
                            {
                                qs.action.Invoke(results[item.ToString()]);
                            }
                            if (qs.option.GetPersistentEventCount() > 0 && 
                                bool.TryParse(results[item.ToString()], out bool bResult))
                            {
                                qs.option.Invoke(bResult);
                            }
                            if (qs.integer.GetPersistentEventCount() > 0 && 
                                int.TryParse(results[item.ToString()], out int iResult))
                            {
                                qs.integer.Invoke(iResult);
                            }
                            if (qs.floatVal.GetPersistentEventCount() > 0 && 
                                float.TryParse(results[item.ToString()], out float fResult))
                            {
                                qs.floatVal.Invoke(fResult);
                            }
                        }
                    }
                }
            }
    #endif
        }
        
        [System.Serializable]
        public class QueryStringCommand
        {
            public string queryString;
            public UnityEvent<string> action;
            public UnityEvent<bool> option;
            public UnityEvent<int> integer;
            public UnityEvent<float> floatVal;
        }
    }
}