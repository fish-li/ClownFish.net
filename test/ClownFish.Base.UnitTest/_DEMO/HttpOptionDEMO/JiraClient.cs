using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base;
using ClownFish.Base.WebClient;
using Microsoft.Win32;

namespace ClownFish.Base.UnitTest._DEMO.HttpOptionDEMO
{
    public class JiraClient
    {
        public class CreateIssueResult
        {
            public string formToken { get; set; }

            public string atl_token { get; set; }
        }

        public class UploadResult
        {
            public string name { get; set; }

            public string id { get; set; }
        }


        private readonly CookieContainer _CookieContainer = new CookieContainer();
        private string _username;
        private int _projectId;

        private CreateIssueResult _issueResult;
        private List<UploadResult> _uploadResults = new List<UploadResult>();


        #region 用户登录
        public void Login()
        {
            string path = @"HKEY_LOCAL_MACHINE\SOFTWARE\FishLi\mememe";
            string uid = Registry.GetValue(path, "uid", string.Empty).ToString();
            string pwd = Registry.GetValue(path, "pwd", string.Empty).ToString();
            pwd = System.Web.HttpUtility.UrlEncode(pwd);


            string text = $@"
POST https://jira.fish-test.com/rest/gadget/1.0/login HTTP/1.1
X-Atlassian-Token: no-check
Content-Type: application/x-www-form-urlencoded; charset=UTF-8

os_username={uid}&os_password={pwd}&os_cookie=true
";

            HttpOption httpOption = HttpOption.FromRawText(text);
            httpOption.Cookie = _CookieContainer;

            string responseText = httpOption.GetResult();
            Console.WriteLine("=======================================================");
            Console.WriteLine("登录成功！");
            Console.WriteLine("=======================================================");

            _username = uid;
        }



        #endregion



        #region 打开创建问题对话框

        public void OpenIssueDialog()
        {
            string text = $@"
POST https://jira.fish-test.com/secure/QuickCreateIssue!default.jspa?decorator=none HTTP/1.1
X-AUSERNAME: {_username}
Accept: */*
X-Requested-With: XMLHttpRequest
Referer: https://jira.fish-test.com/secure/Dashboard.jspa
Accept-Language: en-US,en;q=0.8,zh-Hans-CN;q=0.5,zh-Hans;q=0.3
Accept-Encoding: gzip, deflate
User-Agent: Mozilla/5.0 (Windows NT 6.3; WOW64; Trident/7.0; rv:11.0) like Gecko
Host: jira.fish-test.com
Connection: Keep-Alive
Cache-Control: no-cache
";

            HttpOption httpOption = HttpOption.FromRawText(text);
            httpOption.Cookie = _CookieContainer;


            string json = httpOption.GetResult();
            Console.WriteLine($"GO-URL {httpOption.Url} 成功！");

            _issueResult = json.FromJson<CreateIssueResult>();
        }



        #endregion


        #region 提交表单

        public void SetProjectId(int projectId)
        {
            _projectId = projectId;
        }

        public void UploadFile(string filePath)
        {
            byte[] fileBody = System.IO.File.ReadAllBytes(filePath);

            object args = new {
                filename = System.IO.Path.GetFileName(filePath),
                size = fileBody.Length.ToString(),
                atl_token = _issueResult.atl_token,
                formToken = _issueResult.formToken,
                projectId = _projectId
            };
            string queryString = FormDataCollection.GetQueryString(args);

            string text = $@"
POST https://jira.fish-test.com/rest/internal/2/AttachTemporaryFile?{queryString} HTTP/1.1
Accept: */*
Referer: https://jira.fish-test.com/secure/Dashboard.jspa
Accept-Language: en-US,en;q=0.8,zh-Hans-CN;q=0.5,zh-Hans;q=0.3
Accept-Encoding: gzip, deflate
User-Agent: Mozilla/5.0 (Windows NT 6.3; WOW64; Trident/7.0; rv:11.0) like Gecko
Host: jira.fish-test.com
Connection: Keep-Alive
Cache-Control: no-cache
";


            HttpOption httpOption = HttpOption.FromRawText(text);
            httpOption.Data = fileBody;
            httpOption.Cookie = _CookieContainer;


            string responseText = httpOption.GetResult();
            Console.WriteLine(responseText);
            Console.WriteLine($"#################### 上传附件 成功！ ####################");


            UploadResult uploadResult = responseText.FromJson<UploadResult>();
            _uploadResults.Add(uploadResult);
        }



        public void SubmitIssue(int issuetype, string summary, string assignee, string duedate, string customer, string description)
        {
            FormDataCollection form = new FormDataCollection();
            form.AddString("pid", _projectId.ToString())
                .AddString("issuetype", issuetype.ToString())
                .AddString("atl_token", _issueResult.atl_token)
                .AddString("formToken", _issueResult.formToken)
                .AddString("summary", summary)
                .AddString("assignee", assignee);

            foreach( var x in _uploadResults )
                form.AddString("filetoconvert", x.id);

            form.AddString("components", "11122")
                .AddString("customfield_10004", "193")
                .AddString("duedate", duedate)
                .AddString("customfield_10204", customer)
                .AddString("description", description)
                .AddString("fieldsToRetain", "project")
                .AddString("fieldsToRetain", "issuetype")
                .AddString("fieldsToRetain", "assignee")
                .AddString("fieldsToRetain", "components")
                .AddString("fieldsToRetain", "customfield_10004")
                .AddString("fieldsToRetain", "duedate")
                .AddString("fieldsToRetain", "customfield_10204");

            foreach( var x in _uploadResults )
                form.AddString("fieldsToRetain", "filetoconvert-" + x.id);


            string text = $@"
POST https://jira.fish-test.com/secure/QuickCreateIssue.jspa?decorator=none HTTP/1.1
Accept: */*
Content-Type: application/x-www-form-urlencoded; charset=UTF-8
X-AUSERNAME: {_username}
X-Requested-With: XMLHttpRequest
Referer: https://jira.fish-test.com/secure/Dashboard.jspa
Accept-Language: en-US,en;q=0.8,zh-Hans-CN;q=0.5,zh-Hans;q=0.3
Accept-Encoding: gzip, deflate
User-Agent: Mozilla/5.0 (Windows NT 6.3; WOW64; Trident/7.0; rv:11.0) like Gecko
Host: jira.fish-test.com
Connection: Keep-Alive
Cache-Control: no-cache
";


            HttpOption httpOption = HttpOption.FromRawText(text);
            httpOption.Data = form.ToString();
            httpOption.Cookie = _CookieContainer;


            string responseText = httpOption.GetResult();
            Console.WriteLine($"#################### 创建问题 成功！ ####################");


        }

        #endregion





    }
}
