using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base;
using ClownFish.Base.WebClient;
using Microsoft.Win32;

namespace ClownFish.Base.UnitTest._DEMO.HttpOptionDEMO
{
    public class JiraClientV2
    {
        public class LoginResult
        {
            public bool loginSucceeded { get; set; }
        }

        public class IssueResult
        {
            public string id { get; set; }
        }


        private readonly CookieContainer _CookieContainer = new CookieContainer();

        private IssueResult _issueResult;


        #region 用户登录
        public void Login()
        {
            string path = @"HKEY_LOCAL_MACHINE\SOFTWARE\FishLi\mememe";
            string uid = Registry.GetValue(path, "uid", string.Empty).ToString();
            string pwd = Registry.GetValue(path, "pwd", string.Empty).ToString();


            string text = $@"
POST https://jira.fish-test.com/rest/gadget/1.0/login HTTP/1.1
X-Atlassian-Token: no-check
Content-Type: application/x-www-form-urlencoded; charset=UTF-8
";

            HttpOption httpOption = HttpOption.FromRawText(text);
            httpOption.Cookie = _CookieContainer;
            httpOption.Data = new {
                os_username = uid,
                os_password = pwd,
                os_cookie = true
            };


            string responseText = httpOption.GetResult();
            Console.WriteLine(responseText);
            Console.WriteLine("=======================================================");
            LoginResult loginResult = responseText.FromJson<LoginResult>();
            Console.WriteLine(loginResult.loginSucceeded ? "登录成功！" : "登录失败！");
            Console.WriteLine("=======================================================");
        }


        #endregion




        #region 提交表单

        /* 创建问题的数据格式， "issue-data.json"

        {
            "fields": {
            "project": {
                "id": "10705"
            },
            "summary": "TEST——影视作品中有哪个情节片段击中了你的心？",
            "issuetype": {
                "id": "10400"
            },
            "assignee": {
                "name": "wangf13"
            },
            "description": "问题描述——11111111111111111111111",
            "duedate": "2019-03-11",
            "components": [
                {
                "id": "11122"
                }
            ],
            "customfield_10004": 193,
            "customfield_10204": "客户名称——55555555555555"
            }
        }

        */

        public void SubmitIssue()
        {
            string text = $@"
POST https://jira.fish-test.com/rest/api/2/issue HTTP/1.1
X-Atlassian-Token: no-check
Content-Type: application/json; charset=UTF-8
";

            HttpOption httpOption = HttpOption.FromRawText(text);
            httpOption.Data = File.ReadAllText("issue-data.json");
            httpOption.Cookie = _CookieContainer;

            string responseText = httpOption.GetResult();
            Console.WriteLine(responseText);
            _issueResult = responseText.FromJson<IssueResult>();
            Console.WriteLine($"#################### 创建问题 成功！ ####################");

        }


        public void UploadFile(string filePath)
        {
            HttpOption httpOption = new HttpOption {
                Method = "POST",
                Url = $"https://jira.fish-test.com/rest/api/2/issue/{_issueResult.id}/attachments",
                Headers = new Dictionary<string, string>() {
                    { "X-Atlassian-Token" , "no-check"  }
                },
                Data = new {
                    file = new FileInfo(filePath)
                },
                Cookie = _CookieContainer
            };

            string responseText = httpOption.GetResult();
            //Console.WriteLine(responseText);
            Console.WriteLine($"#################### 上传附件 成功！ ####################");
        }


        public void ShowIssueStatus()
        {
            HttpOption httpOption = new HttpOption {
                Url = $"https://jira.fish-test.com/rest/api/2/issue/{_issueResult.id}?fields=status,creator",
                Cookie = _CookieContainer
            };

            string responseText = httpOption.GetResult();
            Console.WriteLine($"#################### 问题状态描述 ####################");
            Console.WriteLine(responseText);
        }


        #endregion





    }
}
