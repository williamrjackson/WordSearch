using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using System.Text.RegularExpressions;

public class QueryStringParser : MonoBehaviour
{
    [SerializeField]
    private QueryStringCommand[] queryStrings;

    void Start()
    {
#if UNITY_WEBGL
        string url = Application.absoluteURL;
        // string url = "www.test.com?AllowBackwards=false&WordList=Up,In,On,Oh";
        var splitUrl = url.Split(new[] { '?' }, 2);
        if (splitUrl.Length > 1)
        {
            var querySubstring = splitUrl[1];
            System.Collections.Specialized.NameValueCollection results = System.Web.HttpUtility.ParseQueryString(querySubstring);
            foreach (var qs in queryStrings)
            {
                foreach (var item in results)
                {
                    if (item.ToString().ToLower() == qs.queryString.ToLower())
                    {
                        qs.action.Invoke(results[item.ToString()]);
                        qs.option.Invoke(ToBool(results[item.ToString()]));
                    }
                }
            }
        }
#endif
    }
    
    public bool ToBool(string switchStr)
    {
        if (switchStr.ToLower().Trim() == "true")
        {
            return true;
        }
        return false;
    }
    [System.Serializable]
    public class QueryStringCommand
    {
        public string queryString;
        public UnityEvent<string> action;
        public UnityEvent<bool> option;
    }
}
