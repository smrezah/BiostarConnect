using System.DirectoryServices;
using System.Reflection.PortableExecutable;
using System.Text;

namespace webRestaurantBS.Classes
{
    public class LdapAuthentication
    {
        private string _path;
        public string FullNameEn { get; private set; }
        public string DisplayName { get; private set; }
        public string Email { get; private set; }
        public string UserGUID { get; private set; }
        public string ErrorMessage { get; private set; }
        public List<string> OrgUnits { get; private set; }

        public LdapAuthentication(string path)
        {
            _path = path;
        }

        public async Task<bool> IsAuthenticated(string domain, string username, string pwd)
        {
            bool flag = false;

            await Task.Run(() =>
            {
                string domainAndUsername = domain + @"\" + username;
                System.DirectoryServices.DirectoryEntry entry = new System.DirectoryServices.DirectoryEntry(_path, domainAndUsername, pwd);

                try
                {
                    //Bind to the native AdsObject to force authentication.
                    object obj = entry.NativeObject;

                    DirectorySearcher search = new DirectorySearcher(entry)
                    {
                        Filter = "(SAMAccountName=" + username + ")"
                    };
                    search.PropertiesToLoad.Add("cn");
                    SearchResult result = search.FindOne();

                    if (null == result)
                    {
                        flag = false;
                    }
                    else
                    {
                        //Update the new path to the user in the directory.
                        _path = result.Path;
                        FullNameEn = (string)result.Properties["cn"][0];
                        string filterAtt = (string)result.Properties["adspath"][0];
                        flag = true;
                    }

                }
                catch (Exception ex)
                {
                    //The user name or password is incorrect.
                    if (ex.Message.Trim('\n').Trim('\r') == "The user name or password is incorrect.")
                        ErrorMessage = "نام کاربری یا رمز عبور وارد شده صحیح نمی باشد.";
                    else
                        ErrorMessage = ex.Message;
                    flag = false;
                    //throw new Exception("Error authenticating user. " + ex.Message);
                }
            });

            return flag;
        }

        public bool SetAttrUser(string domain, string username, string pwd)
        {
            try
            {
                string domainAndUsername = domain + @"\" + username;
                System.DirectoryServices.DirectoryEntry entry = new System.DirectoryServices.DirectoryEntry(_path, domainAndUsername, pwd);
                DirectorySearcher search = new DirectorySearcher(entry);

                //var retVal = (string)entry.Properties["employeeID"].Value;

                OrgUnits = new List<string>();
                SearchResultCollection res = search.FindAll();
                List<string> str = new List<string>(res[0].Path.Split(new char[] { ',', ' ' }));
                foreach (string item in str)
                {
                    if (item.Contains("OU"))
                    {
                        string[] word = item.Split('=');
                        OrgUnits.Add(word[1].ToLower());
                    }
                }
                DisplayName = res[0].Properties["displayname"][0].ToString();
                FullNameEn = res[0].Properties["name"][0].ToString();
                Email = res[0].Properties["userprincipalname"][0].ToString();
                UserGUID = new Guid((byte[])res[0].Properties["objectguid"][0]).ToString();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public string GetGroups(string domain, string username, string pwd)
        {
            string domainAndUsername = domain + @"\" + username;
            System.DirectoryServices.DirectoryEntry entry = new System.DirectoryServices.DirectoryEntry(_path, domainAndUsername, pwd);

            DirectorySearcher search = new DirectorySearcher(entry)
            {
                Filter = "(cn=" + FullNameEn + ")"
            };
            search.PropertiesToLoad.Add("memberOf");
            StringBuilder groupNames = new StringBuilder();

            try
            {
                SearchResult result = search.FindOne();
                int propertyCount = result.Properties["memberOf"].Count;
                string dn;
                int equalsIndex, commaIndex;

                for (int propertyCounter = 0; propertyCounter < propertyCount; propertyCounter++)
                {
                    dn = (string)result.Properties["memberOf"][propertyCounter];
                    equalsIndex = dn.IndexOf("=", 1);
                    commaIndex = dn.IndexOf(",", 1);
                    if (-1 == equalsIndex)
                    {
                        return null;
                    }
                    groupNames.Append(dn.Substring((equalsIndex + 1), (commaIndex - equalsIndex) - 1));
                    groupNames.Append("|");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error obtaining group names. " + ex.Message);
            }
            return groupNames.ToString();
        }
    }
}
